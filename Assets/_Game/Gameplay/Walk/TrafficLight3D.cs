using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Cycles red ↔ green on a fixed timer. This is the standard real-world
    /// CARS' traffic light, with three separate bulbs:
    ///   IsGreen == true  → cars GO, peds must wait (unsafe to cross).
    ///   IsGreen == false → cars STOP, peds may cross (safe).
    /// The state is INVERTED before being forwarded to
    /// MissionController.SetSignalState because the SafetyEvaluator uses
    /// "green = ped may cross" semantics.
    /// </summary>
    public class TrafficLight3D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MissionController missionController;
        [Tooltip("Legacy single-bulb renderer (used as a fallback / mirror).")]
        [SerializeField] private MeshRenderer headRenderer;
        [SerializeField] private MeshRenderer redBulb;
        [SerializeField] private MeshRenderer yellowBulb;
        [SerializeField] private MeshRenderer greenBulb;

        [Header("Colors (HDR for emission)")]
        [SerializeField] private Color redColor = new Color(1f, 0.1f, 0.1f);
        [SerializeField] private Color yellowColor = new Color(1f, 0.78f, 0.1f);
        [SerializeField] private Color greenColor = new Color(0.1f, 1f, 0.2f);
        [SerializeField] private float emissionIntensity = 3.5f;
        [Tooltip("Dim color used for the bulbs that are currently OFF.")]
        [SerializeField] private Color bulbOffColor = new Color(0.05f, 0.05f, 0.05f);

        [Header("Cycle (seconds)")]
        [Tooltip("How long the light stays GREEN (cars going, peds wait). Level 1 = generous.")]
        [SerializeField] private float greenDuration = 7f;
        [Tooltip("How long the light stays RED (cars stopped, peds may cross). Level 1 = generous.")]
        [SerializeField] private float redDuration = 8f;

        private bool _isGreen;
        private float _switchAt;
        private MaterialPropertyBlock _mpb;

        public bool IsGreen => _isGreen;

        public void Configure(MissionController controller, MeshRenderer head)
        {
            missionController = controller;
            headRenderer = head;
        }

        public void ConfigureBulbs(MeshRenderer red, MeshRenderer yellow, MeshRenderer green)
        {
            redBulb = red;
            yellowBulb = yellow;
            greenBulb = green;
        }

        private void Start()
        {
            _mpb = new MaterialPropertyBlock();
            // Start RED (cars stopped, peds may cross) so the kid's first
            // crossing window — she auto-walks from the home end — is safe.
            SetGreen(false, immediate: true);
        }

        private void Update()
        {
            if (Time.time >= _switchAt)
            {
                SetGreen(!_isGreen, immediate: false);
            }
        }

        private void SetGreen(bool green, bool immediate)
        {
            _isGreen = green;
            _switchAt = Time.time + (green ? greenDuration : redDuration);

            // Drive each bulb individually for a real-traffic-light look.
            PaintBulb(redBulb,    !green ? redColor    : (Color?)null);
            PaintBulb(yellowBulb, null);                                  // yellow unused in MVP
            PaintBulb(greenBulb,   green ? greenColor  : (Color?)null);

            // Legacy single-renderer fallback (for old scene wiring).
            if (headRenderer != null)
            {
                Color c = green ? greenColor : redColor;
                headRenderer.GetPropertyBlock(_mpb);
                _mpb.SetColor("_BaseColor", c);
                _mpb.SetColor("_Color", c);
                _mpb.SetColor("_EmissionColor", c * emissionIntensity);
                headRenderer.SetPropertyBlock(_mpb);
            }

            if (missionController != null)
            {
                // SafetyEvaluator uses "green = ped may cross"; this light is
                // the CARS' light, so invert.
                missionController.SetSignalState(!green);
            }
        }

        // Paints a bulb either lit (with `litColor` glowing via emission) or
        // dark (matte off-black). Pass null for `litColor` to mean "off".
        private void PaintBulb(MeshRenderer mr, Color? litColor)
        {
            if (mr == null) return;
            mr.GetPropertyBlock(_mpb);
            if (litColor.HasValue)
            {
                Color c = litColor.Value;
                _mpb.SetColor("_BaseColor", c);
                _mpb.SetColor("_Color", c);
                _mpb.SetColor("_EmissionColor", c * emissionIntensity);
            }
            else
            {
                _mpb.SetColor("_BaseColor", bulbOffColor);
                _mpb.SetColor("_Color", bulbOffColor);
                _mpb.SetColor("_EmissionColor", Color.black);
            }
            mr.SetPropertyBlock(_mpb);
        }
    }
}
