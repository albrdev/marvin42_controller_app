using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.ExtensionClasses;

namespace Assets.Scripts.Components
{
    public abstract class UIFader : MonoBehaviour
    {
        protected IEnumerable<Graphic> Graphics(bool recursive)
        {
            if(gameObject.activeSelf)
            {
                Graphic graphic = GetComponent<Graphic>();
                if(graphic != null)
                {
                    yield return graphic;
                }
            }

            if(recursive)
            {
                var graphics = GetComponentsInChildren<Graphic>();
                foreach(Graphic graphic in graphics)
                {
                    yield return graphic;
                }
            }
        }

        public void SetAlpha(float value, bool recursive = false)
        {
            foreach(var graphic in Graphics(recursive))
            {
                graphic.canvasRenderer.SetAlpha(value);
            }
        }

        public void StopFade(float value, bool recursive = false)
        {
            foreach(var graphic in Graphics(recursive))
            {
                graphic.StopFade();
                graphic.canvasRenderer.SetAlpha(value);
            }
        }

        public void StopFade(bool recursive = false)
        {
            foreach(var graphic in Graphics(recursive))
            {
                graphic.StopFade();
            }
        }
    }
}
