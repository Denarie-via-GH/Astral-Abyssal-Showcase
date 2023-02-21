using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AstralAbyss
{
    public class FullDimensionVisualizer : MonoBehaviour
    {
        public static FullDimensionVisualizer Instance;

        [Header("Visualize Property")]
        [SerializeField] private List<GameObject> _visualizerObjects = new List<GameObject>();
        [SerializeField] private GameObject EventVisualizerPrefab;
        [SerializeField] private GameObject ObjectiveVisualizerPrefab;
        public bool IsOverrideObjectiveVisualizerPrompt;
        public GameObject OverrideObjectiveVisualizerPrompt;
        public int SelectingVisualizerIndex = -1;
        Vector3 _visualizeCenter;

        [Header("Reference Target")]
        public Transform SpaceshipObject;
        public Transform SpaceshipVisualizer;
        public Transform VisualizerParent;
        public Transform AbsoluteHeightPlane;

        [Header("Visualize Configuration")]
        [Range(0f, 2f)]
        public float VisualizeRangeModifier;
        public float VisualizeBorderRange;
        public float DistancePerUnit;
        public Vector3 VisualizeOffset;
        public Material OnNormalMat;
        public Material OnSelectMat;
        
        private void Awake()
        {
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
        }
        void Start()
        {
            //---> Prepare variable <---//
            SpaceshipObject = OrbiterCore.Instance.transform;
            SpaceshipVisualizer.localPosition = VisualizeOffset;
            _visualizeCenter = SpaceshipVisualizer.position;
        }
        void Update()
        {
            //---> Update UTR rings rotation visualization on radar <---//
            if (OrbiterCore.Instance)
            {
                SpaceshipVisualizer.GetChild(0).transform.rotation = OrbiterCore.Instance.DirectionPivot.rotation;

                #region SCRAP
                //if (ControlCentral.Instance.CurrentUTRMenu == ControlCentral.UTRMenu.CameraRing)
                //{
                //    Renderer C_RingRender = SpaceshipVisualizer.GetChild(0).GetChild(1).GetComponent<Renderer>();
                //    Material C_RingMat = C_RingRender.material;
                //    C_RingMat.SetColor("_EmissionColor", _cRingColor * 5f);
                //    RendererExtensions.UpdateGIMaterials(C_RingRender);
                //    DynamicGI.SetEmissive(C_RingRender, _cRingColor * 5f);
                //    DynamicGI.UpdateEnvironment();

                //    Renderer M_RingRender = SpaceshipVisualizer.GetChild(1).GetChild(1).GetComponent<Renderer>();
                //    Material M_RingMat = M_RingRender.material;
                //    M_RingMat.SetColor("_EmissionColor", _mRingColor * 0.5f);
                //    RendererExtensions.UpdateGIMaterials(M_RingRender);
                //    DynamicGI.SetEmissive(M_RingRender, _mRingColor * 0.5f);
                //    DynamicGI.UpdateEnvironment();
                //}
                //else if(ControlCentral.Instance.CurrentUTRMenu == ControlCentral.UTRMenu.ManeuverRing)
                //{
                //    Renderer C_RingRender = SpaceshipVisualizer.GetChild(0).GetChild(1).GetComponent<Renderer>();
                //    Material C_RingMat = C_RingRender.material;
                //    C_RingMat.SetColor("_EmissionColor", _cRingColor * 0.5f);
                //    RendererExtensions.UpdateGIMaterials(C_RingRender);
                //    DynamicGI.SetEmissive(C_RingRender, _cRingColor * 0.5f);
                //    DynamicGI.UpdateEnvironment();

                //    Renderer M_RingRender = SpaceshipVisualizer.GetChild(1).GetChild(1).GetComponent<Renderer>();
                //    Material M_RingMat = M_RingRender.material;
                //    M_RingMat.SetColor("_EmissionColor", _mRingColor * 5f);
                //    RendererExtensions.UpdateGIMaterials(M_RingRender);
                //    DynamicGI.SetEmissive(M_RingRender, _mRingColor * 5f);
                //    DynamicGI.UpdateEnvironment();  
                //}
                #endregion
            }
        }

        #region VISUALIZATION
        public void InitiateModelVisualization(AstralRadar.RadarType type)
        {
            ResetVisualizers();
            CreateVisualizers(type);
        }
        void CreateVisualizers(AstralRadar.RadarType type)
        {
            switch (type)
            {
                case (AstralRadar.RadarType.Event):
                    foreach (EventInstance eventInstance in AstralRadar.Instance.AvailableEvents)
                    {
                        float T_Distance = Vector3.Distance(SpaceshipObject.position, eventInstance.MapPosition);
                        T_Distance /= DistancePerUnit;
                        T_Distance *= VisualizeRangeModifier;
                        T_Distance = Mathf.Clamp(T_Distance, 0, VisualizeBorderRange);
                        Vector3 T_Direction = (eventInstance.MapPosition - SpaceshipObject.position).normalized;
                        Vector3 Destination = T_Direction * T_Distance;

                        EventVisualizer newEventVisualizer = Instantiate(EventVisualizerPrefab, VisualizerParent.transform).GetComponent<EventVisualizer>();
                        newEventVisualizer.Initiate(_visualizeCenter + Destination, eventInstance, AbsoluteHeightPlane.transform.position.y);
                        _visualizerObjects.Add(newEventVisualizer.gameObject);
                        //newEventVisualizer.transform.position = (_visualizeCenter + Destination);
                        //newEventVisualizer.Event = eventInstance;
                    }
                    break;
                case (AstralRadar.RadarType.Objective):
                    foreach (var obj in EventInstanceController.Instance.Objectives)
                    {
                        if (obj.DisplayOnRadar)
                        {
                            //---> Calculate visualzier position within radar radius <---//
                            float T_Distance = Vector3.Distance(SpaceshipObject.position, obj.gameObject.transform.position);
                            T_Distance /= DistancePerUnit;
                            T_Distance *= VisualizeRangeModifier;
                            T_Distance = Mathf.Clamp(T_Distance, 0, VisualizeBorderRange);
                            Vector3 T_Direction = (obj.gameObject.transform.position - SpaceshipObject.position).normalized;
                            Vector3 Destination = T_Direction * T_Distance;
                            
                            //---> Create new visualizer inside the radar <---//
                            ObjectiveVisualizer newObjectiveVisualizer = Instantiate(ObjectiveVisualizerPrefab, VisualizerParent.transform).GetComponent<ObjectiveVisualizer>();
                            newObjectiveVisualizer.Initiate(_visualizeCenter + Destination, obj, AbsoluteHeightPlane.transform.position.y);
                            _visualizerObjects.Add(newObjectiveVisualizer.gameObject);
                        }
                    }
                    break;
            }
        }
        public void SetVisualizersMaterial(GameObject targetVisualizer)
        {
            foreach (GameObject obj in _visualizerObjects)
            {
                obj.gameObject.GetComponent<MeshRenderer>().material = OnNormalMat;
            }
            targetVisualizer.GetComponent<MeshRenderer>().material = OnSelectMat;
        }
        public void ResetVisualizers()
        {
            GameObject[] GetVisualizer = GameObject.FindGameObjectsWithTag("Visualizer");
            int Counts = GetVisualizer.Length;
            for (int i = 0; i < Counts; i++)
            {
                Destroy(GetVisualizer[Counts - 1 - i].gameObject);
            }
            _visualizerObjects.Clear();
        }
        #endregion

        #region SCRAP: OVERRIDE VISUALIZER
        //public void DisableOverrideObjectiveVisualizerPrompt()
        //{
        //    IsOverrideObjectiveVisualizerPrompt = true;
        //    SelectingVisualizerIndex = -1;
        //    if(OverrideObjectiveVisualizerPrompt != null)
        //        Destroy(OverrideObjectiveVisualizerPrompt);
        //}
        //public void OverrideObjectiveVisualizer()
        //{
        //    foreach(GameObject go in _visualizerObjects)
        //    {
        //        Interactable TEMP = go.GetComponent<Interactable>();
        //        TEMP.IsOverridePromptDisplay = false;
        //    }

        //    Interactable Target = _visualizerObjects[SelectingVisualizerIndex].GetComponent<Interactable>();
        //    Target.IsOverridePromptDisplay = true;
        //    if (Target.PromptDisplay == null)
        //    {
        //        Target.PromptDisplay = UIManager.Instance.AddPromptToUI(Target.PromptPrefab, Target.InteractableName, Target.InteractableAction);
        //        OverrideObjectiveVisualizerPrompt = Target.PromptDisplay;
        //    }
        //}
        //public void CycleVisualizerUp()
        //{
        //    if (_visualizerObjects.Count > 0)
        //    {
        //        if (SelectingVisualizerIndex > 0)
        //            SelectingVisualizerIndex -= 1;

        //        OverrideObjectiveVisualizer();
        //    }
        //}
        //public void CycleVisualizerDown()
        //{
        //    if (_visualizerObjects.Count > 0)
        //    {
        //        if (SelectingVisualizerIndex < _visualizerObjects.Count - 1)
        //            SelectingVisualizerIndex += 1;

        //        OverrideObjectiveVisualizer();
        //    }
        //}
        #endregion
    }
}
