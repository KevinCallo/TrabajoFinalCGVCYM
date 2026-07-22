using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HelicopterAttack
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance;

        [Header("Match Settings")]
        [Tooltip("Total duration of the campaign match in seconds (300 = 5 minutes).")]
        public float m_MatchDurationSeconds = 300.0f;
        public int m_TotalWaves = 5;

        [Header("Current Wave Progress")]
        public float m_TimeRemaining = 300.0f;
        public int m_CurrentWave = 1;
        public int m_KillsInCurrentWave = 0;
        public int m_TargetKillsForCurrentWave = 0;
        public int m_TotalKills = 0;
        public bool m_IsMatchActive = true;

        private List<EnemyTank>[] m_WaveTankGroups;
        private string m_WaveBannerText = "";
        private float m_WaveBannerTimer = 0f;

        private GUIStyle m_TimerStyle;
        private GUIStyle m_WaveBannerStyle;
        private Texture2D m_HudBgTexture;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            m_TimeRemaining = m_MatchDurationSeconds;
            m_IsMatchActive = true;
            m_TotalKills = 0;

            InitGUIStyles();
            OrganizeSceneTanksIntoWaves();
            TriggerWave(1);
        }

        private void OrganizeSceneTanksIntoWaves()
        {
            m_WaveTankGroups = new List<EnemyTank>[m_TotalWaves];
            for (int i = 0; i < m_TotalWaves; i++)
            {
                m_WaveTankGroups[i] = new List<EnemyTank>();
            }

            // Find all pre-placed tanks in the scene
            EnemyTank[] sceneTanks = FindObjectsOfType<EnemyTank>();

            // Ensure ground snapping for all pre-placed tanks so none float
            foreach (EnemyTank tank in sceneTanks)
            {
                if (tank == null) continue;
                Vector3 pos = tank.transform.position;
                if (Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
                {
                    pos.y = hit.point.y + 0.1f;
                    tank.transform.position = pos;
                }
            }

            if (sceneTanks.Length == 0) return;

            // Distribute pre-placed scene tanks evenly across the 5 campaign waves
            for (int i = 0; i < sceneTanks.Length; i++)
            {
                int assignedWave = i % m_TotalWaves;
                m_WaveTankGroups[assignedWave].Add(sceneTanks[i]);
            }

            // Deactivate all tanks for future waves (Waves 2..5)
            for (int waveIdx = 1; waveIdx < m_TotalWaves; waveIdx++)
            {
                foreach (EnemyTank tank in m_WaveTankGroups[waveIdx])
                {
                    if (tank != null)
                    {
                        tank.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void InitGUIStyles()
        {
            m_HudBgTexture = MakeTexture(2, 2, new Color(0.08f, 0.1f, 0.15f, 0.85f));

            m_TimerStyle = new GUIStyle();
            m_TimerStyle.fontSize = 18;
            m_TimerStyle.fontStyle = FontStyle.Bold;
            m_TimerStyle.alignment = TextAnchor.MiddleCenter;
            m_TimerStyle.normal.textColor = Color.yellow;

            m_WaveBannerStyle = new GUIStyle();
            m_WaveBannerStyle.fontSize = 26;
            m_WaveBannerStyle.fontStyle = FontStyle.Bold;
            m_WaveBannerStyle.alignment = TextAnchor.MiddleCenter;
            m_WaveBannerStyle.normal.textColor = new Color(1.0f, 0.5f, 0.1f);
        }

        void Update()
        {
            if (!m_IsMatchActive || GameControl.m_Current == null || GameControl.m_Current.m_GameState != 0)
                return;

            m_TimeRemaining -= Time.deltaTime;

            if (m_WaveBannerTimer > 0f)
            {
                m_WaveBannerTimer -= Time.deltaTime;
            }

            if (m_TimeRemaining <= 0f)
            {
                m_TimeRemaining = 0f;
                m_IsMatchActive = false;
                GameControl.m_Current.HandleWin();
            }
        }

        public void TriggerWave(int waveNumber)
        {
            m_CurrentWave = waveNumber;
            m_KillsInCurrentWave = 0;

            int waveIdx = waveNumber - 1;
            List<EnemyTank> currentWaveTanks = (m_WaveTankGroups != null && waveIdx < m_WaveTankGroups.Length) ? m_WaveTankGroups[waveIdx] : new List<EnemyTank>();

            // Activate pre-placed tanks for this wave
            int activeCount = 0;
            foreach (EnemyTank tank in currentWaveTanks)
            {
                if (tank != null)
                {
                    tank.gameObject.SetActive(true);
                    tank.m_DetectionRange = 350.0f;
                    activeCount++;
                }
            }

            m_TargetKillsForCurrentWave = Mathf.Max(1, activeCount);
            m_WaveBannerText = "¡OLEADA " + m_CurrentWave + " / " + m_TotalWaves + " INICIADA!\nOBJETIVO: ELIMINAR " + m_TargetKillsForCurrentWave + " TANQUES DE ESTA ZONA";
            m_WaveBannerTimer = 4.0f;
        }

        public void OnEnemyKilled()
        {
            if (!m_IsMatchActive) return;

            m_KillsInCurrentWave++;
            m_TotalKills++;

            if (GameControl.m_Current != null)
            {
                GameControl.m_Current.m_TargetDestroyedCount = m_TotalKills;
            }

            // Advance to next wave when all tanks in current wave are destroyed
            if (m_KillsInCurrentWave >= m_TargetKillsForCurrentWave)
            {
                if (m_CurrentWave < m_TotalWaves)
                {
                    TriggerWave(m_CurrentWave + 1);
                }
                else
                {
                    m_IsMatchActive = false;
                    if (GameControl.m_Current != null)
                    {
                        GameControl.m_Current.HandleWin();
                    }
                }
            }
        }

        void OnGUI()
        {
            if (!m_IsMatchActive || GameControl.m_Current == null || GameControl.m_Current.m_GameState != 0)
                return;

            // Draw 5-minute timer and Wave Kill Progress HUD at top center
            float width = 340f;
            float height = 65f;
            float xPos = (Screen.width - width) * 0.5f;
            float yPos = 15f;

            GUI.DrawTexture(new Rect(xPos, yPos, width, height), m_HudBgTexture);
            GUI.Box(new Rect(xPos, yPos, width, height), "");

            int minutes = Mathf.FloorToInt(m_TimeRemaining / 60f);
            int seconds = Mathf.FloorToInt(m_TimeRemaining % 60f);
            string timerText = string.Format("⏱ TIEMPO: {0:00}:{1:00}  |  OLEADA: {2}/{3}", minutes, seconds, m_CurrentWave, m_TotalWaves);
            string killText = string.Format("🎯 OBJETIVO: {0} / {1} TANQUES ELIMINADOS", m_KillsInCurrentWave, m_TargetKillsForCurrentWave);

            GUI.Label(new Rect(xPos, yPos + 6, width, 22), timerText, m_TimerStyle);
            GUI.Label(new Rect(xPos, yPos + 32, width, 22), killText, m_TimerStyle);

            // Draw Big Banner Announcement when new wave triggers
            if (m_WaveBannerTimer > 0f)
            {
                float bannerW = 650f;
                float bannerH = 80f;
                float bannerX = (Screen.width - bannerW) * 0.5f;
                float bannerY = Screen.height * 0.22f;

                GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, bannerH), m_HudBgTexture);
                GUI.Label(new Rect(bannerX, bannerY + 12, bannerW, bannerH), m_WaveBannerText, m_WaveBannerStyle);
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
