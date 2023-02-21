using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class Edible : Interactable
    {
        FoodDispenser _foodDispenserSystem;
        [SerializeField] float _autoDeleteDelay = 30f;
        [SerializeField] Dialogue _edibleCustomDialogue;
        [SerializeField] AudioClip _edibleConsumeClip;
        private void Awake()
        {
            _foodDispenserSystem = FindObjectOfType<FoodDispenser>();

            InteractableAction = "consume";
            CanInteract = true;
        }
        private void Start()
        {
            StartCoroutine(DelayAutoDelete());
        }
        IEnumerator DelayAutoDelete()
        {
            yield return new WaitForSeconds(_autoDeleteDelay);

            _foodDispenserSystem.ResetDispenser();
            Destroy(this.gameObject);
        }

        public override void Interact()
        {
            if (!CanInteract || !InRange)
                return;

            _foodDispenserSystem.ResetDispenser();
            UIManager.Instance.PlayDialogue(_edibleCustomDialogue);
            AudioManager.Instance.PlayGlobal(_edibleConsumeClip);
            Destroy(this.gameObject);
        }
    }
}
