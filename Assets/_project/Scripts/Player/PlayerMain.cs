using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    [RequireComponent(typeof(CharacterController),typeof(CapsuleCollider))]
    public class PlayerMain : MonoBehaviour
    {
        public static PlayerMain Instance;

        [Header("Internal Property")]
        ControlScheme _inputControl;
        CharacterController _playerController;
        Rigidbody _rigibody;
        [SerializeField] Transform _cameraTransform;
        Vector2 _inputVector;
        Vector3 _internalVelocity;
        [SerializeField] Transform _orientation;
        AudioSource _footstepSource;
        Light _flashlight;
        //[SerializeField] LayerMask _groundMask;

        [Header("Character Property")]
        public bool IsFreezed = false;
        public bool IsFlashlightOn = false;
        public bool IsZeroGravity = false;

        [Header("Movement Property")]
        [Range(-100f, -9.81f)]
        public float GravityModifier = -9.81f;
        [Range(2f, 10f)]
        public float MoveSpeed;
        [Range(1f, 3.5f)]
        public float CrouchSpeed;
        public float FlightSpeed;
        public AudioClip FootstepClip;
        public AudioClip FlightClip;
        void Awake()
        {
            #region SINGLETON
            if (Instance != null) 
                Destroy(this.gameObject);
            else 
                Instance = this;
            #endregion

            //---> Initiate property <---//
            _inputControl = new ControlScheme();
            _footstepSource = GetComponent<AudioSource>();
            //_cameraTransform = FirstPersonCamera.Instance.transform;
            _playerController = GetComponentInChildren<CharacterController>();
            _rigibody = GetComponentInChildren<Rigidbody>();
            _flashlight = GetComponentInChildren<Light>();
            
            //IsFreezed = false;
            //IsZeroGravity = false;
            //IsFlashlightOn = false;
        }

        #region UPDATE
        void FixedUpdate()
        {
            if (!IsFreezed)
            {
                if (!IsZeroGravity)
                    Move();
                else if (IsZeroGravity)
                    Flight();
            }
        }
        void Update()
        {
            if (!IsFreezed)
            {
                PlayFootstepSound();
            }
        }
        #endregion

        void PlayFootstepSound()
        {
            if (!_footstepSource.isPlaying)
            {
                if (!IsZeroGravity && _inputVector != Vector2.zero)
                {
                    _footstepSource.clip = FootstepClip;
                    _footstepSource.Play();
                }
                else if (IsZeroGravity && _rigibody.velocity != Vector3.zero)
                {
                    _footstepSource.clip = FlightClip;
                    _footstepSource.Play();
                }
            }
        }
        void Move()
        {
            _playerController.enabled = true;
            _rigibody.isKinematic = true;
            _rigibody.useGravity = false;

            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 moveVector = _orientation.right * x + _orientation.forward * z;

            bool grounded = _playerController.isGrounded;
            if (grounded)
            {
                _internalVelocity.y = 0;
            }

            _playerController.Move(moveVector * MoveSpeed * Time.deltaTime);
            _internalVelocity.y += GravityModifier * Time.deltaTime;
            _playerController.Move(_internalVelocity * Time.deltaTime);
        }
        void Flight()
        {
            _playerController.enabled = false;
            _rigibody.useGravity = false;
            _rigibody.isKinematic = false;
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 moveVector = _cameraTransform.transform.right * x + _cameraTransform.transform.forward * z;
            _rigibody.AddForce(moveVector * (MoveSpeed * 0.75f));

            if (Input.GetKey(KeyCode.Space))
            {
                _rigibody.AddForce(Vector3.up * (MoveSpeed * 0.75f));
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                _rigibody.AddForce(Vector3.up * -(MoveSpeed * 0.75f));
            }
        }

        public void ToggleFlashlight()
        {
            if (IsFreezed)
                return;

            IsFlashlightOn = !IsFlashlightOn;
            _flashlight.enabled = IsFlashlightOn;
        }

        #region ENABLE/DISABLE
        private void OnEnable()
        {
            _inputControl.Enable();
            _inputControl.Player.Move.performed += ctx => _inputVector = ctx.ReadValue<Vector2>();
            _inputControl.Player.Move.canceled += ctx => _inputVector = Vector2.zero;
            _inputControl.Player.Flashlight.performed += ctx => ToggleFlashlight();
        }
        private void OnDisable()
        {
            _inputControl.Disable();
            _inputControl.Player.Move.performed -= ctx => _inputVector = ctx.ReadValue<Vector2>();
            _inputControl.Player.Move.canceled -= ctx => _inputVector = Vector2.zero;
            _inputControl.Player.Flashlight.performed -= ctx => ToggleFlashlight();
        }
        #endregion

        #region RETURN FUNCTION
        public Vector2 GetPlayerInput()
        {
            return _inputVector;
        }
        public void StopMotion()
        {
            _rigibody.velocity = Vector3.zero;
        }
        #endregion
    }
}
