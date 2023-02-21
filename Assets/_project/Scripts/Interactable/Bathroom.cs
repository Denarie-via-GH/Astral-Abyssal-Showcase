using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class Bathroom : Interactable
    {
        [SerializeField] float _bathroomDuration;
        void Awake()
        {
            InteractableName = "Bathroom";
            InteractableAction = "Use";
            CanInteract = true;
        }
        public override void Interact()
        {
            if (!CanInteract || !InRange)
                return;

            StartCoroutine(UsingBathroom());
        }

        IEnumerator UsingBathroom()
        {
            UIManager.Instance.StartTransitionScreen();
            GameManager.Instance.Request_FreezePlayer(true);
            yield return new WaitForSeconds(2f);

            AudioManager.Instance.PlayGlobal((int)SFXClipIndex.USE_BATHROOM);
            yield return new WaitForSeconds(_bathroomDuration);

            UIManager.Instance.CloseTransitionScreen();
            GameManager.Instance.Request_FreezePlayer(false);
        }
    }
}
