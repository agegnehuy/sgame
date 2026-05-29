using System.Collections;
using UnityEngine;

namespace SGame.Gameplay
{
    public class TrafficLightController : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private float redDurationSeconds = 5f;
        [SerializeField] private float greenDurationSeconds = 4f;
        [SerializeField] private bool autoStart = true;

        [Header("Optional visual renderers")]
        [SerializeField] private Renderer redLightRenderer;
        [SerializeField] private Renderer greenLightRenderer;
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color inactiveColor = Color.black;

        private Coroutine _loopCoroutine;
        private TrafficSignalState _currentState = TrafficSignalState.Red;

        private void Start()
        {
            if (autoStart)
            {
                StartLoop();
            }
        }

        public void StartLoop()
        {
            if (_loopCoroutine != null)
            {
                StopCoroutine(_loopCoroutine);
            }

            _loopCoroutine = StartCoroutine(RunLoop());
        }

        public void StopLoop()
        {
            if (_loopCoroutine != null)
            {
                StopCoroutine(_loopCoroutine);
                _loopCoroutine = null;
            }
        }

        public void ConfigureTimings(float redSeconds, float greenSeconds, bool restartLoop = true)
        {
            redDurationSeconds = Mathf.Max(0.5f, redSeconds);
            greenDurationSeconds = Mathf.Max(0.5f, greenSeconds);

            if (restartLoop && autoStart)
            {
                StartLoop();
            }
        }

        private IEnumerator RunLoop()
        {
            while (true)
            {
                SetSignalState(TrafficSignalState.Red);
                yield return new WaitForSeconds(redDurationSeconds);

                SetSignalState(TrafficSignalState.Green);
                yield return new WaitForSeconds(greenDurationSeconds);
            }
        }

        private void SetSignalState(TrafficSignalState state)
        {
            _currentState = state;
            missionController?.SetSignalState(_currentState == TrafficSignalState.Green);
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (redLightRenderer != null)
            {
                redLightRenderer.material.color = _currentState == TrafficSignalState.Red ? activeColor : inactiveColor;
            }

            if (greenLightRenderer != null)
            {
                greenLightRenderer.material.color = _currentState == TrafficSignalState.Green ? activeColor : inactiveColor;
            }
        }
    }
}
