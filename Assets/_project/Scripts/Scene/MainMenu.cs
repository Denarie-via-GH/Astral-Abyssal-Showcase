using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace AstralAbyss
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject Setting;
        [SerializeField] GameObject Credit;

        public void StartButton()
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.LoadIntro());
        }
        public void OpenSetting(bool open)
        {
            Setting.SetActive(open);
        }
        public void QuitButton()
        {
            Application.Quit();
        }
        public void OpenCredit(bool open)
        {
            Credit.SetActive(open);
        }
        
    }
}
