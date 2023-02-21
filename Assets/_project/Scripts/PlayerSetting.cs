using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace AstralAbyss
{
    public class PlayerSetting : MonoBehaviour
    {
        public AudioMixer Mixer;
        public Slider MusicSlider;
        public Slider SFXSlider;
        public Slider SensitivitySlider;
        public const string MIXER_MUSIC = "MusicVolume";
        public const string MIXER_SFX = "SFXVolume";

        private void Awake()
        {
            MusicSlider.onValueChanged.AddListener(SetMusicVolume);
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);
            SensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }
        private void Start()
        {
            MusicSlider.value = PlayerPrefs.GetFloat(GameManager.MUSIC_KEY, 1f);
            SFXSlider.value = PlayerPrefs.GetFloat(GameManager.SFX_KEY, 1f);
            SensitivitySlider.value = PlayerPrefs.GetFloat(GameManager.SENSITIVITY_KEY, 0.25f);
        }
        void SetSensitivity(float value)
        {
            GameManager.Instance.Sensitivity = value;
        }
        void SetMusicVolume(float value)
        {
            Mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
        }
        void SetSFXVolume(float value)
        {
            Mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
        }
        private void OnDisable()
        {
            PlayerPrefs.SetFloat(GameManager.MUSIC_KEY, MusicSlider.value);
            PlayerPrefs.SetFloat(GameManager.SFX_KEY, SFXSlider.value);
            PlayerPrefs.SetFloat(GameManager.SENSITIVITY_KEY, SensitivitySlider.value);
        }
    }
}
