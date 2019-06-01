/// <copyright file="ReadOnlyPropertyAttribute.cs" company="Region Östergötland">
/// Released under Apache License v2.0
/// </copyright>
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-27</date>

namespace UnityEngine
{
    /// <summary>
    /// Renders exposed field in Unity's inspector read-only
    /// </summary>
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
