using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AstralAbyss
{
    public class AnalyzeObjective : ObjectiveInstance
    {
        [Header("Custom Objective Property")]
        public float AnalyzeDuration = 5;
        public float AnalyzeTimer;
        public bool IsAnalyzing = false;
        AudioSource AnalyzeSource;
        private void Awake()
        {
            IsAnalyzing = false;
            AnalyzeSource = GetComponent<AudioSource>();
        }
        protected override void Update()
        {
            base.Update();
            if (IsAnalyzing && OrbiterCore.Instance.LookingObject != this)
            {
                Debug.Log($"{name} look exit");
                StopAnalyze();
            }
        }
        public void Analyze()
        {
            if (!IsObjectiveComplete)
            {
                IsAnalyzing = true;
                AnalyzeTimer += Time.deltaTime;
                AudioManager.Instance.PlaySource(AnalyzeSource);
                float t = AnalyzeTimer / AnalyzeDuration;
                UIManager.Instance.DisplayRecordingOverlay(true, t);
                if (AnalyzeTimer >= AnalyzeDuration)
                {
                    AnalyzeTimer = -1;
                    CompleteObjective();
                }
            }
        }
        public void StopAnalyze()
        {
            if (IsAnalyzing)
            {
                UIManager.Instance.DisplayRecordingOverlay(false, 0);
                AudioManager.Instance.StopSource(AnalyzeSource);
                IsAnalyzing = false;
            }
        }
        public override void CompleteObjective()
        {
            base.CompleteObjective();
            StopAnalyze();
        }
        public override string GetObjectiveType()
        {
            return "ANALYZE";
        }
    }
}
