using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace AstralAbyss
{
    public class ControlCentral : MonoBehaviour
    {
        public static ControlCentral Instance;

        [Header("Internal Property")]
        public bool IsFreezed = false;
        [SerializeField] Transform _dataEntryParent;
        [SerializeField] GameObject _archivedDataDisplay;
        [SerializeField] GameObject _dataEntryButtonPrefab;
        [SerializeField] TextMeshProUGUI _transferButtonText;
        [SerializeField] Dialogue _spacelabInSessionWarning;
        ControlScheme _inputPanel;
        Animator _controlPanelAnim;
        EventSystem _panelEventSystem;
        Vector2 _ringInput;
        float _cameraTiltInput;
        GameObject _referenceButton;

        [Header("Menu & UI")]
        public Menu CurrentMenu = Menu.Default;
        public enum Menu { Default, DataArchive, UTR, Manual }
        
        //public UTRMenu CurrentUTRMenu = UTRMenu.CameraRing;
        //public enum UTRMenu { Default, CameraRing, ManeuverRing, TransferRing }
        //[SerializeField] private List<GameObject> _UTRSystemPanels = new List<GameObject>();

        public List<DataArchive> DocumentedDataArchived = new List<DataArchive>();
        [SerializeField] private List<GameObject> _ManualPanels = new List<GameObject>();

        [Header("UTR Property")]
        public bool InDockMode;
        public bool InMinimap;
        public bool InRadar;
        public float RotationSpeed;
        //public bool IsHoldingRotation;
        //public bool IsHoldingThruster;
        //public int CurrentAxis;
        public EventHandler OnPerformAstralScan;
        public EventHandler OnExitAstralScan;
        ResearchObjective _targetTransfer;

        void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
            #endregion

            //---> Initiate Property <---//
            _inputPanel = new ControlScheme();
            _controlPanelAnim = GetComponent<Animator>();
            _panelEventSystem = GetComponentInChildren<EventSystem>();

            InDockMode = false;
            InMinimap = false;
            InRadar = false;
        }
        void Start()
        {
            _controlPanelAnim.CrossFade("OpenMainMenu", 0, 0);
        }
        void Update()
        {
            if (CurrentMenu == Menu.UTR && !IsFreezed)
            {
                if (InDockMode && !LabControl.Instance.IsLabInSession)
                {
                    GameManager.Instance.Request_FreezePlayer(true);

                    if (!OrbiterCore.Instance.IsAutoRotating)
                    {
                        _ringInput = PlayerMain.Instance.GetPlayerInput();
                        
                        //---> Rotate Camera <---//
                        if (_ringInput != Vector2.zero || _cameraTiltInput != 0)
                        {
                            AudioManager.Instance.PlaySource(OrbiterCore.Instance.RotateSource);
                            Quaternion H_Rotation = Quaternion.AngleAxis(RotationSpeed * _ringInput.x * Time.deltaTime, Vector3.up);
                            OrbiterCore.Instance.AddRotationToRing(H_Rotation);
                            Quaternion V_Rotation = Quaternion.AngleAxis(RotationSpeed * -_ringInput.y * Time.deltaTime, Vector3.right);
                            OrbiterCore.Instance.AddRotationToRing(V_Rotation);
                            Quaternion Extra_Rotation = Quaternion.AngleAxis(RotationSpeed * -_cameraTiltInput * Time.deltaTime, Vector3.forward);
                            OrbiterCore.Instance.AddRotationToRing(Extra_Rotation);
                        }
                        //---> Stop Camera <---//
                        else
                        {
                            AudioManager.Instance.StopSource(OrbiterCore.Instance.RotateSource);
                        }

                        //---> Move Orbiter <---//
                        if (Input.GetKey(KeyCode.Space))
                        {
                            Command_MoveOrbiter(false);
                        }
                        else if (Input.GetKeyUp(KeyCode.Space))
                        {
                            Command_StopOrbiter();
                        }

                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            Command_MoveOrbiter(true);
                        }
                        else if (Input.GetKeyUp(KeyCode.LeftShift))
                        {
                            Command_StopOrbiter();
                        }
                    }
                }

                if (OrbiterCore.Instance.LookingObject != null && OrbiterCore.Instance.LookingObject.InActiveRange)
                {
                    if (OrbiterCore.Instance.LookingObject.GetObjectiveType() == "ANALYZE")
                    {
                        AnalyzeObjective TEMP = OrbiterCore.Instance.LookingObject.GetComponent<AnalyzeObjective>();
                        if (Input.GetKey(KeyCode.G))
                        {
                            TEMP.Analyze();
                        }
                        else if (Input.GetKeyUp(KeyCode.G))
                        {
                            TEMP.StopAnalyze();
                        }
                    }
                    else if(OrbiterCore.Instance.LookingObject.GetObjectiveType() == "RESEARCH")
                    {
                        ResearchObjective TEMP = OrbiterCore.Instance.LookingObject.GetComponent<ResearchObjective>();
                        if (Input.GetKeyDown(KeyCode.T))
                        {
                            Debug.Log(TEMP.gameObject.name);
                            Command_TransferObject(TEMP);
                        }
                    }
                }
                

                #region SCRAP
                //switch (CurrentUTRMenu)
                //{
                //    //---> Update UTR Camera <---//
                //    case (UTRMenu.CameraRing):
                //        {
                //            if (InDockMode)
                //            {
                //                GameManager.Instance.Request_FreezePlayer(true);

                //                if (!OrbiterCore.Instance.IsAutoRotating)
                //                {
                //                    _ringInput = PlayerMain.Instance.GetPlayerInput();
                //                    if (_ringInput != Vector2.zero || _cameraTiltInput != 0)
                //                    {
                //                        AudioManager.Instance.PlaySource(OrbiterCore.Instance.RoverSource);
                //                        Quaternion H_Rotation = Quaternion.AngleAxis(RotationSpeed * _ringInput.x * Time.deltaTime, Vector3.up);
                //                        OrbiterCore.Instance.AddRotationToRing(0, H_Rotation);
                //                        Quaternion V_Rotation = Quaternion.AngleAxis(RotationSpeed * -_ringInput.y * Time.deltaTime, Vector3.right);
                //                        OrbiterCore.Instance.AddRotationToRing(0, V_Rotation);
                //                        Quaternion Extra_Rotation = Quaternion.AngleAxis(RotationSpeed * -_cameraTiltInput * Time.deltaTime, Vector3.forward);
                //                        OrbiterCore.Instance.AddRotationToRing(0, Extra_Rotation);
                //                    }
                //                    else
                //                    {
                //                        AudioManager.Instance.StopSource(OrbiterCore.Instance.RoverSource);
                //                    }

                //                    if (Input.GetKey(KeyCode.Space))
                //                    {
                //                        Command_MoveInDockMode();
                //                    }
                //                }
                //            }
                //        }
                //        break;
                //    //---> Update UTR Maneuver <---//
                //    case (UTRMenu.ManeuverRing):
                //        {
                //            if (FirstPersonCamera.Instance.SelectingButton && !OrbiterCore.Instance.IsAutoRotating)
                //            {
                //                //---> Holding maneuver button
                //                if (Input.GetMouseButton(0) && FirstPersonCamera.Instance.SelectingButton.gameObject == _referenceButton)
                //                {
                //                    if (IsHoldingRotation)
                //                    {
                //                        AudioManager.Instance.PlaySource(OrbiterCore.Instance.RotateSource);
                //                        OrbiterCore.Instance.RotateRingAxis(1, CurrentAxis);
                //                    }
                //                    if (IsHoldingThruster)
                //                    {
                //                        AudioManager.Instance.PlaySource(OrbiterCore.Instance.ThrusterSource);
                //                        OrbiterCore.Instance.Move();
                //                    }
                //                }
                //                //---> Release manuever button
                //                else if (Input.GetMouseButtonUp(0)) ReleaseManeuverButton();
                //            }
                //            else ReleaseManeuverButton();
                //        }
                //        break;
                //    //---> Update UTR Transfer <---//
                //    case (UTRMenu.TransferRing):
                //        {
                //            if (_targetTransfer == null)
                //            {
                //                _transferButtonText.text = "NO TRANSFER OBJECT";
                //            }
                //            else if (_targetTransfer != null && !LabControl.Instance.IsLabInSession)
                //            {
                //                _transferButtonText.text = "BEGIN TRANSFER";
                //            }
                //            else if (LabControl.Instance.IsLabInSession)
                //            {
                //                _transferButtonText.text = "SPACELAB IN SESSION";
                //            }
                //        }
                //        break;
                //}
                #endregion
            }
        }

        public void SetCriticalSystemError()
        {
            IsFreezed = true;
            _controlPanelAnim.CrossFade("CriticalSystem", 0, 0);
        }

        #region CAMERA SYSTEM
        public void Command_EnterDockMode()
        {
            if (LabControl.Instance.IsLabInSession)
            {
                UIManager.Instance.PlayDialogue(_spacelabInSessionWarning);
                return;
            }

            if (!InDockMode && !IsFreezed)
            {
                GameManager.Instance.Request_FreezePlayer(true);
                FirstPersonCamera.Instance.InitiateFocusTarget((int)CameraFocusIndex.UTR_CAMERA);
                UIManager.Instance.SetUTRInterfaceToggle(true);
                AudioManager.Instance.PlayGlobal((int)SFXClipIndex.DOCK_MODE);
                InDockMode = true;
                InMinimap = false;
                InRadar = false;
            }
        }
        public void Command_ExitDockMode()
        {
            if (InDockMode && !IsFreezed)
            {
                GameManager.Instance.Request_FreezePlayer(false);
                FirstPersonCamera.Instance.StopFocusCamera();
                UIManager.Instance.SetUTRInterfaceToggle(false);
                AudioManager.Instance.PlayGlobal((int)SFXClipIndex.DOCK_MODE);
                AudioManager.Instance.StopSource(OrbiterCore.Instance.RotateSource);
                AudioManager.Instance.StopSource(OrbiterCore.Instance.MoveSource);
                InDockMode = false;
                InMinimap = false;
                InRadar = false;
            }
        }
        public void Command_AstralScan()
        {
            if (InDockMode && !IsFreezed)
            {
                EventInstanceController.Instance.InitiateEnvironmentScan();
            }
        }
        public void Command_OverrideFocus(int index)
        {
            if (InDockMode && !IsFreezed)
            {
                if (index == 1)
                {
                    if (InMinimap)
                    {
                        FirstPersonCamera.Instance.OverrideFocusTarget((int)CameraFocusIndex.UTR_CAMERA);
                        InMinimap = false;
                        InRadar = false;
                    }
                    else if (!InMinimap)
                    {
                        FirstPersonCamera.Instance.OverrideFocusTarget((int)CameraFocusIndex.MINIMAP);
                        InMinimap = true;
                    }
                }
                else if (index == 2)
                {
                    if (InRadar) // from rader to camera/minimap
                    {
                        FirstPersonCamera.Instance.OverrideFocusTarget((int)CameraFocusIndex.UTR_CAMERA);
                        InRadar = false;
                        InMinimap = false;
                    }
                    else if (!InRadar) // from camera/minimap to radar
                    {
                        FirstPersonCamera.Instance.OverrideFocusTarget((int)CameraFocusIndex.RADAR);
                        InRadar = true;
                    }
                }
            }
        }
        public void Command_MoveOrbiter(bool isReverse)
        {
            if (InDockMode && !OrbiterCore.Instance.IsAutoRotating && !IsFreezed)
            {
                AudioManager.Instance.PlaySource(OrbiterCore.Instance.MoveSource);
                OrbiterCore.Instance.Move(isReverse);
            }
        }
        public void Command_StopOrbiter()
        {
            AudioManager.Instance.StopSource(OrbiterCore.Instance.MoveSource);
        }
        public void Command_TransferObject(ResearchObjective Target)
        {
            if (LabControl.Instance.IsLabInSession)
            {
                UIManager.Instance.PlayDialogue(_spacelabInSessionWarning);
                return;
            }

            if (!LabControl.Instance.IsLabInSession && !IsFreezed)
            {
                _targetTransfer = Target;
                LabControl.Instance.InitiateSpacelabOnline(_targetTransfer);
                _targetTransfer.InitiateObjectTransfer();
                Command_ExitDockMode();
            }
        }
        #endregion

        #region MANEUVER SYSTEM
        //public void Command_RotateManeuverRing(int axis)
        //{
        //    if (LabControl.Instance.IsLabInSession)
        //    {
        //        UIManager.Instance.PlayDialogue(_spacelabInSessionWarning);
        //        return;
        //    }

        //    if (!OrbiterCore.Instance.IsAutoRotating && !IsFreezed)
        //    {
        //        IsHoldingRotation = true;
        //        CurrentAxis = axis;
        //        _referenceButton = FirstPersonCamera.Instance.SelectingButton.gameObject;
        //    }
        //}
        //public void Command_MoveOrbiter()
        //{
        //    if (LabControl.Instance.IsLabInSession)
        //    {
        //        UIManager.Instance.PlayDialogue(_spacelabInSessionWarning);
        //        return;
        //    }

        //    if (!OrbiterCore.Instance.IsAutoRotating && !IsFreezed)
        //    {
        //        IsHoldingThruster = true;
        //        _referenceButton = FirstPersonCamera.Instance.SelectingButton.gameObject;
        //    }
        //}
        //void ReleaseManeuverButton()
        //{
        //    CurrentAxis = -1;
        //    IsHoldingRotation = false;
        //    IsHoldingThruster = false;
        //    _referenceButton = null;

        //    AudioManager.Instance.StopSource(OrbiterCore.Instance.RotateSource);
        //    AudioManager.Instance.StopSource(OrbiterCore.Instance.ThrusterSource);
        //}
        #endregion

        #region TRANSFER SYSTEM
        #endregion

        #region SUBSCRIPTION FUNCTION
        public void AddDataEntry(DataArchive entry)
        {
            DocumentedDataArchived.Add(entry); //EventInstanceController.Instance.EventInstanceData
            GameObject newDataEntry = Instantiate(_dataEntryButtonPrefab, _dataEntryParent);
            newDataEntry.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"Archive Entry [{DocumentedDataArchived.Count}]";
            Button newDataEntryButton = newDataEntry.GetComponent<Button>();
            newDataEntryButton.onClick.AddListener(delegate { UI_OpenDataPanel(entry.DataID); });
        }
        #endregion

        #region UI PANEL FUNCTION
        public void UI_ChangeMenu(int index)
        {
            switch (index)
            {
                case 0:
                    _panelEventSystem.SetSelectedGameObject(null);
                    _controlPanelAnim.CrossFade("OpenMainMenu", 0, 0);
                    CurrentMenu = Menu.Default;
                    break;
                case 1:
                    _panelEventSystem.SetSelectedGameObject(null);
                    _controlPanelAnim.CrossFade("OpenDataArchiveMenu", 0, 0);
                    CurrentMenu = Menu.DataArchive;
                    break;
                case 2:
                    _panelEventSystem.SetSelectedGameObject(null);
                    _controlPanelAnim.CrossFade("OpenUTRSystemMenu", 0, 0);
                    CurrentMenu = Menu.UTR;
                    break;
                case 3:
                    _panelEventSystem.SetSelectedGameObject(null);
                    _controlPanelAnim.CrossFade("OpenManualMenu", 0, 0);
                    CurrentMenu = Menu.Manual;
                    break;
            }
        }
        public void UI_OpenDataPanel(int index)
        {
            switch (CurrentMenu)
            {
                case (Menu.DataArchive):
                    DataArchive GetData = DocumentedDataArchived.Find(x => x.DataID == index);//DataArchive { DataID = index });
                    _archivedDataDisplay.SetActive(true);
                    _archivedDataDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{GetData.EntryTitle}";
                    _archivedDataDisplay.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Destination: {GetData.Ref_EventCode} \nLocation: {GetData.Ref_EventMapPosition} \nAstral Particle: {GetData.Ref_EventAstralParticle}";
                    _archivedDataDisplay.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = GetData.Data;
                    break;
                case (Menu.UTR):
                    //foreach (GameObject go in _UTRSystemPanels)
                    //    go.SetActive(false);
                    //_UTRSystemPanels[index].SetActive(true);
                    //if (index == 0) CurrentUTRMenu = UTRMenu.CameraRing;
                    //else if (index == 1) CurrentUTRMenu = UTRMenu.ManeuverRing;
                    //else if (index == 2) CurrentUTRMenu = UTRMenu.TransferRing;
                    break;
                case (Menu.Manual):
                    foreach (GameObject go in _ManualPanels)
                        go.SetActive(false);
                    _ManualPanels[index].SetActive(true);
                    break;
            }
        }
        #endregion

        #region Enable/Disable
        private void OnEnable()
        {
            _inputPanel.Enable();
            _inputPanel.ControlPanel.Tilt.performed += ctx => _cameraTiltInput = ctx.ReadValue<float>();
            _inputPanel.ControlPanel.Tilt.canceled += ctx => _cameraTiltInput = 0;
            _inputPanel.ControlPanel.AstralScan.performed += ctx => Command_AstralScan();
            _inputPanel.ControlPanel.Map.performed += ctx => Command_OverrideFocus(1);
            _inputPanel.ControlPanel.Radar.performed += ctx => Command_OverrideFocus(2);
            _inputPanel.UI.Exit.performed += ctx => Command_ExitDockMode();
        }
        private void OnDisable()
        {
            _inputPanel.Disable();
            _inputPanel.ControlPanel.Tilt.performed -= ctx => _cameraTiltInput = ctx.ReadValue<float>();
            _inputPanel.ControlPanel.Tilt.canceled -= ctx => _cameraTiltInput = 0;
            _inputPanel.ControlPanel.AstralScan.performed -= ctx => Command_AstralScan();
            _inputPanel.ControlPanel.Map.performed -= ctx => Command_OverrideFocus(1);
            _inputPanel.ControlPanel.Radar.performed -= ctx => Command_OverrideFocus(2);
            _inputPanel.UI.Exit.performed += ctx => Command_ExitDockMode();
        }
        #endregion
    }
}
