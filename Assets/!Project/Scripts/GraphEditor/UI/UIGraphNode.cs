using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIGraphNode : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IDragHandler,
    IPointerClickHandler,
    IDropHandler
{
    public static event Action<UIGraphNode, PointerEventData> OnBeginDragEvent = delegate { };
    public static event Action<UIGraphNode, PointerEventData> OnDragEvent = delegate { };
    public static event Action<UIGraphNode, PointerEventData> OnEndDragEvent = delegate { };
    public static event Action<UIGraphNode, PointerEventData> OnPointerClickEvent = delegate { };
    public static event Action<UIGraphNode, PointerEventData> OnDropEvent = delegate { };

    [SerializeField]
    public Image mImage;

    [SerializeField]
    private TextMeshProUGUI mText;

    [SerializeField]
    private Color mDefaulColor;

    [SerializeField]
    private Color mPathfindindFromColor;

    [SerializeField]
    private Color mPathfindindToColor;

    public Vector2 mOffset;
    private int mId = -1;

    public int Id {
        set {
            if (mId == -1)
                mId = value;
                mText.text = mId.ToString();
        }

        get { return mId; }
    }

    public void OnEnable() {
        transform.localScale = new Vector3(0.0f, 1.0f);
        transform.DOScaleX(1.0f, 0.15f);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        transform.DOScale(1.1f, 0.15f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        transform.DOScale(1.0f, 0.15f);
    }

    public void OnBeginDrag(PointerEventData eventData) => OnBeginDragEvent.Invoke(this, eventData);
    public void OnDrag(PointerEventData eventData) => OnDragEvent.Invoke(this, eventData);
    public void OnEndDrag(PointerEventData eventData) => OnEndDragEvent.Invoke(this, eventData);
    public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent.Invoke(this, eventData);
    public void OnDrop(PointerEventData eventData) => OnDropEvent.Invoke(this, eventData);

    public void DestroyWithAnimation() {
        DOTween.Sequence()
            .Append(transform.DOScaleX(0.0f, 0.15f))
            .AppendCallback(() => Destroy(gameObject))
        .SetAutoKill();
    }

    public void MarkAsDefault() => mImage.color = mDefaulColor;
    public void MarkAsFrom() => mImage.color = mPathfindindFromColor;
    public void MarkAsTo() => mImage.color = mPathfindindToColor;
}
