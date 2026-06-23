using SGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SGame.UI
{
    /// <summary>
    /// Drives the LoginScene UI: switches between Sign In / Sign Up tabs,
    /// validates input, calls into <see cref="UserSession"/>, and on success
    /// loads the gameplay scene.
    ///
    /// All UI references are wired via <see cref="Configure"/> by
    /// LoginSceneBuilder so the scene is fully procedural — no prefabs or
    /// inspector wiring required.
    /// </summary>
    public class LoginController : MonoBehaviour
    {
        // The scene to load on a successful login. Kept overridable in case
        // we ever want to route to a "Choose Mission" hub between login and
        // the actual mission.
        [SerializeField] private string gameSceneName = "P1_M1_Walk";

        // ── Wired by the scene builder ─────────────────────────────────────
        [Header("Tabs")]
        [SerializeField] private Button signInTab;
        [SerializeField] private Button signUpTab;
        [SerializeField] private Image signInTabUnderline;
        [SerializeField] private Image signUpTabUnderline;

        [Header("Sign In fields")]
        [SerializeField] private InputField signInEmail;
        [SerializeField] private InputField signInPassword;
        [SerializeField] private Button signInSubmit;

        [Header("Sign Up fields")]
        [SerializeField] private InputField signUpName;
        [SerializeField] private InputField signUpEmail;
        [SerializeField] private InputField signUpPassword;
        [SerializeField] private InputField signUpConfirm;
        [SerializeField] private Button signUpSubmit;

        [Header("Shared")]
        [SerializeField] private GameObject signInPanel;
        [SerializeField] private GameObject signUpPanel;
        [SerializeField] private Text statusText;
        [SerializeField] private Button guestButton;

        // Stitch accent colors (re-used from the rest of the game).
        private static readonly Color AccentColor = new Color(0.95f, 0.55f, 0.18f, 1f);
        private static readonly Color MutedColor  = new Color(0.55f, 0.62f, 0.72f, 1f);
        private static readonly Color SuccessColor = new Color(0.31f, 0.80f, 0.38f, 1f);
        private static readonly Color ErrorColor   = new Color(0.95f, 0.30f, 0.30f, 1f);

        public void Configure(
            Button signIn, Button signUp, Image signInUnderline, Image signUpUnderline,
            InputField inEmail, InputField inPassword, Button inSubmit,
            InputField upName, InputField upEmail, InputField upPassword, InputField upConfirm, Button upSubmit,
            GameObject inPanel, GameObject upPanel, Text status, Button guest, string sceneToLoad)
        {
            signInTab = signIn; signUpTab = signUp;
            signInTabUnderline = signInUnderline; signUpTabUnderline = signUpUnderline;
            signInEmail = inEmail; signInPassword = inPassword; signInSubmit = inSubmit;
            signUpName = upName; signUpEmail = upEmail;
            signUpPassword = upPassword; signUpConfirm = upConfirm; signUpSubmit = upSubmit;
            signInPanel = inPanel; signUpPanel = upPanel;
            statusText = status; guestButton = guest;
            if (!string.IsNullOrEmpty(sceneToLoad)) gameSceneName = sceneToLoad;
        }

        private void Start()
        {
            // If the user is already signed in (returning visit), skip the
            // login screen entirely. Comment this block out when iterating
            // on the login UI to keep it reachable.
            if (UserSession.IsLoggedIn)
            {
                EnterGame();
                return;
            }

            ShowTab(signIn: true);
            SetStatus("", neutral: true);

            if (signInTab != null) signInTab.onClick.AddListener(() => ShowTab(signIn: true));
            if (signUpTab != null) signUpTab.onClick.AddListener(() => ShowTab(signIn: false));
            if (signInSubmit != null) signInSubmit.onClick.AddListener(OnSignInPressed);
            if (signUpSubmit != null) signUpSubmit.onClick.AddListener(OnSignUpPressed);
            if (guestButton != null) guestButton.onClick.AddListener(OnGuestPressed);
        }

        private void ShowTab(bool signIn)
        {
            if (signInPanel != null) signInPanel.SetActive(signIn);
            if (signUpPanel != null) signUpPanel.SetActive(!signIn);
            if (signInTabUnderline != null)
                signInTabUnderline.color = signIn ? AccentColor : new Color(0, 0, 0, 0);
            if (signUpTabUnderline != null)
                signUpTabUnderline.color = signIn ? new Color(0, 0, 0, 0) : AccentColor;
            SetStatus("", neutral: true);
        }

        private void OnSignInPressed()
        {
            var result = UserSession.Login(
                signInEmail != null ? signInEmail.text : "",
                signInPassword != null ? signInPassword.text : "");
            HandleAuthResult(result, "Welcome back!");
        }

        private void OnSignUpPressed()
        {
            string name = signUpName != null ? signUpName.text : "";
            string email = signUpEmail != null ? signUpEmail.text : "";
            string pass = signUpPassword != null ? signUpPassword.text : "";
            string confirm = signUpConfirm != null ? signUpConfirm.text : "";

            if (pass != confirm)
            {
                SetStatus("Passwords don't match.", neutral: false);
                return;
            }

            var result = UserSession.Register(name, email, pass);
            HandleAuthResult(result, $"Welcome, {name.Trim()}!");
        }

        private void OnGuestPressed()
        {
            UserSession.LoginAsGuest();
            EnterGame();
        }

        private void HandleAuthResult(UserSession.AuthResult result, string successMessage)
        {
            switch (result)
            {
                case UserSession.AuthResult.Ok:
                    SetStatus(successMessage, neutral: true, success: true);
                    Invoke(nameof(EnterGame), 0.4f);
                    return;
                case UserSession.AuthResult.MissingFields:
                    SetStatus("Please fill in every field.", neutral: false); return;
                case UserSession.AuthResult.BadEmail:
                    SetStatus("That doesn't look like an email address.", neutral: false); return;
                case UserSession.AuthResult.WeakPassword:
                    SetStatus("Password must be at least 6 characters.", neutral: false); return;
                case UserSession.AuthResult.EmailTaken:
                    SetStatus("An account with that email already exists. Sign in instead.", neutral: false); return;
                case UserSession.AuthResult.InvalidCredentials:
                    SetStatus("Email or password is incorrect.", neutral: false); return;
                default:
                    SetStatus("Something went wrong. Try again.", neutral: false); return;
            }
        }

        private void SetStatus(string text, bool neutral, bool success = false)
        {
            if (statusText == null) return;
            statusText.text = text;
            statusText.color = neutral ? (success ? SuccessColor : MutedColor) : ErrorColor;
        }

        private void EnterGame()
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
