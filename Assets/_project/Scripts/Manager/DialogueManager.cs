using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AstralAbyss
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Dialogue Manager Property")]
        public Dialogue CurrentDialogue;
        public TextMeshProUGUI MainTextDisplay;
        [SerializeField] private int _currentDialogeIndex = 0;
        [SerializeField] private float _autoPlayTimer = 0;
        public bool PlayingDialogue = false;
        public GameObject SkipableIcon;
        public EventHandler OnDialogueStart;
        public EventHandler OnDialogueEnd;

        void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
            #endregion
        }
        void Start()
        {
            PlayingDialogue = false;
            _currentDialogeIndex = 0;
        }

        void Update()
        {
            if (CurrentDialogue != null)
            {
                if (CurrentDialogue.Lines[_currentDialogeIndex].CanSkip || MainTextDisplay.text == CurrentDialogue.Lines[_currentDialogeIndex].Text && CurrentDialogue.Lines[_currentDialogeIndex].CanSkip)
                    SkipableIcon.SetActive(true);
                else
                    SkipableIcon.SetActive(false);

                if (MainTextDisplay.text == CurrentDialogue.Lines[_currentDialogeIndex].Text && CurrentDialogue.IsAutoPlay || MainTextDisplay.text == CurrentDialogue.Lines[_currentDialogeIndex].Text && !CurrentDialogue.Lines[_currentDialogeIndex].CanSkip)
                {
                    float currentAutoPlayDelay = CurrentDialogue.Lines[_currentDialogeIndex].Delay;
                    if (currentAutoPlayDelay == 0) currentAutoPlayDelay = CurrentDialogue.Overwrite_AutoPlayDelay;

                    _autoPlayTimer += Time.deltaTime;
                    if (_autoPlayTimer >= currentAutoPlayDelay)
                    {
                        ResetAutoTimer();
                        NextLine();
                    }
                }
            }
        }

        #region DIALOGUE FUNCTION
        public void SetNewDialogue(Dialogue d)
        {
            DialogueEnd();
            AudioManager.Instance.PlayInterface((int)UIClipIndex.DIALOGUE);
            _currentDialogeIndex = 0;
            CurrentDialogue = d;
            InitiateDialogue();
        }
        private void InitiateDialogue()
        {
            //if (PlayingDialogue)
            //    return;
            MainTextDisplay.gameObject.SetActive(true);
            ResetMainTextDisplay();
            StartCoroutine(TypeNewLine());
            PlayingDialogue = true;
            GameManager.Instance.Request_FreezePlayer(true);
        }
        public void RequestNextLine()
        {
            if (CurrentDialogue == null)
                return;
            if (!CurrentDialogue.Lines[_currentDialogeIndex].CanSkip)
                return;

            if(MainTextDisplay.text == CurrentDialogue.Lines[_currentDialogeIndex].Text)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                MainTextDisplay.text = CurrentDialogue.Lines[_currentDialogeIndex].Text;
            }
        }
        void NextLine()
        {
            if(CurrentDialogue.IsAutoPlay)
                ResetAutoTimer();

            if (_currentDialogeIndex < CurrentDialogue.Lines.Count - 1)
            {
                _currentDialogeIndex++;
                InitiateDialogue();
            }
            else
            {
                DialogueEnd();
            }
        }

        IEnumerator TypeNewLine()
        {
            MainTextDisplay.color = CurrentDialogue.Lines[_currentDialogeIndex].Color;

            foreach(char character in CurrentDialogue.Lines[_currentDialogeIndex].Text.ToCharArray())
            {
                float currentTextSpeed = CurrentDialogue.Lines[_currentDialogeIndex].Speed;
                if (currentTextSpeed == 0) currentTextSpeed = CurrentDialogue.Overwrite_DialogueSpeed;

                MainTextDisplay.text += character;
                yield return new WaitForSeconds(currentTextSpeed);
            }
        }
        void DialogueEnd()
        {
            ResetMainTextDisplay();
            CurrentDialogue = null;
            MainTextDisplay.gameObject.SetActive(false);
            OnDialogueEnd?.Invoke(this, EventArgs.Empty);
            PlayingDialogue = false;
            GameManager.Instance.Request_FreezePlayer(false);
        }
        #endregion

        #region RESET FUNCTION
        void ResetMainTextDisplay()
        {
            MainTextDisplay.text = string.Empty;
            MainTextDisplay.color = Color.white;
        }
        void ResetAutoTimer()
        {
            _autoPlayTimer = 0;
        }
        #endregion

    }
}
