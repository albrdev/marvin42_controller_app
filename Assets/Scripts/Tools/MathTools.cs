using System;

namespace Assets.Scripts.Tools
{
    public static class MathTools
    {
        public static float Neg(float value) { return value > 0f ? -value : value; }

        public static sbyte Clamp(sbyte value, sbyte min, sbyte max) { return value < min ? min : (value > max ? max : value); }

        public static bool Approximately(float a, float b, float delta) { return Math.Abs(a - b) < delta; }
    }
}
