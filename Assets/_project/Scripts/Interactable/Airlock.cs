using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class Airlock : Interactable
    {
        public Transform SpawnTarget;
        public float TransitionDuration;
        public bool IsForceLock;
        public Dialogue WarningDialogue;
        private void Awake()
        {
            InteractableName = "Airlock";
            InteractableAction = "Enter";
            CanInteract = true;
        }

        public override void Interact()
        {
            if (!CanInteract || !InRange)
                return;

            if (!IsForceLock)
            {
                PlayerMain.Instance.StopMotion();
                StartCoroutine(StartTransition());
            }
            else if (IsForceLock)
            {
                UIManager.Instance.PlayDialogue(WarningDialogue);
            }
        }

        IEnumerator StartTransition()
        {
            UIManager.Instance.StartTransitionScreen();
            GameManager.Instance.Request_FreezePlayer(true);
            yield return new WaitForSeconds(2f);

            AudioManager.Instance.PlaySource(GetComponent<AudioSource>());
            yield return new WaitForSeconds(TransitionDuration);
            PlayerMain.Instance.transform.position = SpawnTarget.position;
            FirstPersonCamera.Instance.transform.rotation = SpawnTarget.rotation;

            UIManager.Instance.CloseTransitionScreen();
            GameManager.Instance.Request_FreezePlayer(false);
        }
    }
}
