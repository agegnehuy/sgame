using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

namespace SGame.Editor
{
    public class UIBuilder : EditorWindow
    {
        [MenuItem("SGame/Build TestScene (Start Here)")]
        public static void BuildTestScene()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("UIBuilder", "Stop Play mode first, then run this.", "OK");
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ── Camera ───────────────────────────────────────────────────
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.12f, 0.18f);
            cam.orthographic = true;
            camGo.AddComponent<AudioListener>();

            // ── Event System ─────────────────────────────────────────────
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // ── Canvas ───────────────────────────────────────────────────
            var canvasGo = new GameObject("MainCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            CreateFlatPanel(canvasGo.transform, "Background", new Color(0.08f, 0.12f, 0.18f), Vector2.zero, Vector2.one);

            // Title
            var titleTxt = CreateFlatLabel(canvasGo.transform, "Title", "Safe Steps Addis", 72, new Vector2(0.05f, 0.76f), new Vector2(0.95f, 0.88f));
            titleTxt.color = new Color(0.2f, 0.8f, 1f);

            // Subtitle
            CreateFlatLabel(canvasGo.transform, "Subtitle", "Road Safety for Children", 36, new Vector2(0.05f, 0.67f), new Vector2(0.95f, 0.75f));

            // TopBar
            var topBar = CreateFlatPanel(canvasGo.transform, "TopBar", new Color(0.05f, 0.08f, 0.14f), new Vector2(0f, 0.92f), new Vector2(1f, 1f));
            CreateFlatLabel(topBar.transform, "Profile", "Player 1", 26, new Vector2(0.02f, 0f), new Vector2(0.4f, 1f));
            CreateFlatLabel(topBar.transform, "Coins", "Coins: 0", 26, new Vector2(0.4f, 0f), new Vector2(0.7f, 1f));
            CreateFlatLabel(topBar.transform, "Lang", "EN | AM", 24, new Vector2(0.7f, 0f), new Vector2(0.98f, 1f));

            // Mission Panel
            var mapPanel = CreateFlatPanel(canvasGo.transform, "MissionMap", new Color(0.1f, 0.15f, 0.22f), new Vector2(0.03f, 0.54f), new Vector2(0.97f, 0.64f));
            CreateFlatLabel(mapPanel.transform, "MapTitle", "SELECT MISSION", 24, new Vector2(0.02f, 0f), new Vector2(0.98f, 1f));

            string[] mNames = { "M1", "M2", "M3", "M4", "M5" };
            for (int i = 0; i < 5; i++)
            {
                float x0 = 0.02f + i * 0.196f;
                Color c = i < 3 ? new Color(0.2f, 0.7f, 0.3f) : new Color(0.4f, 0.4f, 0.4f);
                CreateFlatButton(canvasGo.transform, "Btn_" + mNames[i], mNames[i], c, new Vector2(x0, 0.44f), new Vector2(x0 + 0.17f, 0.52f));
            }

            // Play Button
            CreateFlatButton(canvasGo.transform, "PlayButton", "PLAY MISSION", new Color(0.1f, 0.7f, 0.4f), new Vector2(0.2f, 0.35f), new Vector2(0.8f, 0.42f));

            // Progress
            var prog = CreateFlatPanel(canvasGo.transform, "Progress", new Color(0.1f, 0.15f, 0.22f), new Vector2(0.03f, 0.27f), new Vector2(0.97f, 0.33f));
            CreateFlatLabel(prog.transform, "ProgText", "Progress: 3/5   Stars: 6", 22, Vector2.zero, Vector2.one);

            // Bottom Bar
            var btmBar = CreateFlatPanel(canvasGo.transform, "BottomBar", new Color(0.05f, 0.08f, 0.14f), new Vector2(0f, 0f), new Vector2(1f, 0.09f));
            string[] btnNames = { "Store", "Avatar", "Reports", "Exit" };
            for (int i = 0; i < 4; i++)
            {
                float x0 = 0.01f + i * 0.247f;
                CreateFlatButton(btmBar.transform, "Btn_" + btnNames[i], btnNames[i], new Color(0.15f, 0.22f, 0.32f), new Vector2(x0, 0.08f), new Vector2(x0 + 0.235f, 0.92f));
            }

            EditorSceneManager.SaveScene(scene, "Assets/_Game/Scenes/TestScene.unity");
            Debug.Log("[UIBuilder] TestScene built and saved.");
            EditorUtility.DisplayDialog("Done!", "TestScene built!\n\nNow:\n1. The scene is already open\n2. Click the Game tab\n3. Press Play", "OK");
        }

        static GameObject CreateFlatPanel(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        static UnityEngine.UI.Text CreateFlatLabel(Transform parent, string name, string text, int size, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var t = go.AddComponent<UnityEngine.UI.Text>();
            t.text = text;
            t.fontSize = size;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return t;
        }

        static GameObject CreateFlatButton(Transform parent, string name, string label, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            go.AddComponent<Button>();
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var lrt = labelGo.AddComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = new Vector2(4, 2); lrt.offsetMax = new Vector2(-4, -2);
            var t = labelGo.AddComponent<UnityEngine.UI.Text>();
            t.text = label;
            t.fontSize = 28;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return go;
        }

        [MenuItem("SGame/Build HomeScene UI")]
        public static void BuildHomeSceneUI()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("UIBuilder", "Stop Play mode first (press ▶ again), then run this menu item.", "OK");
                return;
            }

            var scene = EditorSceneManager.OpenScene("Assets/_Game/Scenes/HomeScene.unity");

            // Remove old canvas and camera if they exist
            foreach (var name in new[] { "MainCanvas", "Main Camera", "EventSystem" })
            {
                var old = GameObject.Find(name);
                if (old != null) Object.DestroyImmediate(old);
            }

            // ── Camera ───────────────────────────────────────────────────
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.12f, 0.18f);
            cam.orthographic = true;
            camGo.AddComponent<AudioListener>();

            // ── Event System ─────────────────────────────────────────────
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // ── Root Canvas ──────────────────────────────────────────────
            var canvasGo = new GameObject("MainCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            var bg = CreatePanel(canvasGo.transform, "Background", new Color(0.08f, 0.12f, 0.18f), new Vector2(0, 0), new Vector2(1, 1));

            // ── Top Bar ──────────────────────────────────────────────────
            var topBar = CreatePanel(canvasGo.transform, "TopBar", new Color(0.05f, 0.08f, 0.14f));
            SetRect(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -100), new Vector2(0, 0));
            CreateLabel(topBar.transform, "ProfileLabel", "👤  Player 1", 28, TextAlignmentOptions.MidlineLeft, new Vector2(0, 0), new Vector2(0.35f, 1));
            CreateLabel(topBar.transform, "CoinLabel", "🪙  0", 28, TextAlignmentOptions.Midline, new Vector2(0.35f, 0), new Vector2(0.65f, 1));
            CreateLabel(topBar.transform, "LangLabel", "EN | AM", 26, TextAlignmentOptions.MidlineRight, new Vector2(0.65f, 0), new Vector2(1f, 1));

            // ── Title ────────────────────────────────────────────────────
            var titlePanel = CreatePanel(canvasGo.transform, "TitlePanel", Color.clear);
            SetRect(titlePanel, new Vector2(0, 0.78f), new Vector2(1, 0.88f), Vector2.zero, Vector2.zero);
            var titleLabel = CreateLabel(titlePanel.transform, "TitleLabel", "Safe Steps Addis", 52, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);
            titleLabel.GetComponent<TextMeshProUGUI>().color = new Color(0.2f, 0.8f, 1f);
            CreateLabel(titlePanel.transform, "SubLabel", "Road Safety for Children", 30, TextAlignmentOptions.Center, new Vector2(0, -0.4f), new Vector2(1, 0.5f));

            // ── Mission Map ──────────────────────────────────────────────
            var mapPanel = CreatePanel(canvasGo.transform, "MissionMapPanel", new Color(0.1f, 0.15f, 0.22f, 0.9f));
            SetRect(mapPanel, new Vector2(0.03f, 0.55f), new Vector2(0.97f, 0.76f), Vector2.zero, Vector2.zero);
            CreateLabel(mapPanel.transform, "MapTitle", "SELECT MISSION", 26, TextAlignmentOptions.TopLeft, new Vector2(0.02f, 0.6f), new Vector2(0.98f, 1f));

            string[] missions = { "M1", "M2", "M3", "M4", "M5" };
            Color[] missionColors = {
                new Color(0.2f, 0.7f, 0.3f),
                new Color(0.2f, 0.7f, 0.3f),
                new Color(0.2f, 0.7f, 0.3f),
                new Color(0.5f, 0.5f, 0.5f),
                new Color(0.5f, 0.5f, 0.5f)
            };
            for (int i = 0; i < 5; i++)
            {
                float xMin = 0.02f + i * 0.196f;
                float xMax = xMin + 0.17f;
                var mBtn = CreateButton(mapPanel.transform, "Mission_" + missions[i], missions[i], missionColors[i], new Vector2(xMin, 0.05f), new Vector2(xMax, 0.58f));
                if (i >= 3)
                {
                    CreateLabel(mBtn.transform, "LockIcon", "🔒", 22, TextAlignmentOptions.Bottom, new Vector2(0, 0), new Vector2(1, 0.4f));
                }
                else
                {
                    CreateLabel(mBtn.transform, "StarIcon", "★★★", 18, TextAlignmentOptions.Bottom, new Vector2(0, 0), new Vector2(1, 0.4f));
                }
            }

            // ── Play Button ──────────────────────────────────────────────
            var playBtn = CreateButton(canvasGo.transform, "PlayButton", "▶  PLAY MISSION", new Color(0.1f, 0.7f, 0.4f), new Vector2(0.2f, 0.46f), new Vector2(0.8f, 0.53f));
            playBtn.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;

            // ── Bottom Action Bar ────────────────────────────────────────
            var bottomBar = CreatePanel(canvasGo.transform, "BottomBar", new Color(0.05f, 0.08f, 0.14f));
            SetRect(bottomBar, new Vector2(0, 0), new Vector2(1, 0.1f), Vector2.zero, Vector2.zero);

            string[] btnLabels = { "🛍 Store", "👤 Avatar", "📊 Reports", "🚪 Exit" };
            Color btnColor = new Color(0.15f, 0.22f, 0.32f);
            for (int i = 0; i < 4; i++)
            {
                float xMin = 0.01f + i * 0.247f;
                float xMax = xMin + 0.235f;
                CreateButton(bottomBar.transform, "Btn_" + btnLabels[i], btnLabels[i], btnColor, new Vector2(xMin, 0.08f), new Vector2(xMax, 0.92f));
            }

            // ── Facilitator Button ───────────────────────────────────────
            CreateButton(canvasGo.transform, "FacilitatorBtn", "⚙ Facilitator", new Color(0.3f, 0.15f, 0.05f), new Vector2(0.6f, 0.11f), new Vector2(0.97f, 0.17f));

            // ── Progress Panel ───────────────────────────────────────────
            var progressPanel = CreatePanel(canvasGo.transform, "ProgressPanel", new Color(0.1f, 0.15f, 0.22f, 0.9f));
            SetRect(progressPanel, new Vector2(0.03f, 0.36f), new Vector2(0.97f, 0.44f), Vector2.zero, Vector2.zero);
            CreateLabel(progressPanel.transform, "ProgressTitle", "Progress: 3/5 missions  |  Total Stars: ★★★★★★", 26, TextAlignmentOptions.Midline, Vector2.zero, Vector2.one);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[UIBuilder] HomeScene UI built successfully.");
        }

        [MenuItem("SGame/Build MissionScene UI")]
        public static void BuildMissionSceneUI()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("UIBuilder", "Stop Play mode first (press ▶ again), then run this menu item.", "OK");
                return;
            }

            var scene = EditorSceneManager.OpenScene("Assets/_Game/Scenes/P1_M1.unity");

            foreach (var name in new[] { "MissionCanvas", "Main Camera", "EventSystem" })
            {
                var old = GameObject.Find(name);
                if (old != null) Object.DestroyImmediate(old);
            }

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.12f, 0.18f);
            cam.orthographic = true;
            camGo.AddComponent<AudioListener>();

            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            var canvasGo = new GameObject("MissionCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            CreatePanel(canvasGo.transform, "Background", new Color(0.08f, 0.12f, 0.18f), new Vector2(0, 0), new Vector2(1, 1));

            // ── HUD Top ──────────────────────────────────────────────────
            var hud = CreatePanel(canvasGo.transform, "HUD", new Color(0.05f, 0.08f, 0.14f));
            SetRect(hud, new Vector2(0, 0.9f), new Vector2(1, 1f), Vector2.zero, Vector2.zero);
            CreateLabel(hud.transform, "MissionLabel", "Mission: P1-M1", 30, TextAlignmentOptions.MidlineLeft, new Vector2(0.02f, 0), new Vector2(0.6f, 1));
            CreateLabel(hud.transform, "HUDCoins", "🪙  0", 30, TextAlignmentOptions.MidlineRight, new Vector2(0.6f, 0), new Vector2(0.98f, 1));

            // ── Traffic Light ────────────────────────────────────────────
            var trafficPanel = CreatePanel(canvasGo.transform, "TrafficPanel", new Color(0.1f, 0.1f, 0.1f, 0.8f));
            SetRect(trafficPanel, new Vector2(0.4f, 0.55f), new Vector2(0.6f, 0.85f), Vector2.zero, Vector2.zero);
            CreateLabel(trafficPanel.transform, "TrafficRed", "🔴", 60, TextAlignmentOptions.Center, new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.95f));
            CreateLabel(trafficPanel.transform, "TrafficGreen", "⚫", 60, TextAlignmentOptions.Center, new Vector2(0.1f, 0.05f), new Vector2(0.9f, 0.5f));
            CreateLabel(trafficPanel.transform, "TrafficLabel", "TRAFFIC LIGHT", 20, TextAlignmentOptions.Center, new Vector2(0, -0.05f), new Vector2(1, 0.1f));

            // ── Road / Scene ─────────────────────────────────────────────
            var roadPanel = CreatePanel(canvasGo.transform, "RoadArea", new Color(0.2f, 0.2f, 0.2f));
            SetRect(roadPanel, new Vector2(0, 0.38f), new Vector2(1, 0.55f), Vector2.zero, Vector2.zero);
            CreateLabel(roadPanel.transform, "RoadLabel", "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", 20, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);

            var crosswalk = CreatePanel(canvasGo.transform, "Crosswalk", new Color(0.9f, 0.9f, 0.9f, 0.15f));
            SetRect(crosswalk, new Vector2(0.35f, 0.38f), new Vector2(0.65f, 0.55f), Vector2.zero, Vector2.zero);
            CreateLabel(crosswalk.transform, "CrossLabel", "CROSSWALK", 22, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);

            // ── Feedback ─────────────────────────────────────────────────
            var feedbackPanel = CreatePanel(canvasGo.transform, "FeedbackPanel", new Color(0.1f, 0.15f, 0.22f, 0.9f));
            SetRect(feedbackPanel, new Vector2(0.03f, 0.28f), new Vector2(0.97f, 0.37f), Vector2.zero, Vector2.zero);
            CreateLabel(feedbackPanel.transform, "FeedbackText", "Watch the traffic light and press CROSS or WAIT", 28, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);

            // ── Action Buttons ───────────────────────────────────────────
            CreateButton(canvasGo.transform, "WaitBtn", "⏸  WAIT", new Color(0.8f, 0.6f, 0.1f), new Vector2(0.03f, 0.15f), new Vector2(0.47f, 0.26f));
            CreateButton(canvasGo.transform, "CrossBtn", "🚶  CROSS", new Color(0.1f, 0.7f, 0.4f), new Vector2(0.53f, 0.15f), new Vector2(0.97f, 0.26f));

            // ── Vehicle Distance Indicator ───────────────────────────────
            var vehiclePanel = CreatePanel(canvasGo.transform, "VehiclePanel", new Color(0.15f, 0.1f, 0.1f, 0.8f));
            SetRect(vehiclePanel, new Vector2(0.03f, 0.56f), new Vector2(0.38f, 0.65f), Vector2.zero, Vector2.zero);
            CreateLabel(vehiclePanel.transform, "VehicleLabel", "🚗  Distance: -- m", 24, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[UIBuilder] MissionScene UI built successfully.");
        }

        // ── Helpers ─────────────────────────────────────────────────────

        static GameObject CreatePanel(Transform parent, string name, Color color, Vector2 anchorMin = default, Vector2 anchorMax = default)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            if (anchorMin != default || anchorMax != default)
            {
                rt.anchorMin = anchorMin;
                rt.anchorMax = anchorMax;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
        }

        static GameObject CreateLabel(Transform parent, string name, string text, float size, TextAlignmentOptions align, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.color = Color.white;
            return go;
        }

        static GameObject CreateButton(Transform parent, string name, string label, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            go.AddComponent<Button>();

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(go.transform, false);
            var trt = textGo.AddComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = new Vector2(8, 4);
            trt.offsetMax = new Vector2(-8, -4);
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 30;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            return go;
        }
    }
}
