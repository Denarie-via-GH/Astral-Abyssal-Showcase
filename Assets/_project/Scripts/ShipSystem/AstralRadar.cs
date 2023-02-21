using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AstralAbyss
{
    public class AstralRadar : MonoBehaviour
    {
        public static AstralRadar Instance;

        [Header("Panel Property")]
        [SerializeField] private Transform _DisplayPanel;
        [SerializeField] private Transform _ButtonParent;
        [SerializeField] private GameObject _scanButton;
        [SerializeField] private List<Button> _destinationButtons = new List<Button>();
        [SerializeField] private GameObject _eventButtonPrefab;

        [Header("Internal Property")]
        FullDimensionVisualizer _visualizerSystem;
        Animator _animator;

        [Header("Radar Property")]
        public List<EventInstance> AvailableEvents = new List<EventInstance>();
        public enum RadarType { Default, Event, Objective}
        public RadarType CurrentRadarType = RadarType.Default;
        public float ScanInterval;
        public float ScanTimer;
        public bool ToggleAutoScan = false;

        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else if(Instance == null)
                Instance = this;
            #endregion

            //---> Initiate Property <---//
            ToggleAutoScan = false;
            _animator = GetComponent<Animator>();
            _visualizerSystem = GetComponentInChildren<FullDimensionVisualizer>();
        }
        private void Update()
        {
            //---> Autoscan Timer <---//
            if (ToggleAutoScan)
            {
                ScanTimer += Time.deltaTime;
                if(ScanTimer >= ScanInterval)
                {
                    ScanTimer = 0;
                    InitiateRadarScan(1);
                }
            }
        }

        #region SUBSCRIPTION FUNCTION
        void OnStandbyPhaseStart_InitiateEventRadar(object o, EventArgs e)
        {
            InitiateRadar(RadarType.Event);
        }
        void OnEventPhaseStart_InitiateObjectiveRadar(object o, EventArgs e)
        {
            InitiateRadar(RadarType.Objective);
        }
        void OnCreatedAvailableEvents_PopulateEventsOnRadar(object o, EventManager.AvailableEventInstances e)
        {
            _scanButton.SetActive(false);

            //---> Assign reference events
            AvailableEvents.Clear();
            AvailableEvents = e.PassAvailableEvents;

            //---> Manage event visual display
            InitiateVisualizeRadar(RadarType.Event);
        }
        #endregion

        public void SetCriticalSystemError()
        {
            ToggleAutoScan = false;
            _animator.CrossFade("CriticalSystem",0,0);
        }

        #region RADAR FUNCTION
        public void InitiateRadar(RadarType type)
        {
            ResetRadarDisplay(RadarType.Event);
            ResetRadarDisplay(RadarType.Objective);
            CurrentRadarType = type;

            switch (CurrentRadarType)
            {
                case RadarType.Event:
                    _scanButton.SetActive(true);
                    ToggleAutoScan = false;
                    _animator.CrossFade("InitiateEventRadar", 0, 0);
                    break;
                case RadarType.Objective:
                    _scanButton.SetActive(false);
                    ToggleAutoScan = true;
                    _animator.CrossFade("InitiateObjectiveRadar", 0, 0);
                    break;
            }
        }
        public void InitiateRadarScan(int type)
        {
            switch (type)
            {
                case 0:
                    AudioManager.Instance.PlayGlobal((int)SFXClipIndex.INTERACT_SCAN);
                    EventManager.Instance.CreateNextEventInstance();
                    break;
                case 1:
                    // Play smaller radar scan sfx
                    InitiateVisualizeRadar(RadarType.Objective);
                    break;
            }
        }
        public void ResetRadarDisplay(RadarType type)
        {
            switch (type) 
            {
                case RadarType.Event:
                    if (_ButtonParent.transform.childCount > 0)
                    {
                        int count = _ButtonParent.transform.childCount;
                        for (int i = count; i > 0; i--)
                        {
                            Destroy(_ButtonParent.transform.GetChild(i - 1).gameObject);
                        }
                    }
                    _destinationButtons.Clear();
                    _visualizerSystem.ResetVisualizers();
                    break;
                case RadarType.Objective:
                    break;
            }
        }
        private void InitiateVisualizeRadar(RadarType type)
        {
            switch (type)
            {
                case (RadarType.Event):
                    
                    // Reset events visualizer on both model & panel
                    ResetRadarDisplay(type);

                    // Start events visualization on radar model
                    _visualizerSystem.InitiateModelVisualization(type);

                    // Start events visualization on radar panel
                    foreach (EventInstance eventInstance in AvailableEvents)
                    {
                        Button btn = Instantiate(_eventButtonPrefab, _ButtonParent).GetComponent<Button>();
                        TextMeshProUGUI btnText = btn.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        btnText.text = $"DZ-L[{eventInstance.GeneratedCode}]: AL PARTICLE {eventInstance.AstralParticle.ToString("F")}/ls";
                        btn.onClick.AddListener(delegate { SelectEventDestination(eventInstance); });
                        _destinationButtons.Add(btn);
                    }
                    break;
                case (RadarType.Objective):
                    // Reset events visualizer on both model & panel
                    ResetRadarDisplay(type);
                    // Start events visualization on radar model
                    _visualizerSystem.InitiateModelVisualization(type);
                    break;
            }
        }
        public void SelectEventDestination(EventInstance eventInstance)
        {
            //---> Set select event to event manager ---//
            int index = AvailableEvents.IndexOf(eventInstance);
            EventManager.Instance.ConfirmEventDestination(AvailableEvents[index]);

            //---> Manage selection visual display <---//
            GameObject target = _visualizerSystem.VisualizerParent.transform.GetChild(index).gameObject;
            _visualizerSystem.SetVisualizersMaterial(target);
            _DisplayPanel.GetComponent<EventSystem>().SetSelectedGameObject(_destinationButtons[index].gameObject);
        }
        #endregion


        #region ENABLE/DISABLE
        private void OnEnable()
        {
            EventManager.Instance.OnStandbyPhaseStart += OnStandbyPhaseStart_InitiateEventRadar;
            EventManager.Instance.OnEventPhaseStart += OnEventPhaseStart_InitiateObjectiveRadar;

            EventManager.Instance.OnCreatedAvailableEvents += OnCreatedAvailableEvents_PopulateEventsOnRadar;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnStandbyPhaseStart -= OnStandbyPhaseStart_InitiateEventRadar;
            EventManager.Instance.OnEventPhaseStart -= OnEventPhaseStart_InitiateObjectiveRadar;

            EventManager.Instance.OnCreatedAvailableEvents -= OnCreatedAvailableEvents_PopulateEventsOnRadar;
        }
        #endregion
    }
}
