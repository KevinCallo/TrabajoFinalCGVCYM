using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelicopterAttack
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class NightCameraEffect : MonoBehaviour
    {
        public KeyCode m_ToggleKey = KeyCode.N;
        public bool m_IsNightVisionActive = false;
        public Shader m_NightShader;

        [Range(0f, 1f)]
        public float m_GrayscaleFactor = 0f;
        public float m_Contrast = 1.1f;
        public float m_Brightness = 1.6f;

        private Material m_NightMaterial;
        private GUIStyle m_HudTextStyle;
        private GUIStyle m_HintStyle;
        private Texture2D m_HudBgTexture;

        void Start()
        {
            if (m_NightShader == null)
            {
                m_NightShader = Shader.Find("Custom/NightVisionShader");
            }

            InitMaterial();

            m_HudTextStyle = new GUIStyle();
            m_HudTextStyle.fontSize = 14;
            m_HudTextStyle.fontStyle = FontStyle.Bold;
            m_HudTextStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);

            m_HintStyle = new GUIStyle();
            m_HintStyle.fontSize = 14;
            m_HintStyle.fontStyle = FontStyle.Bold;
            m_HintStyle.alignment = TextAnchor.MiddleCenter;
            m_HintStyle.normal.textColor = Color.yellow;

            m_HudBgTexture = MakeTexture(2, 2, new Color(0f, 0f, 0f, 0.6f));
        }

        void Update()
        {
            // Toggle Night Vision with N or B key
            if (Input.GetKeyDown(m_ToggleKey) || Input.GetKeyDown(KeyCode.B))
            {
                m_IsNightVisionActive = !m_IsNightVisionActive;
            }

            // Smoothly lerp grayscale factor based on toggle state
            float targetFactor = m_IsNightVisionActive ? 1.0f : 0.0f;
            m_GrayscaleFactor = Mathf.MoveTowards(m_GrayscaleFactor, targetFactor, 4.0f * Time.deltaTime);
        }

        private void InitMaterial()
        {
            if (m_NightShader != null && m_NightMaterial == null)
            {
                m_NightMaterial = new Material(m_NightShader);
                m_NightMaterial.hideFlags = HideFlags.DontSave;
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (m_GrayscaleFactor > 0.01f && m_NightShader != null)
            {
                InitMaterial();
                if (m_NightMaterial != null)
                {
                    m_NightMaterial.SetFloat("_LuminanceAmount", m_GrayscaleFactor);
                    m_NightMaterial.SetFloat("_Contrast", m_Contrast);
                    m_NightMaterial.SetFloat("_Brightness", m_Brightness);
                    Graphics.Blit(source, destination, m_NightMaterial);
                    return;
                }
            }

            Graphics.Blit(source, destination);
        }

        void OnGUI()
        {
            InitGUIStyles();

            if (m_GrayscaleFactor >= 0.4f)
            {
                // 1. Top-Left Timestamp (matching reference image)
                string dateStr = System.DateTime.Now.ToString("dd-MM-yyyy ddd HH:mm:ss");
                GUI.Label(new Rect(30, 25, 300, 30), dateStr, m_CctvTextLeftStyle);

                // 2. Bottom-Right Camera Tag (matching reference image: Camera 01)
                GUI.Label(new Rect(Screen.width - 150, Screen.height - 45, 130, 30), "Camera 01", m_CctvTextRightStyle);

                // 3. Bottom-Left Security Red Badge (matching reference image)
                float badgeW = 140f;
                float badgeH = 28f;
                float badgeX = 20f;
                float badgeY = Screen.height - 42f;

                GUI.DrawTexture(new Rect(badgeX, badgeY, badgeW, badgeH), m_RedBadgeTexture);
                GUI.Label(new Rect(badgeX + 10, badgeY + 4, badgeW, badgeH), "📹 SECURITY CAM", m_BadgeTextStyle);
            }
        }

        private GUIStyle m_CctvTextLeftStyle;
        private GUIStyle m_CctvTextRightStyle;
        private GUIStyle m_BadgeTextStyle;
        private Texture2D m_RedBadgeTexture;

        private void InitGUIStyles()
        {
            if (m_CctvTextLeftStyle != null) return;

            m_HudBgTexture = MakeTexture(2, 2, new Color(0f, 0f, 0f, 0.6f));
            m_RedBadgeTexture = MakeTexture(2, 2, new Color(0.85f, 0.1f, 0.1f, 0.9f));

            m_CctvTextLeftStyle = new GUIStyle();
            m_CctvTextLeftStyle.fontSize = 16;
            m_CctvTextLeftStyle.fontStyle = FontStyle.Bold;
            m_CctvTextLeftStyle.normal.textColor = new Color(0.95f, 0.95f, 0.95f, 0.95f);

            m_CctvTextRightStyle = new GUIStyle();
            m_CctvTextRightStyle.fontSize = 16;
            m_CctvTextRightStyle.fontStyle = FontStyle.Bold;
            m_CctvTextRightStyle.alignment = TextAnchor.MiddleRight;
            m_CctvTextRightStyle.normal.textColor = new Color(0.95f, 0.95f, 0.95f, 0.95f);

            m_BadgeTextStyle = new GUIStyle();
            m_BadgeTextStyle.fontSize = 13;
            m_BadgeTextStyle.fontStyle = FontStyle.Bold;
            m_BadgeTextStyle.normal.textColor = Color.white;
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

        void OnDisable()
        {
            if (m_NightMaterial != null)
            {
                DestroyImmediate(m_NightMaterial);
            }
        }
    }
}
