using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPopup : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI mText;

    [SerializeField]
    private CanvasGroup mCanvasGroup;

    private void OnEnable() {
        UIPopupOnHover.PointerEnterEvent += DisplayInfo;
        UIPopupOnHover.PointerMoveEvent += Move;
        UIPopupOnHover.PointerExitEvent += CloseWindow;
    }

    private void OnDisabled() {
        UIPopupOnHover.PointerEnterEvent -= DisplayInfo;
        UIPopupOnHover.PointerMoveEvent -= Move;
        UIPopupOnHover.PointerExitEvent -= CloseWindow;
    }

    private void DisplayInfo(GameObject go, PointerEventData eventData) {
        UpdatePositionAndText(go, eventData);

        mCanvasGroup.DOFade(1.0f, 0.15f);
    }

    private void Move(GameObject go, PointerEventData eventData) {
        UpdatePositionAndText(go, eventData);
    }

    private void CloseWindow(GameObject go) {
        mCanvasGroup.DOFade(0, 0.15f);
    }

    private void UpdatePositionAndText(GameObject go, PointerEventData eventData) {
        transform.position = eventData.position + new Vector2(150.0f, 0.0f);

        mText.text = new Vector2(go.transform.position.x, go.transform.position.y).ToString();
    }
}
