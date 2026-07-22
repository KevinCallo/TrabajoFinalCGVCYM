using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HelicopterAttack
{
    public class WinUI : MonoBehaviour
    {
        public Button m_BtnContinue;
        public Button m_BtnMainMenu;

        private GUIStyle m_TitleStyle;
        private GUIStyle m_ButtonStyle;
        private Texture2D m_WinBgTexture;

        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (m_BtnContinue != null)
            {
                m_BtnContinue.onClick.AddListener(Continue);
            }
            if (m_BtnMainMenu != null)
            {
                m_BtnMainMenu.onClick.AddListener(GoToMainMenu);
            }

            InitGUIStyles();
        }

        private void InitGUIStyles()
        {
            m_WinBgTexture = MakeTexture(2, 2, new Color(0.08f, 0.12f, 0.18f, 0.92f));

            m_TitleStyle = new GUIStyle();
            m_TitleStyle.fontSize = 32;
            m_TitleStyle.fontStyle = FontStyle.Bold;
            m_TitleStyle.alignment = TextAnchor.MiddleCenter;
            m_TitleStyle.normal.textColor = new Color(0.2f, 0.9f, 0.3f);

            m_ButtonStyle = new GUIStyle();
            m_ButtonStyle.fontSize = 20;
            m_ButtonStyle.fontStyle = FontStyle.Bold;
            m_ButtonStyle.alignment = TextAnchor.MiddleCenter;
            m_ButtonStyle.normal.textColor = Color.white;
            m_ButtonStyle.hover.textColor = Color.yellow;
        }

        void Update()
        {
            // Allow advancing with Space, Enter, or Escape key
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape))
            {
                Continue();
            }
        }

        public void Continue()
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Reload level / restart mission
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene("Scene_MainMenu");
        }

        public void BtnContinue()
        {
            Continue();
        }

        public void BtnMap()
        {
            GoToMainMenu();
        }

        void OnGUI()
        {
            // If WinUI is active and GameState is 1 (Win)
            if (GameControl.m_Current != null && GameControl.m_Current.m_GameState != 1)
                return;

            float width = 450f;
            float height = 250f;
            float x = (Screen.width - width) * 0.5f;
            float y = (Screen.height - height) * 0.5f;

            GUI.DrawTexture(new Rect(x, y, width, height), m_WinBgTexture);
            GUI.Box(new Rect(x, y, width, height), "");

            GUI.Label(new Rect(x, y + 25, width, 45), "🏆 ¡MISIÓN CUMPLIDA!", m_TitleStyle);

            if (GUI.Button(new Rect(x + 50, y + 90, width - 100, 48), "▶ REINICIAR MISIÓN (ESPACIO)"))
            {
                Continue();
            }

            if (GUI.Button(new Rect(x + 50, y + 155, width - 100, 48), "🏠 MENÚ PRINCIPAL"))
            {
                GoToMainMenu();
            }
        }

        private Texture2D MakeTexture(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
