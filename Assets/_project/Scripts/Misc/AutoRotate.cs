using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class AutoRotate : MonoBehaviour
    {
        public Vector3 rotationAxis;
        void FixedUpdate()
        {
            transform.localRotation *= Quaternion.Euler(rotationAxis);
        }
    }
}
