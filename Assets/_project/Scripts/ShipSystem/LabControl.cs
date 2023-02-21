using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AstralAbyss
{
    public class LabControl : MonoBehaviour
    {
        public static LabControl Instance;

        //public EventHandler OnObjectScanned;
        //public EventHandler OnObjectSampled;

        [Header("Internal Property")]
        public bool IsOnline = false;
        public Renderer LabZoneRender;
        public Renderer LabGlassRender;
        public Material LabZoneOnlineMat;
        public Material LabZoneOfflineMat;
        public Material LabGlassOnlineMat;
        public Material LabGlassOfflineMat;
        public GameObject SignDecals;
        [SerializeField] Transform SpecimenPlacement;
        [SerializeField] ResearchObjective ObjectiveReference;
        [SerializeField] AudioSource SpacelabSource;
        public Airlock ExitAirlock;

        [Header("Display UI Property")]
        [SerializeField] GameObject StatusReportUI;
        [SerializeField] GameObject ScanReportUI;
        [SerializeField] GameObject SampleReportUI;
        [SerializeField] TextMeshProUGUI OperationText;
        [SerializeField] TextMeshProUGUI TerminalLogText;


        [Header("Spacelab Operation")]
        public bool IsLabInSession = false;
        public bool IsOperationInProcess = false;
        public bool IsObjectSpawned = false;
        public bool IsObjectScanned = false;
        public bool IsObjectSampled = false;
        public float ScanDuration = 10f;
        public float SampleDuration = 10f;
        [SerializeField] GameObject SampleArmParent;
        //[SerializeField] Animator[] SampleArms;
        void Awake()
        {
            #region SINGLE
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
            #endregion
        }
        void Start()
        {
            InitiateSpacelabOffline();    
        }
        #region UPDATE UI
        void UpdateOperationUI(bool state)
        {
            if (state)
            {
                StatusReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "INCOMPLETE";
                StatusReportUI.GetComponent<Slider>().value = 0.33f;
                ScanReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "INCOMPLETE";
                ScanReportUI.GetComponent<Slider>().value = 0;
                SampleReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "INCOMPLETE";
                SampleReportUI.GetComponent<Slider>().value = 0;
                OperationText.text = "OPERATION: SCAN";
                TerminalLogText.text = "READY FOR OPERATION...";
            }
            else if (!state)
            {
                StatusReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "OFFLINE";
                StatusReportUI.GetComponent<Slider>().value = 0;
                ScanReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "OFFLINE";
                ScanReportUI.GetComponent<Slider>().value = 0;
                SampleReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "OFFLINE";
                SampleReportUI.GetComponent<Slider>().value = 0;
                OperationText.text = "SPACELAB OFFLINE";
                TerminalLogText.text = "";
            }
        }
        public void AddTerminalLog(string logs)
        {
            TerminalLogText.text += "\n" + "=================================";
            TerminalLogText.text += "\n" + logs;
        }
        #endregion
        public void InitiateSpacelabOnline(ResearchObjective obj)
        {
            ObjectiveReference = obj;
            Animator[] ANIMS = SampleArmParent.GetComponentsInChildren<Animator>();
            foreach (Animator anim in ANIMS)
            {
                anim.CrossFade("DeactiveArm", 1f, 0);
            }
            AudioManager.Instance.PlaySource(SpacelabSource, (int)SFXClipIndex.SPACELAB_TRANSITION, true);
            SignDecals.SetActive(false);
            IsLabInSession = true;
            IsOnline = true;
            LabZoneRender.material = LabZoneOnlineMat;
            LabGlassRender.material = LabGlassOnlineMat;
            UpdateOperationUI(true);

            RendererExtensions.UpdateGIMaterials(LabZoneRender);
            RendererExtensions.UpdateGIMaterials(LabGlassRender);
            DynamicGI.UpdateEnvironment();
        }
        public void InitiateSpacelabOffline()
        {
            ObjectiveReference = null;
            Animator[] ANIMS = SampleArmParent.GetComponentsInChildren<Animator>();
            foreach (Animator anim in ANIMS)
            {
                anim.CrossFade("ActiveArm", 1f, 0);
            }
            AudioManager.Instance.PlaySource(SpacelabSource, (int)SFXClipIndex.SPACELAB_TRANSITION, true);
            SignDecals.SetActive(true);
            IsLabInSession = false;
            IsOnline = false;
            LabZoneRender.material = LabZoneOfflineMat;
            LabGlassRender.material = LabGlassOfflineMat;
            UpdateOperationUI(false);

            IsObjectSpawned = false;
            IsObjectScanned = false;
            IsObjectSampled = false;
            ExitAirlock.IsForceLock = false;

            RendererExtensions.UpdateGIMaterials(LabZoneRender);
            RendererExtensions.UpdateGIMaterials(LabGlassRender);
            DynamicGI.UpdateEnvironment();
        }
        public GameObject SpawnObjectInLabGlass(GameObject obj)
        {
            if (!IsLabInSession)
                return null;

            if (!IsObjectSpawned)
            {
                IsObjectSpawned = true;
                AudioManager.Instance.PlaySource(SpacelabSource, (int)SFXClipIndex.TRANSFER_OBJECT, true);
                GameObject LabObject = Instantiate(obj, SpecimenPlacement);
                return LabObject;
            }
            else
            {
                return null;
            }
        }

        #region OPERATION
        public void Command_ProceedOperation()
        {
            if (!IsOperationInProcess && IsLabInSession)
            {
                Debug.Log("PRESS OPERATION");

                if (IsObjectSpawned && !IsObjectScanned && !IsObjectSampled)
                    Operation_ScanObject();
                else if (IsObjectSpawned && IsObjectScanned && !IsObjectSampled)
                    Operation_SampleObject();
                else if (IsObjectSpawned && IsObjectScanned && IsObjectSampled)
                    EndOperation();
            }
        }
        public void Operation_ScanObject()
        {
            if (!IsLabInSession)
                return;

            if (!IsObjectScanned && IsObjectSpawned)
            {
                StartCoroutine(ScanProcess());
                ExitAirlock.IsForceLock = true;
            }
        }
        IEnumerator ScanProcess()
        {
            IsOperationInProcess = true;
            
            AudioManager.Instance.PlayInterface((int)UIClipIndex.START_OPERATION);
            float timer = 0;
            Slider Progress = ScanReportUI.GetComponent<Slider>();
            while (timer < ScanDuration)
            {
                float t = timer / ScanDuration;

                Progress.value = t;
                ObjectiveReference.UpdateLabObjectScanMaterial(true, t);
                AudioManager.Instance.PlaySource(SpacelabSource, (int)SFXClipIndex.SCAN_LOOP);

                timer += Time.deltaTime;
                yield return null;
            }
            Progress.value = 1;
            AudioManager.Instance.StopSource(SpacelabSource);
            StatusReportUI.GetComponent<Slider>().value = 0.66f;
            ScanReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "COMPLETE";
            OperationText.text = "OPERATION: SAMPLE";
            IsObjectScanned = true;
            ObjectiveReference.UpdateObjective_Scanned();
            AudioManager.Instance.PlayInterface((int)UIClipIndex.END_OPERATION);

            IsOperationInProcess = false;
        }
        public void Operation_SampleObject()
        {
            if (!IsLabInSession)
                return;

            if (!IsObjectSampled && IsObjectSpawned)
            {
                StartCoroutine(SampleProcess());
            }
        }
        IEnumerator SampleProcess()
        {
            IsOperationInProcess = true;

            AudioManager.Instance.PlayInterface((int)UIClipIndex.START_OPERATION);
            Animator[] ANIMS = SampleArmParent.GetComponentsInChildren<Animator>();
            foreach(Animator anim in ANIMS)
            {
                anim.CrossFade("ActiveArm", 1f, 0);
            }
            float timer = 0;
            Slider Progress = SampleReportUI.GetComponent<Slider>();
            while (timer < SampleDuration)
            {
                float t = timer / SampleDuration;

                Progress.value = t;
                AudioManager.Instance.PlaySource(SpacelabSource, (int)SFXClipIndex.SAMPLE_LOOP);
                timer += Time.deltaTime;
                yield return null;
            }
            Progress.value = 1;
            AudioManager.Instance.StopSource(SpacelabSource);
            StatusReportUI.GetComponent<Slider>().value = 1f;
            SampleReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "COMPLETE";
            StatusReportUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "COMPLETE";
            OperationText.text = "END OPERATION";
            IsObjectSampled = true;
            foreach (Animator anim in ANIMS)
            {
                anim.CrossFade("DeactiveArm", 1f, 0);
            }
            ObjectiveReference.UpdateObjective_Sampled();
            AudioManager.Instance.PlayInterface((int)UIClipIndex.END_OPERATION);

            IsOperationInProcess = false;
        }
        public void EndOperation()
        {
            if (!IsLabInSession)
                return;

            if(IsObjectScanned && IsObjectSampled)
            {
                StopAllCoroutines();
                AudioManager.Instance.PlayInterface((int)UIClipIndex.END_OPERATION);
                AudioManager.Instance.PlaySource(SpacelabSource, (int)SFXClipIndex.TRANSFER_OBJECT, true);
                ObjectiveReference.UpdateObjective_Complete();
                InitiateSpacelabOffline();
            }
        }
        #endregion
    }
}
