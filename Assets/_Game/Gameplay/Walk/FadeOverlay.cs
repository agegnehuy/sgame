using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Full-screen black fade overlay used to mask teleports and milestone
    /// transitions (entering / exiting the school, arriving home). Procedurally
    /// builds a screen-space Canvas + Image on demand so callers don't have to
    /// wire anything up.
    /// </summary>
    public class FadeOverlay : MonoBehaviour
    {
        private static FadeOverlay _instance;
        private Image _image;
        private Coroutine _running;

        public static FadeOverlay Get()
        {
            if (_instance != null) return _instance;

            var go = new GameObject("FadeOverlay");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 8500;  // above the HUD but below the red flash
            go.AddComponent<CanvasScaler>();

            var imgGo = new GameObject("Image");
            imgGo.transform.SetParent(go.transform, false);
            var rt = imgGo.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var img = imgGo.AddComponent<Image>();
            img.raycastTarget = false;
            img.color = new Color(0f, 0f, 0f, 0f);

            _instance = go.AddComponent<FadeOverlay>();
            _instance._image = img;
            DontDestroyOnLoad(go);
            return _instance;
        }

        /// <summary>Fade from current alpha → target alpha over real-time seconds.</summary>
        public IEnumerator FadeTo(float targetAlpha, float seconds)
        {
            if (_image == null) yield break;
            Color start = _image.color;
            Color end = new Color(0f, 0f, 0f, Mathf.Clamp01(targetAlpha));
            float dur = Mathf.Max(0.001f, seconds);
            float t0 = Time.unscaledTime;
            while (true)
            {
                float t = (Time.unscaledTime - t0) / dur;
                if (t >= 1f) break;
                float eased = Mathf.SmoothStep(0f, 1f, t);
                _image.color = Color.Lerp(start, end, eased);
                yield return null;
            }
            _image.color = end;
        }

        public IEnumerator Hold(float seconds)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, seconds));
        }

        /// <summary>Force the overlay to a specific alpha immediately.</summary>
        public void SetAlpha(float a)
        {
            if (_image == null) return;
            var c = _image.color;
            c.a = Mathf.Clamp01(a);
            _image.color = c;
        }

        /// <summary>True if the overlay is currently fully opaque (mid-transition).</summary>
        public bool IsBlack => _image != null && _image.color.a >= 0.99f;
    }
}
