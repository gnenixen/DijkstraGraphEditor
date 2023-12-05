using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIGraphConnection : MonoBehaviour,
    IPointerClickHandler
{
    public static event Action<UIGraphConnection, PointerEventData> OnPointerClickEvent = delegate { };

    [SerializeField]
    private UILineRenderer mLineRenderer;

    [SerializeField]
    private TextMeshProUGUI mWeightText;

    [SerializeField]
    private CanvasGroup mCanvasGroup;

    [SerializeField]
    private Color mDefaultColor;

    [SerializeField]
    private Color mPathColor;

    public UIGraphNode Node1 { private set; get; }
    public UIGraphNode Node2 { private set; get; }

    public void OnEnable() {
        mLineRenderer.mPoints = new Vector2[2];
        mLineRenderer.color = mDefaultColor;
    }

    public void Update() {
        if (Node1 && Node2) {
            var n1Position = Node1.transform.position;
            var n2Position = Node2.transform.position;

            mLineRenderer.mPoints[0] = n1Position;
            mLineRenderer.mPoints[1] = n2Position;
            mLineRenderer.SetVerticesDirty();

            var diff = n2Position - n1Position;
            var dir = diff.normalized;
            var distance = diff.magnitude;

            mWeightText.transform.position = n1Position + dir * (distance / 2.0f);
            mWeightText.text = ((int)distance).ToString();
        }
    }

    public void SetNodes(UIGraphNode mNode1, UIGraphNode mNode2) {
        this.Node1 = mNode1;
        this.Node2 = mNode2;
    }

    public bool IsConnectedToNodesId(int node1, int node2) {
        if (Node1 == null || Node2 == null)
            return false;
        
        return
            Node1.Id == node1 && Node2.Id == node2 ||
            Node1.Id == node2 && Node2.Id == node1;
    }

    public void SetIsPath(bool bValue) {
        mLineRenderer.color = bValue ? mPathColor : mDefaultColor;
    }

    public void DestroyWithAnimation() {
        DOTween.Sequence()
            .Append(mCanvasGroup.DOFade(0.0f, 0.15f))
            .AppendCallback(() => Destroy(gameObject))
        .SetAutoKill();
    }

    public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent.Invoke(this, eventData);
}