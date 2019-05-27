using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    public class PointerOverlapHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Serializable]
        public class PointerEvent : UnityEvent<PointerEventData> { }

        protected bool m_PointerEntered = false;

        [SerializeField]
        protected PointerEvent m_OnPointerEnterEvent = new PointerEvent();
        [SerializeField]
        protected PointerEvent m_OnPointerExitEvent = new PointerEvent();

        public bool PointerEntered
        {
            get { return m_PointerEntered; }
        }

        public PointerEvent OnPointerEnterEvent
        {
            get { return m_OnPointerEnterEvent; }
        }

        public PointerEvent OnPointerExitEvent
        {
            get { return m_OnPointerExitEvent; }
        }

        public virtual void OnPointerEnter(PointerEventData pEventData)
        {
            m_PointerEntered = true;
            m_OnPointerEnterEvent?.Invoke(pEventData);
        }

        public virtual void OnPointerExit(PointerEventData pEventData)
        {
            m_PointerEntered = false;
            m_OnPointerExitEvent?.Invoke(pEventData);
        }

        protected virtual void Awake()
        {
            m_PointerEntered = false;
        }
    }
}
