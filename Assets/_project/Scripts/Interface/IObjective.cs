using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstralAbyss
{
    public interface IObjective
    {
        void AddEntry();
        void CompleteObjective();
        string GetObjectiveType();
    }
}
