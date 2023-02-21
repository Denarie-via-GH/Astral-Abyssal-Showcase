using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using Cinemachine;

namespace AstralAbyss
{
    public class AsteroidFieldEventController : EventInstanceController
    {
        [Header("Custom Property")]
        public ObjectiveInstance LifeformObjective;
        public ObjectiveInstance PackageObjective;
        public ObjectiveInstance ShipwreckObjective;
        public ObjectiveInstance ArtifactObjective;
        private DevourerManager _devourerSystem;

        protected override void InitiateEvent()
        {
            base.InitiateEvent();

            _devourerSystem = FindObjectOfType<DevourerManager>();
            UIManager.Instance.UpdateObjectiveDisplay("mission: investigate area");
            UIManager.Instance.DisplayObjectiveBar(true);

            LifeformObjective.OnObjectiveComplete   += InfiltratedScriptedEvent;
            LifeformObjective.OnObjectiveComplete   += OnCompleteTriggerDevourer;
            PackageObjective.OnObjectiveComplete    += OnCompleteTriggerDevourer;
            ShipwreckObjective.OnObjectiveComplete  += OnCompleteTriggerDevourer;
            ArtifactObjective.OnObjectiveComplete   += OnCompleteTriggerDevourer;

            SpaceshipElementControl.Instance.SetMainSpotlightIntensity(4.5f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(1, 4f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(2, 2.5f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(3, 3.25f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(4, 3.25f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(5, 3.25f, true);

            IsInitializeDone = true;
        }

        protected override void StartEvent(object o, EventArgs e)
        {
            base.StartEvent(o, e);
            AudioManager.Instance.PlayPrimaryAmbient((int)CustomAmbient);
            if (EnableRandomClip)
            {
                StartCoroutine(AudioManager.Instance.InitiateRandomClip(DeltaUtilLib.DeltaUtil.ReturnRandomRange(RandomClipIntervalMin, RandomClipIntervalMax)));
            }
        }

        #region Inflitrated Scripted Event
        void InfiltratedScriptedEvent(object o, EventArgs e)
        {
            StartCoroutine(ProgressInfiltratedScriptedEvent());
        }
        IEnumerator ProgressInfiltratedScriptedEvent()
        {
            AudioManager.Instance.PlayGlobal((int)SFXClipIndex.EVENT_CREATURE_INFILTRATED);
            SpaceshipElementControl.Instance.DisplayDecalGroup(1, true);

            LifeformObjective.OnObjectiveComplete -= InfiltratedScriptedEvent;
            yield return new WaitForSeconds(1.5f);
        }
        #endregion

        protected override void Update()
        {
            base.Update();
        }
        
        public void OnCompleteTriggerDevourer(object o, EventArgs e)
        {
            if (!_devourerSystem.DevourerActive)
            {
                _devourerSystem.DevourerActive = true;
                _devourerSystem.CurrentTrack = DevourerManager.TrackType.Approach;
                _devourerSystem.GenerateTrack(_devourerSystem.CurrentTrack);
                AudioManager.Instance.PlaySecondaryAmbient((int)AmbientClipIndex.SEC_DEVOURER_APPEAR, false);
            }
            else
            {
                _devourerSystem.EncounterRate -= 0.25f;
            }
        }

        protected override void CompleteEvent()
        {
            base.CompleteEvent();
            _devourerSystem.StopDevourer();
        }
    }
}
