using UnityEngine;
using UnityEngine.UI;

namespace SGame.Core
{
    public class RuntimeUITest : MonoBehaviour
    {
        void Awake()
        {
            BuildUI();
        }

        void BuildUI()
        {
            Camera cam = FindObjectOfType<Camera>();
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
            }
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.12f, 0.18f);
            cam.orthographic = true;

            var canvasGo = new GameObject("MainCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            MakePanel(canvasGo.transform, "BG", new Color(0.08f, 0.12f, 0.18f), 0, 0, 1, 1);

            var titleTxt = MakeLabel(canvasGo.transform, "Title", "Safe Steps Addis", 72, 0.05f, 0.76f, 0.95f, 0.88f);
            titleTxt.color = new Color(0.2f, 0.8f, 1f);
            MakeLabel(canvasGo.transform, "Sub", "Road Safety for Children", 36, 0.05f, 0.67f, 0.95f, 0.75f);

            var topBar = MakePanel(canvasGo.transform, "TopBar", new Color(0.05f, 0.08f, 0.14f), 0, 0.92f, 1, 1f);
            MakeLabel(topBar.transform, "P", "Player 1", 24, 0.02f, 0, 0.4f, 1);
            MakeLabel(topBar.transform, "C", "Coins: 0", 24, 0.4f, 0, 0.7f, 1);
            MakeLabel(topBar.transform, "L", "EN | AM", 22, 0.7f, 0, 0.98f, 1);

            var mapPanel = MakePanel(canvasGo.transform, "MissionMap", new Color(0.1f, 0.15f, 0.22f), 0.03f, 0.54f, 0.97f, 0.64f);
            MakeLabel(mapPanel.transform, "MapTitle", "SELECT MISSION", 22, 0.02f, 0, 0.98f, 1);

            string[] mNames = { "M1", "M2", "M3", "M4", "M5" };
            for (int i = 0; i < 5; i++)
            {
                float x0 = 0.02f + i * 0.196f;
                Color c = i < 3 ? new Color(0.2f, 0.7f, 0.3f) : new Color(0.45f, 0.45f, 0.45f);
                MakeButton(canvasGo.transform, mNames[i], mNames[i], c, x0, 0.44f, x0 + 0.17f, 0.52f);
            }

            MakeButton(canvasGo.transform, "Play", "PLAY MISSION", new Color(0.1f, 0.7f, 0.4f), 0.2f, 0.35f, 0.8f, 0.42f);

            var prog = MakePanel(canvasGo.transform, "Progress", new Color(0.1f, 0.15f, 0.22f), 0.03f, 0.27f, 0.97f, 0.33f);
            MakeLabel(prog.transform, "PT", "Progress: 3/5   Stars: 6", 20, 0, 0, 1, 1);

            var btm = MakePanel(canvasGo.transform, "BottomBar", new Color(0.05f, 0.08f, 0.14f), 0, 0, 1, 0.09f);
            string[] btns = { "Store", "Avatar", "Reports", "Exit" };
            for (int i = 0; i < 4; i++)
            {
                float x0 = 0.01f + i * 0.247f;
                MakeButton(btm.transform, btns[i], btns[i], new Color(0.15f, 0.22f, 0.32f), x0, 0.08f, x0 + 0.235f, 0.92f);
            }
        }

        static GameObject MakePanel(Transform parent, string name, Color color, float x0, float y0, float x1, float y1)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(x0, y0);
            rt.anchorMax = new Vector2(x1, y1);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            go.AddComponent<Image>().color = color;
            return go;
        }

        static Text MakeLabel(Transform parent, string name, string text, int size, float x0, float y0, float x1, float y1)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(x0, y0);
            rt.anchorMax = new Vector2(x1, y1);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.text = text;
            t.fontSize = size;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.resizeTextForBestFit = true;
            t.resizeTextMinSize = 10;
            t.resizeTextMaxSize = size;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return t;
        }

        static void MakeButton(Transform parent, string name, string label, Color color, float x0, float y0, float x1, float y1)
        {
            var go = MakePanel(parent, name, color, x0, y0, x1, y1);
            go.AddComponent<Button>();
            MakeLabel(go.transform, "L", label, 28, 0.05f, 0.1f, 0.95f, 0.9f);
        }
    }
}
