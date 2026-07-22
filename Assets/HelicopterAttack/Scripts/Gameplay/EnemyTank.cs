using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HelicopterAttack
{
    public class EnemyTank : MonoBehaviour
    {
        public GameObject m_Explosion;
        public GameObject m_RocketPrefab;
        public Transform m_FirePoint;

        [Header("AI Movement Settings")]
        public float m_MoveSpeed = 4.0f;
        public float m_RotateSpeed = 3.0f;
        public float m_DetectionRange = 300.0f;
        public float m_MinDistance = 15.0f;

        [Header("Weapon Settings")]
        public float m_FireInterval = 2.5f;
        public float m_RocketDamage = 15.0f;
        public float m_RocketSpeed = 65.0f;

        private float m_FireTimer = 0f;
        private bool dead = false;

        void Start()
        {
            if (GameControl.m_Current != null)
            {
                GameControl.m_Current.m_MaxTargetCount++;
            }
            m_FireTimer = Random.Range(0.5f, 2.0f);
        }

        void Update()
        {
            if (dead) return;

            DamageControl damage = GetComponent<DamageControl>();
            if (damage != null && damage.IsDead)
            {
                HandleDeath();
                return;
            }

            HandleAIMovementAndCombat();
        }

        private void HandleAIMovementAndCombat()
        {
            if (PlayerHeli.Current == null || !PlayerHeli.Current.gameObject.activeInHierarchy) return;

            Vector3 targetPos = PlayerHeli.Current.transform.position;
            Vector3 directionToPlayer = targetPos - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer <= m_DetectionRange)
            {
                // Rotate tank towards player (yaw only)
                Vector3 lookDir = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
                if (lookDir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, m_RotateSpeed * Time.deltaTime);
                }

                // Move towards player if further than min distance
                if (distanceToPlayer > m_MinDistance)
                {
                    transform.position += transform.forward * m_MoveSpeed * Time.deltaTime;
                }

                // Rocket Shooting logic
                m_FireTimer -= Time.deltaTime;
                if (m_FireTimer <= 0f)
                {
                    m_FireTimer = m_FireInterval;
                    FireRocket(targetPos);
                }
            }
        }

        private void FireRocket(Vector3 targetPosition)
        {
            GameObject rocketPrefabToUse = m_RocketPrefab;
            if (rocketPrefabToUse == null && PlayerHeli.Current != null)
            {
                rocketPrefabToUse = PlayerHeli.Current.RocketPrefab;
            }

            if (rocketPrefabToUse == null) return;

            Vector3 spawnPos = (m_FirePoint != null) ? m_FirePoint.position : transform.position + transform.forward * 2.5f + Vector3.up * 1.5f;
            Vector3 fireDirection = (targetPosition + Vector3.up * 0.5f - spawnPos).normalized;

            GameObject rocket = Instantiate(rocketPrefabToUse, spawnPos, Quaternion.LookRotation(fireDirection));
            Projectile_Base proj = rocket.GetComponent<Projectile_Base>();
            if (proj != null)
            {
                proj.Creator = gameObject;
                proj.Speed = m_RocketSpeed;
                proj.Damage = m_RocketDamage;
            }
        }

        private void HandleDeath()
        {
            if (m_Explosion != null)
            {
                GameObject obj = Instantiate(m_Explosion);
                obj.transform.position = transform.position;
                obj.transform.localScale = 2 * Vector3.one;
            }

            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyKilled();
            }
            else if (GameControl.m_Current != null)
            {
                GameControl.m_Current.m_TargetDestroyedCount++;
            }

            dead = true;
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            Destroy(gameObject);
        }
    }
}
