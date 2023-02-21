using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class FoodDispenser : Interactable
    {
        [Header("Custom Interactable Property")]
        [SerializeField] List<GameObject> _ediblePrefabList = new List<GameObject>();
        [SerializeField] Transform _spawnPosition;
        [SerializeField] Edible _dispensedEdible;
        AudioSource _dispenserSource;
        bool _isOccupied = false;
        void Awake()
        {
            _dispenserSource = GetComponent<AudioSource>();

            InteractableName = "Food Dispenser";
        }

        public void DispenseFood()
        {
            if (_dispensedEdible == null && !_isOccupied)
            {
                AudioManager.Instance.PlaySource(_dispenserSource, (int)SFXClipIndex.DISPENSE, true);
                _dispensedEdible = Instantiate(_ediblePrefabList[0], _spawnPosition).GetComponent<Edible>();
                _isOccupied = true;
            }
        }
        public void DispenseDrink()
        {
            if (_dispensedEdible == null && !_isOccupied)
            {
                AudioManager.Instance.PlaySource(_dispenserSource, (int)SFXClipIndex.DISPENSE, true);
                _dispensedEdible = Instantiate(_ediblePrefabList[1], _spawnPosition).GetComponent<Edible>();
                _isOccupied = true;
            }
        }
        public void ResetDispenser()
        {
            _isOccupied = false;
            _dispensedEdible = null;
        }
    }
}
