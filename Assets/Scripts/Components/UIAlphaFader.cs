/// <summary>
/// Handles alpha fading of UI components
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-30</date>
/// </summary>

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
