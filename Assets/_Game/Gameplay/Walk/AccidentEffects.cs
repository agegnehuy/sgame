using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Runtime-built audio + visual effects for the accident cinematic.
    /// Everything in here is procedurally generated so we don't depend on
    /// any external assets — drop into any scene and it just works.
    /// </summary>
    public static class AccidentEffects
    {
        // ──────────────────────────────────────────────────────────────────
        // Audio (synthesized once, cached, reused).
        // ──────────────────────────────────────────────────────────────────
        private static AudioClip _screechClip;
        private static AudioClip _thudClip;

        public static AudioClip ScreechClip
        {
            get { if (_screechClip == null) _screechClip = BuildScreechClip(); return _screechClip; }
        }

        public static AudioClip ThudClip
        {
            get { if (_thudClip == null) _thudClip = BuildThudClip(); return _thudClip; }
        }

        public static void PlayScreech(Transform follow, float volume = 1f)
        {
            PlayOneShotAt(ScreechClip, follow != null ? follow.position : Vector3.zero,
                follow, volume, pitch: 1f);
        }

        public static void PlayScreech(Vector3 worldPos, float volume = 1f)
        {
            PlayOneShotAt(ScreechClip, worldPos, follow: null, volume, pitch: 1f);
        }

        public static void PlayThud(Vector3 worldPos, float volume = 1f)
        {
            PlayOneShotAt(ThudClip, worldPos, follow: null, volume, pitch: 0.9f);
        }

        private static void PlayOneShotAt(AudioClip clip, Vector3 worldPos, Transform follow,
                                          float volume, float pitch)
        {
            if (clip == null) return;
            var go = new GameObject($"OneShot_{clip.name}");
            if (follow != null)
            {
                go.transform.SetParent(follow, worldPositionStays: false);
                go.transform.localPosition = Vector3.zero;
            }
            else
            {
                go.transform.position = worldPos;
            }
            var src = go.AddComponent<AudioSource>();
            src.clip = clip;
            src.volume = volume;
            src.pitch = pitch;
            src.spatialBlend = 0.85f;
            src.minDistance = 3f;
            src.maxDistance = 60f;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.Play();
            Object.Destroy(go, clip.length / Mathf.Max(0.01f, pitch) + 0.25f);
        }

        // 1.4s of tire screech: high-pitched whine that drops in frequency
        // mixed with band-pass-ish noise.
        private static AudioClip BuildScreechClip()
        {
            const int sampleRate = 22050;
            int length = (int)(sampleRate * 1.4f);
            var samples = new float[length];
            var rng = new System.Random(1);
            float phase = 0f;
            for (int i = 0; i < length; i++)
            {
                float t = (float)i / length;
                float freq = Mathf.Lerp(880f, 320f, Mathf.Pow(t, 0.85f));
                phase += freq / sampleRate;
                float tone = Mathf.Sin(phase * Mathf.PI * 2f);
                float harmonic = Mathf.Sin(phase * Mathf.PI * 4f) * 0.35f;
                float noise = ((float)rng.NextDouble() * 2f - 1f) * 0.55f;
                // Quick attack (10ms), slow decay near the end.
                float attack = Mathf.Min(1f, t / 0.01f);
                float release = Mathf.Min(1f, (1f - t) / 0.15f);
                float env = attack * release;
                samples[i] = (tone * 0.45f + harmonic + noise) * env * 0.65f;
            }
            var clip = AudioClip.Create("AccidentScreech", length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        // 0.6s of metallic crunch: low-freq sine thud + brief noise burst.
        private static AudioClip BuildThudClip()
        {
            const int sampleRate = 22050;
            int length = (int)(sampleRate * 0.6f);
            var samples = new float[length];
            var rng = new System.Random(2);
            float phase = 0f;
            for (int i = 0; i < length; i++)
            {
                float t = (float)i / length;
                float freq = Mathf.Lerp(110f, 40f, t);
                phase += freq / sampleRate;
                float tone = Mathf.Sin(phase * Mathf.PI * 2f);
                float noise = ((float)rng.NextDouble() * 2f - 1f);
                float env = Mathf.Exp(-t * 6.5f);
                float noiseEnv = Mathf.Exp(-t * 25f); // burst of metal at the very start
                samples[i] = (tone * 0.85f + noise * 0.6f * noiseEnv) * env * 0.95f;
            }
            var clip = AudioClip.Create("AccidentThud", length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        // ──────────────────────────────────────────────────────────────────
        // Red full-screen flash at the moment of impact.
        // ──────────────────────────────────────────────────────────────────
        public static IEnumerator RedFlash(MonoBehaviour host, float duration = 0.55f, float peakAlpha = 0.7f)
        {
            var go = new GameObject("AccidentRedFlash");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9000;
            go.AddComponent<CanvasScaler>();

            var imgGo = new GameObject("Flash");
            imgGo.transform.SetParent(go.transform, false);
            var rt = imgGo.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var img = imgGo.AddComponent<Image>();
            img.raycastTarget = false;
            img.color = new Color(0.7f, 0.05f, 0.05f, peakAlpha);

            // Quick fade-in (first 15%) then a smooth fade-out.
            float start = Time.unscaledTime;
            while (true)
            {
                float t = (Time.unscaledTime - start) / duration;
                if (t >= 1f) break;
                float a = t < 0.15f
                    ? Mathf.Lerp(0f, peakAlpha, t / 0.15f)
                    : Mathf.Lerp(peakAlpha, 0f, (t - 0.15f) / 0.85f);
                var c = img.color; c.a = a; img.color = c;
                yield return null;
            }
            Object.Destroy(go);
        }

        // ──────────────────────────────────────────────────────────────────
        // Dust + debris particle burst at the impact point.
        // ──────────────────────────────────────────────────────────────────
        public static void SpawnDust(Vector3 worldPos)
        {
            var go = new GameObject("AccidentDust");
            go.transform.position = worldPos + Vector3.up * 0.6f;
            var ps = go.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 0.6f;
            main.loop = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.9f, 1.5f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 5.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.6f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.useUnscaledTime = true; // play at real speed even during slow-mo
            main.gravityModifier = 0.15f;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)45) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.35f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(
                new[] {
                    new GradientColorKey(new Color(0.82f, 0.74f, 0.55f), 0f),
                    new GradientColorKey(new Color(0.55f, 0.5f, 0.42f), 1f),
                },
                new[] {
                    new GradientAlphaKey(0.85f, 0f),
                    new GradientAlphaKey(0.6f, 0.5f),
                    new GradientAlphaKey(0f, 1f),
                });
            col.color = new ParticleSystem.MinMaxGradient(grad);

            var size = ps.sizeOverLifetime;
            size.enabled = true;
            var curve = new AnimationCurve(
                new Keyframe(0f, 0.6f), new Keyframe(0.4f, 1.0f), new Keyframe(1f, 1.4f));
            size.size = new ParticleSystem.MinMaxCurve(1f, curve);

            var psr = go.GetComponent<ParticleSystemRenderer>();
            psr.material = new Material(Shader.Find("Sprites/Default"));

            ps.Play();
            Object.Destroy(go, 3.5f);
        }

        // ──────────────────────────────────────────────────────────────────
        // Two thin black skid marks behind the impact car's wheels.
        // ──────────────────────────────────────────────────────────────────
        public static void PaintSkidMarks(CarMover car, float length = 9f)
        {
            if (car == null) return;
            Vector3 carPos = car.transform.position;

            // Direction AWAY from the car's nose — that's the trail behind it.
            Vector3 awayFromNose = car.DriveWest ? Vector3.right : Vector3.left;

            Material black = new Material(Shader.Find("Unlit/Color"));
            black.color = new Color(0.04f, 0.04f, 0.04f, 1f);

            for (int side = -1; side <= 1; side += 2)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = "SkidMark";
                Object.Destroy(go.GetComponent<Collider>());

                // After 90° X rotation the quad lies flat with its local X → world X
                // and local Y → world -Z. Scaling (length, 0.18, 1) puts the long
                // edge along the road and a thin tire-width along Z.
                go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                go.transform.localScale = new Vector3(length, 0.18f, 1f);

                // Center the quad halfway between the car's current X and the
                // point it started from, so the trail extends behind the car.
                Vector3 lateral = new Vector3(0f, 0f, side * 0.75f);
                go.transform.position =
                    carPos + awayFromNose * (length * 0.5f) + lateral + Vector3.up * 0.015f;

                go.GetComponent<MeshRenderer>().material = black;
                Object.Destroy(go, 8f);
            }
        }
    }
}
