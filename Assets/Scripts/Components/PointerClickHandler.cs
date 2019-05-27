using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    public class PointerClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Serializable]
        public class PointerEvent : UnityEvent<PointerEventData> { }

        protected bool m_PointerDown = false;

        [SerializeField]
        protected PointerEvent m_OnPointerDownEvent = new PointerEvent();
        [SerializeField]
        protected PointerEvent m_OnPointerUpEvent = new PointerEvent();
        [SerializeField]
        protected PointerEvent m_OnPointerClickEvent = new PointerEvent();
        [SerializeField]
        protected PointerEvent m_OnPointerDoubleClickEvent = new PointerEvent();

        public bool PointerDown
        {
            get { return m_PointerDown; }
        }

        public PointerEvent OnPointerDownEvent
        {
            get { return m_OnPointerDownEvent; }
        }

        public PointerEvent OnPointerUpEvent
        {
            get { return m_OnPointerUpEvent; }
        }

        public PointerEvent OnPointerClickEvent
        {
            get { return m_OnPointerClickEvent; }
        }

        public PointerEvent OnPointerDoubleClickEvent
        {
            get { return m_OnPointerDoubleClickEvent; }
        }

        public virtual void OnPointerDown(PointerEventData pEventData)
        {
            m_PointerDown = true;
            m_OnPointerDownEvent?.Invoke(pEventData);
        }

        public virtual void OnPointerUp(PointerEventData pEventData)
        {
            m_PointerDown = false;
            m_OnPointerUpEvent?.Invoke(pEventData);
        }

        public virtual void OnPointerClick(PointerEventData pEventData)
        {
            m_OnPointerClickEvent?.Invoke(pEventData);
            if(pEventData.clickCount >= 2) {
                m_OnPointerDoubleClickEvent?.Invoke(pEventData);
            }
        }

        protected virtual void Awake()
        {
            m_PointerDown = false;
        }
    }
}
