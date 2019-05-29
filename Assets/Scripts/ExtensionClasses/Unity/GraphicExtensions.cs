using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ExtensionClasses
{
    public static class GraphicExtensions
    {
        public static void FadeAlpha(this Graphic self, float value, float time, bool realtime)
        {
            if(self == null || self.canvasRenderer.GetAlpha() == value)
                return;

            self.CrossFadeAlpha(value, time, realtime);
        }

        public static void FadeAlpha(this Graphic self, float value, float time, float delay, bool realtime)
        {
            if(self.canvasRenderer.GetAlpha() == value)
                return;

            if(delay > 0f)
            {
                Func<float, Action, IEnumerator> method = realtime ? (Func<float, Action, IEnumerator>)DelayRealtime : (Func<float, Action, IEnumerator>)Delay;
                self.StartCoroutine(method(delay, () => FadeAlpha(self, value, time, realtime)));
            }
            else
            {
                FadeAlpha(self, value, time, realtime);
            }
        }

        public static void StopFade(this Graphic self)
        {
            self.StopAllCoroutines(); // FIXME: Maybe a better way of stopping coroutines
            self.CrossFadeColor(self.canvasRenderer.GetColor(), 0f, true, true);
            self.CrossFadeAlpha(self.canvasRenderer.GetAlpha(), 0f, true);
        }

        private static IEnumerator Delay(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }

        private static IEnumerator DelayRealtime(float delay, Action callback)
        {
            yield return new WaitForSecondsRealtime(delay);
            callback();
        }
    }
}
