/// <copyright file="UIAlphaFader.cs" company="Region Östergötland">
/// Released under Apache License v2.0
/// </copyright>
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-30</date>

using UnityEngine;
using Assets.Scripts.ExtensionClasses;

namespace Assets.Scripts.Components
{
    /// <summary>
    /// Fades alpha in UI components (containing Graphics component) over time
    /// </summary>
    [AddComponentMenu("GameObject/UI/UIAlphaFader")]
    public class UIAlphaFader : UIFader
    {
        /// <summary>
        /// Gradually fades graphics component's alpha channel from current value to 'value' under a specific time
        /// </summary>
        /// <param name="value">Target alpha</param>
        /// <param name="duration">Fade duration</param>
        /// <param name="delay">Delay before fade starts</param>
        /// <param name="recursive">Also fades Graphics components in children</param>
        /// <param name="realtime">Fade in realtime or by Unity's timescale</param>
        public void Fade(float value, float duration, float delay = 0f, bool recursive = false, bool realtime = true)
        {
            foreach(var graphic in Graphics(recursive))
            {
                graphic.FadeAlpha(value, duration, delay, realtime);
            }
        }
    }
}
