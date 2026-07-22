using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HelicopterAttack
{
    public class InputControl : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 m_WorldAimPosition;

        //--inputs
        [HideInInspector]
        public Vector3 m_Movement;
        [HideInInspector]
        public Vector3 m_Look;
        [HideInInspector]
        public bool m_Fire;
        [HideInInspector]
        public bool m_Fire2;


        public bool m_mobileControl = false;

        public static InputControl m_Main;

        void Awake()
        {
            m_Main = this;
            #if !UNITY_ANDROID && !UNITY_IOS
            m_mobileControl = false;
            #endif
        }
        // Start is called before the first frame update
        void Start()
        {
            m_WorldAimPosition = Vector3.zero;

            if (!m_mobileControl)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            m_Movement = Vector3.zero;
            m_Look = Vector3.zero;
            m_Fire = false;
            m_Fire2 = false;

            if (PauseUI.IsPaused)
            {
                return;
            }

            if (m_mobileControl)
            {
                if (Joystick.m_Main != null)
                {
                    m_Movement.x = Joystick.m_Main.LeftStick.StickDirection.x;
                    m_Movement.z = Joystick.m_Main.LeftStick.StickDirection.y;

                    m_Look.x = Joystick.m_Main.RightStick.StickDirection.x;
                    m_Look.y = Joystick.m_Main.RightStick.StickDirection.y;

                    if (Joystick.m_Main.ButtonA.Hold)
                        m_Fire = true;

                    if (Joystick.m_Main.ButtonB.Hold)
                        m_Fire2 = true;
                }
            }
            else
            {
                // Toggle cursor lock with Escape key
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }

                // Relock cursor when left clicking while unlocked
                if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                // Horizontal (A/D) and Vertical (W/S) translation
                m_Movement.x = Input.GetAxis("Horizontal");
                m_Movement.z = Input.GetAxis("Vertical");

                // Height control (Space / E = Up, LeftShift / LeftControl / Q / C = Down)
                float heightInput = 0f;
                if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
                {
                    heightInput += 1.0f;
                }
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.C))
                {
                    heightInput -= 1.0f;
                }
                m_Movement.y = heightInput;

                // Mouse look / aim (Mouse X, Mouse Y)
                m_Look.y = Input.GetAxis("Mouse Y");
                m_Look.x = Input.GetAxis("Mouse X");

                // Weapons (Left click = Primary bullet, Right click = Secondary rocket)
                if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.J))
                    m_Fire = true;

                if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.K))
                    m_Fire2 = true;
            }

            Vector2 horizMovement = Vector2.ClampMagnitude(new Vector2(m_Movement.x, m_Movement.z), 1.0f);
            m_Movement.x = horizMovement.x;
            m_Movement.z = horizMovement.y;
            m_Look = Vector3.ClampMagnitude(m_Look, 1.0f);
        }
    }
}