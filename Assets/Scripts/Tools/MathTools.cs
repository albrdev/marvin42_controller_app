using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Assets.Scripts.Tools
{
    public static class MathTools
    {
        public static float Neg(float value) { return value > 0f ? -value : value; }

        public static sbyte Clamp(sbyte value, sbyte min, sbyte max) { return value < min ? min : (value > max ? max : value); }
    }
}
