using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AstralAbyss
{
    public class AbyssEventController : EventInstanceController
    {
        [Header("Custom Property")]
        public EventTriggerSignal BlackholeEventTrigger;
        public EventTriggerSignal CrashEventTrigger;
        public GameObject Heart;
        public PlayableDirector Cutscene;
        public GameObject LookTrigger;
        public Transform NewPosition;
        public GameObject Devourer;
        public bool IsNearHeart = false;
        public bool ReadyFinalCutscene = false;
        public float NearHeartParameter = 5f;
        public float NearExitParameter = 10f;

        protected override void InitiateEvent()
        {
            base.InitiateEvent();
            UIManager.Instance.UpdateObjectiveDisplay("mission: reach the star");
            UIManager.Instance.DisplayObjectiveBar(false);
            OrbiterCore.Instance.DirectionPivot.GetComponentInChildren<Camera>().farClipPlane = 2000;

            BlackholeEventTrigger.OnTrigger += PlayBlackholeCutscene;
            CrashEventTrigger.OnTrigger += PlayCrashEvent;

            SpaceshipElementControl.Instance.DisplayDecalGroup(0, false);
            SpaceshipElementControl.Instance.DisplayDecalGroup(1, false);
            SpaceshipElementControl.Instance.SetSpaceshipMaterial(0);
            SpaceshipElementControl.Instance.SetMainSpotlightIntensity(7.5f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(1, 5f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(2, 5f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(3, 5f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(4, 5f, true);
            SpaceshipElementControl.Instance.SetRealtimeLightEmission(5, 5f, true);



            IsInitializeDone = true;
        }
        protected override void StartEvent(object o, EventArgs e)
        {
            base.StartEvent(o, e);
        }

        protected override void Update()
        {
            base.Update();
            if(ReadyFinalCutscene)
            {
                if (!IsNearHeart)
                {
                    float Distance = Vector3.Distance(PlayerMain.Instance.transform.position, Heart.transform.position);
                    if (Distance < NearHeartParameter)
                    {
                        PreprareFinalCutscene();
                    }
                }

                if (IsNearHeart && LookTrigger != null)
                {
                    float Distance = Vector3.Distance(PlayerMain.Instance.transform.position, LookTrigger.transform.position);
                    if (Distance < NearExitParameter)
                    {
                        Vector3 directionToPlayer = (PlayerMain.Instance.transform.position - LookTrigger.transform.position).normalized;
                        float DOT = Vector3.Dot(directionToPlayer, FirstPersonCamera.Instance.transform.TransformDirection(Vector3.forward));
                        if (-DOT >= 0.9f)
                        {
                            StartCoroutine(FinalCutscene());
                        }
                    }
                }
            }
        }

        void PlayBlackholeCutscene(object o, EventArgs e)
        {
            BlackholeEventTrigger.OnTrigger -= PlayBlackholeCutscene;
            Destroy(BlackholeEventTrigger.gameObject);
            Cutscene.Play();

            Bathroom bathroom = GameObject.FindObjectOfType<Bathroom>();
            bathroom.CanInteract = false;
            FoodDispenserButton foodbutton = GameObject.FindObjectOfType<FoodDispenserButton>();
            foodbutton.CanInteract = false;
            DrinkDispenserButton drinkbutton = GameObject.FindObjectOfType<DrinkDispenserButton>();
            drinkbutton.CanInteract = false;

            #region SET INSIDE SPACESHIP ENVIRONMENT
            SpaceshipElementControl.Instance.SetMainSpotlightIntensity(0f, true);
            for (int i = 0; i < SpaceshipElementControl.Instance.Emitters.Count; i++)
            {
                SpaceshipElementControl.Instance.SetRealtimeLightEmission(i, 0, false);
            }

            SpaceshipElementControl.Instance.DisplayDecalGroup(2, false);
            SpaceshipElementControl.Instance.DisplayDecalGroup(3, false);
            Renderer[] objects = SpaceshipElementControl.Instance.transform.GetComponentsInChildren<Renderer>();
            foreach (Renderer go in objects)
            {
                go.material = SpaceshipElementControl.Instance.SpaceshipMaterialVarient[2];
            }

            AudioManager.Instance.PlayPrimaryAmbient((int)CustomAmbient);
            if (EnableRandomClip)
            {
                StartCoroutine(AudioManager.Instance.InitiateRandomClip(DeltaUtilLib.DeltaUtil.ReturnRandomRange(RandomClipIntervalMin, RandomClipIntervalMax)));
            }
            UIManager.Instance.UpdateObjectiveDisplay("reach the star?");
            #endregion
        }
        void PlayCrashEvent(object o, EventArgs e)
        {
            StartCoroutine(CrashEventCorutine());
        }
        IEnumerator CrashEventCorutine()
        {
            GameManager.Instance.Request_FreezePlayer(true);
            GameManager.Instance.Request_FreezeControl(true);
            GameManager.Instance.Request_FreezeOrbiter(true);
            AudioManager.Instance.PlayGlobal((int)SFXClipIndex.CRASH_EVENT);
            FirstPersonCamera.Instance.ShakeCam(5, 5);
            yield return new WaitForSeconds(2.5f);
            UIManager.Instance.CutScreen.SetActive(true);
            yield return new WaitForSeconds(5);
            UIManager.Instance.CutScreen.SetActive(false);

            UIManager.Instance.UpdateObjectiveDisplay("");


            OrbiterCore.Instance.transform.position = NewPosition.position;
            OrbiterCore.Instance.transform.rotation = NewPosition.rotation;

            GameManager.Instance.Request_FreezePlayer(false);
            GameManager.Instance.Request_FreezeControl(false);
            GameManager.Instance.Request_FreezeOrbiter(false);

            ControlCentral.Instance.Command_ExitDockMode();
            ControlCentral.Instance.SetCriticalSystemError();
            AstralRadar.Instance.SetCriticalSystemError();
            
            SpaceshipElementControl.Instance.SwitchLab();
            
            GameManager.Instance.Request_FreezeControl(true);
            GameManager.Instance.Request_FreezeOrbiter(true);

            Heart = GameObject.FindGameObjectWithTag("FinalTrigger");
            ReadyFinalCutscene = true;
        }

        void PreprareFinalCutscene()
        {
            LookTrigger = GameObject.FindGameObjectWithTag("FinalEvent");
            IsNearHeart = true;
        }
        IEnumerator FinalCutscene()
        {
            ReadyFinalCutscene = false;
            IsNearHeart = false;
            Devourer.SetActive(true);
            AudioManager.Instance.PlayGlobal((int)SFXClipIndex.FINAL_JUMPSCARE);

            Debug.Log("PLAY FINAL CUTSCENE");
            yield return new WaitForSeconds(0.65f);
            UIManager.Instance.CutScreen.SetActive(true);

            AudioManager.Instance.StopPrimaryAmbient();
            AudioManager.Instance.StopSecondaryAmbient();
            AudioManager.Instance.StopPlayingRandomClip();
            
            yield return new WaitForSeconds(3f);
            StartCoroutine(GameManager.Instance.LoadOutro()); //Application.Quit();
        }

        private void OnDrawGizmos()
        {
            if (ReadyFinalCutscene)
            {
                Gizmos.DrawWireSphere(Heart.transform.position, NearHeartParameter);
                Gizmos.DrawWireSphere(LookTrigger.transform.position, NearExitParameter);
            }
        }
    }
}
