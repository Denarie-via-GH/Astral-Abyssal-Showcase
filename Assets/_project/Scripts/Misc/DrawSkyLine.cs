using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class DrawSkyLine : MonoBehaviour
    {
        LineRenderer _line;
        void Start()
        {
            _line = GetComponent<LineRenderer>();
        }

        void Update()
        {
            _line.SetPosition(0, transform.position);
            _line.SetPosition(1, transform.position + (Vector3.up * 100));
        }
    }
}
