using UnityEngine;
using Assets.Scripts.ExtensionClasses;

namespace Assets.Scripts.Components
{
    [AddComponentMenu("GameObject/UI/UIAlphaFader")]
    public class UIAlphaFader : UIFader
    {
        public void Fade(float value, float time, float delay = 0f, bool recursive = false, bool realtime = true)
        {
            foreach(var graphic in Graphics(recursive))
            {
                graphic.FadeAlpha(value, time, delay, realtime);
            }
        }
    }
}
