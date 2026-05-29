using SGame.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Mobile-friendly HUD for the walk mission. Buttons are always active:
    /// WAIT stops the kid wherever she is; CROSS resumes auto-walking. If the
    /// kid steps onto the road while the cars' light is green an accident
    /// fires and the failure overlay is shown with a RETRY button.
    /// </summary>
    public class WalkHUD : MonoBehaviour
    {
        // External references
        [SerializeField] private MissionController missionController;
        [SerializeField] private TrafficLight3D trafficLight;
        [SerializeField] private CrosswalkZone crosswalkZone;
        [SerializeField] private SchoolGoal schoolGoal;

        // Cached mission result — shown only after the kid physically reaches school.
        private bool _hasPendingResult;
        private int _pendingScore;
        private int _pendingStars;
        private int _pendingCoins;
        private int _pendingTotalCoins;

        // Coins picked up this run (not yet saved to wallet).
        private int _coinsThisRun;
        private int _walletAtStart;

        // Top bar
        [SerializeField] private Text titleText;
        [SerializeField] private Text coinsText;

        // Mission card status
        [SerializeField] private Text statusText;

        // 3-light traffic light widget (Stitch style)
        [SerializeField] private Image redLightImage;
        [SerializeField] private Image yellowLightImage;
        [SerializeField] private Image greenLightImage;

        // Action buttons
        [SerializeField] private Button crossButton;
        [SerializeField] private Button waitButton;

        // Result overlay (success)
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text resultTitle;
        [SerializeField] private Text resultScore;
        [SerializeField] private Text resultStars;
        [SerializeField] private Text resultCoins;

        // Failure overlay
        [SerializeField] private GameObject failurePanel;
        [SerializeField] private Text failureTitle;
        [SerializeField] private Text failureMessage;
        [SerializeField] private Button retryButton;

        // Stitch traffic-light bulb colors
        private static readonly Color BulbRedOn      = new Color(0.95f, 0.18f, 0.18f);
        private static readonly Color BulbRedOff     = new Color(0.30f, 0.08f, 0.08f);
        private static readonly Color BulbYellowOff  = new Color(0.30f, 0.25f, 0.05f);
        private static readonly Color BulbGreenOn    = new Color(0.31f, 0.80f, 0.38f);
        private static readonly Color BulbGreenOff   = new Color(0.10f, 0.28f, 0.12f);

        public void Configure(
            MissionController controller, TrafficLight3D light, CrosswalkZone zone, SchoolGoal goal,
            Text title, Text coins, Text status,
            Image redLight, Image yellowLight, Image greenLight,
            Button cross, Button wait,
            GameObject results, Text rTitle, Text rScore, Text rStars, Text rCoins,
            GameObject failure, Text fTitle, Text fMessage, Button retry)
        {
            missionController = controller;
            trafficLight = light;
            crosswalkZone = zone;
            schoolGoal = goal;
            titleText = title;
            coinsText = coins;
            statusText = status;
            redLightImage = redLight;
            yellowLightImage = yellowLight;
            greenLightImage = greenLight;
            crossButton = cross;
            waitButton = wait;
            resultPanel = results;
            resultTitle = rTitle;
            resultScore = rScore;
            resultStars = rStars;
            resultCoins = rCoins;
            failurePanel = failure;
            failureTitle = fTitle;
            failureMessage = fMessage;
            retryButton = retry;

            if (resultPanel != null) resultPanel.SetActive(false);
            if (failurePanel != null) failurePanel.SetActive(false);
        }

        private void OnEnable()
        {
            if (crossButton != null)
            {
                crossButton.onClick.RemoveListener(OnCrossClicked);
                crossButton.onClick.AddListener(OnCrossClicked);
            }
            if (waitButton != null)
            {
                waitButton.onClick.RemoveListener(OnWaitClicked);
                waitButton.onClick.AddListener(OnWaitClicked);
            }
            if (retryButton != null)
            {
                retryButton.onClick.RemoveListener(OnRetryClicked);
                retryButton.onClick.AddListener(OnRetryClicked);
            }

            if (missionController != null)
            {
                missionController.MissionCompleted += OnMissionCompleted;
            }
            if (crosswalkZone != null)
            {
                crosswalkZone.MissionFailed += OnMissionFailedFromCrosswalk;
            }
            if (schoolGoal != null)
            {
                schoolGoal.GoalReached += OnSchoolReached;
            }
            Coin.Collected += OnCoinCollected;
        }

        private void OnDisable()
        {
            if (crossButton != null) crossButton.onClick.RemoveListener(OnCrossClicked);
            if (waitButton != null) waitButton.onClick.RemoveListener(OnWaitClicked);
            if (retryButton != null) retryButton.onClick.RemoveListener(OnRetryClicked);

            if (missionController != null)
            {
                missionController.MissionCompleted -= OnMissionCompleted;
            }
            if (crosswalkZone != null)
            {
                crosswalkZone.MissionFailed -= OnMissionFailedFromCrosswalk;
            }
            if (schoolGoal != null)
            {
                schoolGoal.GoalReached -= OnSchoolReached;
            }
            Coin.Collected -= OnCoinCollected;
        }

        private void Start()
        {
            // titleText is the static "MISSION" header on the card — leave it.
            _walletAtStart = SaveService.GetCoins(PlayerSession.ActiveProfileId);
            _coinsThisRun = 0;
            UpdateCoinsLabel();
        }

        private void Update()
        {
            if (missionController == null) return;

            // CARS-light semantics: IsGreen=true → cars are GOING, peds wait.
            bool carsGreen = trafficLight != null && trafficLight.IsGreen;

            if (redLightImage != null)
                redLightImage.color = carsGreen ? BulbRedOff : BulbRedOn;
            if (yellowLightImage != null)
                yellowLightImage.color = BulbYellowOff;
            if (greenLightImage != null)
                greenLightImage.color = carsGreen ? BulbGreenOn : BulbGreenOff;

            if (statusText != null && !(failurePanel != null && failurePanel.activeSelf)
                                    && !(resultPanel != null && resultPanel.activeSelf))
            {
                if (crosswalkZone != null && crosswalkZone.FarSideReached)
                {
                    statusText.text = "Safe! Walk to school.";
                }
                else
                {
                    statusText.text = carsGreen
                        ? "GREEN light. Cars going — tap WAIT!"
                        : "RED light. Cars stopped. Cross safely.";
                }
            }
        }

        private void UpdateCoinsLabel()
        {
            if (coinsText == null) return;
            // Show the wallet at scene start + coins picked up this run, but
            // the picked-up portion is only persisted on a successful win.
            coinsText.text = (_walletAtStart + _coinsThisRun).ToString();
        }

        private void OnCoinCollected(int value)
        {
            _coinsThisRun += value;
            UpdateCoinsLabel();
        }

        private void OnCrossClicked()
        {
            Debug.Log("[WalkHUD] CROSS tapped.");
            if (crosswalkZone != null) crosswalkZone.RequestCross();
        }

        private void OnWaitClicked()
        {
            Debug.Log("[WalkHUD] WAIT tapped.");
            if (crosswalkZone != null) crosswalkZone.RequestWait();
        }

        private void OnRetryClicked()
        {
            Debug.Log("[WalkHUD] RETRY tapped. Reloading scene.");
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }

        private void OnMissionCompleted(int score, int stars, int coins, int totalCoins)
        {
            // The kid has crossed safely. Cache the result but DO NOT show the panel
            // yet — wait for her to physically reach the school door.
            _hasPendingResult = true;
            _pendingScore = score;
            _pendingStars = stars;
            _pendingCoins = coins;
            _pendingTotalCoins = totalCoins;
        }

        private void OnSchoolReached()
        {
            if (!_hasPendingResult)
            {
                _pendingScore = 100;
                _pendingStars = 3;
                _pendingCoins = 0;
                _pendingTotalCoins = SaveService.GetCoins(PlayerSession.ActiveProfileId);
            }

            // Commit the picked-up coins to the wallet now that the run succeeded.
            int earnedThisRun = _coinsThisRun + _pendingCoins;
            int newTotal = _pendingTotalCoins;
            if (earnedThisRun > 0)
            {
                newTotal = SaveService.AddCoins(PlayerSession.ActiveProfileId, _coinsThisRun);
                // _pendingCoins were already added by MissionController.CompleteMission,
                // so we only add the picked-up coins here. newTotal now reflects both.
            }
            _walletAtStart = newTotal - _coinsThisRun;
            _coinsThisRun = 0;
            UpdateCoinsLabel();

            if (resultPanel != null) resultPanel.SetActive(true);
            if (resultTitle != null)
            {
                resultTitle.text = _pendingStars >= 2
                    ? "You crossed Bole Road\nlike a pro!"
                    : "You reached school!";
            }
            if (resultScore != null) resultScore.text = $"{_pendingScore}%";
            if (resultCoins != null) resultCoins.text = $"+{earnedThisRun}";
            if (resultStars != null) resultStars.text = "★";
        }

        private void OnMissionFailedFromCrosswalk()
        {
            int lost = _coinsThisRun;
            _coinsThisRun = 0;
            UpdateCoinsLabel();

            if (failurePanel != null) failurePanel.SetActive(true);
            if (failureTitle != null) failureTitle.text = "Accident!";
            if (failureMessage != null)
            {
                string lostLine = lost > 0
                    ? $"\nYou lost the {lost} coins you picked up!"
                    : "";
                failureMessage.text = "You were on the zebra when the cars had a GREEN light." + lostLine +
                                      "\nWait for RED, then cross safely.";
            }
            if (statusText != null) statusText.text = "Accident! Tap RETRY to try again.";
        }
    }
}
