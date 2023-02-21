using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRandom = UnityEngine.Random;
using DeltaUtilLib;

namespace AstralAbyss
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        #region EVENTHANDLAER
        public event EventHandler OnStandbyPhaseStart;
        public event EventHandler OnTransitionPhaseStart;
        public event EventHandler OnEventPhaseStart;
        public event EventHandler<AvailableEventInstances> OnCreatedAvailableEvents;
        public class AvailableEventInstances : EventArgs
        {
            public List<EventInstance> PassAvailableEvents = new List<EventInstance>();
        }
        #endregion

        [Header("Event Property")]
        public EventInstance[] Events;
        public List<int> EncounteredEventID = new List<int>();
        public List<EventInstance> AvailableEvents = new List<EventInstance>();
        public EventInstanceController CurrentEventInstanceController;
        public EventInstance NextEventTarget;
        public int EventProgression = 0;
        public int NextEventScene = -1;
        public int CurrentEventScene = -1;
        public int PreviousEventScene = -1;

        [Header("Phase Property")]
        public float TransitionPhaseDelay = 5f;
        public enum Phase { Standby, Transition, Event }
        public Phase CurrentPhase = Phase.Standby;

        void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
            #endregion

            Events = Resources.LoadAll<EventInstance>("Prefabs/Data/Event");
        }

        private void Start()
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.GameInitiation());
        }

        #region GAMEPLAY PHASE
        public void InitiateStandbyPhase()
        {
            Debug.Log("ENTER STANDBY PHASE");

            CurrentPhase = Phase.Standby;

            //===> Stuff to do when entering standby phase <===//
            if(LabControl.Instance.IsOnline)
                LabControl.Instance.InitiateSpacelabOffline();
            AudioManager.Instance.StopPlayingRandomClip();
            UIManager.Instance.UpdateObjectiveDisplay("mission: select new destination");
            //===================================================

            OnStandbyPhaseStart?.Invoke(this, EventArgs.Empty);
        }
        public void InitiateTransitionPhase()
        {
            Debug.Log("ENTER TRANSITION PHASE");

            CurrentPhase = Phase.Transition;

            ResetAvailableEvents();
            AudioManager.Instance.StopPrimaryAmbient();
            AudioManager.Instance.StopSecondaryAmbient();
            StartCoroutine(StartTransition());
        }
        public IEnumerator StartTransition()
        {
            OnTransitionPhaseStart?.Invoke(this, EventArgs.Empty);

            //---> Initiate transition process <---//
            //UIManager.Instance.StartTransitionScreen();
            //yield return new WaitForSeconds(2);

            GameManager.Instance.Request_FreezePlayer(true);
            yield return new WaitForSeconds(TransitionPhaseDelay);

            //---> Create event instance level <---//
            PreviousEventScene = CurrentEventScene;
            GameManager.Instance.StartCoroutine((GameManager.Instance.LoadEventScene(NextEventTarget.EventSceneIndex)));
        }
        public IEnumerator StopTransition()
        {
            GameManager.Instance.Request_FreezePlayer(false);
            yield return new WaitForSeconds(TransitionPhaseDelay);

            //UIManager.Instance.CloseTransitionScreen();
            //yield return null;
        }
        public void InitiateEventPhase()
        {
            Debug.Log("ENTER EVENT PHASE");

            CurrentPhase = Phase.Event;
            CurrentEventScene = NextEventScene;
            NextEventScene = -1;
            OnEventPhaseStart?.Invoke(this, EventArgs.Empty);
        }
        #endregion


        #region MANAGE EVENT
        public void CreateNextEventInstance()
        {
            ResetAvailableEvents();

            EventInstance NextEvent = CloneEvent(EventProgression);
            NextEvent.MapPosition = DeltaUtil.ReturnRandomVector3(-100, 100);
            NextEvent.GeneratedCode = (int)DeltaUtil.ReturnRandomRange(0, 100);
            //NextEvent.AstralParticle = DeltaUtil.ReturnRandomRange(0, 1000);
            AvailableEvents.Add(NextEvent);

            OnCreatedAvailableEvents?.Invoke(this, new AvailableEventInstances { PassAvailableEvents = AvailableEvents });
        }

        // Create randomize pool of events, which is displays on the Astral Radar
        //public void RandomizeAvailableEventInstances()
        //{
        //    // Clear previous event pool
        //    ResetAvailableEvents();

        //    // Randomize size of available event pool, then randomize event and add it to the pool
        //    int RandomizePoolSize = (int)DeltaUtil.ReturnRandomRange(3, 5);
        //    for (int i = 0; i < RandomizePoolSize; i++)
        //    {
        //        //---> Clone new event instance from randomized id <---//
        //        int RandomizeEventID = (int)DeltaUtil.ReturnRandomRange(0, Events.Length - 1);
        //        EventInstance RandomizedEvent = CloneEvent(RandomizeEventID);
        //        RandomizedEvent.MapPosition = DeltaUtil.ReturnRandomVector3(-100, 100);
        //        RandomizedEvent.GeneratedCode = (int)DeltaUtil.ReturnRandomRange(0, 100);
        //        RandomizedEvent.AstralParticle = DeltaUtil.ReturnRandomRange(0, 1000);

        //        //---> Event duplication & encountered check <---//
        //        int duplicationCheck = 0;
        //        int encounteredCheck = 0;
        //        foreach (EventInstance Event in AvailableEvents)
        //        {
        //            if (Event.ID == RandomizedEvent.ID)
        //                duplicationCheck = -1;
        //            if (EncounteredEventID.Contains(Event.ID))
        //                encounteredCheck = -1;
        //        }
                
        //        //---> Add event into available events list <---//
        //        if (AvailableEvents.Count == 0 && encounteredCheck == 0)
        //        {
        //            AvailableEvents.Add(RandomizedEvent);
        //        }
        //        else
        //        {
        //            if (duplicationCheck == 0 && encounteredCheck == 0)
        //            {
        //                AvailableEvents.Add(RandomizedEvent);
        //            }
        //        }
        //    }

        //    OnCreatedAvailableEvents?.Invoke(this, new AvailableEventInstances { PassAvailableEvents = AvailableEvents });
        //}
        // Confirm event from selecting on Astral Radar

        public void ConfirmEventDestination(EventInstance targetEvent)
        {
            NextEventTarget = targetEvent;
            NextEventScene = (int)NextEventTarget.EventSceneIndex;
            UIManager.Instance.UpdateObjectiveDisplay("mission: enter cryogenic station");
        }
        void ResetAvailableEvents()
        {
            AvailableEvents.Clear();
            AvailableEvents = new List<EventInstance>();

            AstralRadar.Instance.ResetRadarDisplay(AstralRadar.RadarType.Event);
        }
        // Initiate new event after finish navigation phase
        public void InitiateEventInstance()
        {
            InitiateEventPhase();
        }
        #endregion


        #region INTERNAL
        public EventInstance CloneEvent(int index)
        {
            EventInstance newEventInstance = ScriptableObject.CreateInstance<EventInstance>();
            newEventInstance.ID = index;
            newEventInstance.EventInstanceType = Events[index].EventInstanceType;
            newEventInstance.MapPosition = Events[index].MapPosition;
            newEventInstance.AstralParticle = Events[index].AstralParticle;
            newEventInstance.EventSceneIndex = Events[index].EventSceneIndex;
            return newEventInstance;
        }
        #endregion
    }
}
