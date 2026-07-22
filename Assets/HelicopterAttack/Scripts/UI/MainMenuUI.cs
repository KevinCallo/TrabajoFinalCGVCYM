using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace HelicopterAttack
{
    public class MainMenuUI : MonoBehaviour
    {
        public static int SelectedCampaignMode = 0; // 0 = Oleadas (5 min / 5 waves), 1 = Supervivencia (1 min / infinite tanks)
        public enum MenuState { Main, CampaignSelect, Volume }

        [Header("Menu State")]
        public MenuState m_CurrentState = MenuState.Main;

        [Header("Canvas Panel References (Optional)")]
        public GameObject m_MainPanel;
        public GameObject m_CampaignPanel;
        public GameObject m_VolumePanel;
        public Slider m_VolumeSlider;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        public float m_MasterVolume = 1.0f;

        [SerializeField, Space]
        private DataStorage m_DataStorage;

        private GUIStyle m_TitleStyle;
        private GUIStyle m_SubTitleStyle;
        private GUIStyle m_ButtonStyle;
        private GUIStyle m_LabelStyle;
        private Texture2D m_MenuBgTexture;

        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (m_DataStorage != null)
            {
                m_DataStorage.Load();
            }

            m_MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            AudioListener.volume = m_MasterVolume;

            if (m_VolumeSlider != null)
            {
                m_VolumeSlider.value = m_MasterVolume;
                m_VolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }

            UpdatePanelVisibility();
            InitGUIStyles();
        }

        private void InitGUIStyles()
        {
            m_MenuBgTexture = MakeTexture(2, 2, new Color(0.08f, 0.1f, 0.15f, 0.88f));

            m_TitleStyle = new GUIStyle();
            m_TitleStyle.fontSize = 32;
            m_TitleStyle.fontStyle = FontStyle.Bold;
            m_TitleStyle.alignment = TextAnchor.MiddleCenter;
            m_TitleStyle.normal.textColor = new Color(0.2f, 0.8f, 1f);

            m_SubTitleStyle = new GUIStyle();
            m_SubTitleStyle.fontSize = 18;
            m_SubTitleStyle.fontStyle = FontStyle.Bold;
            m_SubTitleStyle.alignment = TextAnchor.MiddleCenter;
            m_SubTitleStyle.normal.textColor = Color.yellow;

            m_ButtonStyle = new GUIStyle();
            m_ButtonStyle.fontSize = 18;
            m_ButtonStyle.fontStyle = FontStyle.Bold;
            m_ButtonStyle.alignment = TextAnchor.MiddleCenter;
            m_ButtonStyle.normal.textColor = Color.white;
            m_ButtonStyle.hover.textColor = Color.cyan;

            m_LabelStyle = new GUIStyle();
            m_LabelStyle.fontSize = 16;
            m_LabelStyle.fontStyle = FontStyle.Bold;
            m_LabelStyle.alignment = TextAnchor.MiddleLeft;
            m_LabelStyle.normal.textColor = Color.white;
        }

        public void OpenCampaignMenu()
        {
            m_CurrentState = MenuState.CampaignSelect;
            UpdatePanelVisibility();
        }

        public void OpenVolumeMenu()
        {
            m_CurrentState = MenuState.Volume;
            UpdatePanelVisibility();
        }

        public void OpenMainMenu()
        {
            m_CurrentState = MenuState.Main;
            UpdatePanelVisibility();
        }

        public void SetMasterVolume(float vol)
        {
            m_MasterVolume = Mathf.Clamp01(vol);
            AudioListener.volume = m_MasterVolume;
            PlayerPrefs.SetFloat("MasterVolume", m_MasterVolume);
            PlayerPrefs.Save();
        }

        public void SelectCampaign(int campaignIndex)
        {
            Time.timeScale = 1.0f;
            switch (campaignIndex)
            {
                case 0:
                default:
                    SceneManager.LoadScene("Scene_1");
                    break;
            }
        }

        public void BtnStart()
        {
            SelectCampaign(0);
        }

        public void BtnPlay()
        {
            OpenCampaignMenu();
        }

        public void BtnExit()
        {
            Application.Quit();
        }

        public void BtnMap(int num)
        {
            SelectCampaign(num);
        }

        private void UpdatePanelVisibility()
        {
            if (m_MainPanel != null) m_MainPanel.SetActive(m_CurrentState == MenuState.Main);
            if (m_CampaignPanel != null) m_CampaignPanel.SetActive(m_CurrentState == MenuState.CampaignSelect);
            if (m_VolumePanel != null) m_VolumePanel.SetActive(m_CurrentState == MenuState.Volume);
        }

        void OnGUI()
        {
            // If custom Canvas panels are assigned, skip OnGUI fallback
            if (m_MainPanel != null || m_CampaignPanel != null || m_VolumePanel != null) return;

            float panelWidth = 420f;
            float panelHeight = 360f;
            float xPos = (Screen.width - panelWidth) * 0.5f;
            float yPos = (Screen.height - panelHeight) * 0.5f;

            GUI.DrawTexture(new Rect(xPos, yPos, panelWidth, panelHeight), m_MenuBgTexture);
            GUI.Box(new Rect(xPos, yPos, panelWidth, panelHeight), "");

            GUI.Label(new Rect(xPos, yPos + 20, panelWidth, 40), "HELICÓPTERO ATTACK", m_TitleStyle);

            switch (m_CurrentState)
            {
                case MenuState.Main:
                    GUI.Label(new Rect(xPos, yPos + 65, panelWidth, 30), "MENÚ PRINCIPAL", m_SubTitleStyle);

                    if (GUI.Button(new Rect(xPos + 60, yPos + 110, panelWidth - 120, 45), "▶  JUGAR (CAMPAÑAS)"))
                    {
                        OpenCampaignMenu();
                    }

                    if (GUI.Button(new Rect(xPos + 60, yPos + 170, panelWidth - 120, 45), "🔊  VOLUMEN"))
                    {
                        OpenVolumeMenu();
                    }

                    if (GUI.Button(new Rect(xPos + 60, yPos + 230, panelWidth - 120, 45), "🚪  SALIR"))
                    {
                        BtnExit();
                    }
                    break;

                case MenuState.CampaignSelect:
                    GUI.Label(new Rect(xPos, yPos + 65, panelWidth, 30), "SELECCIÓN DE CAMPAÑA", m_SubTitleStyle);

                    if (GUI.Button(new Rect(xPos + 40, yPos + 110, panelWidth - 80, 40), "🚁 CAMPAÑA 1: OLEADAS"))
                    {
                        SelectedCampaignMode = 0;
                        SelectCampaign(0);
                    }

                    if (GUI.Button(new Rect(xPos + 40, yPos + 160, panelWidth - 80, 40), "🔥 CAMPAÑA 2: SUPERVIVENCIA (1 MIN)"))
                    {
                        SelectedCampaignMode = 1;
                        SelectCampaign(0);
                    }

                    if (GUI.Button(new Rect(xPos + 40, yPos + 210, panelWidth - 80, 40), "🏰 CAMPAÑA 3: FORTALEZA ENEMIGA"))
                    {
                        SelectedCampaignMode = 0;
                        SelectCampaign(0);
                    }

                    if (GUI.Button(new Rect(xPos + 60, yPos + 275, panelWidth - 120, 40), "⬅  VOLVER AL MENÚ"))
                    {
                        OpenMainMenu();
                    }
                    break;

                case MenuState.Volume:
                    GUI.Label(new Rect(xPos, yPos + 65, panelWidth, 30), "CONTROL DE VOLUMEN", m_SubTitleStyle);

                    GUI.Label(new Rect(xPos + 50, yPos + 120, panelWidth - 100, 30), "VOLUMEN GENERAL: " + Mathf.RoundToInt(m_MasterVolume * 100) + "%", m_LabelStyle);

                    float newVol = GUI.HorizontalSlider(new Rect(xPos + 50, yPos + 160, panelWidth - 100, 30), m_MasterVolume, 0f, 1f);
                    if (Mathf.Abs(newVol - m_MasterVolume) > 0.001f)
                    {
                        SetMasterVolume(newVol);
                    }

                    if (GUI.Button(new Rect(xPos + 60, yPos + 240, panelWidth - 120, 45), "⬅  VOLVER AL MENÚ"))
                    {
                        OpenMainMenu();
                    }
                    break;
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
