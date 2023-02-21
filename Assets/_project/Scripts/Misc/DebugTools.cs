using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class DebugTools : MonoBehaviour
    {
        //bool _isDebugMenuOpen = false;
        //public GameObject DebugMenu;
        private void Start()
        {
            //DebugMenu = GameObject.FindGameObjectWithTag("DebugMenu");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                EventInstanceController.Instance.FORCE_COMPLETE();
            }
        }
    }
}
