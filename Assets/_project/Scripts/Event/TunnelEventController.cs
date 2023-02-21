using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AstralAbyss
{
    public class TunnelEventController : EventInstanceController
    {
        [Header("Custom Property")]
        public EventTriggerSignal SoundTrigger;
        public EventTriggerSignal EncounterSignal;
        public EventTriggerSignal ExitSignal;
        public GameObject TunnelBlock;
        public Renderer Tunnel;
        public Material TunnelMaterial;
        public Transform Origin;
        public Transform Destination;
        public Transform Tracker;
        public Dictionary<string, Vector4> AbyssFieldIntensity = new Dictionary<string, Vector4>()
        {
            {"LowIntensity",    new Vector4(6, 8, 0.2f, 150) },
            {"HighIntensity",   new Vector4(8, 10, 0.4f, 70) }
        };
        public AudioClip EventClip;
        
        protected override void InitiateEvent()
        {
            base.InitiateEvent();

            Tracker = OrbiterCore.Instance.transform;
            TunnelMaterial = Tunnel.material;

            OrbiterCore.Instance.DirectionPivot.GetComponentInChildren<Camera>().farClipPlane = 2000;
            SoundTrigger.OnTrigger += SoundSignalTrigger;
            EncounterSignal.OnTrigger += EncounterSentinels;
            ExitSignal.OnTrigger += ReachExit;

            UIManager.Instance.UpdateObjectiveDisplay("mission: find an exit");
            UIManager.Instance.DisplayObjectiveBar(false);

            SpaceshipElementControl.Instance.SetMainSpotlightIntensity(3f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(1, 3f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(2, 1.75f, false);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(3, 2f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(4, 2f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(5, 2f, true);
            SpaceshipElementControl.Instance.SetSpaceshipMaterial(1);
            SpaceshipElementControl.Instance.DisplayDecalGroup(0, true);

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

        protected override void Update()
        {
            base.Update();

            if (IsInitializeDone && !IsEventComplete)
            {
                UpdateTunnelIntensity();
            }

        }

        void UpdateTunnelIntensity()
        {
            float spectrum = Vector3.Distance(Origin.position, Destination.position);
            float t = Vector3.Distance(Destination.position, Tracker.position) / spectrum;

            float dynamicFrequency  = Mathf.Lerp(AbyssFieldIntensity["HighIntensity"].x, AbyssFieldIntensity["LowIntensity"].x, t);
            float dynamicRate       = Mathf.Lerp(AbyssFieldIntensity["HighIntensity"].y, AbyssFieldIntensity["LowIntensity"].y, t);
            float dynamicIntensity  = Mathf.Lerp(AbyssFieldIntensity["HighIntensity"].z, AbyssFieldIntensity["LowIntensity"].z, t);
            float dynamicScroll     = Mathf.Lerp(AbyssFieldIntensity["HighIntensity"].w, AbyssFieldIntensity["LowIntensity"].w, t);
            
            TunnelMaterial.SetFloat("_WaveFrequency", dynamicFrequency);
            TunnelMaterial.SetFloat("_WaveRate", dynamicRate);
            TunnelMaterial.SetFloat("_WaveIntensity", dynamicIntensity);
            TunnelMaterial.SetFloat("_ScrollSpeed", dynamicScroll);

        }

        #region TRIGGER SIGNAL
        void SoundSignalTrigger(object o, EventArgs e)
        {
            SoundTrigger.OnTrigger -= SoundSignalTrigger;

            AudioManager.Instance.PlayGlobal(EventClip);
            Destroy(SoundTrigger.gameObject);
        }
        void EncounterSentinels(object o, EventArgs e)
        {
            EncounterSignal.OnTrigger -= EncounterSentinels;

            AudioManager.Instance.PlayGlobal((int)SFXClipIndex.ENCOUNTER_SENTINELS);
            Destroy(EncounterSignal.gameObject);
        }
        void ReachExit(object o, EventArgs e)
        {
            ExitSignal.OnTrigger -= ReachExit;

            TunnelBlock.SetActive(true);
            AudioManager.Instance.PlaySecondaryAmbient((int)AmbientClipIndex.SEC_TUNNEL_EXIT, true);
            Destroy(ExitSignal.gameObject);
            CompleteEvent();
        }
        #endregion
    }
}
