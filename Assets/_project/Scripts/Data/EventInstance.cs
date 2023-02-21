using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    [CreateAssetMenu(fileName = "NewEvent", menuName = "Data/EventInstance")]
    public class EventInstance : ScriptableObject
    {
        public enum EventType { Default, Landscape, Object, Lifeform, Anomaly}
        public EventType EventInstanceType = EventType.Default;
        public int ID;
        public SceneIndex EventSceneIndex;
        public float GeneratedCode;
        public float AstralParticle;
        public Vector3 MapPosition;
    }
}
