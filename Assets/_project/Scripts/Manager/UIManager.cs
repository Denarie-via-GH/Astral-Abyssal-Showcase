using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AstralAbyss
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }


        [Header("Internal Property")]
        [SerializeField] GameObject _essentialOverlays;
        DialogueManager _dialogueManager;
        ControlScheme _inputUI;
        Animator _animator;

        [Header("Crosshair Property")]
        [SerializeField] Image _crosshair;
        [SerializeField] List<Sprite> _crosshairSprites;

        [Header("Additional UI Property")]
        public GameObject CutScreen;
        [SerializeField] GameObject _pauseMenu;
        [SerializeField] GameObject _collisionWarningOverlay;
        [SerializeField] GameObject _UTRInterface;
        [SerializeField] GameObject _interfaceUIOverlay;
        [SerializeField] GameObject _analyzeBarOverlay;
        [SerializeField] Slider _objectiveProgress;
        public bool ToggleUTRInterface = false;
        public List<GameObject> PromptDisplays = new List<GameObject>();

        void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
            #endregion

            //---> Inititate Propertt <---//
            _inputUI = new ControlScheme();
            _dialogueManager = GetComponent<DialogueManager>();
            _animator = GetComponent<Animator>();

            //---> Set cursor hidden <---//
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        void Update()
        {
            UpdateDynamicCrosshair();
            if (ToggleUTRInterface)
            {
                UpdateUTRInterfaceElements();
            }
        }

        #region TRANSITION (BLACK SCREEN)
        public void StartTransitionScreen()
        {
            _animator.CrossFade("StartTransition", 0, 0);
            //StartCoroutine(StartTransitionProcess(optionalFreeze));
        }
        //IEnumerator StartTransitionProcess(bool optionalFreeze)
        //{
        //    GameManager.Instance.Request_FreezePlayer(optionalFreeze);
        //    _animator.CrossFade("StartTransition", 0, 0);
        //    yield return null;
        //}
        public void CloseTransitionScreen()
        {
            _animator.CrossFade("ExitTransition", 0, 0);
            //StartCoroutine(CloseTransitionProcess(optionalFreeze));
        }
        //IEnumerator CloseTransitionProcess(bool optionalFreeze)
        //{
        //    _animator.CrossFade("ExitTransition", 0, 0);
        //    yield return new WaitForSeconds(0.5f);
        //    GameManager.Instance.Request_FreezePlayer(optionalFreeze);
        //}
        #endregion

        public void PlayDialogue(Dialogue d)
        {
            _dialogueManager.SetNewDialogue(d);
        }
        public GameObject AddPromptToUI(GameObject obj, string objName, string objAction)
        {
            GameObject newPrompt = Instantiate(obj, transform);
            newPrompt.transform.SetSiblingIndex(1);
            newPrompt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objName;
            newPrompt.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = objAction;
            newPrompt.SetActive(false);
            return newPrompt;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        #region UI DISPLAY
        public void UpdateDynamicCrosshair()
        {
            if (ControlCentral.Instance.InDockMode)
            {
                _crosshair.gameObject.SetActive(false);
            }
            else if (FirstPersonCamera.Instance.IsPointerOnCanvas)
            {
                _crosshair.gameObject.SetActive(true);
                _crosshair.sprite = _crosshairSprites[2];
            }
            else if (FirstPersonCamera.Instance.CanInteractObject)
            {
                _crosshair.gameObject.SetActive(true);
                _crosshair.sprite = _crosshairSprites[1];
            }
            else
            {
                _crosshair.gameObject.SetActive(true);
                _crosshair.sprite = _crosshairSprites[0];
            }
        }
        public void UpdateObjectiveDisplay(string objective)
        {
            _interfaceUIOverlay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objective;
        }
        public void DisplayObjectiveBar(bool value)
        {
            _interfaceUIOverlay.transform.GetChild(1).gameObject.SetActive(value);
        }
        public void DisplayEssentialOverlay(bool value)
        {
            _essentialOverlays.SetActive(value);
        }
        public void SetUTRInterfaceToggle(bool value)
        {
            ToggleUTRInterface = value;
            _UTRInterface.SetActive(value);
        }
        public void UpdateUTRInterfaceElements()
        {
            ObjectiveInstance Objective = OrbiterCore.Instance.LookingObject;
            if (Objective != null)
            {
                if (Vector3.Distance(OrbiterCore.Instance.transform.position, Objective.transform.position) <= Objective.ActiveRangeDistance && !Objective.IsObjectiveComplete)
                {
                    _UTRInterface.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "detected objective";
                    if (OrbiterCore.Instance.LookingObject.GetObjectiveType() == "ANALYZE")
                        _UTRInterface.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "hold [g] to perform analysis on objective";
                    else if(OrbiterCore.Instance.LookingObject.GetObjectiveType() == "RESEARCH")
                        _UTRInterface.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "press [t] to transfer objective inside spacelab";
                }
                else
                {
                    _UTRInterface.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "no objective detected";
                    _UTRInterface.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "no action available";
                }
            }
            else
            {
                _UTRInterface.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "no objective detected";
                _UTRInterface.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "no action available";
            }
        }
        public void UpdateCollisionWarning(bool left, bool right, bool up, bool down)
        {
            if (ControlCentral.Instance.InDockMode && !ControlCentral.Instance.InMinimap && !ControlCentral.Instance.InRadar)
            {
                _collisionWarningOverlay.SetActive(true);

                if (left || right || up || down)
                    AudioManager.Instance.PlaySource(_collisionWarningOverlay.GetComponent<AudioSource>());
                else
                    AudioManager.Instance.StopSource(_collisionWarningOverlay.GetComponent<AudioSource>());

                _collisionWarningOverlay.transform.GetChild(0).gameObject.SetActive(left);
                _collisionWarningOverlay.transform.GetChild(1).gameObject.SetActive(right);
                _collisionWarningOverlay.transform.GetChild(2).gameObject.SetActive(up);
                _collisionWarningOverlay.transform.GetChild(3).gameObject.SetActive(down);
            }
            else
            {
                _collisionWarningOverlay.SetActive(false);
                AudioManager.Instance.StopSource(_collisionWarningOverlay.GetComponent<AudioSource>());
            }
        }
        public void DisplayRecordingOverlay(bool value, float progress)
        {
            _analyzeBarOverlay.SetActive(value);
            _analyzeBarOverlay.transform.GetChild(0).GetComponent<Slider>().value = progress;
        }
        public void UpdateObjectiveProgress(float value)
        {
            _objectiveProgress.value = value;
            _objectiveProgress.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"data collection progress {_objectiveProgress.value.ToString("F")}";
        }
        #endregion

        #region ENABLE/DISABLE
        private void OnEnable()
        {
            _inputUI.Enable();
            _inputUI.UI.NextLine.performed += ctx => DialogueManager.Instance.RequestNextLine();
        }
        private void OnDisable()
        {
            _inputUI.Disable();
            _inputUI.UI.NextLine.performed -= ctx => DialogueManager.Instance.RequestNextLine();
        }
        #endregion
    }
}
