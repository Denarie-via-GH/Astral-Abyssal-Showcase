using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRandom = UnityEngine.Random;

namespace DeltaUtilLib
{
    public static class DeltaUtil
    {
        public static float ReturnRandomRange(float min, float max)
        {
            return UnityRandom.Range(min, max);
        }
        public static Vector2 ReturnRandomVector2(float min, float max)
        {
            return new Vector2(ReturnRandomRange(min,max), ReturnRandomRange(min,max));
        }
        public static Vector3 ReturnRandomVector3(float min, float max)
        {
            return new Vector3(ReturnRandomRange(min, max), ReturnRandomRange(min, max), ReturnRandomRange(min, max));
        }
    }

    public static class GizmoLib
    {
        public static void DrawWireCircle(Vector3 pos, Quaternion rot, float radius, int DL = 32)
        {
            Vector2[] points3D = new Vector2[DL];
            for (int i = 0; i < DL; i++)
            {
                float t = i / (float)DL;
                float angRad = t * MathLib.TAU;

                Vector2 point2D = MathLib.GetVector2ByAngle(angRad) * radius;

                points3D[i] = pos + rot * point2D;
            };

            for (int i = 0; i < DL - 1; i++)
            {
                Gizmos.DrawLine(points3D[i], points3D[i + 1]);
            }

            Gizmos.DrawLine(points3D[DL - 1], points3D[0]);
        }
    }

    public static class MathLib
    {
        public const float TAU = 6.28318530718f;

        public static Vector2 GetVector2ByAngle(float angRad)
        {
            return new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
        }

        public static float DirToAngRad(Vector2 v2)
        {
            v2 = v2.normalized;
            float n = Mathf.Atan2(v2.y, v2.x);
            return n;
        }
        public static float DirToAngRad(Vector3 v3)
        {
            v3 = v3.normalized;
            float n = Mathf.Atan2(v3.y, v3.x);
            return n;
        }

        public static float GetMinuteFromEscalatedTime(float eTime) => (int)(eTime / 60);
        public static float GetSecondFromEscalatedTime(float eTime) => (int)(eTime % 60);
        
    }

    public static class SceneLib
    {
        public static void GoToScene(string target) => SceneManager.LoadScene(target, LoadSceneMode.Single);
    }
}