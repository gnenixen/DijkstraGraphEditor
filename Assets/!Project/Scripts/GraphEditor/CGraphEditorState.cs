using UnityEngine;
using UnityEngine.EventSystems;

public class CGraphEditorState : IState {
    public CGraphEditor Editor;

    protected CGraphEditorProcessor<Vector2> GraphEditorProcessor => Editor.mGraphEditorProcessor;

    #region Editor display info

    public virtual string StateName { get; }
    public virtual string StateInputDescription { get; }

    #endregion

    #region FSM methods

    public virtual void OnRegistered() {}
    public virtual void OnEnter() {}
    public virtual void OnExit() {}
    public virtual void Update() {}

    #endregion

    #region UIGraphField

    public virtual void UIGraphField_OnPointerClick(PointerEventData eventData) {}

    #endregion

    #region UINode

    public virtual void UINode_OnBeginDrag(UIGraphNode node, PointerEventData eventData) {}
    public virtual void UINode_OnDrag(UIGraphNode node, PointerEventData eventData) {}
    public virtual void UINode_OnEndDrag(UIGraphNode node, PointerEventData eventData) {}
    public virtual void UINode_OnPointerClick(UIGraphNode node, PointerEventData eventData) {}
    public virtual void UINode_OnDrop(UIGraphNode node, PointerEventData eventData) {}

    #endregion

    #region UIConnection

    public virtual void UIConnection_OnPointerClick(UIGraphConnection connection, PointerEventData eventData) {}

    #endregion

    #region Utils

    protected void Transition(EGraphEditorStates to) {
        Editor.mFSM.Transition(to);
    }

    #endregion
}

