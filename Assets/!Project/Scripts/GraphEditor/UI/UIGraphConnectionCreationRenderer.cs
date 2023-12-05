using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIGraphConnectionCreationRenderer : MonoBehaviour,
    IBeginDragHandler,
    IEndDragHandler,
    IDragHandler
{
    [SerializeField]
    private UILineRenderer mLineRenderer;

    [SerializeField]
    private CanvasGroup mCanvasGroup;

    public UIGraphNode InitialNode;
    private Vector2 mInitialOffset;

    public void OnEnable() {
        mLineRenderer.mPoints = new Vector2[2];
        mCanvasGroup.DOFade(1.0f, 0.15f);
    }

    public void OnDisabled() {
        InitialNode = null;
        mCanvasGroup.DOFade(0.0f, 0.15f);
    }

    public void Update() {
        if (InitialNode) {
            mLineRenderer.mPoints[0] = InitialNode.transform.position;
            mLineRenderer.SetVerticesDirty();
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        transform.SetAsFirstSibling();

        mInitialOffset = eventData.position - new Vector2(InitialNode.transform.position.x, InitialNode.transform.position.y);
    }

    public void OnDrag(PointerEventData eventData) {
        mLineRenderer.mPoints[1] = eventData.position - mInitialOffset;
    }

    public void OnEndDrag(PointerEventData eventData) {}
}
