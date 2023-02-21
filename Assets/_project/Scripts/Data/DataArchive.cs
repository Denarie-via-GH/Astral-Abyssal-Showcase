using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    [CreateAssetMenu(fileName = "NewArchive", menuName = "Data/DataArchive")]
    [System.Serializable]
    public class DataArchive : ScriptableObject
    {
        public int DataID;
        public string EntryTitle;
        public int Ref_EventID;
        public float Ref_EventCode;
        public float Ref_EventAstralParticle;
        public Vector3 Ref_EventMapPosition;
        public EventInstance.EventType Ref_EventType;

        [TextArea]
        public string Data;
    }
}
