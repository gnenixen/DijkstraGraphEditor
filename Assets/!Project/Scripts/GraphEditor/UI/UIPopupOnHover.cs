using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPopupOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler {
    public static event Action<GameObject, PointerEventData> PointerEnterEvent = delegate { };
    public static event Action<GameObject, PointerEventData> PointerMoveEvent = delegate { };
    public static event Action<GameObject> PointerExitEvent = delegate { };

    public void OnPointerEnter(PointerEventData eventData) => PointerEnterEvent.Invoke(gameObject, eventData);
    public void OnPointerMove(PointerEventData eventData) => PointerMoveEvent.Invoke(gameObject, eventData);
    public void OnPointerExit(PointerEventData eventData) => PointerExitEvent.Invoke(gameObject);
}
