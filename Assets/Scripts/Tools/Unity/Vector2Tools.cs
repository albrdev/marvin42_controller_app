/// <copyright file="Vector2Tools.cs" company="Region Östergötland">
/// Released under Apache License v2.0
/// </copyright>
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-28</date>

using UnityEngine;

namespace Assets.Scripts.Tools
{
    /// <summary>
    /// Helper methods for Unity's Vector2
    /// </summary>
    public static class Vector2Tools
    {
        /// <summary>
        /// Calculates the normalized dot product
        /// </summary>
        /// <returns>The normalized dot product of two vectors</returns>
        /// <param name="a">Vector A</param>
        /// <param name="b">Vector B</param>
        public static float DotNormalized(Vector2 a, Vector2 b)
        {
            return (Mathf.Asin(Vector2.Dot(a.normalized, b.normalized)) / Mathf.PI) * 2;
        }

        /// <summary>
        /// Calculates the normalized cross product
        /// </summary>
        /// <returns>The normalized cross product of two vectors</returns>
        /// <param name="a">Vector A</param>
        /// <param name="b">Vector B</param>
        public static Vector2 CrossNormalized(Vector2 a, Vector2 b)
        {
            return Vector2.Perpendicular(a.normalized - b.normalized).normalized;
        }
    }
}
