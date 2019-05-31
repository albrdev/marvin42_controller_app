/// <summary>
/// Renders exposed field in Unity's inspector read-only
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-27</date>
/// </summary>

namespace UnityEngine
{
    public class ReadOnlyPropertyAttribute : PropertyAttribute
    {
        public string DisplayName { get; protected set; } = null;

        public ReadOnlyPropertyAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public ReadOnlyPropertyAttribute() { }
    }
}
