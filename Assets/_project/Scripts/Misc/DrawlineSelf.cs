using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class DrawlineSelf : MonoBehaviour
    {
        public float length;
        void Update()
        {
            GetComponent<LineRenderer>().SetPosition(0,transform.position);
            GetComponent<LineRenderer>().SetPosition(1,transform.position + transform.forward * length);
        }
    }
}
