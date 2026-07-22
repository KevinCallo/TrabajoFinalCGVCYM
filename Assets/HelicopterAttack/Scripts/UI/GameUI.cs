using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace HelicopterAttack
{
    public class GameUI : MonoBehaviour
    {
        public Text m_TargetCount;
        public Text m_HealthText;
        public Image m_HealthBarFill;
        public Slider m_HealthSlider;

        private GUIStyle m_HealthTextStyle;
        private Texture2D m_BgTexture;
        private Texture2D m_FillTexture;

        void Start()
        {
            m_HealthTextStyle = new GUIStyle();
            m_HealthTextStyle.fontSize = 16;
            m_HealthTextStyle.fontStyle = FontStyle.Bold;
            m_HealthTextStyle.normal.textColor = Color.white;

            m_BgTexture = MakeTexture(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.75f));
            m_FillTexture = MakeTexture(2, 2, Color.green);
        }

        void Update()
        {
            if (GameControl.m_Current != null && m_TargetCount != null)
            {
                m_TargetCount.text = GameControl.m_Current.m_TargetDestroyedCount.ToString() + " / " + GameControl.m_Current.m_MaxTargetCount.ToString();
            }

            if (PlayerHeli.Current != null && PlayerHeli.Current.m_MyDamage != null)
            {
                float currentHealth = Mathf.Max(0, PlayerHeli.Current.m_MyDamage.Damage);
                float maxHealth = PlayerHeli.Current.m_MyDamage.MaxDamage;
                float ratio = Mathf.Clamp01(currentHealth / maxHealth);

                if (m_HealthText != null)
                {
                    m_HealthText.text = "SALUD: " + Mathf.CeilToInt(currentHealth) + " / " + Mathf.CeilToInt(maxHealth);
                }

                if (m_HealthBarFill != null)
                {
                    m_HealthBarFill.fillAmount = ratio;
                    m_HealthBarFill.color = Color.Lerp(Color.red, Color.green, ratio);
                }

                if (m_HealthSlider != null)
                {
                    m_HealthSlider.value = ratio;
                }
            }
        }

        void OnGUI()
        {
            if (PlayerHeli.Current == null || PlayerHeli.Current.m_MyDamage == null) return;
            if (m_HealthSlider != null || m_HealthBarFill != null) return;

            float currentHealth = Mathf.Max(0, PlayerHeli.Current.m_MyDamage.Damage);
            float maxHealth = PlayerHeli.Current.m_MyDamage.MaxDamage;
            float ratio = Mathf.Clamp01(currentHealth / maxHealth);

            // Draw Health Bar HUD at top-right
            float barWidth = 240f;
            float barHeight = 24f;
            float xPos = Screen.width - barWidth - 25f;
            float yPos = 20f;

            GUI.DrawTexture(new Rect(xPos - 4, yPos - 4, barWidth + 8, barHeight + 28), m_BgTexture);

            GUI.Label(new Rect(xPos, yPos, barWidth, 20), "HELICÓPTERO: " + Mathf.CeilToInt(currentHealth) + " / " + Mathf.CeilToInt(maxHealth), m_HealthTextStyle);

            // Background bar
            GUI.DrawTexture(new Rect(xPos, yPos + 22, barWidth, barHeight), m_BgTexture);

            // Health Fill bar
            Color barColor = Color.Lerp(Color.red, Color.green, ratio);
            GUI.color = barColor;
            GUI.DrawTexture(new Rect(xPos, yPos + 22, barWidth * ratio, barHeight), m_FillTexture);
            GUI.color = Color.white;
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

        public void BtnExit()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Scene_MainMenu");
        }
    }
}