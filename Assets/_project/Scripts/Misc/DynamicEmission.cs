using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public class DynamicEmission : MonoBehaviour
    {
        [SerializeField] Material _mat;
        [SerializeField] Renderer _line;
        [SerializeField] Color _emissionColor;
        [SerializeField] float _transitionDuration = 5;
        [SerializeField] float _minEmission;
        [SerializeField] float _maxEmission;
        void Awake()
        {
            _line = GetComponentInChildren<Renderer>();
            _mat = _line.material;
            _emissionColor = _mat.GetColor("_EmissionColor");
            _mat.EnableKeyword("_EMISSION");
            //_mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            //RendererExtensions.UpdateGIMaterials(GetComponentInChildren<Renderer>());
        }
        void Start()
        {
            QueInmission();
        }

        void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.K))
            {
                _mat.EnableKeyword("_EMISSION");
                RendererExtensions.UpdateGIMaterials(_line);
                DynamicGI.UpdateEnvironment();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                _mat.DisableKeyword("_EMISSION");
                RendererExtensions.UpdateGIMaterials(_line);
                DynamicGI.UpdateEnvironment();
            }*/
        }

        void QueInmission()
        {
            StartCoroutine(IncreaseEmission(_minEmission, _maxEmission));
        }
        void QueDemission()
        {
            StartCoroutine(DecreaseEmission(_maxEmission, _minEmission));
        }
        IEnumerator IncreaseEmission(float origin, float target)
        {
            float timer = 0;
            while (timer < _transitionDuration)
            {
                float t = timer / _transitionDuration;
                float intensity = Mathf.Lerp(origin, target, t);

                _mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                _mat.SetColor("_EmissionColor", _emissionColor * intensity);
                RendererExtensions.UpdateGIMaterials(_line);
                DynamicGI.SetEmissive(_line, _emissionColor * intensity);
                DynamicGI.UpdateEnvironment();

                timer += Time.deltaTime;
                yield return null;
            }
            QueDemission();
        }
        IEnumerator DecreaseEmission(float origin, float target)
        {
            float timer = 0;
            while (timer < _transitionDuration)
            {
                float t = timer / _transitionDuration;
                float intensity = Mathf.Lerp(origin, target, t);

                _mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                _mat.SetColor("_EmissionColor", _emissionColor * intensity);
                RendererExtensions.UpdateGIMaterials(_line);
                DynamicGI.SetEmissive(_line, _emissionColor * intensity);
                DynamicGI.UpdateEnvironment();

                timer += Time.deltaTime;
                yield return null;
            }
            QueInmission();
        }
    }
}
