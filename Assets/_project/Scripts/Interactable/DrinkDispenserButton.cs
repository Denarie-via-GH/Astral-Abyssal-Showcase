using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class DrinkDispenserButton : Interactable
    {
        FoodDispenser _foodDispenserSystem;
        AudioSource _buttonSource;
        void Awake()
        {
            _foodDispenserSystem = FindObjectOfType<FoodDispenser>();
            _buttonSource = GetComponent<AudioSource>();

            InteractableName = "Dispense Drink";
            InteractableAction = "Use";
            CanInteract = true;
        }
        public override void Interact()
        {
            if (!CanInteract || !InRange)
                return;

            AudioManager.Instance.PlaySource(_buttonSource, (int)SFXClipIndex.BUTTON_1, true);
            _foodDispenserSystem.DispenseDrink();
        }
    }
}
