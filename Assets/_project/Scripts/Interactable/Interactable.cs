using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AstralAbyss
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        //public enum InteractType { System, Object, Log};
        //public InteractType Type;
        //public enum ActivationType { Trigger, Active, Auto}
        //public ActivationType Active;

        [Header("Interaction Property")]
        public Transform PromptTarget;
        public GameObject PromptPrefab;
        public GameObject PromptDisplay;
        public string InteractableName;
        public string InteractableAction;
        public bool CanInteract;
        public bool InRange;

        public virtual void Interact()
        {
            if (!CanInteract || !InRange)
                return;
        }

        void FixedUpdate()
        {
            DisplayPrompt();
        }

        public void DisplayPrompt()
        {
            if (InRange && PromptDisplay != null)
            {
                if (FirstPersonCamera.Instance.SelectingObject == this && CanInteract)
                {
                    PromptDisplay.SetActive(true);
                    PromptDisplay.transform.position = Camera.main.WorldToScreenPoint(PromptTarget.position);
                }
                else
                {
                    PromptDisplay.SetActive(false);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                InRange = true;
                if(PromptDisplay == null)
                    PromptDisplay = UIManager.Instance.AddPromptToUI(PromptPrefab, InteractableName, InteractableAction);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                InRange = false;
                if(PromptDisplay != null)
                    Destroy(PromptDisplay);
            }
        }

        #region ENABLE/DISABLE
        private void OnDisable()
        {
            if (PromptDisplay != null)
                Destroy(PromptDisplay);
        }
        #endregion
    }
}
