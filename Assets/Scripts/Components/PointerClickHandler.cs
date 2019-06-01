/// <copyright file="PointerClickHandler.cs" company="Region Östergötland">
/// Released under Apache License v2.0
/// </copyright>
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-27</date>

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    /// <summary>
    /// Attach to UI components to capture pointer clicks
    /// Can be accessed from other components outside
    /// </summary>
    public class PointerClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Serializable]
        public class PointerEvent : UnityEvent<PointerEventData> { }

        protected bool m_PointerDown = false;

        [SerializeField, Tooltip("Pointer down event handler")]
        protected PointerEvent m_OnPointerDownEvent = new PointerEvent();
        [SerializeField, Tooltip("Pointer up event handler")]
        protected PointerEvent m_OnPointerUpEvent = new PointerEvent();
        [SerializeField, Tooltip("Pointer click event handler")]
        protected PointerEvent m_OnPointerClickEvent = new PointerEvent();
        [SerializeField, Tooltip("Pointer double-click event handler")]
        protected PointerEvent m_OnPointerDoubleClickEvent = new PointerEvent();

        /// <summary>
        /// Check if pointer is pressed or not, over this UI element
        /// </summary>
        /// <returns>'true' if pointer is down, 'false' otherwise</returns>
        public bool PointerDown
        {
            get { return m_PointerDown; }
        }

        /// <summary>
        /// Register a method to be called automatically when pointer is pressed over this UI element
        /// </summary>
        public PointerEvent OnPointerDownEvent
        {
            get { return m_OnPointerDownEvent; }
        }

        /// <summary>
        /// Register a method to be called automatically when pointer is released over this UI element
        /// </summary>
        public PointerEvent OnPointerUpEvent
        {
            get { return m_OnPointerUpEvent; }
        }

        /// <summary>
        /// Register a method to be called automatically when pointer is clicked (pointer press + release) over this UI element
        /// </summary>
        public PointerEvent OnPointerClickEvent
        {
            get { return m_OnPointerClickEvent; }
        }

        /// <summary>
        /// Register a method to be called automatically when pointer is double-clicked over this UI element
        /// </summary>
        public PointerEvent OnPointerDoubleClickEvent
        {
            get { return m_OnPointerDoubleClickEvent; }
        }

        /// <summary>
        /// Used only internally to capture and handle Unity's pointer down events
        /// </summary>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            m_PointerDown = true;
            m_OnPointerDownEvent?.Invoke(eventData);
        }

        /// <summary>
        /// Used only internally to capture and handle Unity's pointer up events
        /// </summary>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            m_PointerDown = false;
            m_OnPointerUpEvent?.Invoke(eventData);
        }

        /// <summary>
        /// Used only internally to capture and handle Unity's pointer click events
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            m_OnPointerClickEvent?.Invoke(eventData);

            // Multiple clicks triggers double-click event
            if(eventData.clickCount >= 2)
            {
                m_OnPointerDoubleClickEvent?.Invoke(eventData);
            }
        }

        /// <summary>
        /// Initialize class
        /// </summary>
        protected virtual void Awake()
        {
            m_PointerDown = false;
        }
    }
}
