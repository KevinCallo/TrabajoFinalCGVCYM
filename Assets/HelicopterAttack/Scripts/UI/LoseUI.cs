using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HelicopterAttack
{
    public class LoseUI : MonoBehaviour
    {
        [SerializeField]
        public float m_AutoRestartDelay = 3.0f;
        private float m_Timer = 0f;

        void OnEnable()
        {
            m_Timer = m_AutoRestartDelay;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void Update()
        {
            m_Timer -= Time.unscaledDeltaTime;
            if (m_Timer <= 0f || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                BtnRestart();
            }
        }

        public void BtnRestart()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BtnExit()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Scene_MainMenu");
        }
    }
}
