using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

namespace AstralAbyss
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        //public class AudioArg
        //{
        //    public AudioSource targetSource;
        //    public int targetClip;
        //    public bool isOverlap = false;
        //}

        [Header("Internal Property")]
        public bool IsPlayingPrimaryAmbient = false;
        public bool IsPlayingSecondaryAmbient = false;
        public bool IsPlayingRandomClip = false;
        public bool IsEnableRandomClip = false;
        public float RandomClipIntervalTimer;

        [Header("AudioSource Property")]
        public AudioSource MusicSource;
        public AudioSource GlobalSource;
        public AudioSource InterfaceSource;
        public AudioSource PrimaryAmbientSource;
        public AudioSource SecondaryAmbientSource;
        public AudioSource RandomSource;

        [Header("AudioClips Propertyy")]
        public List<AudioClip> SFX_Clips;
        public List<AudioClip> UI_Clips;
        public List<AudioClip> AMBIENT_Clips;

        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else
                Instance = this;
            #endregion
        }

        private void Update()
        {
            if (IsPlayingRandomClip)
            {
                RandomClipIntervalTimer -= Time.deltaTime;
                if(RandomClipIntervalTimer <= 0)
                {
                    IsPlayingRandomClip = false;
                    StartPlayingRandomClip();
                }
            }
        }

        #region INTERNAL FUNCTION
        public void PauseSource(string type)
        {
            if (type == "internal")
            {
                GlobalSource.Pause();
                //if (InGameSource != null)
                //{
                //    InGameSource.Pause();
                //}
            }
            else if (type == "external")
            {
                MusicSource.Pause();
                InterfaceSource.Pause();
            }
        }
        public void UnpauseSource(string type)
        {
            if (type == "internal")
            {
                GlobalSource.UnPause();
                //if (InGameSource != null)
                //{
                //    InGameSource.UnPause();
                //}
            }
            else if (type == "external")
            {
                MusicSource.UnPause();
                InterfaceSource.UnPause();
            }
        }
        #endregion

        #region AUDIO FUNCTION
        public void PlayInterface(int Clip)
        {
           InterfaceSource.PlayOneShot(UI_Clips[Clip]);
        }
        public void PlayInterface(AudioClip Clip)
        {
            InterfaceSource.PlayOneShot(Clip);
        }
        public void PlayGlobal(int Clip)
        {
            GlobalSource.PlayOneShot(SFX_Clips[Clip]);
        }
        public void PlayGlobal(AudioClip Clip)
        {
            GlobalSource.PlayOneShot(Clip);
        }
        public void PlayGlobalDelay(AudioClip clip, float delay)
        {
            StartCoroutine(PlayGlobalDelayProcess(clip, delay));
        }
        IEnumerator PlayGlobalDelayProcess(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayGlobal(clip);
        }

        #region MANAGE AUDIO SOURCE
        public void PlaySource(AudioSource source, AudioClip clip, bool isOverlap)
        {
            if (isOverlap)
            {
                if (clip)
                    source.PlayOneShot(clip);
                else
                    source.PlayOneShot(source.clip);
            }
            else if (!isOverlap)
            {
                if (!source.isPlaying)
                {
                    if (clip) 
                        source.clip = clip;
                    source.Play();
                }
            }
        }
        public void PlaySource(AudioSource source, AudioClip clip)
        {
            PlaySource(source, clip, false);
        }
        public void PlaySource(AudioSource source, int index)
        {
            PlaySource(source, SFX_Clips[index], false);
        }
        public void PlaySource(AudioSource source, int index, bool isOverlap)
        {
            PlaySource(source, SFX_Clips[index], isOverlap);
        }
        public void PlaySource(AudioSource source)
        {
            PlaySource(source, null, false);
        }
        public void StopSource(AudioSource source)
        {
            if(source.isPlaying)
                source.Stop();
        }
        #endregion
        #endregion

        #region AMBIENT MANAGEMENT
        public void PlayPrimaryAmbient(int index)
        {
            Debug.Log($"PLAY AMBIENT {index}");
            PrimaryAmbientSource.clip = AMBIENT_Clips[index];
            PrimaryAmbientSource.loop = true;
            PrimaryAmbientSource.Play();
            IsPlayingPrimaryAmbient = true;
        }
        public void PlaySecondaryAmbient(int index, bool isLoop)
        {
            SecondaryAmbientSource.clip = AMBIENT_Clips[index];
            SecondaryAmbientSource.loop = isLoop;
            SecondaryAmbientSource.Play();
            IsPlayingSecondaryAmbient = true;
        }
        public void StopPrimaryAmbient()
        {
            PrimaryAmbientSource.Stop();
            IsPlayingPrimaryAmbient = false;
        }
        public void StopSecondaryAmbient()
        {
            SecondaryAmbientSource.Stop();
            IsPlayingSecondaryAmbient = false;
        }
        #endregion

        #region RANDOM CLIP EVENT
        public IEnumerator InitiateRandomClip(float startup)
        {
            IsEnableRandomClip = true;
            yield return new WaitForSeconds(startup);
            StartPlayingRandomClip();
        }
        public void StartPlayingRandomClip()
        {
            if (IsEnableRandomClip && !IsPlayingRandomClip)
            {
                int index = (int)DeltaUtilLib.DeltaUtil.ReturnRandomRange(0, EventInstanceController.Instance.RandomClips.Count - 1);
                RandomSource.clip = EventInstanceController.Instance.RandomClips[index];
                RandomClipIntervalTimer = RandomSource.clip.length + DeltaUtilLib.DeltaUtil.ReturnRandomRange(EventInstanceController.Instance.RandomClipIntervalMin, EventInstanceController.Instance.RandomClipIntervalMax);

                RandomSource.Play();
                IsPlayingRandomClip = true;
            }
        }
        public void StopPlayingRandomClip()
        {
            StopAllCoroutines();
            IsEnableRandomClip = false;
            IsPlayingRandomClip = false;
            RandomClipIntervalTimer = 0;
            RandomSource.clip = null;
            RandomSource.Stop();
        }
        #endregion

        #region SUBSCRIPTION FUNCTION
        //public void OnStandbyPhaseStart_StopRandomSFX(object o, EventArgs e)
        //{
        //    if (IsEnableRandomClip)
        //    {
        //        StopPlayingRandomClip();
        //    }
        //}
        //public void OnEventPhaseStart_PlayEventSceneAmbient(object o, EventArgs e)
        //{
        //    PlayPrimaryAmbient((int)EventInstanceController.Instance.CustomAmbient);
        //    if(EventInstanceController.Instance.EnableRandomClip)
        //    {
        //        StartCoroutine(InitiateRandomClip(DeltaUtilLib.DeltaUtil.ReturnRandomRange(EventInstanceController.Instance.RandomClipIntervalMin, EventInstanceController.Instance.RandomClipIntervalMax)));
        //    }
        //}
        #endregion

        #region ENABLE/DISABLE
        //void OnEnable()
        //{
        //EventManager.Instance.OnStandbyPhaseStart += OnStandbyPhaseStart_StopRandomSFX;
        //EventManager.Instance.OnEventPhaseStart += OnEventPhaseStart_PlayEventSceneAmbient;
        //}
        //void OnDisable()
        //{
        //    EventManager.Instance.OnStandbyPhaseStart -= OnStandbyPhaseStart_StopRandomSFX;
        //    EventManager.Instance.OnEventPhaseStart -= OnEventPhaseStart_PlayEventSceneAmbient;
        //}
        #endregion

        //private void OnEnable()
        //{
        //    MusicOn = Setting.musicsetting;
        //    SoundOn = Setting.soundsetting;
        //    MusicToggle.isOn = Setting.musicsetting;
        //    SoundToggle.isOn = Setting.soundsetting;
        //}
        //private void OnDisable()
        //{
        //    Setting.musicsetting = MusicOn;
        //    Setting.soundsetting = SoundOn;
        //}
    }
}
