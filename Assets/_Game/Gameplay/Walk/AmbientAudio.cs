using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Procedural ambient soundscape — built entirely at runtime so nothing
    /// has to be imported into the project. Adds:
    ///   • A low, looping city rumble (traffic + wind base layer)
    ///   • Random distant car horns every 12-25 s
    ///   • Random distant chatter "blips" every 8-15 s
    ///   • Footstep clicks tied to the kid's run cadence
    /// All synthesised from white noise + sine tones. Volume is tame so it
    /// reads as background life rather than a soundtrack.
    /// </summary>
    [DefaultExecutionOrder(50)]
    public class AmbientAudio : MonoBehaviour
    {
        [SerializeField] private KidPlayer kid;
        [SerializeField] private float baseVolume = 0.30f;
        [SerializeField] private float hornVolume = 0.35f;
        [SerializeField] private float chatterVolume = 0.18f;
        [SerializeField] private float footVolume = 0.25f;

        private AudioSource _baseSrc;
        private AudioSource _eventSrc;
        private AudioSource _footSrc;
        private AudioClip _cityLoop;
        private AudioClip _hornClip;
        private AudioClip _chatterClip;
        private AudioClip _footClip;

        private float _nextHornAt;
        private float _nextChatterAt;
        private float _footPhase;

        public void Configure(KidPlayer kidPlayer) { kid = kidPlayer; }

        private void Awake()
        {
            _cityLoop    = BuildCityLoop();
            _hornClip    = BuildHornClip();
            _chatterClip = BuildChatterClip();
            _footClip    = BuildFootstepClip();

            _baseSrc = gameObject.AddComponent<AudioSource>();
            _baseSrc.clip = _cityLoop;
            _baseSrc.loop = true;
            _baseSrc.volume = baseVolume;
            _baseSrc.spatialBlend = 0f;
            _baseSrc.priority = 64;
            _baseSrc.playOnAwake = false;

            var eventGo = new GameObject("AmbientEvents");
            eventGo.transform.SetParent(transform, false);
            _eventSrc = eventGo.AddComponent<AudioSource>();
            _eventSrc.spatialBlend = 0f;
            _eventSrc.volume = 1f;
            _eventSrc.priority = 96;
            _eventSrc.playOnAwake = false;

            var footGo = new GameObject("Footsteps");
            footGo.transform.SetParent(transform, false);
            _footSrc = footGo.AddComponent<AudioSource>();
            _footSrc.spatialBlend = 0f;
            _footSrc.volume = footVolume;
            _footSrc.priority = 80;
            _footSrc.playOnAwake = false;
        }

        private void Start()
        {
            if (_baseSrc != null) _baseSrc.Play();
            _nextHornAt = Time.unscaledTime + Random.Range(5f, 14f);
            _nextChatterAt = Time.unscaledTime + Random.Range(3f, 10f);
        }

        private void Update()
        {
            float now = Time.unscaledTime;

            if (now >= _nextHornAt && _eventSrc != null && _hornClip != null)
            {
                _eventSrc.pitch = Random.Range(0.85f, 1.15f);
                _eventSrc.PlayOneShot(_hornClip, hornVolume);
                _nextHornAt = now + Random.Range(12f, 25f);
            }
            if (now >= _nextChatterAt && _eventSrc != null && _chatterClip != null)
            {
                _eventSrc.pitch = Random.Range(0.92f, 1.08f);
                _eventSrc.PlayOneShot(_chatterClip, chatterVolume);
                _nextChatterAt = now + Random.Range(8f, 15f);
            }

            // Footsteps — phase advances with the kid's current speed; we fire
            // a click each time the phase crosses an integer (i.e., once per
            // stride). No clip if she's not really moving.
            if (kid != null && _footClip != null && _footSrc != null)
            {
                float speedT = Mathf.Clamp01(kid.CurrentSpeed / Mathf.Max(0.01f, kid.MaxSpeed));
                if (speedT > 0.15f)
                {
                    // ~2.4 strides/sec at full run.
                    float stepsPerSec = Mathf.Lerp(1.2f, 2.6f, speedT);
                    float prev = _footPhase;
                    _footPhase += Time.deltaTime * stepsPerSec;
                    if (Mathf.FloorToInt(_footPhase) != Mathf.FloorToInt(prev))
                    {
                        _footSrc.pitch = Random.Range(0.92f, 1.10f);
                        _footSrc.PlayOneShot(_footClip, footVolume * (0.4f + 0.6f * speedT));
                    }
                }
                else
                {
                    _footPhase = 0f;
                }
            }
        }

        // ── Procedural clips ──────────────────────────────────────────────

        // 6-second seamless city rumble = filtered noise + a couple of low
        // tones that slowly drift. Gentle enough to sit under everything.
        private static AudioClip BuildCityLoop()
        {
            const int rate = 22050;
            const float seconds = 6.0f;
            int len = Mathf.RoundToInt(rate * seconds);
            var samples = new float[len];
            var rng = new System.Random(11);
            float lp1 = 0f, lp2 = 0f;
            for (int i = 0; i < len; i++)
            {
                float t = (float)i / rate;
                // Two slow sines around 50 Hz / 75 Hz for a low rumble.
                float bass = Mathf.Sin(t * 2f * Mathf.PI * 50f) * 0.22f
                           + Mathf.Sin(t * 2f * Mathf.PI * 75.3f) * 0.18f;
                // Pink-ish noise via two stacked low-pass filters.
                float n = (float)(rng.NextDouble() * 2.0 - 1.0);
                lp1 += (n - lp1) * 0.08f;
                lp2 += (lp1 - lp2) * 0.30f;
                float mix = bass * 0.6f + lp2 * 1.6f;
                // Cross-fade the last 0.6s with the first 0.6s so the loop is seamless.
                float fade = 0.6f;
                if (t < fade)     mix *= Mathf.SmoothStep(0.7f, 1f, t / fade);
                if (seconds - t < fade) mix *= Mathf.SmoothStep(0.7f, 1f, (seconds - t) / fade);
                samples[i] = Mathf.Clamp(mix * 0.45f, -0.95f, 0.95f);
            }
            var clip = AudioClip.Create("CityLoop", len, 1, rate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        // 0.8 s of "honk" — a low square-ish blat with a slight pitch dip.
        private static AudioClip BuildHornClip()
        {
            const int rate = 22050;
            int len = (int)(rate * 0.8f);
            var samples = new float[len];
            float phase = 0f;
            for (int i = 0; i < len; i++)
            {
                float t = (float)i / len;
                float freq = Mathf.Lerp(210f, 180f, t);
                phase += freq / rate;
                float saw = (Mathf.Repeat(phase, 1f) - 0.5f) * 2f;
                float square = Mathf.Sign(Mathf.Sin(phase * Mathf.PI * 2f));
                float env = Mathf.Min(1f, t / 0.05f) * Mathf.Min(1f, (1f - t) / 0.15f);
                samples[i] = (saw * 0.3f + square * 0.3f) * env * 0.55f;
            }
            var clip = AudioClip.Create("Horn", len, 1, rate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        // 0.7 s of muffled chatter — bandpass-filtered noise with random
        // 80 ms "syllable" envelopes. Sounds like a distant playground.
        private static AudioClip BuildChatterClip()
        {
            const int rate = 22050;
            int len = (int)(rate * 0.7f);
            var samples = new float[len];
            var rng = new System.Random(29);
            float bp1 = 0f, bp2 = 0f;
            for (int i = 0; i < len; i++)
            {
                float t = (float)i / len;
                float syllable = 0.5f
                    + 0.5f * Mathf.Sin(t * 2f * Mathf.PI * 7f
                                       + (float)rng.NextDouble() * 0.4f);
                float n = (float)(rng.NextDouble() * 2.0 - 1.0);
                bp1 += (n - bp1) * 0.40f;
                bp2 += (bp1 - bp2) * 0.55f;
                float band = bp1 - bp2;
                float env = Mathf.Min(1f, t / 0.05f) * Mathf.Min(1f, (1f - t) / 0.10f);
                samples[i] = band * 1.8f * syllable * env * 0.5f;
            }
            var clip = AudioClip.Create("Chatter", len, 1, rate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        // 80 ms of a short "tap" — high-freq noise burst with a quick decay.
        private static AudioClip BuildFootstepClip()
        {
            const int rate = 22050;
            int len = (int)(rate * 0.08f);
            var samples = new float[len];
            var rng = new System.Random(7);
            for (int i = 0; i < len; i++)
            {
                float t = (float)i / len;
                float n = (float)(rng.NextDouble() * 2.0 - 1.0);
                float env = Mathf.Exp(-t * 18f);
                samples[i] = n * env * 0.55f;
            }
            var clip = AudioClip.Create("Footstep", len, 1, rate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
