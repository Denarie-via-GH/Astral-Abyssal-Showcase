using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AstralAbyss
{
    public class ObjectiveInstance : MonoBehaviour, IObjective
    {
        [Header("Objective Property")]
        public float ActiveRangeDistance = 200;
        public bool InActiveRange = false;
        public bool IsObjectiveComplete = false;
        public bool DisplayOnRadar = true;
        public bool DestoryOnComplete = false;
        public bool HasFirstTimeSFX = false;
        public AudioClip FirstTimeSFX;
        bool _playedFirstTimeSFX = false;

        public EventHandler OnObjectiveComplete;
        [SerializeField] protected GameObject MinimapIcon;
        [SerializeField] protected DataArchive ObjectEntry;
        public virtual void CompleteObjective()
        {
            IsObjectiveComplete = true;
            MinimapIcon.SetActive(false);
            EventInstanceController.Instance.ClearObjecteive(this);
            AudioManager.Instance.PlayInterface((int)UIClipIndex.COMPLETE_OBJECTIVE);
            AddEntry();

            OnObjectiveComplete?.Invoke(this, EventArgs.Empty);

            if (DestoryOnComplete)
                DestroyObjective();
        }
        protected virtual void Update()
        {
            if (!IsObjectiveComplete)
            {
                if (Vector3.Distance(OrbiterCore.Instance.transform.position, transform.position) <= ActiveRangeDistance)
                {
                    InActiveRange = true;
                }
                else
                {
                    InActiveRange = false;
                }

                if (HasFirstTimeSFX && !_playedFirstTimeSFX && InActiveRange && OrbiterCore.Instance.LookingObject == this)
                {
                    AudioManager.Instance.PlayGlobal(FirstTimeSFX);
                    _playedFirstTimeSFX = true;
                }
            }

            
        }
        public void AddEntry() 
        {
            ControlCentral.Instance.AddDataEntry(CloneDataInstance());
        }

        public virtual string GetObjectiveType()
        {
            return null;
        }

        DataArchive CloneDataInstance()
        {
            DataArchive newEventData = ScriptableObject.CreateInstance<DataArchive>();
            EventInstance CurrentEvent = EventInstanceController.Instance.Ref_CurrentEventInstance;
            newEventData.Data                       = ObjectEntry.Data;
            newEventData.DataID                     = ObjectEntry.DataID;
            newEventData.EntryTitle                 = ObjectEntry.EntryTitle;
            newEventData.Ref_EventID                = CurrentEvent.ID;
            newEventData.Ref_EventCode              = CurrentEvent.GeneratedCode;
            newEventData.Ref_EventMapPosition       = CurrentEvent.MapPosition;
            newEventData.Ref_EventAstralParticle    = CurrentEvent.AstralParticle;
            newEventData.Ref_EventType              = CurrentEvent.EventInstanceType;

            return newEventData;
        }

        public void DestroyObjective()
        {
            Destroy(this.gameObject);
        }
        #region SCRAP
        /*
        [System.Serializable]
        public class Condition
        {
            public enum ConditionType { Look, Scan, Distance, Collision }
            public ConditionType Type;
            public bool IsComplete;
        }

        public List<Condition> CompositeCondition = new List<Condition>();
        public void InitiateObjectiveInstance()
        {
            if(CompositeCondition.Contains(new Condition { Type = Condition.ConditionType.Scan }))
            {
                OrbiterCore.Instance.OnAstralScan += UpdateScanProgress;
            }
        }
        void Update()
        {

        }

        void UpdateObjectiveComplete()
        {
            int completeCheck = 0;
            foreach(var con in CompositeCondition)
            {
                if (con.IsComplete) completeCheck += 1;
            }
            if (completeCheck >= CompositeCondition.Count) IsObjectiveComplete = true;
        }
        
        void UpdateScanProgress(object o, EventArgs e)
        {
            CompositeCondition.Find(x => x.Type == Condition.ConditionType.Scan).IsComplete = true;
        }

        private void OnDisable()
        {
            if (CompositeCondition.Contains(new Condition { Type = Condition.ConditionType.Scan }))
            {
                OrbiterCore.Instance.OnAstralScan -= UpdateScanProgress;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 DirToPlayer = (OrbiterCore.Instance.transform.position - transform.position).normalized;
            Gizmos.DrawLine(transform.position, transform.position + (DirToPlayer * 100));

            Gizmos.color = Color.white;
            Gizmos.DrawLine(OrbiterCore.Instance.transform.position, OrbiterCore.Instance.transform.position + (OrbiterCore.Instance.CameraRingPivot.forward * 100));
        }
        */
        #endregion
    }
}
