using UnityEngine;

namespace SGame.Core
{
    public class QuickUI : MonoBehaviour
    {
        enum GameScreen { Home, Mission, Result }
        GameScreen current = GameScreen.Home;
        int selectedMission = 0;
        int coins = 0;
        string feedback = "Watch the traffic light and decide!";
        bool trafficGreen = false;
        float trafficTimer = 0f;
        bool missionOver = false;
        int attempts = 0;
        int correct = 0;

        void Update()
        {
            if (current == GameScreen.Mission && !missionOver)
            {
                trafficTimer += Time.deltaTime;
                if (trafficTimer > 3f)
                {
                    trafficGreen = !trafficGreen;
                    trafficTimer = 0f;
                    feedback = trafficGreen ? "Light is GREEN — safe to cross!" : "Light is RED — stop and wait!";
                }
            }
        }

        void OnGUI()
        {
            float w = UnityEngine.Screen.width;
            float h = UnityEngine.Screen.height;

            // Dark background
            GUI.color = new Color(0.08f, 0.12f, 0.18f);
            GUI.DrawTexture(new Rect(0, 0, w, h), Texture2D.whiteTexture);
            GUI.color = Color.white;

            if (current == GameScreen.Home) DrawHome(w, h);
            else if (current == GameScreen.Mission) DrawMission(w, h);
            else DrawResult(w, h);
        }

        void DrawHome(float w, float h)
        {
            GUIStyle title = Style(Mathf.RoundToInt(h * 0.07f), new Color(0.2f, 0.8f, 1f), TextAnchor.MiddleCenter, true);
            GUIStyle sub   = Style(Mathf.RoundToInt(h * 0.035f), new Color(0.8f, 0.8f, 0.8f), TextAnchor.MiddleCenter);
            GUIStyle sec   = Style(Mathf.RoundToInt(h * 0.03f), Color.white, TextAnchor.MiddleCenter);
            GUIStyle barSt = Style(Mathf.RoundToInt(h * 0.028f), Color.white, TextAnchor.MiddleLeft);

            // Top bar
            DrawRect(0, 0, w, h * 0.07f, new Color(0.05f, 0.08f, 0.14f));
            GUI.Label(new Rect(w * 0.02f, 0, w * 0.35f, h * 0.07f), "Player 1", barSt);
            GUI.Label(new Rect(w * 0.35f, 0, w * 0.3f, h * 0.07f), "Coins: " + coins, Style(Mathf.RoundToInt(h * 0.028f), Color.white, TextAnchor.MiddleCenter));
            GUI.Label(new Rect(w * 0.65f, 0, w * 0.33f, h * 0.07f), "EN | AM", Style(Mathf.RoundToInt(h * 0.028f), Color.white, TextAnchor.MiddleRight));

            GUI.Label(new Rect(0, h * 0.12f, w, h * 0.12f), "Safe Steps Addis", title);
            GUI.Label(new Rect(0, h * 0.25f, w, h * 0.08f), "Road Safety for Children", sub);

            // Mission map
            DrawRect(w * 0.03f, h * 0.36f, w * 0.94f, h * 0.07f, new Color(0.1f, 0.15f, 0.22f));
            GUI.Label(new Rect(w * 0.03f, h * 0.36f, w * 0.94f, h * 0.07f), "SELECT MISSION", sec);

            string[] mNames = { "M1", "M2", "M3", "M4", "M5" };
            for (int i = 0; i < 5; i++)
            {
                float bx = w * (0.03f + i * 0.19f);
                Color c = i < 3 ? (selectedMission == i ? new Color(0.3f, 0.9f, 0.4f) : new Color(0.2f, 0.6f, 0.3f)) : new Color(0.35f, 0.35f, 0.35f);
                DrawRect(bx, h * 0.45f, w * 0.16f, h * 0.08f, c);
                if (i < 3 && GUI.Button(new Rect(bx, h * 0.45f, w * 0.16f, h * 0.08f), "", GUIStyle.none))
                    selectedMission = i;
                GUI.Label(new Rect(bx, h * 0.45f, w * 0.16f, h * 0.08f), mNames[i], sec);
            }

            // Play button
            DrawRect(w * 0.2f, h * 0.56f, w * 0.6f, h * 0.07f, new Color(0.1f, 0.7f, 0.4f));
            if (GUI.Button(new Rect(w * 0.2f, h * 0.56f, w * 0.6f, h * 0.07f), "", GUIStyle.none))
            {
                attempts = 0; correct = 0; missionOver = false;
                trafficGreen = false; trafficTimer = 0f;
                feedback = "Watch the traffic light and decide!";
                current = GameScreen.Mission;
            }
            GUI.Label(new Rect(w * 0.2f, h * 0.56f, w * 0.6f, h * 0.07f),
                "▶  PLAY MISSION " + (selectedMission + 1), Style(Mathf.RoundToInt(h * 0.04f), Color.white, TextAnchor.MiddleCenter, true));

            // Progress
            DrawRect(w * 0.03f, h * 0.66f, w * 0.94f, h * 0.07f, new Color(0.1f, 0.15f, 0.22f));
            GUI.Label(new Rect(w * 0.03f, h * 0.66f, w * 0.94f, h * 0.07f),
                "Progress: " + correct + "/5   Stars: " + (correct * 2), sec);

            // Bottom bar
            DrawRect(0, h * 0.9f, w, h * 0.1f, new Color(0.05f, 0.08f, 0.14f));
            string[] btns = { "Store", "Avatar", "Reports", "Exit" };
            for (int i = 0; i < 4; i++)
            {
                float bx = w * (0.02f + i * 0.245f);
                DrawRect(bx, h * 0.91f, w * 0.22f, h * 0.08f, new Color(0.18f, 0.26f, 0.38f));
                if (GUI.Button(new Rect(bx, h * 0.91f, w * 0.22f, h * 0.08f), "", GUIStyle.none))
                    Debug.Log("[QuickUI] Clicked: " + btns[i]);
                GUI.Label(new Rect(bx, h * 0.91f, w * 0.22f, h * 0.08f), btns[i], sec);
            }
        }

        void DrawMission(float w, float h)
        {
            GUIStyle sec  = Style(Mathf.RoundToInt(h * 0.032f), Color.white, TextAnchor.MiddleCenter);
            GUIStyle hud  = Style(Mathf.RoundToInt(h * 0.03f), Color.white, TextAnchor.MiddleLeft);
            GUIStyle big  = Style(Mathf.RoundToInt(h * 0.09f), Color.white, TextAnchor.MiddleCenter, true);
            GUIStyle fbk  = Style(Mathf.RoundToInt(h * 0.034f), Color.white, TextAnchor.MiddleCenter);

            // HUD
            DrawRect(0, 0, w, h * 0.07f, new Color(0.05f, 0.08f, 0.14f));
            GUI.Label(new Rect(w * 0.02f, 0, w * 0.6f, h * 0.07f), "Mission P1-M" + (selectedMission + 1), hud);
            GUI.Label(new Rect(w * 0.65f, 0, w * 0.33f, h * 0.07f), "Coins: " + coins,
                Style(Mathf.RoundToInt(h * 0.03f), Color.white, TextAnchor.MiddleRight));

            // Traffic light box
            Color lightBg = new Color(0.12f, 0.12f, 0.12f);
            DrawRect(w * 0.35f, h * 0.12f, w * 0.3f, h * 0.35f, lightBg);
            GUI.Label(new Rect(w * 0.35f, h * 0.13f, w * 0.3f, h * 0.1f), "TRAFFIC LIGHT",
                Style(Mathf.RoundToInt(h * 0.025f), new Color(0.7f, 0.7f, 0.7f), TextAnchor.MiddleCenter));

            // Red light
            GUI.color = trafficGreen ? new Color(0.25f, 0.1f, 0.1f) : Color.red;
            GUI.DrawTexture(new Rect(w * 0.42f, h * 0.19f, w * 0.16f, h * 0.12f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(w * 0.42f, h * 0.19f, w * 0.16f, h * 0.12f), "RED", Style(Mathf.RoundToInt(h * 0.035f), Color.white, TextAnchor.MiddleCenter, true));

            // Green light
            GUI.color = trafficGreen ? Color.green : new Color(0.1f, 0.25f, 0.1f);
            GUI.DrawTexture(new Rect(w * 0.42f, h * 0.33f, w * 0.16f, h * 0.12f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(w * 0.42f, h * 0.33f, w * 0.16f, h * 0.12f), "GREEN", Style(Mathf.RoundToInt(h * 0.035f), Color.white, TextAnchor.MiddleCenter, true));

            // Road
            DrawRect(0, h * 0.5f, w, h * 0.12f, new Color(0.25f, 0.25f, 0.25f));
            DrawRect(w * 0.35f, h * 0.5f, w * 0.3f, h * 0.12f, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            GUI.Label(new Rect(0, h * 0.5f, w, h * 0.12f), "━━━━━━━━━━━━━━━━━━━━━━━━",
                Style(Mathf.RoundToInt(h * 0.028f), new Color(0.9f, 0.9f, 0.9f, 0.4f), TextAnchor.MiddleCenter));

            // Feedback
            DrawRect(w * 0.03f, h * 0.64f, w * 0.94f, h * 0.09f, new Color(0.1f, 0.15f, 0.22f));
            GUI.Label(new Rect(w * 0.03f, h * 0.64f, w * 0.94f, h * 0.09f), feedback, fbk);

            if (!missionOver)
            {
                // WAIT button
                DrawRect(w * 0.03f, h * 0.75f, w * 0.44f, h * 0.1f, new Color(0.8f, 0.55f, 0.1f));
                if (GUI.Button(new Rect(w * 0.03f, h * 0.75f, w * 0.44f, h * 0.1f), "", GUIStyle.none))
                    HandleChoice(false);
                GUI.Label(new Rect(w * 0.03f, h * 0.75f, w * 0.44f, h * 0.1f), "WAIT", big);

                // CROSS button
                DrawRect(w * 0.53f, h * 0.75f, w * 0.44f, h * 0.1f, new Color(0.1f, 0.65f, 0.35f));
                if (GUI.Button(new Rect(w * 0.53f, h * 0.75f, w * 0.44f, h * 0.1f), "", GUIStyle.none))
                    HandleChoice(true);
                GUI.Label(new Rect(w * 0.53f, h * 0.75f, w * 0.44f, h * 0.1f), "CROSS", big);
            }
            else
            {
                DrawRect(w * 0.1f, h * 0.75f, w * 0.8f, h * 0.1f, new Color(0.1f, 0.5f, 0.8f));
                if (GUI.Button(new Rect(w * 0.1f, h * 0.75f, w * 0.8f, h * 0.1f), "", GUIStyle.none))
                    current = GameScreen.Result;
                GUI.Label(new Rect(w * 0.1f, h * 0.75f, w * 0.8f, h * 0.1f), "SEE RESULTS", big);
            }

            // Score
            DrawRect(0, h * 0.87f, w, h * 0.06f, new Color(0.05f, 0.08f, 0.14f));
            GUI.Label(new Rect(0, h * 0.87f, w, h * 0.06f),
                "Correct: " + correct + " / " + attempts, sec);

            // Back
            DrawRect(0, h * 0.93f, w * 0.25f, h * 0.07f, new Color(0.2f, 0.1f, 0.1f));
            if (GUI.Button(new Rect(0, h * 0.93f, w * 0.25f, h * 0.07f), "", GUIStyle.none))
                current = GameScreen.Home;
            GUI.Label(new Rect(0, h * 0.93f, w * 0.25f, h * 0.07f), "← Back", sec);
        }

        void DrawResult(float w, float h)
        {
            GUIStyle big  = Style(Mathf.RoundToInt(h * 0.08f), Color.white, TextAnchor.MiddleCenter, true);
            GUIStyle med  = Style(Mathf.RoundToInt(h * 0.045f), Color.white, TextAnchor.MiddleCenter);
            GUIStyle sec  = Style(Mathf.RoundToInt(h * 0.032f), Color.white, TextAnchor.MiddleCenter);

            bool passed = correct >= 3;
            GUI.Label(new Rect(0, h * 0.12f, w, h * 0.12f),
                passed ? "GREAT JOB!" : "TRY AGAIN", Style(Mathf.RoundToInt(h * 0.09f),
                passed ? new Color(0.2f, 1f, 0.4f) : new Color(1f, 0.4f, 0.2f), TextAnchor.MiddleCenter, true));

            GUI.Label(new Rect(0, h * 0.27f, w, h * 0.08f),
                "Mission P1-M" + (selectedMission + 1) + " Complete", med);

            DrawRect(w * 0.1f, h * 0.38f, w * 0.8f, h * 0.28f, new Color(0.1f, 0.15f, 0.22f));
            GUI.Label(new Rect(w * 0.1f, h * 0.40f, w * 0.8f, h * 0.06f), "Correct Answers: " + correct + " / " + attempts, sec);
            GUI.Label(new Rect(w * 0.1f, h * 0.48f, w * 0.8f, h * 0.06f), "Stars Earned:  " + (correct >= 4 ? "★★★" : correct >= 2 ? "★★" : "★"), sec);
            GUI.Label(new Rect(w * 0.1f, h * 0.56f, w * 0.8f, h * 0.06f), "Coins Earned:  +" + (correct * 10), sec);

            DrawRect(w * 0.1f, h * 0.72f, w * 0.8f, h * 0.09f, new Color(0.1f, 0.7f, 0.4f));
            if (GUI.Button(new Rect(w * 0.1f, h * 0.72f, w * 0.8f, h * 0.09f), "", GUIStyle.none))
            {
                coins += correct * 10;
                current = GameScreen.Home;
            }
            GUI.Label(new Rect(w * 0.1f, h * 0.72f, w * 0.8f, h * 0.09f), "Back to Home", big);
        }

        void HandleChoice(bool wantsToCross)
        {
            attempts++;
            bool correct_choice = wantsToCross == trafficGreen;
            if (correct_choice)
            {
                correct++;
                feedback = wantsToCross
                    ? "Correct! Green means GO — you crossed safely!"
                    : "Correct! Red means STOP — good job waiting!";
            }
            else
            {
                feedback = wantsToCross
                    ? "DANGER! You crossed on RED — always wait!"
                    : "You waited on GREEN — it was safe to cross!";
            }
            if (attempts >= 5) missionOver = true;
        }

        static void DrawRect(float x, float y, float w, float h, Color c)
        {
            GUI.color = c;
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        static GUIStyle Style(int size, Color color, TextAnchor anchor, bool bold = false)
        {
            var s = new GUIStyle(GUI.skin.label);
            s.fontSize = size;
            s.normal.textColor = color;
            s.alignment = anchor;
            if (bold) s.fontStyle = FontStyle.Bold;
            return s;
        }
    }
}
