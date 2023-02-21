using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AstralAbyss
{
    public class ObjectiveVisualizer : Interactable
    {
        [Header("Property")]
        public Transform Target;
        public ObjectiveInstance ObjectiveReference;
        private string _objectiveType;
        private bool _objectiveInRange;
        private Renderer _renderer;

        public Material AnalyzeType;
        public Material AnalyzeInRange;
        public Material AnalyzeOutOfRange;

        public Material ResearchType;
        public Material ResearchInRange;
        public Material ResearchOutOfRange;

        public void Initiate(Vector3 position, ObjectiveInstance objectiveReference, float displacement)
        {
            transform.position = position;
            ObjectiveReference = objectiveReference;
            Target = ObjectiveReference.transform;
            _objectiveType = ObjectiveReference.GetObjectiveType();

            float distanceToObjective = Vector3.Distance(OrbiterCore.Instance.transform.position, ObjectiveReference.transform.position);
            UpdateVisualizerMaterial(distanceToObjective);

            LineRenderer line = GetComponent<LineRenderer>();
            line.SetPosition(0, transform.position);
            line.SetPosition(1, new Vector3(transform.position.x, displacement, transform.position.z));
        }
        void Awake()
        {
            _renderer = GetComponent<Renderer>();

            InteractableName = "Objective";
            InteractableAction = "Set Direction";
            CanInteract = true;
        }
        void Update()
        {
            float distanceToObjective = Vector3.Distance(OrbiterCore.Instance.transform.position, ObjectiveReference.transform.position);
            UpdateVisualizerMaterial(distanceToObjective);

            if (PromptDisplay != null)
            {
                Transform Extra = PromptDisplay.transform.GetChild(2);
                TextMeshProUGUI InRangeText = Extra.GetChild(0).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI DistanceText = Extra.GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI TypeText = Extra.GetChild(2).GetComponent<TextMeshProUGUI>();
                DistanceText.text = $"DISTANCE: {distanceToObjective}";
                TypeText.text = $"TYPE: {_objectiveType}";

                if (_objectiveInRange)
                    InRangeText.text = "IN RANGE";
                else
                    InRangeText.text = "NOT IN RANGE";
            }
        }
        private void UpdateVisualizerMaterial(float distance)
        {
            if (distance <= ObjectiveReference.ActiveRangeDistance) _objectiveInRange = true;
            else _objectiveInRange = false;
            bool OutofBound = false;
            if (Vector3.Distance(Vector3.zero, transform.localPosition) > FullDimensionVisualizer.Instance.VisualizeBorderRange - 0.1) OutofBound = true;
            else OutofBound = false;

            if (_objectiveType == "ANALYZE")
            {
                if (OutofBound) _renderer.material = AnalyzeOutOfRange;
                else if(!OutofBound && _objectiveInRange) _renderer.material = AnalyzeInRange;
                else _renderer.material = AnalyzeType;
            }
            else if(_objectiveType == "RESEARCH")
            {
                if (OutofBound) _renderer.material = ResearchOutOfRange;
                else if (!OutofBound && _objectiveInRange) _renderer.material = ResearchInRange;
                else _renderer.material = ResearchType;
            }
        }
        public override void Interact()
        {
            if (!CanInteract || !InRange || LabControl.Instance.IsLabInSession)
                return;

            AudioManager.Instance.PlayGlobal((int)SFXClipIndex.INTERACT_VISUALIZER);
            OrbiterCore.Instance.InitiateRingAutoOrientation(Target);
        }
    }
}
