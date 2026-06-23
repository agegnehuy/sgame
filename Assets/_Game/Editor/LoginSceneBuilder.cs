using System.Collections.Generic;
using SGame.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SGame.EditorTools
{
    /// <summary>
    /// Procedurally builds a beautiful Stitch-style login scene with two
    /// tabs (Sign In / Sign Up), a Continue as Guest link, and the
    /// "Safe Steps Addis" brand header. Mirrors the procedural-UI approach
    /// used by WalkSceneBuilder so the project keeps a single visual
    /// language without depending on any external prefabs.
    /// </summary>
    public static class LoginSceneBuilder
    {
        private const string ScenePath = "Assets/_Game/Scenes/LoginScene.unity";
        private const string GameSceneName = "P1_M1_Walk";

        // ── Stitch palette ──────────────────────────────────────────────────
        private static readonly Color BgTop      = new Color(0.07f, 0.10f, 0.14f, 1f);
        private static readonly Color BgBottom   = new Color(0.13f, 0.17f, 0.21f, 1f);
        private static readonly Color CardColor  = new Color(0.18f, 0.23f, 0.30f, 1f);
        private static readonly Color CardShadow = new Color(0f, 0f, 0f, 0.45f);
        private static readonly Color FieldBg    = new Color(0.10f, 0.14f, 0.19f, 1f);
        private static readonly Color FieldBorder = new Color(0.27f, 0.34f, 0.42f, 1f);
        private static readonly Color TextPrimary = new Color(0.95f, 0.97f, 1.00f, 1f);
        private static readonly Color TextMuted   = new Color(0.62f, 0.70f, 0.80f, 1f);
        private static readonly Color TextDim     = new Color(0.45f, 0.52f, 0.62f, 1f);
        private static readonly Color Accent      = new Color(0.95f, 0.55f, 0.18f, 1f);
        private static readonly Color AccentDeep  = new Color(0.80f, 0.42f, 0.10f, 1f);
        private static readonly Color GuestBg     = new Color(0.20f, 0.27f, 0.36f, 1f);

        [MenuItem("SGame/Build Login Scene")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            BuildEventSystem();
            BuildBackground();
            var canvas = BuildCanvas();
            BuildLoginCard(canvas);

            EditorSceneManager.MarkSceneDirty(scene);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettingsFirst(ScenePath);

            EditorUtility.DisplayDialog(
                "Login scene built",
                $"Saved to {ScenePath}\n\nIt is now the first scene in Build Settings, " +
                $"so the app boots into it. Successful sign-in / sign-up / guest " +
                $"loads '{GameSceneName}'.",
                "OK");
            Debug.Log("[LoginSceneBuilder] Done.");
        }

        // ── Scene root pieces ──────────────────────────────────────────────

        private static void BuildEventSystem()
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        private static void BuildBackground()
        {
            // A camera + a fullscreen quad behind the canvas would also work,
            // but a UI gradient is cheaper on mobile. We just put two stacked
            // canvases — the bottom one is the gradient backdrop, the top one
            // is the login card.
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = BgBottom;
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            camGo.AddComponent<AudioListener>();
        }

        private static RectTransform BuildCanvas()
        {
            var canvasGo = new GameObject("LoginCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            // Designed for a 1920×1080 landscape screen (matches the
            // P1_M1_Walk camera framing). Match mode lets it gracefully
            // adapt to taller / wider phones.
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            // Two-stop vertical gradient: dark blue → navy.
            var bg = MakePanel((RectTransform)canvasGo.transform, "Background",
                Vector2.zero, Vector2.one, BgBottom);
            var gradient = MakePanel(bg, "GradientTop",
                new Vector2(0f, 0.55f), new Vector2(1f, 1f), BgTop);
            gradient.GetComponent<Image>().raycastTarget = false;
            bg.GetComponent<Image>().raycastTarget = false;

            return (RectTransform)canvasGo.transform;
        }

        // ── The login card ─────────────────────────────────────────────────

        private static void BuildLoginCard(RectTransform canvas)
        {
            // Centered card. Width is fixed; height accommodates whichever
            // panel (Sign In or Sign Up) is currently showing.
            var card = MakePanel(canvas, "LoginCard",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), CardColor);
            card.sizeDelta = new Vector2(720f, 820f);
            ApplyRounded(card.GetComponent<Image>(), 0.5f);

            // Drop shadow behind the card.
            var shadow = MakePanel(canvas, "LoginCardShadow",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), CardShadow);
            shadow.sizeDelta = new Vector2(740f, 840f);
            shadow.anchoredPosition = new Vector2(0f, -8f);
            ApplyRounded(shadow.GetComponent<Image>(), 0.5f);
            shadow.SetSiblingIndex(card.GetSiblingIndex());

            BuildHeader(card);
            var (signInTab, signInUnderline, signUpTab, signUpUnderline) = BuildTabs(card);
            var (signInPanel, inEmail, inPass, inSubmit) = BuildSignInPanel(card);
            var (signUpPanel, upName, upEmail, upPass, upConfirm, upSubmit) = BuildSignUpPanel(card);
            var status = BuildStatus(card);
            var guest = BuildGuestButton(canvas);

            var ctrlGo = new GameObject("LoginController");
            ctrlGo.transform.SetParent(canvas, false);
            var ctrl = ctrlGo.AddComponent<LoginController>();
            ctrl.Configure(
                signInTab, signUpTab, signInUnderline, signUpUnderline,
                inEmail, inPass, inSubmit,
                upName, upEmail, upPass, upConfirm, upSubmit,
                signInPanel, signUpPanel, status, guest, GameSceneName);
        }

        private static void BuildHeader(RectTransform card)
        {
            // Brand title + subtitle.
            MakeLabel(card, "Title", "Safe Steps Addis",
                new Vector2(0f, 0.86f), new Vector2(1f, 0.94f),
                52, FontStyle.Bold, TextAnchor.MiddleCenter, TextPrimary);

            MakeLabel(card, "Subtitle", "Cross safely. Learn the rules. Get home.",
                new Vector2(0f, 0.80f), new Vector2(1f, 0.86f),
                22, FontStyle.Normal, TextAnchor.MiddleCenter, TextMuted);
        }

        private static (Button, Image, Button, Image) BuildTabs(RectTransform card)
        {
            var tabRow = MakePanel(card, "TabRow",
                new Vector2(0.08f, 0.71f), new Vector2(0.92f, 0.78f),
                new Color(0, 0, 0, 0));

            var signIn = MakeTabButton(tabRow, "SignInTab", "Sign In", isLeft: true);
            var signInLine = MakeUnderline(tabRow, "SignInUnderline", isLeft: true, active: true);
            var signUp = MakeTabButton(tabRow, "SignUpTab", "Sign Up", isLeft: false);
            var signUpLine = MakeUnderline(tabRow, "SignUpUnderline", isLeft: false, active: false);

            return (signIn, signInLine, signUp, signUpLine);
        }

        private static Button MakeTabButton(RectTransform parent, string name, string label, bool isLeft)
        {
            var rect = MakePanel(parent, name,
                new Vector2(isLeft ? 0f : 0.5f, 0f),
                new Vector2(isLeft ? 0.5f : 1f, 1f),
                new Color(0, 0, 0, 0));
            var btn = rect.gameObject.AddComponent<Button>();
            // Transparent button — visual is just the underline + label.
            rect.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            var colors = btn.colors;
            colors.normalColor = new Color(1, 1, 1, 1);
            colors.highlightedColor = new Color(1, 1, 1, 0.95f);
            colors.pressedColor = new Color(1, 1, 1, 0.85f);
            btn.colors = colors;

            MakeLabel(rect, "Label", label,
                Vector2.zero, Vector2.one,
                28, FontStyle.Bold, TextAnchor.MiddleCenter, TextPrimary);
            return btn;
        }

        private static Image MakeUnderline(RectTransform parent, string name, bool isLeft, bool active)
        {
            var u = MakeImage(parent, name,
                new Vector2(isLeft ? 0.15f : 0.65f, 0f),
                new Vector2(isLeft ? 0.35f : 0.85f, 0.04f),
                active ? Accent : new Color(0, 0, 0, 0));
            ApplyRounded(u, 0.3f);
            return u;
        }

        // ── Sign In panel ──────────────────────────────────────────────────

        private static (GameObject, InputField, InputField, Button) BuildSignInPanel(RectTransform card)
        {
            var panel = MakePanel(card, "SignInPanel",
                new Vector2(0f, 0f), new Vector2(1f, 0.71f),
                new Color(0, 0, 0, 0));
            panel.GetComponent<Image>().raycastTarget = false;

            float top = 0.85f;
            float step = 0.18f;

            var emailField = MakeInputField(panel, "Email", "Email",
                new Vector2(0.08f, top - step), new Vector2(0.92f, top - step + 0.13f),
                InputField.ContentType.EmailAddress);

            var passField = MakeInputField(panel, "Password", "Password",
                new Vector2(0.08f, top - 2 * step), new Vector2(0.92f, top - 2 * step + 0.13f),
                InputField.ContentType.Password);

            var submit = MakePrimaryButton(panel, "SignInSubmit", "SIGN IN",
                new Vector2(0.08f, top - 3 * step - 0.04f),
                new Vector2(0.92f, top - 3 * step + 0.13f - 0.04f));

            MakeLabel(panel, "ForgotLabel", "Forgot password?",
                new Vector2(0.5f, top - 3 * step - 0.14f),
                new Vector2(1.0f, top - 3 * step - 0.08f),
                18, FontStyle.Normal, TextAnchor.MiddleRight, TextDim);

            return (panel.gameObject, emailField, passField, submit);
        }

        // ── Sign Up panel ──────────────────────────────────────────────────

        private static (GameObject, InputField, InputField, InputField, InputField, Button) BuildSignUpPanel(RectTransform card)
        {
            var panel = MakePanel(card, "SignUpPanel",
                new Vector2(0f, 0f), new Vector2(1f, 0.71f),
                new Color(0, 0, 0, 0));
            panel.GetComponent<Image>().raycastTarget = false;
            panel.gameObject.SetActive(false);

            float top = 0.92f;
            float step = 0.14f;

            var name = MakeInputField(panel, "Name", "Full name",
                new Vector2(0.08f, top - step), new Vector2(0.92f, top - step + 0.1f),
                InputField.ContentType.Name);

            var email = MakeInputField(panel, "Email", "Email",
                new Vector2(0.08f, top - 2 * step), new Vector2(0.92f, top - 2 * step + 0.1f),
                InputField.ContentType.EmailAddress);

            var pass = MakeInputField(panel, "Password", "Password (6+ chars)",
                new Vector2(0.08f, top - 3 * step), new Vector2(0.92f, top - 3 * step + 0.1f),
                InputField.ContentType.Password);

            var confirm = MakeInputField(panel, "Confirm", "Confirm password",
                new Vector2(0.08f, top - 4 * step), new Vector2(0.92f, top - 4 * step + 0.1f),
                InputField.ContentType.Password);

            var submit = MakePrimaryButton(panel, "SignUpSubmit", "CREATE ACCOUNT",
                new Vector2(0.08f, top - 5 * step - 0.02f),
                new Vector2(0.92f, top - 5 * step + 0.1f - 0.02f));

            return (panel.gameObject, name, email, pass, confirm, submit);
        }

        private static Text BuildStatus(RectTransform card)
        {
            return MakeLabel(card, "Status", "",
                new Vector2(0.08f, 0.04f), new Vector2(0.92f, 0.10f),
                20, FontStyle.Normal, TextAnchor.MiddleCenter, TextMuted);
        }

        private static Button BuildGuestButton(RectTransform canvas)
        {
            var btnRect = MakePanel(canvas, "GuestButton",
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), GuestBg);
            btnRect.sizeDelta = new Vector2(360f, 64f);
            btnRect.anchoredPosition = new Vector2(0f, 56f);
            ApplyRounded(btnRect.GetComponent<Image>(), 0.3f);
            var btn = btnRect.gameObject.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.95f, 1f);
            colors.pressedColor = new Color(0.75f, 0.85f, 0.95f);
            btn.colors = colors;
            MakeLabel(btnRect, "Label", "Continue as Guest →",
                Vector2.zero, Vector2.one,
                22, FontStyle.Bold, TextAnchor.MiddleCenter, TextPrimary);
            return btn;
        }

        // ── UI primitives ──────────────────────────────────────────────────

        private static InputField MakeInputField(RectTransform parent, string name, string placeholder,
            Vector2 anchorMin, Vector2 anchorMax, InputField.ContentType contentType)
        {
            // Outer rounded background.
            var bg = MakePanel(parent, name, anchorMin, anchorMax, FieldBg);
            ApplyRounded(bg.GetComponent<Image>(), 0.35f);
            // Subtle 1-pixel rim using an inner stroke approximated by a
            // slightly lighter inset frame.
            var rim = MakePanel(bg, "Rim",
                new Vector2(0f, 0f), new Vector2(1f, 0.04f), FieldBorder);
            rim.GetComponent<Image>().raycastTarget = false;

            // Text component (the part that visibly shows the typed value).
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(bg, false);
            var textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = new Vector2(0.04f, 0f);
            textRt.anchorMax = new Vector2(0.96f, 1f);
            textRt.offsetMin = textRt.offsetMax = Vector2.zero;
            var text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.color = TextPrimary;
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleLeft;
            text.supportRichText = false;

            // Placeholder.
            var phGo = new GameObject("Placeholder");
            phGo.transform.SetParent(bg, false);
            var phRt = phGo.AddComponent<RectTransform>();
            phRt.anchorMin = new Vector2(0.04f, 0f);
            phRt.anchorMax = new Vector2(0.96f, 1f);
            phRt.offsetMin = phRt.offsetMax = Vector2.zero;
            var ph = phGo.AddComponent<Text>();
            ph.font = text.font;
            ph.fontStyle = FontStyle.Italic;
            ph.color = TextDim;
            ph.fontSize = 24;
            ph.alignment = TextAnchor.MiddleLeft;
            ph.text = placeholder;

            var field = bg.gameObject.AddComponent<InputField>();
            field.textComponent = text;
            field.placeholder = ph;
            field.contentType = contentType;
            field.targetGraphic = bg.GetComponent<Image>();
            var colors = field.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f);
            colors.pressedColor = new Color(0.92f, 0.92f, 0.92f);
            colors.selectedColor = new Color(1f, 1f, 1f);
            field.colors = colors;

            return field;
        }

        private static Button MakePrimaryButton(RectTransform parent, string name, string label,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var bg = MakePanel(parent, name, anchorMin, anchorMax, Accent);
            ApplyRounded(bg.GetComponent<Image>(), 0.3f);
            var btn = bg.gameObject.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.92f, 0.85f);
            colors.pressedColor = new Color(0.92f, 0.82f, 0.75f);
            colors.selectedColor = Color.white;
            btn.colors = colors;
            MakeLabel(bg, "Label", label,
                Vector2.zero, Vector2.one,
                28, FontStyle.Bold, TextAnchor.MiddleCenter, TextPrimary);
            return btn;
        }

        // ── Low-level helpers (mirrors WalkSceneBuilder for visual consistency) ──

        private static RectTransform MakePanel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            return rt;
        }

        private static RectTransform MakePanel(RectTransform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            return MakePanel((Transform)parent, name, anchorMin, anchorMax, color);
        }

        private static Image MakeImage(RectTransform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var rt = MakePanel(parent, name, anchorMin, anchorMax, color);
            return rt.GetComponent<Image>();
        }

        private static Text MakeLabel(RectTransform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax, int fontSize, FontStyle style,
            TextAnchor align, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.text = text;
            t.alignment = align;
            t.color = color;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Truncate;
            return t;
        }

        // Same rounded-corner trick WalkSceneBuilder uses.
        private static void ApplyRounded(Image img, float roundness)
        {
            var rounded = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            if (rounded == null) return;
            img.sprite = rounded;
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = Mathf.Max(0.05f, roundness);
        }

        // ── Build settings wiring ──────────────────────────────────────────

        private static void AddSceneToBuildSettingsFirst(string path)
        {
            var existing = EditorBuildSettings.scenes;
            var list = new List<EditorBuildSettingsScene>();
            // New scene goes first so the app boots into the login screen.
            list.Add(new EditorBuildSettingsScene(path, true));
            for (int i = 0; i < existing.Length; i++)
            {
                if (existing[i].path == path) continue;
                list.Add(existing[i]);
            }
            EditorBuildSettings.scenes = list.ToArray();
        }
    }
}
