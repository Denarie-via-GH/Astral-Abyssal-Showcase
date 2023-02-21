using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaUtilLib;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Rendering;
using Cinemachine;

namespace AstralAbyss
{
    public class FirstPersonCamera : MonoBehaviour
    {
        public static FirstPersonCamera Instance;

        [Header("Internal Property")]
        ControlScheme _inputCamera;
        //public Camera ViewCamera;
        Vector2 _camRotation;
        Vector2 _scrollInput;
        EventSystem _defaultEventSystem;
        EventSystem _currentUIEventSystem;
        [SerializeField] Transform _orientation;
        public CinemachineVirtualCamera VirtualCam;

        [Header("Camera Property")]
        [SerializeField] private float _sensitivity;
        [SerializeField] private float _sensitivityMultiplier = 10;
        [SerializeField] private float _scrollSpeed;
        public float NearClipPlane = 0.3f;
        public float FarClipPlane = 1000f;
        public float FieldOfView = 60f;
        public enum VirtualPointerType { Click, Hold }
        public VirtualPointerType CurrentPointerType = VirtualPointerType.Click;
        public float ShakeTimer;
        public float ShakeTimerTotal;
        private float StartShakeIntensity;

        [Header("Focus Target Property")]
        public List<CameraFocusTarget> FocusTarget;
        public float FocusDuration = 0.75f;
        public bool IsFocus;
        Vector3 _originPosition;
        Quaternion _originRotation;
        public bool IsOverrideFocus;
        Vector3 _overridePosition;
        Quaternion _overrideRotation;
        [System.Serializable]
        public class CameraFocusTarget
        {
            public string TargetName;
            public int TargetID;
            public Transform TargetTransform;
            public Vector3 PositionOffset;
            public float FOV;

        }

        [Header("Object Raycast Property")]
        public LayerMask RaycastMask;
        public Interactable SelectingObject;
        public bool CanInteractObject;

        [Header("UI Raycast Property")]
        public LayerMask UIRaycastMask;
        public Button SelectingButton;
        public Scrollbar SelectingBar;
        public Slider SelectingSlider;
        public bool IsPointerOnCanvas;
        public float ContactDistance;

        void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else
                Instance = this;
            #endregion

            //---> Inititae Property <---//
            _inputCamera = new ControlScheme();
            VirtualCam = GetComponent<CinemachineVirtualCamera>();
            _originPosition = transform.localPosition;
            _originRotation = transform.rotation;
            _overridePosition = transform.localPosition;
            _overrideRotation = transform.rotation;
            IsFocus = false;
            IsOverrideFocus = false;
            IsPointerOnCanvas = false;
        }
        void Start()
        {
            //---> Disable all other event systems <---//
            EventSystem[] ES = FindObjectsOfType<EventSystem>();
            foreach(EventSystem obj in ES)
            {
                obj.enabled = false;
            }
            _defaultEventSystem = GameObject.FindGameObjectWithTag("MainEventSystem").GetComponent<EventSystem>();
            _currentUIEventSystem = _defaultEventSystem;
            _currentUIEventSystem.enabled = true; // Enable camera event system
        }
        void Update()
        {
            if (!GameManager.Instance.IsPause)
            {
                if (!PlayerMain.Instance.IsFreezed)
                {
                    if (!IsFocus)
                    {
                        _sensitivity = GameManager.Instance.Sensitivity * _sensitivityMultiplier;
                        _camRotation.x += Input.GetAxis("Mouse X") * _sensitivity;
                        _camRotation.y += Input.GetAxis("Mouse Y") * _sensitivity;
                        _camRotation.y = Mathf.Clamp(_camRotation.y, -88, 88);

                        Quaternion yQuaternion = Quaternion.AngleAxis(_camRotation.y, Vector3.left);
                        Quaternion xQuaternion = Quaternion.AngleAxis(_camRotation.x, Vector3.up);
                        transform.localRotation = xQuaternion * yQuaternion;
                        _orientation.rotation = xQuaternion;
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ActiveInteractable();
                    }

                    VirtualInteractionOnUI();
                }

                if (ShakeTimer > 0)
                {
                    ShakeTimer -= Time.deltaTime;
                    CinemachineBasicMultiChannelPerlin BasicMultiChannelPerlin = FirstPersonCamera.Instance.VirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    BasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(StartShakeIntensity, 0f, 1 - (ShakeTimer / ShakeTimerTotal));
                }
            }
        }
        void FixedUpdate()
        {
            if (!GameManager.Instance.IsPause)
            {
                if (!PlayerMain.Instance.IsFreezed)
                {
                    RaycastObject();
                }
            }
        }
        public void ShakeCam(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin BasicMultiChannelPerlin = FirstPersonCamera.Instance.VirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            BasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            
            StartShakeIntensity = intensity;
            ShakeTimerTotal = time;
            ShakeTimer = time;
        }

        #region FOCUS TARGET
        public void InitiateFocusTarget(int index)
        {
            if (IsFocus || IsOverrideFocus)
                return;

            if (PlayerMain.Instance.IsFlashlightOn)
                PlayerMain.Instance.ToggleFlashlight();
            StartCoroutine(FocusCamera(index));
        }
        public void OverrideFocusTarget(int index)
        {
            StopAllCoroutines();
            StartCoroutine(OverrideFocusCamera(index));
        }
        IEnumerator OverrideFocusCamera(int id)
        {
            IsOverrideFocus = true;
            CameraFocusTarget gettarget = FocusTarget[id];

            float timer = 0;
            while (timer < FocusDuration)
            {
                float t = timer / FocusDuration;
                transform.position = Vector3.Lerp(transform.position, (gettarget.TargetTransform.position + gettarget.PositionOffset), t);
                transform.rotation = Quaternion.Lerp(transform.rotation, gettarget.TargetTransform.rotation, t);
                //_camera.fieldOfView = Mathf.Lerp(FieldOfView, gettarget.FOV, t);
                timer += Time.deltaTime;
                yield return null;
            }
        }
        IEnumerator FocusCamera(int id)
        {
            //---> Save original transform <---//
            IsFocus = true;
            _originPosition = transform.localPosition;
            _originRotation = transform.rotation;
            CameraFocusTarget gettarget = FocusTarget[id];

            //---> Lerp to target transform by focus duration <---//
            float timer = 0;
            while(timer < FocusDuration)
            {
                float t = timer / FocusDuration;
                transform.position = Vector3.Lerp(transform.position, (gettarget.TargetTransform.position + gettarget.PositionOffset), t);
                transform.rotation = Quaternion.Lerp(transform.rotation, gettarget.TargetTransform.rotation, t);
                //_camera.fieldOfView = Mathf.Lerp(FieldOfView, gettarget.FOV, t);
                timer += Time.deltaTime;
                yield return null;
            }
        }
        public void StopFocusCamera()
        {
            if (IsFocus)
            {
                StopAllCoroutines();
                transform.localPosition = _originPosition;
                transform.rotation = _originRotation;
                //_camera.fieldOfView = FieldOfView;
                IsFocus = false;
                IsOverrideFocus = false;
            }
        }
        #endregion

        #region VIRTUAL POINTER
        //---> Run this function on update to allow virtual pointer feature on world ui <---//
        void VirtualInteractionOnUI()
        {
            //---> Check if virtual pointer over world canvas and player isn't freezed <---//
            IsPointerOnCanvas = VirtualPointerOverUI();
            if (IsPointerOnCanvas && !PlayerMain.Instance.IsFreezed)
            {
                //---> Get any pointing ui element <---//
                SelectingButton = GetPointingButton();
                SelectingBar = GetPointingScrollbar();
                SelectingSlider = GetPointingSlider();
                //---> Virutal button feature <---//
                if (SelectingButton != null)
                {
                    if (CurrentPointerType == VirtualPointerType.Click && Input.GetMouseButtonDown(0) || CurrentPointerType == VirtualPointerType.Hold && Input.GetMouseButton(0))
                        ClickButtonVirtual(SelectingButton);
                    if (Input.GetMouseButtonUp(0))
                        ReleaseButtonVirtual(SelectingButton);
                }
                //---> Virtual scrolling feature <---//
                if (SelectingBar != null && _scrollInput != Vector2.zero)
                {
                    ScrollbarVirtual(SelectingBar);
                }
                //---> Virtual slider feature <---//
                if(SelectingSlider != null && _scrollInput != Vector2.zero)
                {
                    ScrollSliderVirtual(SelectingSlider);
                }
            }
            else
            {
                SelectingButton = null;
                SelectingBar = null;
            }
        }

        #region VIRUTAL CONTROL
        void HightlightButtonVirtual(Button btn)
        {
            btn.OnPointerEnter(VirtualPointerEventData());
        }
        void ClickButtonVirtual(Button btn)
        {
            AudioManager.Instance.PlayInterface(0);
            btn.onClick.Invoke();
            btn.OnPointerDown(VirtualPointerEventData());
        }
        void ReleaseButtonVirtual(Button btn)
        {
            btn.OnPointerUp(VirtualPointerEventData());
        }
        void ExitButtonVirtual(Button btn)
        {
            btn.OnPointerExit(VirtualPointerEventData());
        }
        void ScrollbarVirtual(Scrollbar bar)
        {
            bar.value += _scrollSpeed * (_scrollInput.y / 120);
        }
        void ScrollSliderVirtual(Slider slider)
        {
            slider.value += _scrollSpeed * (_scrollInput.y / 120);
        }
        #endregion

        #endregion

        #region RAYCAST UI
        Button GetPointingButton()
        {
            GameObject hitUI = VerifyUITarget();
            if (hitUI)
            {
                Button checkButton = hitUI.GetComponent<Button>();
                if (checkButton != null)
                {
                    if(checkButton != SelectingButton && SelectingButton != null)
                    {
                        ExitButtonVirtual(SelectingButton);
                    }
                    HightlightButtonVirtual(checkButton);
                    return checkButton;
                }
            }
            else if (hitUI == null && SelectingButton != null)
            {
                ExitButtonVirtual(SelectingButton);
                return null;
            }

            return null;
        }
        Scrollbar GetPointingScrollbar()
        {
            GameObject hitUI = VerifyUITarget();
            if (hitUI)
            {
                Scrollbar checkScrollbar = hitUI.GetComponent<Scrollbar>();
                return checkScrollbar;
            }
            return null;
        }
        Slider GetPointingSlider()
        {
            GameObject hitUI = VerifyUITarget();
            if (hitUI)
            {
                Slider checkSlider = hitUI.GetComponent<Slider>();
                return checkSlider;
            }
            return null;
        }
        GameObject VerifyUITarget()
        {
            if (RaycastUI() == null)
                return null;
            else
                return RaycastUI().CompareTag("UITarget") ? RaycastUI() : null;
        }
        GameObject RaycastUI()
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(VirtualPointerEventData(), results);
            return results.Count < 1 ? null : results[0].gameObject;
        }
        PointerEventData VirtualPointerEventData()
        {
            PointerEventData data = new PointerEventData(_currentUIEventSystem);
            data.position = new Vector2(Screen.width / 2, Screen.height / 2);
            return data;
        }
        bool VirtualPointerOverUI()
        {
            var hitObject = RaycastUI();
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, ContactDistance, UIRaycastMask))
            {
                if (hit.transform.gameObject.CompareTag("UIPanel"))
                {
                    if (_currentUIEventSystem != null)
                        _currentUIEventSystem.enabled = false;

                    _currentUIEventSystem = hit.transform.gameObject.GetComponent<EventSystem>();
                    _currentUIEventSystem.enabled = true;
                    return true;
                }
            }
            else
            {
                _currentUIEventSystem.enabled = false;

                _currentUIEventSystem = _defaultEventSystem;
                _currentUIEventSystem.enabled = true;
                return false;
            }

            return hitObject != null && hitObject.layer == LayerMask.NameToLayer("WorldUI");
        }
        public void ResetEventSystem()
        {
            _currentUIEventSystem.enabled = false;

            _currentUIEventSystem = _defaultEventSystem;
            _currentUIEventSystem.enabled = true;
        }
        #endregion

        #region RAYCAST OBJECT
        void RaycastObject()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, RaycastMask))
            {
                SelectingObject = hit.transform.gameObject.GetComponent<Interactable>();
                if (SelectingObject)
                {
                    if (SelectingObject.CanInteract && SelectingObject.InRange)
                        CanInteractObject = true;
                }
                else
                {
                    CanInteractObject = false;
                }
            }
            else
            {
                SelectingObject = null;
                CanInteractObject = false;
            }
        }
        public void ActiveInteractable()
        {
            if (SelectingObject != null && !PlayerMain.Instance.IsFreezed)
            {
                SelectingObject.Interact();
            }
        }
        #endregion

        #region ENABLE/DISABLE
        void OnEnable()
        {
            _inputCamera.Enable();
            //_inputCamera.UI.Interact.performed += ctx => ActiveInteractable();
            _inputCamera.UI.Scroll.performed += ctx => _scrollInput = ctx.ReadValue<Vector2>();
            _inputCamera.UI.Scroll.canceled += ctx => _scrollInput = Vector2.zero;

            //RenderPipelineManager.beginCameraRendering += SetViewCamera;
        }
        void OnDisable()
        {
            _inputCamera.Disable();
            //_inputCamera.UI.Interact.performed -= ctx => ActiveInteractable();
            _inputCamera.UI.Scroll.performed -= ctx => _scrollInput = ctx.ReadValue<Vector2>();
            _inputCamera.UI.Scroll.canceled -= ctx => _scrollInput = Vector2.zero;

            //RenderPipelineManager.beginCameraRendering -= SetViewCamera;
        }
        #endregion
    }
}