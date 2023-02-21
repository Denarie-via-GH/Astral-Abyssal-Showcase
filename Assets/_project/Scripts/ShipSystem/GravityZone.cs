using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class GravityZone : MonoBehaviour
    {
        public static GravityZone Instance;

        public bool IsEnableZGravity;
        public bool PlayerInProcessArea;
        public bool GateDoorOpen;
        public bool ProcessDoorOpen;
        [SerializeField] GateDoorButton GateDoor;
        [SerializeField] ProcessDoorButton ProcessDoor;

        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else
                Instance = this;
            #endregion

            IsEnableZGravity = false;
            PlayerInProcessArea = false;
            GateDoorOpen = false;
            ProcessDoorOpen = false;
        }

        public void ProcessEnterZone()
        {
            if(PlayerInProcessArea && GateDoorOpen)
            {
                IsEnableZGravity = true;
                PlayerMain.Instance.IsZeroGravity = true;
                GateDoor.ForceInteract(false);
                ProcessDoor.ForceInteract(true);
                AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.VENTILATION, true);
                // Close First Door
                // Open Second Door
                // Enable ZGravity
            }
        }
        public void ProcessExitZone()
        {
            if(PlayerInProcessArea && ProcessDoorOpen)
            {
                IsEnableZGravity = false;
                PlayerMain.Instance.IsZeroGravity = false;
                GateDoor.ForceInteract(true);
                ProcessDoor.ForceInteract(false);
                AudioManager.Instance.PlaySource(GetComponent<AudioSource>(), (int)SFXClipIndex.VENTILATION, true);
                // Close Second Door
                // Open First Door
                // Disable ZGravity
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                PlayerInProcessArea = true;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                PlayerInProcessArea = false;
        }
    }
}
