using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HelicopterAttack
{
    public class PauseUI : MonoBehaviour
    {
        public static PauseUI Current;
        public static bool IsPaused = false;

        [SerializeField]
        private GameObject m_PausePanel;

        void Awake()
        {
            Current = this;
        }

        void Start()
        {
            IsPaused = false;
            Time.timeScale = 1.0f;
            if (m_PausePanel != null)
            {
                m_PausePanel.SetActive(false);
            }
        }

        void Update()
        {
            // Toggle pause with Escape or P key
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                // Only allow pause during active gameplay
                if (GameControl.m_Current != null && GameControl.m_Current.m_GameState == 0)
                {
                    TogglePause();
                }
            }
        }

        public void TogglePause()
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            IsPaused = true;
            Time.timeScale = 0f;
            if (m_PausePanel != null)
            {
                m_PausePanel.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            IsPaused = false;
            Time.timeScale = 1.0f;
            if (m_PausePanel != null)
            {
                m_PausePanel.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void BtnResume()
        {
            ResumeGame();
        }

        public void BtnRestart()
        {
            IsPaused = false;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BtnMainMenu()
        {
            IsPaused = false;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Scene_MainMenu");
        }

        public void BtnExit()
        {
            Application.Quit();
        }
    }
}
