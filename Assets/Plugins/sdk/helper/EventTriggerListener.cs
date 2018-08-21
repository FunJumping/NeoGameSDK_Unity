using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public delegate void VectorDelegate(GameObject go, Vector2 delta);
    public VoidDelegate onClick;
    public VectorDelegate onPointClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VectorDelegate onMove;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onDragIn;
    public VectorDelegate onDrag;
    public VoidDelegate onDragOut;
    public VectorDelegate onDragEnd;
    public VectorDelegate onInPoDrag;


    static public EventTriggerListener Get(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("EventTriggerListener_go_is_NULL");
            return null;
        }
        else
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            return listener;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onDragIn != null) onDragIn(gameObject);
    }

    //public override void OnScroll(PointerEventData eventData)
    //{
    //    base.OnScroll(eventData);
    //}

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (onInPoDrag != null) onInPoDrag(gameObject, eventData.position);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(gameObject, eventData.delta);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onDragOut != null) onDragOut(gameObject);
        if (onDragEnd != null) onDragEnd(gameObject, eventData.position);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(gameObject);
        if (onPointClick != null) onPointClick(gameObject, eventData.position);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (onMove != null) onMove(gameObject, eventData.moveVector);
    }

    public void clearAllListener()
    {
        onClick = null;
        onDown = null;
        onEnter = null;
        onExit = null;
        onUp = null;
        onSelect = null;
        onUpdateSelect = null;
        onDrag = null;
        onDragOut = null;
        onDragIn = null;
        onMove = null;
        onInPoDrag = null;
        onDragEnd = null;
        Destroy(gameObject.GetComponent<EventTriggerListener>());
    }
}