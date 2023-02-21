using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using Cinemachine;

namespace AstralAbyss
{
    public class DevourerManager : MonoBehaviour
    {
        public enum TrackType { Sleep, Approach, Swarm, HorizonPass, VerticalPass }
        public TrackType CurrentTrack;

        [Header("Internal Property")]
        public Cinemachine.CinemachineSmoothPath PathTrack;
        public Cinemachine.CinemachineDollyCart DevourerCart;
        public GameObject Devourer;
        public bool DevourerActive;
        public bool IsLoop;
        public bool IsActivating;
        public bool IsDeactivating;
        public Transform ViewerTransform;
        public Transform ViewerLooker;
        [ColorUsage(true, true)]
        public Color ActiveColor;
        [ColorUsage(true, true)]
        public Color DeactiveColor;
        public float DeactiveDelay;
        public GameObject[] BodySegments;

        [Header("Encounter Property")]
        public float MinDevourerEncounterDelay;
        public float MaxDevourerEncounterDelay;
        public float DevourerEncounterInterval;
        public float EncounterRate = 1;
        //public float DevourerEncounterTimer;

        [Header("Camera Shake Property")]
        public float DevourerShakeIntensity;
        public float DevourerShakeDuration;

        [Header("Approach Track")]
        public float ApproachStartRadius;
        public float ApproachIntersectRadius;
        public float ApproachEndRadius;
        public float ApproachOffsetDistance;
        public float ApproachSpeed;
        public List<AudioClip> ApproachClips = new List<AudioClip>();

        [Header("Swarm Track")]
        public float SwarmRadius;
        public float SwarmIntersectVerticalOffsetDistance;
        public float SwarmIntersectHorizontalOffsetDistance;
        public float SwarmSpeed;
        public List<AudioClip> SwarmClips = new List<AudioClip>();

        [Header("Horizontal Track")]
        public float HPassOffsetDistance;
        public float HPassIntersectRadius;
        public float HPassDistanceMin;
        public float HPassDistanceMax;
        public float HPassSpeed;
        public List<AudioClip> HPassClips = new List<AudioClip>();

        [Header("Vertical Track")]
        public float VPassOffsetDistance;
        public float VPassIntersectRadius;
        public float VPassDistanceMin;
        public float VPassDistanceMax;
        public float VPassSpeed;
        public List<AudioClip> VPassClips = new List<AudioClip>();

        private void Awake()
        {
            PathTrack = FindObjectOfType<Cinemachine.CinemachineSmoothPath>();
            DevourerCart = FindObjectOfType<Cinemachine.CinemachineDollyCart>();
            Devourer = DevourerCart.gameObject.transform.GetChild(0).gameObject;
            BodySegments = GameObject.FindGameObjectsWithTag("DevourerSegment");
            
            ViewerTransform = OrbiterCore.Instance.transform;
            ViewerLooker = OrbiterCore.Instance.DirectionPivot;
        }
        private void Start()
        {
            foreach (GameObject seg in BodySegments)
            {
                seg.SetActive(false);
            }

            DevourerActive = false;
            IsDeactivating = false;
            IsActivating = false;
            IsLoop = true;

            EncounterRate = 1;
        }
        void Update()
        {
            if (DevourerActive)
            {
                if (IsLoop)
                {
                    if (IsActivating)
                    {
                        if (DevourerCart.m_Position >= PathTrack.PathLength && !IsDeactivating)
                        {
                            CurrentTrack = TrackType.Sleep;
                            StartNextEncoutnerInterval();
                            StartCoroutine(DeactiveDevourerProcess());
                        }
                    }
                    else if (!IsActivating)
                    {
                        if (DevourerEncounterInterval > 0)
                        {
                            DevourerEncounterInterval -= Time.deltaTime;
                            if (DevourerEncounterInterval <= 0)
                            {
                                DevourerEncounterInterval = 0;
                                CurrentTrack = (TrackType)UnityRandom.Range(1, 4);
                                GenerateTrack(CurrentTrack);
                            }
                        }
                    }
                }
            }
        }
        public void StopDevourer()
        {
            StopAllCoroutines();
            CurrentTrack = TrackType.Sleep;
            StartCoroutine(DeactiveDevourerProcess());
            DevourerActive = false;
        }
        public void StartNextEncoutnerInterval()
        {
            float randomize = UnityRandom.Range(MinDevourerEncounterDelay * EncounterRate, MaxDevourerEncounterDelay * EncounterRate);
            DevourerEncounterInterval = randomize;
            //DevourerEncounterTimer = randomize;
        }
        public void GenerateTrack(TrackType type)
        {
            Debug.Log("Generate New Track");
            Vector3 viewerPosition = ViewerTransform.position;
            Vector3 startPosition;
            Vector3 endPosition;
            int index;
            AudioClip clip;

            switch (type)
            {
                case (TrackType.Approach):
                    Vector3 randomSphereStart = UnityRandom.insideUnitSphere * ApproachStartRadius;
                    Vector3 randomSphereIntersect = UnityRandom.insideUnitSphere * ApproachIntersectRadius;
                    Vector3 randomSphereEnd = UnityRandom.insideUnitSphere * ApproachEndRadius;

                    startPosition = (viewerPosition + (ViewerLooker.TransformDirection(Vector3.forward) * ApproachOffsetDistance)) + randomSphereStart;
                    Vector3 intersect = (viewerPosition + ((ViewerLooker.TransformDirection(Vector3.forward) * ApproachOffsetDistance) / 2) + randomSphereIntersect);
                    endPosition = (viewerPosition - (ViewerLooker.TransformDirection(Vector3.forward) * ApproachOffsetDistance) * 1.5f) + randomSphereEnd;
                    
                    PathTrack.m_Waypoints[0].position = startPosition;
                    PathTrack.m_Waypoints[1].position = intersect;
                    PathTrack.m_Waypoints[2].position = endPosition;
                    PathTrack.InvalidateDistanceCache();

                    index = UnityRandom.Range(0, ApproachClips.Count - 1);
                    clip = ApproachClips[index];
                    AudioManager.Instance.PlayGlobalDelay(clip, 1.5f);

                    StartCoroutine(SetDevourerActive(ApproachSpeed));
                    break;
                case (TrackType.Swarm):
                    Vector3 randomSphereSwarmStart = UnityRandom.insideUnitSphere * SwarmRadius;
                    Vector3 randomSphereSwarmEnd = UnityRandom.insideUnitSphere * SwarmRadius;

                    startPosition = viewerPosition + randomSphereSwarmStart;
                    endPosition = viewerPosition - randomSphereSwarmEnd;
                    float randomVerticalOffset = UnityRandom.Range(-SwarmIntersectVerticalOffsetDistance, SwarmIntersectVerticalOffsetDistance);
                    PathTrack.m_Waypoints[0].position = startPosition;
                    PathTrack.m_Waypoints[1].position = viewerPosition + (ViewerLooker.TransformDirection(Vector3.up) * randomVerticalOffset) + ((ViewerLooker.TransformDirection(Vector3.forward) * SwarmIntersectHorizontalOffsetDistance));
                    PathTrack.m_Waypoints[2].position = endPosition;
                    PathTrack.InvalidateDistanceCache();

                    index = UnityRandom.Range(0, SwarmClips.Count - 1);
                    clip = SwarmClips[index];
                    AudioManager.Instance.PlayGlobalDelay(clip, 1.5f);

                    StartCoroutine(SetDevourerActive(SwarmSpeed));
                    break;
                case (TrackType.HorizonPass):
                    Vector3 randomSphereHPassStart = UnityRandom.insideUnitSphere * HPassIntersectRadius;
                    Vector3 randomSphereHPassEnd = UnityRandom.insideUnitSphere * HPassIntersectRadius;
                    Vector3 randomSphereHPassIntersect = UnityRandom.insideUnitSphere * HPassIntersectRadius;
                    Vector3 HaxisIntersect = ViewerTransform.position + (ViewerLooker.TransformDirection(Vector3.forward) * HPassOffsetDistance) + randomSphereHPassIntersect;
                    float randomHPass = UnityRandom.Range(HPassDistanceMin, HPassDistanceMax);

                    startPosition = HaxisIntersect + (ViewerLooker.TransformDirection(Vector3.right) * randomHPass) + randomSphereHPassStart;
                    endPosition = HaxisIntersect - (ViewerLooker.TransformDirection(Vector3.right) * randomHPass) + randomSphereHPassEnd;
                    PathTrack.m_Waypoints[0].position = startPosition;
                    PathTrack.m_Waypoints[1].position = HaxisIntersect;
                    PathTrack.m_Waypoints[2].position = endPosition;
                    PathTrack.InvalidateDistanceCache();

                    index = UnityRandom.Range(0, HPassClips.Count - 1);
                    clip = HPassClips[index];
                    AudioManager.Instance.PlayGlobalDelay(clip, 1.5f);

                    StartCoroutine(SetDevourerActive(HPassSpeed));
                    break;
                case (TrackType.VerticalPass):
                    Vector3 randomSphereVPassStart = UnityRandom.insideUnitSphere * VPassIntersectRadius;
                    Vector3 randomSphereVPassEnd = UnityRandom.insideUnitSphere * VPassIntersectRadius;
                    Vector3 randomSphereVPassIntersect = UnityRandom.insideUnitSphere * VPassIntersectRadius;
                    Vector3 VaxisIntersect = ViewerTransform.position + (ViewerLooker.TransformDirection(Vector3.forward) * VPassOffsetDistance) + randomSphereVPassIntersect;
                    float randomVPass = UnityRandom.Range(VPassDistanceMin, VPassDistanceMax);

                    startPosition = VaxisIntersect - (ViewerLooker.TransformDirection(Vector3.up) * randomVPass);
                    endPosition = VaxisIntersect + (ViewerLooker.TransformDirection(Vector3.up) * randomVPass);
                    PathTrack.m_Waypoints[0].position = startPosition;
                    PathTrack.m_Waypoints[1].position = VaxisIntersect;
                    PathTrack.m_Waypoints[2].position = endPosition;
                    PathTrack.InvalidateDistanceCache();

                    index = UnityRandom.Range(0, VPassClips.Count - 1);
                    clip = VPassClips[index];
                    AudioManager.Instance.PlayGlobalDelay(clip, 1.5f);

                    StartCoroutine(SetDevourerActive(VPassSpeed));
                    break;
            }
        }
        IEnumerator SetDevourerActive(float speed)
        {
            IsActivating = true;

            DevourerCart.m_Position = 0;
            DevourerCart.m_Speed = speed;

            yield return new WaitForSeconds(0.25f);
            FirstPersonCamera.Instance.ShakeCam(DevourerShakeIntensity, DevourerShakeDuration);

            foreach (GameObject seg in BodySegments)
            {
                seg.SetActive(true);
                seg.GetComponent<Renderer>().material.SetColor("_FresnelColor", ActiveColor);
            }
        }

        IEnumerator DeactiveDevourerProcess()
        {
            IsDeactivating = true;

            float timer = 0;
            while (timer < DeactiveDelay)
            {
                float t = timer / DeactiveDelay;

                GameObject[] Segments = GameObject.FindGameObjectsWithTag("DevourerSegment");
                foreach (GameObject seg in Segments)
                {
                    seg.GetComponent<Renderer>().material.SetColor("_FresnelColor", Color.Lerp(ActiveColor, DeactiveColor, t));
                }

                timer += Time.deltaTime;
                yield return null;
            }
            foreach (GameObject seg in BodySegments)
            {
                seg.SetActive(false);
            }

            IsActivating = false;
            IsDeactivating = false;
        }

        private void OnDrawGizmos()
        {
            if(CurrentTrack == TrackType.Approach)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(ViewerTransform.position + (ViewerLooker.TransformDirection(Vector3.forward) * ApproachOffsetDistance), ApproachStartRadius);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(ViewerTransform.position, ViewerTransform.position + ViewerLooker.TransformDirection(Vector3.forward) * ApproachOffsetDistance);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(ViewerTransform.position + ((ViewerLooker.TransformDirection(Vector3.forward) * ApproachOffsetDistance) / 2), ApproachIntersectRadius);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(ViewerTransform.position - (ViewerLooker.TransformDirection(Vector3.forward) * ApproachOffsetDistance * 1.5f), ApproachEndRadius);
            }
            else if(CurrentTrack == TrackType.Swarm)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(ViewerTransform.position, SwarmRadius);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(ViewerTransform.position + ViewerLooker.TransformDirection(Vector3.forward) * SwarmIntersectHorizontalOffsetDistance, SwarmIntersectVerticalOffsetDistance);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(ViewerTransform.position, ViewerTransform.position + ViewerLooker.TransformDirection(Vector3.forward) * SwarmIntersectHorizontalOffsetDistance);
            }
            else if(CurrentTrack == TrackType.HorizonPass)
            {
                Vector3 intersect = ViewerTransform.position + ViewerLooker.TransformDirection(Vector3.forward) * HPassOffsetDistance;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(intersect, intersect + (ViewerLooker.TransformDirection(Vector3.right) * HPassDistanceMax));
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(intersect, HPassIntersectRadius);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(intersect, intersect - (ViewerLooker.TransformDirection(Vector3.right) * HPassDistanceMax));
            }
            else if (CurrentTrack == TrackType.VerticalPass)
            {
                Vector3 intersect = ViewerTransform.position + ViewerLooker.TransformDirection(Vector3.forward) * VPassOffsetDistance;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(intersect, intersect + (ViewerLooker.TransformDirection(Vector3.up) * VPassDistanceMax));
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(intersect, VPassIntersectRadius);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(intersect, intersect - (ViewerLooker.TransformDirection(Vector3.up) * VPassDistanceMax));
            }
        }
    }
}
