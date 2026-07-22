using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelicopterAttack
{
    public class Explosion : MonoBehaviour
    {

        // Use this for initialization
        public float Radius = 5;
        public float M_Damage = 10;
        void Start()
        {
            // Add dynamic explosion light flash
            Light expLight = gameObject.AddComponent<Light>();
            expLight.type = LightType.Point;
            expLight.color = new Color(1.0f, 0.65f, 0.25f);
            expLight.intensity = 8.0f;
            expLight.range = 30.0f;
            expLight.shadows = LightShadows.Soft;
            Destroy(expLight, 0.4f);


            Collider[] colls = Physics.OverlapSphere(transform.position, Radius);
            foreach (Collider col in colls)
            {
                if (col.gameObject.tag == "Player")
                {

                }
                else if (col.gameObject.tag == "Block")
                {
                    Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForceAtPosition(col.gameObject.transform.position - transform.forward, transform.position);
                    }

                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(M_Damage, transform.forward, 1);
                    }
                }
            }

            //Destroy(gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}