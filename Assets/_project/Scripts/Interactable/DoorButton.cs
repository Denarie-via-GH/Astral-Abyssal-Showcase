using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class DoorButton : Interactable
    {
        [Header("Property")]
        public GameObject DoorTarget;
        public Animator DoorAnim;
        public Vector3 OpenPOS;
        public Vector3 ClosePOS;
        public bool IsOpen;
        //protected bool _isOpening;
        public float TransitionDuration;
        public int DoorID;

        void Awake()
        {
            CanInteract = true;
            InteractableName = "Door";
            InteractableAction = "Open";
        }
        public override void Interact()
        {
            if (!CanInteract || !InRange)
                return;

            if (IsOpen)
            {
                AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.BUTTON_1, true);
                OpenDoor(false);
                //StartCoroutine(LerpDoorPosition(OpenPOS, ClosePOS));
            }
            else if (!IsOpen)
            {
                AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.BUTTON_1, true);
                OpenDoor(true);
                //StartCoroutine(LerpDoorPosition(ClosePOS, OpenPOS));
            }
        }

        protected void SignalOtherButtons()
        {
            DoorButton[] Doors = FindObjectsOfType<DoorButton>();
            foreach (DoorButton Door in Doors)
            {
                if (Door.DoorID == this.DoorID)
                {
                    Door.IsOpen = this.IsOpen;
                }
            }
        }
        protected void OpenDoor(bool value)
        {
            IsOpen = value;
            DoorAnim.SetBool("Open", IsOpen);
            SignalOtherButtons();
            GameManager.Instance.Request_FreezePlayerDelay(0.5f);
        }

        /*
        protected IEnumerator LerpDoorPosition(Vector3 origin, Vector3 target)
        {
            //_isOpening = true;
            GameManager.Instance.Request_FreezePlayer(true);
            float timer = 0;
            while (timer < TransitionDuration)
            {
                float t = timer / TransitionDuration;
                DoorTarget.transform.localPosition = Vector3.Lerp(origin, target, t);
                timer += Time.deltaTime;
                yield return null;
            }

            IsOpen = !IsOpen;
            DoorButton[] Doors = FindObjectsOfType<DoorButton>();
            foreach (DoorButton Door in Doors)
            {
                if (Door.DoorID == this.DoorID)
                {
                    Door.IsOpen = this.IsOpen;
                }
            }
            GameManager.Instance.Request_FreezePlayer(false);
            //_isOpening = false;
        }
        */
    }
}
