using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class AutoLockDirection : MonoBehaviour
    {
        public enum TargetType { Player, Orbiter}
        public TargetType LockOnTarget;

        void FixedUpdate()
        {
            Vector3 TargetDirection = Vector3.zero;
            if (LockOnTarget == TargetType.Player)
            {
                TargetDirection = (FirstPersonCamera.Instance.transform.position - transform.position).normalized;
            }
            else if(LockOnTarget == TargetType.Orbiter)
            {
                TargetDirection = (OrbiterCore.Instance.DirectionPivot.position - transform.position).normalized;
            }

            Quaternion Target = Quaternion.LookRotation(TargetDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, Target, Time.deltaTime);
        }
    }
}
