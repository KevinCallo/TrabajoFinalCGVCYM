using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HelicopterAttack
{
    public class PlayerHeli : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 CenterPosition;

        [SerializeField]
        private GameObject m_FireParticle;
        [SerializeField]
        private GameObject m_CrashParticle;
        [SerializeField]
        private GameObject m_SmokeParticle;
        [SerializeField]
        private GameObject m_HitParticle;
        [SerializeField]
        private GameObject m_BulletPrefab;
        [SerializeField]
        private GameObject m_RocketPrefab;
        public GameObject RocketPrefab => m_RocketPrefab;

        [SerializeField]
        private Transform[] m_FirePoints;

        [HideInInspector]
        private float m_FireTimer = 0;
        [HideInInspector]
        private float m_FireTimer2 = 0;

        [HideInInspector]
        public DamageControl m_MyDamage;
        public static PlayerHeli Current;
        [HideInInspector]
        public Vector3 m_TargetPoint;
        [SerializeField]
        private Transform m_TargetPointTransform;

        [HideInInspector]
        public int m_RocketFireNum = 0;

        public Transform m_CamTransform;
        [HideInInspector]
        public Vector3 m_MoveInput;
        [HideInInspector]
        public Vector2 m_BodyAngle;
        [HideInInspector]
        public Vector2 m_ViewAngle;
        public Transform m_BodyBase;

        [Header("Camera Settings")]
        public bool m_IsFirstPerson = false;
        [SerializeField]
        private Vector3 m_ThirdPersonOffset = new Vector3(4f, 4f, -20f);
        [SerializeField]
        private Vector3 m_FirstPersonOffset = new Vector3(0f, 1.2f, 2.5f);
        private Vector3 m_CurrentCamOffset;

        void Awake()
        {
            Current = this;
        }

        void Start()
        {
            m_MyDamage = GetComponent<DamageControl>();
            m_CamTransform.SetParent(null);
            m_CurrentCamOffset = m_IsFirstPerson ? m_FirstPersonOffset : m_ThirdPersonOffset;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_MyDamage.Damage <= 0)
            {
                GameObject obj = Instantiate(m_CrashParticle);
                obj.transform.position = transform.position;
                obj.transform.localScale = 2 * Vector3.one;

                GameControl.m_Current.HandleGameOver();

                gameObject.SetActive(false);
            }

            // Toggle 1st Person / 3rd Person camera view with V, C or Tab
            if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Tab))
            {
                m_IsFirstPerson = !m_IsFirstPerson;
            }

            m_MoveInput = Vector3.zero;
            if (InputControl.m_Main != null)
            {
                m_MoveInput = InputControl.m_Main.m_Movement;
            }

            m_ViewAngle.y -= 3 * InputControl.m_Main.m_Look.y;
            m_ViewAngle.x += 3 * InputControl.m_Main.m_Look.x;

            m_ViewAngle.y = Mathf.Clamp(m_ViewAngle.y, -60, 60);



            Quaternion targetRotation = Quaternion.Euler(.2f * m_ViewAngle.y, m_ViewAngle.x, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2 * Time.deltaTime);


            float range = 300;
            m_TargetPoint = m_CamTransform.position + range * m_CamTransform.forward;
            Ray ray = new Ray(m_CamTransform.position, m_CamTransform.forward);
            RaycastHit[] hits1 = Physics.RaycastAll(ray, range);
            if (hits1.Length > 0)
            {
                RaycastHit closestHit = hits1[0];

                foreach (RaycastHit h in hits1)
                {
                    if (h.distance < closestHit.distance)
                    {
                        closestHit = h;
                    }
                }

                m_TargetPoint = closestHit.point;
            }
            m_TargetPointTransform.position = m_TargetPoint;

            HandleShooting();


            m_BodyBase.localRotation = Quaternion.Lerp(m_BodyBase.localRotation, Quaternion.Euler(20 * m_MoveInput.z, 0, -30 * m_MoveInput.x), 2 * Time.deltaTime);



        }

        public void HandleShooting()
        {
            m_FireTimer -= Time.deltaTime;
            if (m_FireTimer <= 0)
                m_FireTimer = 0;

            m_FireTimer2 -= Time.deltaTime;
            if (m_FireTimer2 <= 0)
                m_FireTimer2 = 0;


            if (InputControl.m_Main.m_Fire)
            {
                if (m_FireTimer == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject obj = Instantiate(m_BulletPrefab);
                        obj.transform.position = m_FirePoints[i].position;
                        obj.transform.forward = (m_TargetPoint - m_FirePoints[i].position).normalized;

                        Projectile_Base proj = obj.GetComponent<Projectile_Base>();
                        if (proj != null)
                        {
                            proj.Creator = gameObject;
                        }

                        obj = Instantiate(m_FireParticle);
                        obj.transform.position = m_FirePoints[i].position;
                        obj.transform.SetParent(m_FirePoints[i]);
                        Destroy(obj, 2);
                    }


                    m_FireTimer = .1f;
                }
            }

            if (InputControl.m_Main.m_Fire2)
            {
                if (m_FireTimer2 == 0)
                {

                    GameObject obj = Instantiate(m_RocketPrefab);
                    obj.transform.position = m_FirePoints[m_RocketFireNum].position;
                    obj.transform.forward = (m_TargetPoint - m_FirePoints[m_RocketFireNum].position).normalized;

                    Projectile_Base proj = obj.GetComponent<Projectile_Base>();
                    if (proj != null)
                    {
                        proj.Creator = gameObject;
                    }

                    obj = Instantiate(m_FireParticle);
                    obj.transform.position = m_FirePoints[m_RocketFireNum].position;
                    obj.transform.SetParent(m_FirePoints[m_RocketFireNum]);
                    Destroy(obj, 2);

                    if (m_RocketFireNum == 0)
                        m_RocketFireNum = 1;
                    else
                        m_RocketFireNum = 0;

                    m_FireTimer2 = .4f;
                }
            }

        }

        void FixedUpdate()
        {
            Rigidbody body = GetComponent<Rigidbody>();
            body.velocity += transform.rotation * (2f * m_MoveInput);
            body.velocity -= 0.04f * body.velocity;


        }

        void LateUpdate()
        {
            Vector3 targetOffset = m_IsFirstPerson ? m_FirstPersonOffset : m_ThirdPersonOffset;
            m_CurrentCamOffset = Vector3.Lerp(m_CurrentCamOffset, targetOffset, 8f * Time.deltaTime);

            Quaternion faceRotation = Quaternion.Euler(m_ViewAngle.y, m_ViewAngle.x, 0);
            Vector3 targetPos = transform.position + (faceRotation * m_CurrentCamOffset);
            m_CamTransform.position = targetPos;
            m_CamTransform.rotation = faceRotation;
        }
    }
}
