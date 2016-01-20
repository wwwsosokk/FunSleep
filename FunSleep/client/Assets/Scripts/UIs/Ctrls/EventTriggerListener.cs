using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class EventTriggerListener
    : MonoBehaviour, IBeginDragHandler, IDragHandler,
    IEndDragHandler, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler,
    IDropHandler
{
    public bool ignoreDragginClick = false;

    public bool ignoreLongTimePress = false;
    private bool isLongTimePress = false;
    private float longTimePressStartTime = 0f;
    private const float longTimePressTime = 0.25f;

    public PointerEvent onBeginDrag = new PointerEvent();
    public PointerEvent onDrag = new PointerEvent();
    public PointerEvent onEndDrag = new PointerEvent();
    public PointerEvent onPointerDown = new PointerEvent();
    public PointerEvent onPointerClick = new PointerEvent();
    public PointerEvent onPointerUp = new PointerEvent();
    public PointerEvent onPointerEnter = new PointerEvent();
    public PointerEvent onPointerExit = new PointerEvent();
    public PointerEvent onDrop = new PointerEvent();

    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (null != onBeginDrag)
            onBeginDrag.Invoke(eventData);
    }
    #endregion

    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData)
    {
        if (null != onDrag)
            onDrag.Invoke(eventData);
    }

    #endregion

    #region IEndDragHandler implementation

    public void OnEndDrag(PointerEventData eventData)
    {
        if (null != onEndDrag)
            onEndDrag.Invoke(eventData);
    }

    #endregion

    #region IPointerDownHandler implementation

    public void OnPointerDown(PointerEventData eventData)
    {
        if (null != onPointerDown)
            onPointerDown.Invoke(eventData);

        isLongTimePress = false;
        longTimePressStartTime = Time.time;
    }

    #endregion

    #region IPointerClickHandler implementation

    public void OnPointerClick(PointerEventData eventData)
    {
        if (null != onPointerClick
            && (! ignoreDragginClick || ! eventData.dragging)
            && (! ignoreLongTimePress || ! isLongTimePress))
            onPointerClick.Invoke(eventData);
    }

    #endregion

    #region IPointerUpHandler implementation

    public void OnPointerUp(PointerEventData eventData)
    {
        if (null != onPointerUp)
            onPointerUp.Invoke(eventData);

        isLongTimePress = Time.time - longTimePressStartTime > longTimePressTime;
    }

    #endregion

    #region IPointerEnterHandler implementation

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (null != onPointerEnter)
            onPointerEnter.Invoke(eventData);
    }

    #endregion

    #region IPointerExitHandler implementation

    public void OnPointerExit(PointerEventData eventData)
    {
        if (null != onPointerExit)
            onPointerExit.Invoke(eventData);
    }

    #endregion

    public void OnDrop(PointerEventData eventData)
    {
        onDrop.Invoke(eventData);
    }

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public class PointerEvent : UnityEvent<PointerEventData>
    {
    }
}
