using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

namespace AstralAbyss
{
    public class SpaceshipElementControl : MonoBehaviour
    {
        public static SpaceshipElementControl Instance;
        
        [Header("Component Property")]
        public List<GameObject> DecalGroups = new List<GameObject>();
        [SerializeField] Light _mainSpotLight;
        public List<EmissionObject> Emitters = new List<EmissionObject>();
        [System.Serializable]
        public class EmissionObject
        {
            public GameObject _object;
            public Renderer _renderer;
            public Material _material;
            public Color _emissionColor;
            public float _emissionIntensity;
            public bool _selectOn;
        }
        public List<Interactable> InteractableSystem = new List<Interactable>();
        public List<Material> SpaceshipMaterialVarient = new List<Material>();
        Renderer _spaceshipRender;
        public GameObject NormalLab;
        public GameObject FinalLab;

        #region INITIALIZATION
        private void Awake()
        {
            #region SINGLETON
            if(Instance != null)
            {
                Destroy(this.gameObject);
            }
            else if(Instance == null)
            {
                Instance = this;
            }
            #endregion

            //---> Initialize realtime emitter property <---//
            foreach (EmissionObject obj in Emitters)
            {
                obj._renderer = obj._object.GetComponent<Renderer>();
                obj._material = obj._renderer.material;
                obj._material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                obj._emissionColor = obj._material.GetColor("_EmissionColor");
                obj._selectOn = true;
            }
            _spaceshipRender = transform.GetChild(0).GetComponent<Renderer>();
        }
        private void Start()
        {
            //---> Set all realtime emiiter emission intensity <---//
            foreach (EmissionObject obj in Emitters)
            {
                obj._material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                obj._material.SetColor("_EmissionColor", obj._emissionColor * obj._emissionIntensity);
                RendererExtensions.UpdateGIMaterials(obj._renderer);
                DynamicGI.SetEmissive(obj._renderer, obj._emissionColor * obj._emissionIntensity);
                DynamicGI.UpdateEnvironment();
            }
        }
        #endregion

        #region SHIP ELEMENT
        public void SetMainSpotlightIntensity(float intensity, bool isOn)
        {
            _mainSpotLight.enabled = isOn;
            _mainSpotLight.intensity = intensity;
        }
        public void SetRealtimeLightEmission(int index, float value, bool isOn)
        {
            if (isOn)
            {
                Emitters[index]._material.EnableKeyword("_EMISSION");
                Emitters[index]._material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                Emitters[index]._material.SetColor("_EmissionColor", Emitters[index]._emissionColor * value);
                Emitters[index]._selectOn = isOn;

                RendererExtensions.UpdateGIMaterials(Emitters[index]._renderer);
                DynamicGI.SetEmissive(Emitters[index]._renderer, Emitters[index]._emissionColor * value);
                DynamicGI.UpdateEnvironment();
            }
            else if(!isOn)
            {
                Emitters[index]._material.DisableKeyword("_EMISSION");
                Emitters[index]._material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                Emitters[index]._material.SetColor("_EmissionColor", Color.black);
                Emitters[index]._selectOn = isOn;

                RendererExtensions.UpdateGIMaterials(Emitters[index]._renderer);
                DynamicGI.SetEmissive(Emitters[index]._renderer, Color.black);
                DynamicGI.UpdateEnvironment();
            }
        }
        public void DisplayDecalGroup(int index, bool value)
        {
            DecalGroups[index].SetActive(value);
        }
        public void SetSystemEnable(int index, bool value)
        {
            InteractableSystem[index].CanInteract = value;
        }
        public void SetSpaceshipMaterial(int index)
        {
            _spaceshipRender.material = SpaceshipMaterialVarient[index];
        }
        public void SwitchLab()
        {
            LabControl.Instance.InitiateSpacelabOffline();
            NormalLab.SetActive(false);
            FinalLab.SetActive(true);
        }
        #endregion
    }
}
