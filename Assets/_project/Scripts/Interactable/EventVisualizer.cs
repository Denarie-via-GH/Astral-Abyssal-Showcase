using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class EventVisualizer : Interactable
    {
        [Header("Property")]
        public int VisualizerIndex;
        public EventInstance Event;

        public void Initiate(Vector3 position, EventInstance eventReference, float displacement)
        {
            transform.position = position;
            Event = eventReference;

            LineRenderer line = GetComponent<LineRenderer>();
            line.SetPosition(0, transform.position);
            line.SetPosition(1, new Vector3(transform.position.x, displacement, transform.position.z));
        }
        void Awake()
        {
            InteractableName = "Detected Instance";
            InteractableAction = "Set Destination";
            CanInteract = true;
        }

        public override void Interact()
        {
            if (!CanInteract || !InRange)
                return;

            AudioManager.Instance.PlayGlobal((int)SFXClipIndex.INTERACT_VISUALIZER);
            AstralRadar.Instance.SelectEventDestination(Event);
        }
    }
}
