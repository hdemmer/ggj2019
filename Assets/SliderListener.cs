using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderListener : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public System.Action onPointerUp;
    public System.Action onPointerDown;
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null)
        {
            onPointerUp();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null)
        {
            onPointerDown();
        }
    }
}
