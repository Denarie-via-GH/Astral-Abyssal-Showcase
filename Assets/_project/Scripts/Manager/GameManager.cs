using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace AstralAbyss
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        #region VARIEABLE
        [Header("Internal Property")]
        public GameObject LoadingScreen;
        public bool InGame = false;
        public bool IsPause = false;
        [SerializeField] private float _introductionDialogueDelay = 0.01f;
        public Dialogue IntroductionDialogue;

        [Header("Scene Property")]
        [SerializeField] private float _eventSceneLoadDelay = 5f;
        List<AsyncOperation> _loadingScenes = new List<AsyncOperation>();

        [Header("Setting Property")]
        public float Sensitivity = 0.25f;
        [SerializeField] AudioMixer Mixer;
        public const string SENSITIVITY_KEY = "sensitivity";
        public const string MUSIC_KEY = "MusicVolume";
        public const string SFX_KEY = "SFXVolume";
        #endregion

        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
            #endregion

            LoadSetting();

            //---> Load Main Menu <---//
            SceneManager.LoadSceneAsync((int)SceneIndex.MAIN_MENU, LoadSceneMode.Additive);
        }
        void LoadSetting()
        {
            float sensitivitySetting = PlayerPrefs.GetFloat(SENSITIVITY_KEY, 0.25f);
            float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
            float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

            Sensitivity = sensitivitySetting;
            Mixer.SetFloat(PlayerSetting.MIXER_MUSIC, Mathf.Log10(musicVolume) * 20);
            Mixer.SetFloat(PlayerSetting.MIXER_SFX, Mathf.Log10(sfxVolume) * 20);
        }

        #region PAUSE FUNCTION (SCRAP)
        //public void PauseGame()
        //{
        //    //---> set property
        //    IsPause = true;
        //    Time.timeScale = 0;
        //    //---> pause listenser
        //    AudioListener.pause = true;
        //    //---> unlock mouse
        //    Cursor.visible = true;
        //    Cursor.lockState = CursorLockMode.None;
        //    //---> open pause menu
        //    UIManager.Instance.TogglePauseMenu(true);
        //    FirstPersonCamera.Instance.ResetEventSystem();
        //}
        //public void UnpauseGame()
        //{
        //    //---> set property
        //    IsPause = false;
        //    Time.timeScale = 1;
        //    //---> play listener
        //    AudioListener.pause = false;
        //    //---> lock mouse
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.Locked;
        //    //---> close pause menu
        //    UIManager.Instance.TogglePauseMenu(false);
        //}
        #endregion

        #region GAME INITIALIZATION
        public IEnumerator LoadOutro()
        {
            //---> open loading screen and wait for transition <---//
            LoadingScreen.SetActive(true);
            LoadingScreen.GetComponent<Animator>().Play("FadeIn");
            yield return new WaitForSeconds(1f);

            InGame = false;
            //---> unload main menu then load intro scene <---//
            _loadingScenes.Add(SceneManager.UnloadSceneAsync(EventManager.Instance.CurrentEventScene));
            _loadingScenes.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.ORBITER));
            _loadingScenes.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.CONTROL_ROOM));

            _loadingScenes.Add(SceneManager.LoadSceneAsync((int)SceneIndex.OUTRO_SCENE, LoadSceneMode.Additive));

            //---> close loading screen after all scene is loaded <---//
            StartCoroutine(GetSceneLoadProgress());
        }
        public IEnumerator LoadMenu()
        {
            //---> open loading screen and wait for transition <---//
            LoadingScreen.SetActive(true);
            LoadingScreen.GetComponent<Animator>().Play("FadeIn");
            yield return new WaitForSeconds(1f);

            //---> unload main menu then load intro scene <---//
            _loadingScenes.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.OUTRO_SCENE));
            _loadingScenes.Add(SceneManager.LoadSceneAsync((int)SceneIndex.MAIN_MENU, LoadSceneMode.Additive));

            //---> close loading screen after all scene is loaded <---//
            StartCoroutine(GetSceneLoadProgress());
        }
        public IEnumerator LoadIntro()
        {
            //---> open loading screen and wait for transition <---//
            LoadingScreen.SetActive(true);
            LoadingScreen.GetComponent<Animator>().Play("FadeIn");
            yield return new WaitForSeconds(1f);

            //---> unload main menu then load intro scene <---//
            _loadingScenes.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.MAIN_MENU));
            _loadingScenes.Add(SceneManager.LoadSceneAsync((int)SceneIndex.INTRO_SCENE, LoadSceneMode.Additive));

            //---> close loading screen after all scene is loaded <---//
            StartCoroutine(GetSceneLoadProgress());
        }
        public IEnumerator LoadGame()
        {
            //---> open loading screen and wait for transition <---//
            LoadingScreen.SetActive(true);
            LoadingScreen.GetComponent<Animator>().Play("FadeIn");
            yield return new WaitForSeconds(1f);

            //---> unload intro scene then load into game <---//
            _loadingScenes.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.INTRO_SCENE));
            _loadingScenes.Add(SceneManager.LoadSceneAsync((int)SceneIndex.ORBITER, LoadSceneMode.Additive));
            _loadingScenes.Add(SceneManager.LoadSceneAsync((int)SceneIndex.CONTROL_ROOM, LoadSceneMode.Additive));
            
            //---> close loading screen after all scene is loaded <---//
            StartCoroutine(GetSceneLoadProgress());
        }
        public IEnumerator GameInitiation()
        {
            yield return new WaitForSeconds(_introductionDialogueDelay);
            UIManager.Instance.PlayDialogue(IntroductionDialogue);
            EventManager.Instance.InitiateStandbyPhase();
        }
        #endregion

        #region LOAD EVENT SCENE
        public IEnumerator LoadEventScene(SceneIndex index)
        {
            //---> open loading screen and wait for transition <---//
            InGame = false;
            LoadingScreen.SetActive(true);
            LoadingScreen.GetComponent<Animator>().Play("FadeIn");

            yield return new WaitForSeconds(_eventSceneLoadDelay);

            // unload previous event scene
            if (EventManager.Instance.PreviousEventScene != -1)
            {
                EventInstanceController.Instance.DisableEventInstance();
                _loadingScenes.Add(SceneManager.UnloadSceneAsync(EventManager.Instance.PreviousEventScene));
            }
            // load new event scene
            _loadingScenes.Add(SceneManager.LoadSceneAsync((int)index, LoadSceneMode.Additive));

            // Check scene progress
            StartCoroutine(GetSceneLoadProgress());
            StartCoroutine(GetEventInitializationProgress());
        }

        //public IEnumerator GetUnloadSceneProgress()
        //{
        //    for (int i = 0; i < _unloadingScenes.Count; i++)
        //    {
        //        while (!_unloadingScenes[i].isDone)
        //        {
        //            yield return null;
        //        }
        //    }
        //}

        public IEnumerator GetSceneLoadProgress()
        {
            //---> loop check progress of loading scenes <---//
            for (int i = 0; i < _loadingScenes.Count; i++)
            {
                while (!_loadingScenes[i].isDone)
                {
                    yield return null;
                }
            }

            //---> close loading screen after transition when confirm all scene loaded <---//
            LoadingScreen.GetComponent<Animator>().Play("FadeOut");
            yield return new WaitForSeconds(1);
            LoadingScreen.SetActive(false);
        }
        public IEnumerator GetEventInitializationProgress()
        {
            while(EventInstanceController.Instance == null || !EventInstanceController.Instance.IsInitializeDone)
            {
                yield return null;
            }

            EventManager.Instance.StartCoroutine(EventManager.Instance.StopTransition());
            EventManager.Instance.InitiateEventPhase();
            InGame = true;
        }
        #endregion

        #region GLOBAL FUNCTION
        public void Request_FreezeControl(bool value)
        {
            ControlCentral.Instance.IsFreezed = value;
        }
        public void Request_FreezeOrbiter(bool value)
        {
            OrbiterCore.Instance.IsFreezed = value;
        }
        public void Request_FreezePlayer(bool value)
        {
            PlayerMain.Instance.IsFreezed = value;
        }
        public void Request_FreezePlayerDelay(float delay)
        {
            StartCoroutine(DelayFreezePlayer(delay));
        }
        IEnumerator DelayFreezePlayer(float delay)
        {
            PlayerMain.Instance.IsFreezed = true;
            yield return new WaitForSeconds(delay);
            PlayerMain.Instance.IsFreezed = false;
        }
        #endregion
    }
}
