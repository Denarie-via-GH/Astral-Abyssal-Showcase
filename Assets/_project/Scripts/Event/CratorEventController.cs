using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class CratorEventController : EventInstanceController
    {
        [Header("Custom Event Property")]
        [SerializeField] ObjectiveInstance TargetObjective;
        [ColorUsage(true, true)]
        public Color AlternativeColor;
        public Material CloseEyeMat;
        public Material OpenEyeMat;

        protected override void InitiateEvent()
        {
            base.InitiateEvent();
            UIManager.Instance.UpdateObjectiveDisplay("mission: investigate area");
            UIManager.Instance.DisplayObjectiveBar(true);
            TargetObjective.OnObjectiveComplete += FirstEyeEvent;

            IsInitializeDone = true;
        }

        protected override void StartEvent(object o, EventArgs e)
        {
            AudioManager.Instance.PlayPrimaryAmbient((int)CustomAmbient);
            if (EnableRandomClip)
            {
                StartCoroutine(AudioManager.Instance.InitiateRandomClip(DeltaUtilLib.DeltaUtil.ReturnRandomRange(RandomClipIntervalMin, RandomClipIntervalMax)));
            }
        }

        #region SCRIPTED EVENT
        void FirstEyeEvent(object o, EventArgs e)
        {
            StartCoroutine(FirstEyeEventSequence());
        }
        IEnumerator FirstEyeEventSequence()
        {
            GameManager.Instance.Request_FreezeControl(true);
            GameManager.Instance.Request_FreezeOrbiter(true);
            UIManager.Instance.DisplayEssentialOverlay(false);
            AudioManager.Instance.PlayGlobal((int)SFXClipIndex.EVENT_FIRST_EYE);
            
            yield return new WaitForSeconds(0.5f);

            TargetObjective.transform.GetChild(0).GetComponent<Renderer>().material = OpenEyeMat;
            TargetObjective.GetComponent<Animator>().CrossFade("FirstEyeEvent", 1f, 0);
            ForceEnvironmentDescan();
            ActiveColor = AlternativeColor;

            yield return new WaitForSeconds(0.75f);

            Quaternion start = TargetObjective.transform.rotation;
            Vector3 TargetDirection = (OrbiterCore.Instance.DirectionPivot.position - TargetObjective.transform.position).normalized;
            Quaternion end = Quaternion.LookRotation(TargetDirection, Vector3.up);

            float timer = 0;
            while (timer < 0.2f)
            {
                float t = timer / 0.2f;
                TargetObjective.transform.rotation = Quaternion.Lerp(start, end,t);
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            
            TargetObjective.OnObjectiveComplete -= FirstEyeEvent;
            Destroy(TargetObjective.gameObject);
            AudioManager.Instance.PlaySecondaryAmbient((int)AmbientClipIndex.SEC_FIRST_EYE, true);
            UIManager.Instance.DisplayEssentialOverlay(true);
            GameManager.Instance.Request_FreezeControl(false);
            GameManager.Instance.Request_FreezeOrbiter(false);
        }
        #endregion
    }
}
