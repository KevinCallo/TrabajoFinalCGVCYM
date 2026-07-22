using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HelicopterAttack
{
    public class EnvironmentLighting : MonoBehaviour
    {
        [Header("Sun / Directional Light Settings")]
        public Color m_SunColor = new Color(1.0f, 0.95f, 0.85f);
        public float m_SunIntensity = 1.4f;
        public LightShadows m_ShadowType = LightShadows.Soft;

        [Header("Ambient & Environment Lighting")]
        public Color m_SkyColor = new Color(0.6f, 0.75f, 0.95f);
        public Color m_EquatorColor = new Color(0.7f, 0.7f, 0.75f);
        public Color m_GroundColor = new Color(0.4f, 0.35f, 0.3f);
        public float m_AmbientIntensity = 1.3f;

        [Header("Atmospheric Fog")]
        public bool m_EnableFog = true;
        public Color m_FogColor = new Color(0.65f, 0.75f, 0.88f);
        public float m_FogDensity = 0.003f;

        void Awake()
        {
            ApplyLuminositySettings();
        }

        void Start()
        {
            ApplyLuminositySettings();
        }

        public void ApplyLuminositySettings()
        {
            // Configure Ambient Lighting
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = m_SkyColor * m_AmbientIntensity;
            RenderSettings.ambientEquatorColor = m_EquatorColor * m_AmbientIntensity;
            RenderSettings.ambientGroundColor = m_GroundColor * m_AmbientIntensity;
            RenderSettings.ambientIntensity = m_AmbientIntensity;

            // Find and configure main Directional Light (Sun)
            Light sunLight = GetComponent<Light>();
            if (sunLight == null || sunLight.type != LightType.Directional)
            {
                Light[] allLights = FindObjectsOfType<Light>();
                foreach (Light l in allLights)
                {
                    if (l.type == LightType.Directional)
                    {
                        sunLight = l;
                        break;
                    }
                }
            }

            if (sunLight != null)
            {
                sunLight.color = m_SunColor;
                sunLight.intensity = m_SunIntensity;
                sunLight.shadows = m_ShadowType;
                sunLight.shadowStrength = 0.85f;
            }

            // Configure Fog Settings for atmospheric depth
            if (m_EnableFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogColor = m_FogColor;
                RenderSettings.fogDensity = m_FogDensity;
            }
        }
    }
}
