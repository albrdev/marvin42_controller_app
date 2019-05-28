using UnityEngine;

namespace Assets.Scripts.Tools
{
    public static class Vector2Tools
    {
        public static float DotNormalized(Vector2 a, Vector2 b)
        {
            return (Mathf.Asin(Vector2.Dot(a.normalized, b.normalized)) / Mathf.PI) * 2;
        }

        public static Vector2 CrossNormalized(Vector2 a, Vector2 b)
        {
            return Vector2.Perpendicular(a.normalized - b.normalized).normalized;
        }
    }
}
