using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;

namespace AstralAbyss
{
    public class EventInstanceController : MonoBehaviour
    {
        public static EventInstanceController Instance;

        [Header("Internal Propertyy")]
        public bool IsInitializeDone = false;
        public bool IsEventComplete = false;
        public EventInstance Ref_CurrentEventInstance;
        [SerializeField] Camera _minimapCamera;

        [Header("Objective Property")]
        public List<ObjectiveInstance> Objectives = new List<ObjectiveInstance>();
        public int EventObjectiveCount;
        public float EventObjectiveProgress;

        [Header("Level Property")]
        public Transform ShipSpawnPosition;
        public bool UseTerrain;
        public Terrain LevelTerrain;
        public float minFresnel;
        public float maxFresnel;
        [ColorUsage(true, true)]
        public Color DeActiveColor;
        [ColorUsage(true, true)]
        public Color ActiveColor;
        public AmbientClipIndex CustomAmbient;
        public bool EnableRandomClip = false;
        public List<AudioClip> RandomClips = new List<AudioClip>();
        public float RandomClipIntervalMin;
        public float RandomClipIntervalMax;

        [Header("Scan Property")]
        public bool IsTuning = false;
        public bool IsDetuning = false;
        public bool IsScanActive = false;
        public float TuningDuration = 2.5f;
        float TuningTimer = 0;
        public float ScanActiveDuration = 5;
        float ScanActiveTimer = 0;

        Coroutine A;
        Coroutine B;

        #region EVENT SIGNAL
        public EventHandler OnEnvironmentScan;
        public EventHandler OnCompleteEvent;
        #endregion

        void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
            #endregion

            InitiateEvent();
        }
        protected virtual void Update()
        {
            if (EventObjectiveProgress >= 1 && !IsEventComplete)
            {
                CompleteEvent();
            }

            if (IsScanActive)
            {
                ScanActiveTimer += Time.deltaTime;
                if (ScanActiveTimer >= ScanActiveDuration)
                {
                    ResetTimers();
                    B = StartCoroutine(EnvironmentDescanningProcess());
                    IsScanActive = false;
                }
            }
        }

        #region EVENT CONTROL
        protected virtual void InitiateEvent()
        {
            //---> Initiate internal properties <---//
            IsEventComplete = false;
            IsInitializeDone = false;
            _minimapCamera = GetComponentInChildren<Camera>();

            //---> Initiate objective properties <---//
            Objectives.Clear();
            GameObject[] AllObjectives = GameObject.FindGameObjectsWithTag("Objective");
            foreach (GameObject obj in AllObjectives)
            {
                Objectives.Add(obj.GetComponent<ObjectiveInstance>());
            }
            EventObjectiveCount = Objectives.Count;
            EventObjectiveProgress = 0;

            //---> Initiate event properties <---//
            Ref_CurrentEventInstance = EventManager.Instance.NextEventTarget;
            EventManager.Instance.CurrentEventInstanceController = this;
            EventManager.Instance.EncounteredEventID.Add(Ref_CurrentEventInstance.ID);
            EventManager.Instance.NextEventTarget = null;

            //---> Set deactive color to all gameObject with astral shader <---//
            if (UseTerrain)
            {
                LevelTerrain = GetComponentInChildren<Terrain>();
                LevelTerrain.materialTemplate.SetColor("_FresnelColor", DeActiveColor);
            }
            GameObject[] Environments = GameObject.FindGameObjectsWithTag("Environment");
            foreach (GameObject environment in Environments)
            {
                environment.GetComponentInChildren<Renderer>().material.SetColor("_FresnelColor", DeActiveColor);
            }

            //---> Setup UI overlays <---//
            UIManager.Instance.UpdateObjectiveProgress(0);

            //---> Set ORBITER spawn position/rotation <---//
            OrbiterCore.Instance.transform.position = ShipSpawnPosition.position;
            OrbiterCore.Instance.transform.rotation = ShipSpawnPosition.rotation;
            OrbiterCore.Instance.DirectionPivot.rotation = ShipSpawnPosition.rotation;
        }
        protected virtual void StartEvent(object o, EventArgs e)
        {
            Debug.Log("START EVENT");
        }
        protected virtual void CompleteEvent()
        {
            IsEventComplete = true;
            OnCompleteEvent?.Invoke(this, EventArgs.Empty);
            EventManager.Instance.EventProgression += 1;
            EventManager.Instance.InitiateStandbyPhase();
            UIManager.Instance.DisplayObjectiveBar(false);
        }
        public void ClearObjecteive(ObjectiveInstance obj)
        {
            int check = Objectives.FindIndex(x => x == obj);
            if (check != -1)
            {
                Objectives.RemoveAt(check);
                EventObjectiveProgress = Mathf.Abs(((float)Objectives.Count / (float)EventObjectiveCount) - 1f);
                UIManager.Instance.UpdateObjectiveProgress(EventObjectiveProgress);
            }
        }
        #endregion

        #region DEBUG
        public void FORCE_COMPLETE()
        {
            CompleteEvent();
        }
        #endregion

        #region ENVIRONMENT SCAN
        public void InitiateEnvironmentScan()
        {
            if (IsTuning || IsScanActive || IsDetuning)
                return;
            else
            {
                IsTuning = true;
                AudioManager.Instance.PlayGlobal((int)SFXClipIndex.INTERACT_SCAN);
                OnEnvironmentScan?.Invoke(this, EventArgs.Empty);
                A = StartCoroutine(EnvironmentScanningProcess());
            }
        }
        public void ForceEnvironmentDescan()
        {
            StopCoroutine(A);
            StopCoroutine(B);
            ResetTimers();

            if (UseTerrain)
            {
                LevelTerrain.materialTemplate.SetFloat("_FresnelStrength", maxFresnel);
                LevelTerrain.materialTemplate.SetColor("_FresnelColor", DeActiveColor);
            }
            GameObject[] Environments = GameObject.FindGameObjectsWithTag("Environment");
            foreach (GameObject environment in Environments)
            {
                environment.GetComponentInChildren<Renderer>().material.SetFloat("_FresnelStrength", maxFresnel);
                environment.GetComponentInChildren<Renderer>().material.SetColor("_FresnelColor", DeActiveColor);
            }

            IsTuning = false;
            IsDetuning = false;
            IsScanActive = false;
        }
        IEnumerator EnvironmentScanningProcess()
        {
            IsTuning = true;
            while (TuningTimer < TuningDuration)
            {
                float t = TuningTimer / TuningDuration;

                //---> Lerp color from deactive to active <---//
                if (UseTerrain)
                {
                    LevelTerrain.materialTemplate.SetFloat("_FresnelStrength", Mathf.Lerp(maxFresnel, minFresnel, t));
                    LevelTerrain.materialTemplate.SetColor("_FresnelColor", Color.Lerp(DeActiveColor, ActiveColor, t));
                }
                GameObject[] Environments = GameObject.FindGameObjectsWithTag("Environment");
                foreach (GameObject environment in Environments)
                {
                    environment.GetComponentInChildren<Renderer>().material.SetFloat("_FresnelStrength", Mathf.Lerp(maxFresnel, minFresnel, t));
                    environment.GetComponentInChildren<Renderer>().material.SetColor("_FresnelColor", Color.Lerp(DeActiveColor, ActiveColor, t));
                }

                TuningTimer += Time.deltaTime;
                yield return null;
            }
            TuningTimer = 0;
            IsScanActive = true;
            IsTuning = false;
            yield return null;
        }
        IEnumerator EnvironmentDescanningProcess()
        {
            IsDetuning = true;
            while (TuningTimer < TuningDuration)
            {
                float t = TuningTimer / TuningDuration;

                //---> Lerp color from active to deactive <---//
                if (UseTerrain)
                {
                    LevelTerrain.materialTemplate.SetFloat("_FresnelStrength", Mathf.Lerp(minFresnel, maxFresnel, t));
                    LevelTerrain.materialTemplate.SetColor("_FresnelColor", Color.Lerp(ActiveColor, DeActiveColor, t));
                }
                GameObject[] Environments = GameObject.FindGameObjectsWithTag("Environment");
                foreach (GameObject environment in Environments)
                {
                    environment.GetComponentInChildren<Renderer>().material.SetFloat("_FresnelStrength", Mathf.Lerp(minFresnel, maxFresnel, t));
                    environment.GetComponentInChildren<Renderer>().material.SetColor("_FresnelColor", Color.Lerp(ActiveColor, DeActiveColor, t));
                }

                TuningTimer += Time.deltaTime;
                yield return null;
            }
            TuningTimer = 0;
            IsScanActive = false;
            IsDetuning = false;
            yield return null;
        }
        void ResetTimers()
        {
            TuningTimer = 0;
            ScanActiveTimer = 0;
        }
        #endregion

        #region ENABLE/DISABLE
        private void OnEnable()
        {
            EventManager.Instance.OnEventPhaseStart += StartEvent;
        }
        public void DisableEventInstance()
        {
            EventManager.Instance.OnEventPhaseStart -= StartEvent;
            Instance = null;
        }
        #endregion
    }
}
