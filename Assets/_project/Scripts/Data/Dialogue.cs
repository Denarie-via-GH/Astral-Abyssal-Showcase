using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Data/DialogueSequence")]
    public class Dialogue : ScriptableObject
    {
        [System.Serializable]
        public class Line
        {
            public string Text = string.Empty;
            public float Speed = 0;
            public float Delay = 0;
            public Color Color = new Color (1,1,1,1);
            public bool CanSkip = true;
        }

        [Header("Dialogue Property")]
        public List<Line> Lines = new List<Line>();
        public Color TextColor = Color.black;
        public float Overwrite_DialogueSpeed = 0.08f;
        public float Overwrite_AutoPlayDelay = 3.5f;
        public bool IsAutoPlay = false;
    }
}
