/// <copyright file="GraphicExtensions.cs" company="Region Östergötland">
/// Released under Apache License v2.0
/// </copyright>
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-30</date>

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ExtensionClasses
{
    /// <summary>
    /// Contains extensions/helper methods for Unity's Graphics component
    /// </summary>
    public static class GraphicExtensions
    {
        /// <summary>
        /// Gradually fades graphics alpha channel from current value to 'value' under a specific time
        /// </summary>
        /// <param name="value">Target alpha</param>
        /// <param name="duration">Fade duration</param>
        /// <param name="realtime">Fade in realtime or by Unity's timescale</param>
        public static void FadeAlpha(this Graphic self, float value, float duration, bool realtime)
        {
            if(self == null || self.canvasRenderer.GetAlpha() == value)
                return;

            self.CrossFadeAlpha(value, duration, realtime);
        }

        /// <summary>
        /// Gradually fades graphics alpha channel from current value to 'value' under a specific time. Start after a specific delay
        /// </summary>
        /// <param name="value">Target alpha</param>
        /// <param name="duration">Fade duration</param>
        /// <param name="delay">Delay before fade starts</param>
        /// <param name="realtime">Fade in realtime or by Unity's timescale</param>
        public static void FadeAlpha(this Graphic self, float value, float duration, float delay, bool realtime)
        {
            if(self.canvasRenderer.GetAlpha() == value)
                return;

            if(delay > 0f)
            {
                Func<float, Action, IEnumerator> method = realtime ? (Func<float, Action, IEnumerator>)DelayRealtime : (Func<float, Action, IEnumerator>)Delay;
                self.StartCoroutine(method(delay, () => FadeAlpha(self, value, duration, realtime)));
            }
            else
            {
                FadeAlpha(self, value, duration, realtime);
            }
        }

        /// <summary>
        /// Stops current fading in Graphics Component immediately, also sets final alpha
        /// </summary>
        public static void StopFade(this Graphic self)
        {
            self.StopAllCoroutines(); // FIXME: Maybe a better way of stopping coroutines
            self.CrossFadeColor(self.canvasRenderer.GetColor(), 0f, true, true);
            self.CrossFadeAlpha(self.canvasRenderer.GetAlpha(), 0f, true);
        }

        /// <summary>
        /// IEnumerator method used to produce a delay (affected by Unity's timescale)
        /// </summary>
        private static IEnumerator Delay(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }

        /// <summary>
        /// IEnumerator method used to produce a delay (realtime)
        /// </summary>
        private static IEnumerator DelayRealtime(float delay, Action callback)
        {
            yield return new WaitForSecondsRealtime(delay);
            callback();
        }
    }
}
