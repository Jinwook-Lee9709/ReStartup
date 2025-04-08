using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoubleScrollRect : ScrollRect
{
    bool routeToParent = false;

    void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            foreach (var component in parent.GetComponents<Component>())
            {
                if (component is T)
                {
                    action((T)(IEventSystemHandler)component);
                }
            }
            parent = parent.parent;
        }
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
        base.OnInitializePotentialDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (routeToParent) DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
        else base.OnDrag(eventData);
    }


    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y)) routeToParent = true;
        else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y)) routeToParent = true;
        else routeToParent = false;

        if (routeToParent) DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
        else base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent) DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
        else base.OnEndDrag(eventData);
    }
}
