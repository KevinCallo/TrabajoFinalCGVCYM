using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelicopterAttack
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Custom Audio Clips (Optional - Procedural Audio used if empty)")]
        public AudioClip m_MusicClip;
        public AudioClip m_HelicopterEngineClip;
        public AudioClip m_MachineGunClip;
        public AudioClip m_MissileLaunchClip;
        public AudioClip m_HeliHitClip;
        public AudioClip m_TankExplosionClip;

        [Header("Volume Controls")]
        [Range(0f, 1f)] public float m_MasterVolume = 1f;
        [Range(0f, 1f)] public float m_MusicVolume = 0.5f;
        [Range(0f, 1f)] public float m_SfxVolume = 0.8f;

        private AudioSource m_MusicSource;
        private AudioSource m_HeliEngineSource;
        private AudioSource m_MachineGunSource;
        private AudioSource m_MissileSource;
        private AudioSource m_SfxSource;

        private bool m_IsMachineGunPlaying = false;
        private bool m_IsMissilePlaying = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitAudioSources();
        }

        void Start()
        {
            UpdateVolumeFromPrefs();
            GenerateFallbackClipsIfNeeded();
            StartAmbientMusic();
            StartHelicopterEngine();
        }

        private void InitAudioSources()
        {
            m_MusicSource = gameObject.AddComponent<AudioSource>();
            m_MusicSource.loop = true;
            m_MusicSource.playOnAwake = false;

            m_HeliEngineSource = gameObject.AddComponent<AudioSource>();
            m_HeliEngineSource.loop = true;
            m_HeliEngineSource.playOnAwake = false;

            m_MachineGunSource = gameObject.AddComponent<AudioSource>();
            m_MachineGunSource.loop = true;
            m_MachineGunSource.playOnAwake = false;

            m_MissileSource = gameObject.AddComponent<AudioSource>();
            m_MissileSource.loop = true;
            m_MissileSource.playOnAwake = false;

            m_SfxSource = gameObject.AddComponent<AudioSource>();
            m_SfxSource.loop = false;
            m_SfxSource.playOnAwake = false;
        }

        public void UpdateVolumeFromPrefs()
        {
            m_MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            AudioListener.volume = m_MasterVolume;

            if (m_MusicSource != null) m_MusicSource.volume = m_MusicVolume * m_MasterVolume;
            if (m_HeliEngineSource != null) m_HeliEngineSource.volume = 0.4f * m_MasterVolume;
            if (m_MachineGunSource != null) m_MachineGunSource.volume = m_SfxVolume * m_MasterVolume;
            if (m_MissileSource != null) m_MissileSource.volume = m_SfxVolume * m_MasterVolume;
            if (m_SfxSource != null) m_SfxSource.volume = m_SfxVolume * m_MasterVolume;
        }

        void Update()
        {
            UpdateVolumeFromPrefs();

            // Only check shooting audio during gameplay when player is alive
            if (PlayerHeli.Current != null && PlayerHeli.Current.gameObject.activeInHierarchy && GameControl.m_Current != null && GameControl.m_Current.m_GameState == 0)
            {
                HandleShootingAudio();
            }
            else
            {
                StopShootingAudio();
            }
        }

        private void HandleShootingAudio()
        {
            // 2. Machine Gun Audio (Looping while Left Click held, stops when released)
            bool isFiringMG = InputControl.m_Main != null && InputControl.m_Main.m_Fire;
            if (isFiringMG)
            {
                if (!m_IsMachineGunPlaying)
                {
                    m_MachineGunSource.Play();
                    m_IsMachineGunPlaying = true;
                }
            }
            else
            {
                if (m_IsMachineGunPlaying)
                {
                    m_MachineGunSource.Stop();
                    m_IsMachineGunPlaying = false;
                }
            }

            // 3. Missile Launch Audio (Looping while Right Click held, stops when released)
            bool isFiringMissile = InputControl.m_Main != null && InputControl.m_Main.m_Fire2;
            if (isFiringMissile)
            {
                if (!m_IsMissilePlaying)
                {
                    m_MissileSource.Play();
                    m_IsMissilePlaying = true;
                }
            }
            else
            {
                if (m_IsMissilePlaying)
                {
                    m_MissileSource.Stop();
                    m_IsMissilePlaying = false;
                }
            }
        }

        private void StopShootingAudio()
        {
            if (m_IsMachineGunPlaying)
            {
                m_MachineGunSource.Stop();
                m_IsMachineGunPlaying = false;
            }
            if (m_IsMissilePlaying)
            {
                m_MissileSource.Stop();
                m_IsMissilePlaying = false;
            }
        }

        // 1. Ambient Background Music
        public void StartAmbientMusic()
        {
            if (m_MusicSource != null && !m_MusicSource.isPlaying)
            {
                m_MusicSource.clip = m_MusicClip;
                m_MusicSource.Play();
            }
        }

        // 4. Helicopter Engine / Rotor Sound
        public void StartHelicopterEngine()
        {
            if (m_HeliEngineSource != null && !m_HeliEngineSource.isPlaying)
            {
                m_HeliEngineSource.clip = m_HelicopterEngineClip;
                m_HeliEngineSource.Play();
            }
        }

        // 5. Impact sound when player helicopter takes damage
        public void PlayHeliDamageSound()
        {
            if (m_SfxSource != null && m_HeliHitClip != null)
            {
                m_SfxSource.PlayOneShot(m_HeliHitClip, 0.9f * m_MasterVolume);
            }
        }

        // 6. Tank Destruction Explosion Sound
        public void PlayTankExplosionSound()
        {
            if (m_SfxSource != null && m_TankExplosionClip != null)
            {
                m_SfxSource.PlayOneShot(m_TankExplosionClip, 1.0f * m_MasterVolume);
            }
        }

        #region Procedural Audio Generators (Fallbacks if no custom WAV/MP3 assigned)
        private void GenerateFallbackClipsIfNeeded()
        {
            if (m_MusicClip == null) m_MusicClip = CreateAmbientMusicClip();
            if (m_HelicopterEngineClip == null) m_HelicopterEngineClip = CreateHeliEngineClip();
            if (m_MachineGunClip == null) m_MachineGunClip = CreateMachineGunClip();
            if (m_MissileLaunchClip == null) m_MissileLaunchClip = CreateMissileLaunchClip();
            if (m_HeliHitClip == null) m_HeliHitClip = CreateHeliHitClip();
            if (m_TankExplosionClip == null) m_TankExplosionClip = CreateTankExplosionClip();

            m_MusicSource.clip = m_MusicClip;
            m_HeliEngineSource.clip = m_HelicopterEngineClip;
            m_MachineGunSource.clip = m_MachineGunClip;
            m_MissileSource.clip = m_MissileLaunchClip;
        }

        private AudioClip CreateAmbientMusicClip()
        {
            int sampleRate = 44100;
            float duration = 4.0f;
            int sampleCount = (int)(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float bass = Mathf.Sin(2 * Mathf.PI * 110f * t) * 0.15f;
                float chord = (Mathf.Sin(2 * Mathf.PI * 220f * t) + Mathf.Sin(2 * Mathf.PI * 277.18f * t) + Mathf.Sin(2 * Mathf.PI * 329.63f * t)) * 0.05f;
                float lfo = (Mathf.Sin(2 * Mathf.PI * 0.5f * t) + 1f) * 0.5f;
                samples[i] = (bass + chord) * lfo;
            }

            AudioClip clip = AudioClip.Create("AmbientMusic", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateHeliEngineClip()
        {
            int sampleRate = 44100;
            float duration = 1.0f;
            int sampleCount = (int)(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float rotorPulse = Mathf.Pow(Mathf.Sin(2 * Mathf.PI * 12f * t), 4f); // 12 Hz rotor blade thumps
                float hum = Mathf.Sin(2 * Mathf.PI * 65f * t) * 0.3f;
                float noise = (Random.value * 2f - 1f) * 0.1f;
                samples[i] = (hum + noise) * rotorPulse * 0.4f;
            }

            AudioClip clip = AudioClip.Create("HeliEngine", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateMachineGunClip()
        {
            int sampleRate = 44100;
            float duration = 0.5f;
            int sampleCount = (int)(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float burst = Mathf.Repeat(t * 15f, 1.0f); // 15 shots/sec
                float env = Mathf.Exp(-burst * 12f);
                float noise = (Random.value * 2f - 1f);
                float tone = Mathf.Sin(2 * Mathf.PI * 180f * t);
                samples[i] = (noise * 0.7f + tone * 0.3f) * env * 0.5f;
            }

            AudioClip clip = AudioClip.Create("MachineGun", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateMissileLaunchClip()
        {
            int sampleRate = 44100;
            float duration = 1.0f;
            int sampleCount = (int)(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float noise = (Random.value * 2f - 1f);
                float env = Mathf.Sin(t / duration * Mathf.PI);
                float lowPass = Mathf.Sin(2 * Mathf.PI * (120f + 150f * t) * t);
                samples[i] = (noise * 0.6f + lowPass * 0.4f) * env * 0.6f;
            }

            AudioClip clip = AudioClip.Create("MissileLaunch", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateHeliHitClip()
        {
            int sampleRate = 44100;
            float duration = 0.4f;
            int sampleCount = (int)(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 8f);
                float metal = Mathf.Sin(2 * Mathf.PI * 450f * t) + Mathf.Sin(2 * Mathf.PI * 820f * t);
                float noise = (Random.value * 2f - 1f);
                samples[i] = (metal * 0.5f + noise * 0.5f) * env * 0.7f;
            }

            AudioClip clip = AudioClip.Create("HeliHit", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateTankExplosionClip()
        {
            int sampleRate = 44100;
            float duration = 1.2f;
            int sampleCount = (int)(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 3.5f);
                float noise = (Random.value * 2f - 1f);
                float boom = Mathf.Sin(2 * Mathf.PI * 55f * t) * env;
                samples[i] = (noise * 0.7f + boom * 0.5f) * env * 0.8f;
            }

            AudioClip clip = AudioClip.Create("TankExplosion", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
        #endregion
    }
}
