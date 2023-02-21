using UnityEngine;
using System;

namespace AstralAbyss
{
    public class EventTriggerSignal : MonoBehaviour
    {
        public EventHandler OnTrigger;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Orbiter"))
            {
                GetComponent<Collider>().enabled = false;
                OnTrigger?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
