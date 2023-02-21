using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class GateDoorButton : DoorButton
    {
        public override void Interact()
        {
            if (!CanInteract || !InRange)
                return;

            if (GravityZone.Instance.PlayerInProcessArea && GravityZone.Instance.ProcessDoorOpen)
            {
                GravityZone.Instance.ProcessExitZone();
            }
            else if (!GravityZone.Instance.PlayerInProcessArea) // Come from outside process area
            {
                if (IsOpen)//&& !_isOpening
                {
                    GravityZone.Instance.GateDoorOpen = false;
                    AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.DOOR_BUTTON_2, true);
                    OpenDoor(false);
                    AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.DOOR_SFX, true);
                    //StartCoroutine(LerpDoorPosition(OpenPOS, ClosePOS));
                }
                else if (!IsOpen)//&& !_isOpening
                {
                    GravityZone.Instance.GateDoorOpen = true;
                    AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.DOOR_BUTTON_2, true);
                    OpenDoor(true);
                    AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.DOOR_SFX, true);
                    //StartCoroutine(LerpDoorPosition(ClosePOS, OpenPOS));
                }
            }
        }

        public void ForceInteract(bool open)
        {
            if (open)
            {
                GravityZone.Instance.GateDoorOpen = true;
                AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.DOOR_BUTTON_2);
                OpenDoor(true);
                AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.DOOR_SFX, true);
                //StartCoroutine(LerpDoorPosition(ClosePOS, OpenPOS));
            }
            else
            {
                GravityZone.Instance.GateDoorOpen = false;
                AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.DOOR_BUTTON_2);
                OpenDoor(false);
                AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.DOOR_SFX, true);
                //StartCoroutine(LerpDoorPosition(OpenPOS, ClosePOS));
            }
        }
    }
}
