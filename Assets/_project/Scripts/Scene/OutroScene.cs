using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class OutroScene : MonoBehaviour
    {
        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        public void ContinueButton()
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.LoadMenu());
        }
    }
}
