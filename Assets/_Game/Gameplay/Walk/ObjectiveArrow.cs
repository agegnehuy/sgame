using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// A floating 3D arrow that hovers above the player and points to the
    /// current objective (school front door → school back exit → home).
    /// Automatically picks the next target from a sequence based on the
    /// round-trip state.
    /// </summary>
    public class ObjectiveArrow : MonoBehaviour
    {
        [SerializeField] private Transform follow;        // hover above this transform
        [SerializeField] private Transform arrowMesh;     // child that gets rotated to point at target
        [SerializeField] private float hoverHeight = 2.6f;
        [SerializeField] private float hoverBobAmp = 0.18f;
        [SerializeField] private float hoverBobFreq = 1.8f;
        [SerializeField] private float spinDegPerSec = 30f;
        [SerializeField] private float fadeDistance = 6f; // hide when player is this close to target

        private Transform _target;
        private MeshRenderer[] _renderers;
        private float _bobPhase;
        private float _alpha = 1f;

        public void Configure(Transform followTarget, Transform arrow)
        {
            follow = followTarget;
            arrowMesh = arrow;
            _bobPhase = Random.value * 6.28f;
            CacheRenderers();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            // Show whenever the target changes.
            if (gameObject.activeSelf == false) gameObject.SetActive(true);
        }

        public void Hide()
        {
            _target = null;
            gameObject.SetActive(false);
        }

        private void CacheRenderers()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>(true);
        }

        private void Update()
        {
            if (follow == null) { gameObject.SetActive(false); return; }
            if (_target == null)
            {
                FadeOut();
                return;
            }

            // Hover above the player with a gentle sine bob.
            _bobPhase += Time.deltaTime * hoverBobFreq * Mathf.PI * 2f;
            float bob = Mathf.Sin(_bobPhase) * hoverBobAmp;
            transform.position = follow.position + Vector3.up * (hoverHeight + bob);

            // Rotate the arrow so it points horizontally at the target.
            Vector3 toTarget = _target.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.0001f && arrowMesh != null)
            {
                Quaternion look = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                arrowMesh.rotation = Quaternion.Slerp(arrowMesh.rotation, look,
                    1f - Mathf.Exp(-8f * Time.deltaTime));
            }

            // Subtle yaw spin layered on top to make it eye-catching.
            transform.Rotate(0f, spinDegPerSec * Time.deltaTime, 0f, Space.World);

            // Fade out as the player approaches the target.
            float dist = Mathf.Sqrt(toTarget.sqrMagnitude);
            float targetAlpha = Mathf.Clamp01((dist - fadeDistance * 0.5f) / fadeDistance);
            _alpha = Mathf.Lerp(_alpha, targetAlpha, 1f - Mathf.Exp(-5f * Time.deltaTime));
            ApplyAlpha(_alpha);
        }

        private void FadeOut()
        {
            _alpha = Mathf.Lerp(_alpha, 0f, 1f - Mathf.Exp(-6f * Time.deltaTime));
            ApplyAlpha(_alpha);
            if (_alpha < 0.01f) gameObject.SetActive(false);
        }

        private void ApplyAlpha(float a)
        {
            if (_renderers == null) return;
            for (int i = 0; i < _renderers.Length; i++)
            {
                var r = _renderers[i];
                if (r == null || r.material == null) continue;
                if (r.material.HasProperty("_Color"))
                {
                    var c = r.material.GetColor("_Color");
                    c.a = a;
                    r.material.SetColor("_Color", c);
                }
                if (r.material.HasProperty("_BaseColor"))
                {
                    var c = r.material.GetColor("_BaseColor");
                    c.a = a;
                    r.material.SetColor("_BaseColor", c);
                }
            }
        }
    }
}
