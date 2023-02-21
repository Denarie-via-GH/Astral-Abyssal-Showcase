using UnityEngine;
using UnityEngine.SceneManagement;

namespace AstralAbyss
{
    public class IntroScene : MonoBehaviour
    {
        public void ContinueButton()
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.LoadGame());
        }
    }
}
