using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AstralAbyss
{
    public class SleepStation : Interactable
    {
        [Header("Property")]
        [SerializeField] Dialogue _noDestinationDialogue;

        void Awake()
        {
            InteractableName = "Cryogenic Station";
            InteractableAction = "Sleep";
        }

        public override void Interact()
        {
            if (!CanInteract || !InRange)
                return;

            if (EventManager.Instance.NextEventTarget)
            {
                AudioManager.Instance.PlayGlobal((int)SFXClipIndex.INTERACT_SLEEPSTATION);
                EventManager.Instance.InitiateTransitionPhase();
            }
            else
            {
                UIManager.Instance.PlayDialogue(_noDestinationDialogue);
            }
        }

        #region SUBSCRIPTION FUNCTION
        private void OnStandbyPhaseStart_InitiateSystem(object o, EventArgs e)
        {
            CanInteract = true;
        }
        private void OnEventPhaseStart_ShutdownSystem(object o, EventArgs e)
        {
            CanInteract = false;
        }
        #endregion

        #region ENABLE/DISABLE
        private void OnEnable()
        {
            EventManager.Instance.OnStandbyPhaseStart += OnStandbyPhaseStart_InitiateSystem;
            EventManager.Instance.OnEventPhaseStart += OnEventPhaseStart_ShutdownSystem;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnStandbyPhaseStart -= OnStandbyPhaseStart_InitiateSystem;
            EventManager.Instance.OnEventPhaseStart -= OnEventPhaseStart_ShutdownSystem;
        }
        #endregion
    }
}
