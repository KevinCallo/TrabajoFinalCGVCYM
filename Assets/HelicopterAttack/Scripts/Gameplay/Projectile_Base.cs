using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HelicopterAttack
{
    public class Projectile_Base : MonoBehaviour
    {

        public GameObject HitParticlePrefab1;
        [HideInInspector]
        public GameObject Creator;

        public float Speed = 100;
        public float Damage = 1;

        public GameObject m_DetachingParticle;
        // Use this for initialization
        void Start()
        {
            // Add dynamic glowing light to projectile
            Light projLight = gameObject.AddComponent<Light>();
            projLight.type = LightType.Point;
            projLight.color = new Color(1.0f, 0.75f, 0.35f);
            projLight.intensity = 3.5f;
            projLight.range = 12.0f;
        }

        void Update()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.4f, transform.forward, Speed * Time.deltaTime);
            foreach (RaycastHit hit in hits)
            {
                Collider col = hit.collider;

                // Ignore creator, creator's children, and self-shooting player
                if (Creator != null)
                {
                    if (col.gameObject == Creator || col.transform.IsChildOf(Creator.transform))
                        continue;
                    if (Creator.GetComponent<PlayerHeli>() != null && col.GetComponentInParent<PlayerHeli>() != null)
                        continue;
                }
                else if (PlayerHeli.Current != null && (col.gameObject == PlayerHeli.Current.gameObject || col.transform.IsChildOf(PlayerHeli.Current.transform)))
                {
                    continue;
                }

                DamageControl d = col.gameObject.GetComponentInParent<DamageControl>();
                if (d == null)
                {
                    d = col.gameObject.GetComponent<DamageControl>();
                }

                if (d != null)
                {
                    d.ApplyDamage(Damage, transform.forward, 1);
                    Destroyed(hit.point);
                    return;
                }
                else if (!col.isTrigger && col.gameObject.tag != "IgnoreBullet")
                {
                    Destroyed(hit.point);
                    return;
                }
            }

            transform.position += Speed * Time.deltaTime * transform.forward;
        }

        public virtual void Destroyed(Vector3 pos)
        {

            GameObject obj = Instantiate(HitParticlePrefab1);
            obj.transform.position = pos;
            Destroy(obj, 6);
            //obj.transform.localScale = 0.4f * Vector3.one;

            if (m_DetachingParticle != null)
            {
                m_DetachingParticle.transform.SetParent(null);
                Destroy(m_DetachingParticle, 6);
            }


            Destroy(gameObject);
        }
    }
}