using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class TunnelGateLook : MonoBehaviour
    {
        [Header("Tunnel Gate Property")]
        public bool IsLooking = false;
        public bool IsGateOpen = false;
        [Range(0, 1)]
        public float Threshold = 0.8f;
        public float LookDuration = 2.5f;
        public float LookTimer;
        public float LookActiveRange = 50;
        Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        void Update()
        {
            if (Vector3.Distance(OrbiterCore.Instance.transform.position, transform.position) <= LookActiveRange)
                UpdateLookProgress();
        }

        public void UpdateLookProgress()
        {
            if (!IsGateOpen)
            {
                Vector3 DirectionToOrbiter = (OrbiterCore.Instance.transform.position - transform.position).normalized;
                float Dot = Vector3.Dot(DirectionToOrbiter, OrbiterCore.Instance.DirectionPivot.forward);
                if (-Dot >= Threshold && ControlCentral.Instance.InDockMode && !ControlCentral.Instance.InMinimap && !ControlCentral.Instance.InRadar)
                {
                    LookTimer += Time.deltaTime;
                    if (LookTimer >= LookDuration)
                    {
                        LookTimer = 0;
                        OpenTunnelGate();
                    }
                }
            }
        }

        private void OpenTunnelGate()
        {
            _animator.CrossFade("OpenGate", 1, 0);
            IsGateOpen = true;
        }
    }
}
