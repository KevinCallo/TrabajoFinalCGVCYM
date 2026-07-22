using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HelicopterAttack
{
    public class DayNightCycle : MonoBehaviour
    {
        [Header("Cycle Configuration")]
        [Tooltip("Time in seconds for a full 24-hour day-night cycle.")]
        public float m_CycleDurationInSeconds = 60.0f;

        [Header("Sun Reference")]
        public Light m_SunLight;

        [Header("Sun Light Colors")]
        public Color m_NoonSunColor = new Color(1.0f, 0.95f, 0.85f);
        public Color m_SunsetSunColor = new Color(1.0f, 0.45f, 0.15f);
        public Color m_NightSunColor = new Color(0.15f, 0.25f, 0.5f);

        [Header("Ambient Lighting Colors")]
        public Color m_NoonAmbientSky = new Color(0.6f, 0.75f, 0.95f);
        public Color m_SunsetAmbientSky = new Color(0.85f, 0.45f, 0.25f);
        public Color m_NightAmbientSky = new Color(0.35f, 0.42f, 0.55f);

        [Header("Fog Colors")]
        public Color m_NoonFogColor = new Color(0.65f, 0.75f, 0.88f);
        public Color m_SunsetFogColor = new Color(0.8f, 0.5f, 0.35f);
        public Color m_NightFogColor = new Color(0.12f, 0.15f, 0.25f);

        [Header("Intensities")]
        public float m_NoonSunIntensity = 1.4f;
        public float m_NightSunIntensity = 0.45f;
        public float m_NoonAmbientIntensity = 1.3f;
        public float m_NightAmbientIntensity = 0.65f;

        private float m_TimeOfDayNormalized = 0.25f; // Starts in morning (0.25 = 9AM, 0.5 = Noon, 0.75 = Sunset, 1.0/0.0 = Midnight)

        void Start()
        {
            if (m_SunLight == null)
            {
                Light[] lights = FindObjectsOfType<Light>();
                foreach (Light l in lights)
                {
                    if (l.type == LightType.Directional)
                    {
                        m_SunLight = l;
                        break;
                    }
                }
            }

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.003f;
        }

        void Update()
        {
            if (m_CycleDurationInSeconds <= 0f) m_CycleDurationInSeconds = 60f;

            // Advance time of day
            m_TimeOfDayNormalized += (Time.deltaTime / m_CycleDurationInSeconds);
            if (m_TimeOfDayNormalized >= 1.0f)
            {
                m_TimeOfDayNormalized -= 1.0f;
            }

            // Calculate sun rotation (0.25 = 90 deg = sunrise, 0.5 = 180 deg = noon, 0.75 = 270 deg = sunset)
            float sunAngleX = (m_TimeOfDayNormalized * 360.0f) - 90.0f;
            if (m_SunLight != null)
            {
                m_SunLight.transform.rotation = Quaternion.Euler(sunAngleX, 45.0f, 0.0f);
            }

            // Calculate day/night factor (0 = Night, 1 = Noon)
            float sunDot = Mathf.Sin(m_TimeOfDayNormalized * Mathf.PI * 2.0f); // Positive during day, negative during night
            float dayFactor = Mathf.Clamp01((sunDot + 0.2f) / 1.2f);
            float sunsetFactor = Mathf.Clamp01(1.0f - Mathf.Abs(sunDot - 0.1f) * 3f);

            // Interpolate Sun Color & Intensity
            if (m_SunLight != null)
            {
                Color currentSunColor = Color.Lerp(m_NightSunColor, m_NoonSunColor, dayFactor);
                currentSunColor = Color.Lerp(currentSunColor, m_SunsetSunColor, sunsetFactor);
                m_SunLight.color = currentSunColor;
                m_SunLight.intensity = Mathf.Lerp(m_NightSunIntensity, m_NoonSunIntensity, dayFactor);
            }

            // Interpolate Ambient Sky Color & Intensity
            Color currentAmbient = Color.Lerp(m_NightAmbientSky, m_NoonAmbientSky, dayFactor);
            currentAmbient = Color.Lerp(currentAmbient, m_SunsetAmbientSky, sunsetFactor);

            RenderSettings.ambientSkyColor = currentAmbient;
            RenderSettings.ambientEquatorColor = Color.Lerp(m_NightAmbientSky * 0.7f, m_NoonAmbientSky * 0.8f, dayFactor);
            RenderSettings.ambientGroundColor = Color.Lerp(new Color(0.02f, 0.02f, 0.05f), new Color(0.4f, 0.35f, 0.3f), dayFactor);
            RenderSettings.ambientIntensity = Mathf.Lerp(m_NightAmbientIntensity, m_NoonAmbientIntensity, dayFactor);

            // Interpolate Fog Color
            Color currentFog = Color.Lerp(m_NightFogColor, m_NoonFogColor, dayFactor);
            currentFog = Color.Lerp(currentFog, m_SunsetFogColor, sunsetFactor);
            RenderSettings.fogColor = currentFog;

            // Attach NightCameraEffect to main camera for key toggle (N / B)
            Camera mainCam = Camera.main;
            if (mainCam != null && mainCam.GetComponent<NightCameraEffect>() == null)
            {
                mainCam.gameObject.AddComponent<NightCameraEffect>();
            }
        }
    }
}
