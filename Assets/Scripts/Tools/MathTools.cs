/// <copyright file="MathTools.cs" company="Region Östergötland">
/// Released under Apache License v2.0
/// </copyright>
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-28</date>

using System;

namespace Assets.Scripts.Tools
{
    /// <summary>
    /// Generic math functions
    /// </summary>
    public static class MathTools
    {
        /// <summary>
        /// Negates a value
        /// </summary>
        /// <returns>The negative of 'value' if positive, returns 'value' otherwise</returns>
        /// <param name="value">Value to be negated</param>
        public static float Neg(float value) { return value > 0f ? -value : value; }

        /// <summary>
        /// Ensures a value stays between a minimum and maximum value
        /// </summary>
        /// <returns>'min' if 'value' is less than 'min', 'max if 'value' is greter than 'max', returns 'value' otherwise</returns>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum value to clamp to</param>
        /// <param name="max">Maximum value to clamp to</param>
        public static sbyte Clamp(sbyte value, sbyte min, sbyte max) { return value < min ? min : (value > max ? max : value); }

        /// <summary>
        /// Check if 'a' and 'b' is in proximity of each other
        /// </summary>
        /// <returns>'true' if 'a' and 'b' is within 'delta' of each other, 'false' otherwise</returns>
        /// <param name="a">Value to be compared</param>
        /// <param name="b">Other value to be compared</param>
        /// <param name="delta">The maximum difference the values expect to be in proximity with</param>
        public static bool Approximately(float a, float b, float delta) { return Math.Abs(a - b) < delta; }
    }
}
