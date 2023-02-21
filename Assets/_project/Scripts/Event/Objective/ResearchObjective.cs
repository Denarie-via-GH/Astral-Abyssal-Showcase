using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class ResearchObjective : ObjectiveInstance
    {
        [Header("Custom Objective Property")]
        //public bool IsReadyTransfer;
        public bool IsTransfered;
        public GameObject ObjectPrefab;

        [TextArea]
        public string ScanDescription;
        [TextArea]
        public string SampleDescription;

        [ColorUsage(true, true)]
        public Color ScanDisableColor;
        [ColorUsage(true, true)]
        public Color ScanEnableColor;

        [SerializeField] GameObject _objectDisplay;
        [SerializeField] GameObject _labObject;

        public float startOffset;
        public float endOffset;

        public void InitiateObjectTransfer()
        {
            _labObject = LabControl.Instance.SpawnObjectInLabGlass(ObjectPrefab);
            _objectDisplay.SetActive(false);
            MinimapIcon.SetActive(false);

            IsTransfered = true;
            DisplayOnRadar = false;
        }
        public override void CompleteObjective()
        {
            base.CompleteObjective();
            Destroy(_labObject);
        }

        public void UpdateLabObjectScanMaterial(bool enable, float value) // valune scan from 0 -> 1
        {
            Renderer Render = _labObject.transform.GetChild(0).GetComponent<Renderer>();
            Color ScanColor = Render.material.GetColor("_ScanlineColor");
            if (enable)
            {
                float scanProgress = Mathf.Lerp(startOffset, endOffset, value);
                Render.material.SetFloat("_ScanlineOffset", scanProgress);
                Render.material.SetColor("_ScanlineColor", ScanEnableColor);
            }
            else if (!enable)
            {
                Render.material.SetFloat("_ScanlineOffset", 0);
                Render.material.SetColor("_ScanlineColor", ScanDisableColor);
            }
        }

        public override string GetObjectiveType()
        {
            return "RESEARCH";
        }

        public GameObject GetLabObject()
        {
            return _labObject;
        }
        #region SUBSCRIPTION
        public void UpdateObjective_Scanned()
        {
            LabControl.Instance.AddTerminalLog(ScanDescription);
            UpdateLabObjectScanMaterial(false, 0);
        }
        public void UpdateObjective_Sampled()
        {
            LabControl.Instance.AddTerminalLog(SampleDescription);
        }
        public void UpdateObjective_Complete()
        {
            CompleteObjective();
        }
        #endregion
    }
}
