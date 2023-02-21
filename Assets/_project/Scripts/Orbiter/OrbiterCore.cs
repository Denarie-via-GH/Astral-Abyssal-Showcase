using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AstralAbyss
{
    public class OrbiterCore : MonoBehaviour
    {
        public static OrbiterCore Instance;

        [Header("Internal Property")]
        [SerializeField] LayerMask _objectiveRaycastMask;
        public ObjectiveInstance LookingObject;
        private Rigidbody _rigibody;
        public float CollisionDetectionRadius;
        public float CollisionDetectionDistance;
        public LayerMask CollisionDetectionMask;
        public bool IsFreezed = false;
        public bool NearLeft, NearRight, NearUp, NearDown;

        [Header("UTR Property")]
        public Transform DirectionPivot;
        public float RaycastDistance = 500;
        public Vector3 MoveDirection;
        public float RotationSpeed;
        [SerializeField] float MoveSpeed;
        [SerializeField] float MoveSpeedMultiplier;
        [SerializeField] float MaxSpeed;
        public float CameraFOV;
        public bool IsAutoRotating = false;
        public float AutoSetDuration = 5f;
        Camera _UTRCamera;

        [Header("Astral Scan & Tuning Property")]
        public bool IsTuning = false;
        public bool IsDetuning = false;
        public bool IsScanActive = false;
        public float TuningDuration = 2.5f;
        public float TuningTimer = 0;
        public float ScanActiveTimer = 0;
        public float ScanActiveDuration = 5;
        public float MinClipPlane = 0.5f;
        public float MaxClipPlane = 50f;
        public float MinFogDensity = 0.08f;
        public float MaxFogDensity = 0.8f;
        public EventHandler OnAstralScan;

        [Header("AudioSource Property")]
        public AudioSource BodySource;
        public AudioSource RotateSource;
        public AudioSource MoveSource;
        public AudioSource AutoSource;


        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
                Destroy(this.gameObject);
            else if (Instance == null)
                Instance = this;
            #endregion

            //---> Inititate property <---//
            _UTRCamera = DirectionPivot.GetComponentInChildren<Camera>();
            _UTRCamera.nearClipPlane = MinClipPlane;
            _UTRCamera.farClipPlane = MaxClipPlane;
            _UTRCamera.fieldOfView = CameraFOV;
            _rigibody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!IsFreezed)
            {
                RaycastObjective();
                MoveDirection = DirectionPivot.forward; //ManeuverRingPivot.forward;
                if(_rigibody.velocity.magnitude > MaxSpeed)
                {
                    _rigibody.velocity = Vector3.ClampMagnitude(_rigibody.velocity, MaxSpeed);
                }
            }
            else if (IsFreezed)
            {
                _rigibody.velocity = Vector3.zero;
            }

            
            

            NearLeft = CollisionCheck(0);
            NearRight = CollisionCheck(1);
            NearUp = CollisionCheck(2);
            NearDown = CollisionCheck(3);
            UIManager.Instance.UpdateCollisionWarning(NearLeft, NearRight, NearUp, NearDown);
            #region SCRAP
            /*
            if (IsTuning)
            {
                float t = TuningTimer / TuningDuration;
                TuningTimer += Time.deltaTime;
                _UTRCamera.farClipPlane = Mathf.Lerp(MinClipPlane, MaxClipPlane, t);
                if (TuningTimer >= TuningDuration)
                {
                    ResetTuningTimer();
                    IsTuning = false;
                    IsScanActive = true;
                }
            }

            if (IsDetuning)
            {
                float t = TuningTimer / TuningDuration;
                TuningTimer += Time.deltaTime;
                _UTRCamera.farClipPlane = Mathf.Lerp(MaxClipPlane, MinClipPlane, t);
                if (TuningTimer >= TuningDuration)
                {
                    ResetTuningTimer();
                    IsDetuning = false;
                    IsScanActive = false;
                }
            }
            */
            #endregion
        }
        bool CollisionCheck(int axis) // 0 = left 1 = right 2 = top 3 = down
        {
            switch (axis)
            {
                case (0):
                    Collider[] leftOverlap = Physics.OverlapSphere(transform.position + DirectionPivot.TransformDirection(Vector3.left) * CollisionDetectionDistance, CollisionDetectionRadius, CollisionDetectionMask);
                    if (leftOverlap.Length > 0) return true;
                    break;
                case (1):
                    Collider[] rightOverlap = Physics.OverlapSphere(transform.position + DirectionPivot.TransformDirection(Vector3.right) * CollisionDetectionDistance, CollisionDetectionRadius, CollisionDetectionMask);
                    if (rightOverlap.Length > 0) return true;
                    break;
                case (2):
                    Collider[] upOverlap = Physics.OverlapSphere(transform.position + DirectionPivot.TransformDirection(Vector3.up) * CollisionDetectionDistance, CollisionDetectionRadius, CollisionDetectionMask);
                    if (upOverlap.Length > 0) return true;
                    break;
                case (3):
                    Collider[] downOverlap = Physics.OverlapSphere(transform.position + DirectionPivot.TransformDirection(Vector3.down) * CollisionDetectionDistance, CollisionDetectionRadius, CollisionDetectionMask);
                    if (downOverlap.Length > 0) return true;
                    break;
            }

            #region SCRAP
            //Collider[] hitColliders = Physics.OverlapSphere(DirectionPivot.position, CollisionDetectionRadius, CollisionDetectionMask);
            //ContactPoint[] contacts = new ContactPoint[hitColliders.Length];

            //foreach (var hitCollider in hitColliders)
            //{
            //    Vector3 OverlapPoint = Vector3.zero;
            //    if (hitCollider.gameObject.CompareTag("Level"))
            //    {
            //        OverlapPoint = hitCollider.gameObject.GetComponent<TerrainCollider>().ClosestPointOnBounds(transform.position);
            //    }
            //    else
            //    {
            //         OverlapPoint = hitCollider.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            //    }
            //    Vector3 DirectionToOrbiter = (transform.position - OverlapPoint).normalized;
            //    float HeightCheck = (transform.position.y - OverlapPoint.y);

            //    Debug.Log($"Overlaping {hitCollider.name} at {OverlapPoint}");

            //    switch (axis) 
            //    {
            //        case (0):
            //            float dotCheckLeft = -Vector3.Dot(DirectionPivot.transform.TransformDirection(Vector3.left), DirectionToOrbiter);
            //            if (dotCheckLeft >= 0.4f) //(HeightCheck < 4 && HeightCheck > -4)
            //            {
            //                Debug.Log("Detect Left");
            //                return true;
            //            }
            //            break;
            //        case (1):
            //            float dotCheckRight = -Vector3.Dot(DirectionPivot.transform.TransformDirection(Vector3.right), DirectionToOrbiter);
            //            if (dotCheckRight >= 0.4f) //(HeightCheck < 4 && HeightCheck > -4)
            //            {
            //                Debug.Log("Detect Right");
            //                return true;
            //            }
            //            break;
            //        case (2):
            //            if(HeightCheck < -CollisionHeightCheck)
            //            {
            //                Debug.Log("Detect Top");
            //                return true;
            //            }
            //            break;
            //        case (3):
            //            if(HeightCheck > CollisionHeightCheck)
            //            {
            //                Debug.Log("Detect Down");
            //                return true;
            //            }
            //            break;
            //    }
            //    return false;
            //}
            #endregion

            return false;
        }
        void RaycastObjective()
        {
            RaycastHit hit;
            if (Physics.Raycast(DirectionPivot.position, DirectionPivot.forward, out hit, RaycastDistance, _objectiveRaycastMask))
            {
                LookingObject = hit.transform.gameObject.GetComponent<ObjectiveInstance>();
            }
            else
            {
                LookingObject = null;
            }
        }

        #region ASTRAL SCANNING & TUNING
        //public void InitiateAstralScanning()
        //{
        //    if (IsTuning || IsScanActive || IsDetuning)
        //        return;
        //    else
        //    {
        //        IsTuning = true;
        //        AudioManager.Instance.PlayGlobal((int)SFXClipIndex.INTERACT_SCAN);
        //        OnAstralScan?.Invoke(this, EventArgs.Empty);
        //        StartCoroutine(TuningProcess());
        //    }
        //}
        //public IEnumerator TuningProcess()
        //{
        //    IsTuning = true;
        //    while (TuningTimer < TuningDuration)
        //    {
        //        float t = TuningTimer / TuningDuration;

        //        //_UTRCamera.farClipPlane = Mathf.Lerp(MinClipPlane, MaxClipPlane, t);

        //        TuningTimer += Time.deltaTime;
        //        yield return null;
        //    }
        //    TuningTimer = 0;
        //    IsScanActive = true;
        //    IsTuning = false;
        //    yield return null;

        //}
        //public IEnumerator DeTuningProcess()
        //{
        //    IsDetuning = true;
        //    while (TuningTimer < TuningDuration)
        //    {
        //        float t = TuningTimer / TuningDuration;

        //        //_UTRCamera.farClipPlane = Mathf.Lerp(MaxClipPlane, MinClipPlane, t);

        //        TuningTimer += Time.deltaTime;
        //        yield return null;
        //    }
        //    TuningTimer = 0;
        //    IsScanActive = false;
        //    IsDetuning = false;
        //    yield return null;

        //}
        //void ResetTuningTimer()
        //{
        //    TuningTimer = 0;
        //    ScanActiveTimer = 0;
        //}
        #endregion

        #region ROTATION/NAVIGATION FUNCTION
        public void InitiateRingAutoOrientation(Transform target)
        {
            if (IsTuning || IsDetuning || IsAutoRotating)
            {
                Debug.LogError("Orbiter is doing other action.");
                return;
            }

            StopAllCoroutines();
            AudioManager.Instance.PlaySource(AutoSource, AutoSource.clip, false);
            StartCoroutine(LerpDirectionToTarget(target));
        }

        //public void InitiateSyncOrientation(int index, Transform target)
        //{
        //    if (IsTuning || IsDetuning || IsAutoRotating)
        //    {
        //        Debug.LogError("Orbiter is doing other action.");
        //        return;
        //    }

        //    StopAllCoroutines();
        //    AudioManager.Instance.PlaySource(AutoSource, AutoSource.clip, false);
        //    StartCoroutine(LerpRingMatchTarget(index, target));
        //}
        
        IEnumerator LerpDirectionToTarget(Transform target)
        {
            IsAutoRotating = true;
            Vector3 TargetDirection = Vector3.zero;
            Quaternion FinalDirection = Quaternion.identity;
            Quaternion saveRotation = Quaternion.identity;
            TargetDirection = (target.position - DirectionPivot.position).normalized;
            FinalDirection = Quaternion.LookRotation(TargetDirection, Vector3.up);
            saveRotation = DirectionPivot.rotation;
            
            float timer = 0;
            while (timer < AutoSetDuration)
            {
                float t = timer / AutoSetDuration;
                DirectionPivot.rotation = Quaternion.Lerp(saveRotation, FinalDirection, t);
                timer += Time.deltaTime;
                yield return null;
            }
            IsAutoRotating = false;
        }

        //IEnumerator LerpRingToTarget(int index, Transform target)
        //{
        //    //---> Save original transform <---//
        //    IsAutoRotating = true;
        //    Vector3 TargetDirection = Vector3.zero;
        //    Quaternion rotation = Quaternion.identity;
        //    Quaternion saveRotation = Quaternion.identity;
        //    if (index == 0)
        //    {
        //        TargetDirection = (target.position - CameraRingPivot.position).normalized;
        //        rotation = Quaternion.LookRotation(TargetDirection, Vector3.up);
        //        saveRotation = CameraRingPivot.rotation;
        //    }
        //    else if (index == 1)
        //    {
        //        TargetDirection = (target.position - ManeuverRingPivot.position).normalized;
        //        rotation = Quaternion.LookRotation(TargetDirection, Vector3.up);
        //        saveRotation = ManeuverRingPivot.rotation;
        //    }

        //    //---> Lerp to target rotation toward target position<---//
        //    float timer = 0;
        //    while (timer < AutoSetDuration)
        //    {
        //        float t = timer / AutoSetDuration;
        //        if (index == 0)
        //            CameraRingPivot.rotation = Quaternion.Lerp(saveRotation, rotation, t);
        //        else if (index == 1)
        //            ManeuverRingPivot.rotation = Quaternion.Lerp(saveRotation, rotation, t);
        //        timer += Time.deltaTime;
        //        yield return null;
        //    }
        //    IsAutoRotating = false;
        //}

        //IEnumerator LerpRingMatchTarget(int index, Transform target)
        //{
        //    //---> Save original transform <---//
        //    IsAutoRotating = true;
        //    Quaternion saveRotation = Quaternion.identity;
        //    if (index == 0)
        //        saveRotation = CameraRingPivot.rotation;
        //    else if (index == 1)
        //        saveRotation = ManeuverRingPivot.rotation;

        //    //---> Lerp to target rotation to match target rotation <---//
        //    float timer = 0;
        //    while (timer < AutoSetDuration)
        //    {
        //        float t = timer / AutoSetDuration;
        //        if(index == 0)
        //            CameraRingPivot.rotation = Quaternion.Lerp(saveRotation, target.rotation, t);
        //        else if (index == 1)
        //            ManeuverRingPivot.rotation = Quaternion.Lerp(saveRotation, target.rotation, t);
        //        timer += Time.deltaTime;
        //        yield return null;
        //    }
        //    IsAutoRotating = false;
        //}

        public void AddRotationToRing(Quaternion value)
        {
            DirectionPivot.rotation *= value;
            //if (index == 0)
            //    CameraRingPivot.rotation *= value;
            //else if (index == 1)
            //    ManeuverRingPivot.rotation *= value;
        }
        public void RotateRingAxis(int index, int axis)
        {
            switch (axis)
            {
                case (0): // Up
                    Quaternion Up = Quaternion.AngleAxis(RotationSpeed * -1 * Time.deltaTime, Vector3.right);
                    AddRotationToRing(Up);
                    break;
                case (1): // Down
                    Quaternion Down = Quaternion.AngleAxis(RotationSpeed * 1 * Time.deltaTime, Vector3.right);
                    AddRotationToRing(Down);
                    break;
                case (2): // Left
                    Quaternion Left = Quaternion.AngleAxis(RotationSpeed * -1 * Time.deltaTime, Vector3.up);
                    AddRotationToRing(Left);
                    break;
                case (3): // Right
                    Quaternion Right = Quaternion.AngleAxis(RotationSpeed * 1 * Time.deltaTime, Vector3.up);
                    AddRotationToRing(Right);
                    break;
            }
        }
        public void Move(bool isReverse)
        {
            if(IsAutoRotating || IsTuning || IsDetuning)
            {
                Debug.LogError("Orbiter is doing other action.");
                return;
            }

            if(!isReverse)
                _rigibody.AddForce(MoveDirection * (MoveSpeed * MoveSpeedMultiplier), ForceMode.Force);
            else if(isReverse)
                _rigibody.AddForce(-MoveDirection * (MoveSpeed * MoveSpeedMultiplier), ForceMode.Force);

            //transform.Translate(MoveDirection * MoveSpeed * Time.deltaTime, Space.World);
            //transform.Translate(MoveDirection * MoveSpeed * Time.deltaTime);
        }
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + DirectionPivot.TransformDirection(Vector3.up) * CollisionDetectionDistance);
            Gizmos.DrawWireSphere(transform.position + DirectionPivot.TransformDirection(Vector3.up) * CollisionDetectionDistance, CollisionDetectionRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + DirectionPivot.TransformDirection(Vector3.down) * CollisionDetectionDistance);
            Gizmos.DrawWireSphere(transform.position + DirectionPivot.TransformDirection(Vector3.down) * CollisionDetectionDistance, CollisionDetectionRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + DirectionPivot.TransformDirection(Vector3.left) * CollisionDetectionDistance);
            Gizmos.DrawWireSphere(transform.position + DirectionPivot.TransformDirection(Vector3.left) * CollisionDetectionDistance, CollisionDetectionRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + DirectionPivot.TransformDirection(Vector3.right) * CollisionDetectionDistance);
            Gizmos.DrawWireSphere(transform.position + DirectionPivot.TransformDirection(Vector3.right) * CollisionDetectionDistance, CollisionDetectionRadius);
        }
    }
}
