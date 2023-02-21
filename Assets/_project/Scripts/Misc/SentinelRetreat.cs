using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class SentinelRetreat : MonoBehaviour
    {
        [Header("Property")]
        bool _inRange = false;
        bool _isRetreating = false;
        public float RetreatDuration = 5;
        [Range(0f, 1f)]
        public float Threshold = 0.8f;
        
        void Update()
        {
            if (_inRange && !_isRetreating)
            {
                Vector3 DirectionToOrbiter = (OrbiterCore.Instance.transform.position - transform.position).normalized;
                float Dot = Vector3.Dot(DirectionToOrbiter, OrbiterCore.Instance.DirectionPivot.forward);
                if (-Dot >= Threshold && ControlCentral.Instance.InDockMode && !ControlCentral.Instance.InMinimap && !ControlCentral.Instance.InRadar)
                {
                    StartCoroutine(Retreat());
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Orbiter") && !_isRetreating)
            {
                _inRange = true;
            }
        }
        IEnumerator Retreat()
        {
            _isRetreating = true;
            Vector3 origin = transform.position;
            Vector3 destination = transform.position + new Vector3(0, 100, 0);
            float timer = 0;
            while(timer < RetreatDuration)
            {
                float t = timer / RetreatDuration;
                transform.position = Vector3.Lerp(origin, destination, t);
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(this.gameObject);
        }
    }
}
