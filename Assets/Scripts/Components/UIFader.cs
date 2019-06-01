/// <copyright file="UIFader.cs" company="Region Östergötland">
/// Released under Apache License v2.0
/// </copyright>
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-30</date>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.ExtensionClasses;

namespace Assets.Scripts.Components
{
    /// <summary>
    /// Base class for fading UI components (containing Graphics component) over time
    /// </summary>
    public abstract class UIFader : MonoBehaviour
    {
        /// <summary>
        /// Gets all components derrived from Unity's 'Graphics' class
        /// </summary>
        /// <returns>An enumerator of 'Graphics'</returns>
        /// <param name="recursive">Also returns Graphics components in children</param>
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

        /// <summary>
        /// Sets alpha of graphic(s) immediately
        /// </summary>
        /// <param name="value">Alpha value</param>
        /// <param name="recursive">Also sets alpha in children</param>
        public void SetAlpha(float value, bool recursive = false)
        {
            foreach(var graphic in Graphics(recursive))
            {
                graphic.canvasRenderer.SetAlpha(value);
            }
        }

        /// <summary>
        /// Stops current fading in graphic(s) immediately, also sets final alpha
        /// </summary>
        /// <param name="value">Alpha value to reset to</param>
        /// <param name="recursive">Also stops fading in children</param>
        public void StopFade(float value, bool recursive = false)
        {
            foreach(var graphic in Graphics(recursive))
            {
                graphic.StopFade();
                graphic.canvasRenderer.SetAlpha(value);
            }
        }

        /// <summary>
        /// Stops current fading in graphic(s) immediately
        /// </summary>
        /// <param name="recursive">Also stops fading in children</param>
        public void StopFade(bool recursive = false)
        {
            foreach(var graphic in Graphics(recursive))
            {
                graphic.StopFade();
            }
        }
    }
}
