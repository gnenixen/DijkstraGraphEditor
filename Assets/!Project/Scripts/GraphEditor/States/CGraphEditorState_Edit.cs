using UnityEngine;
using UnityEngine.EventSystems;

public class CGraphEditorState_Edit : CGraphEditorState {
    private UIGraphConnectionCreationRenderer mConnectionCreationRenderer;

    public override string StateName { get { return "Edit"; } }

    public override string StateInputDescription { get{ return @"<b>Edit mode:</b>
● <indent=10%>LMB - create node/
move node</indent>
● <indent=10%>Shift + LMB - connect nodes</indent>
● <indent=10%>RMB - remove node/
connection</indent>
● <indent=10%>Z - undo</indent>
● <indent=10%>R - redo</indent>"; } }

    public override void OnRegistered() {
        mConnectionCreationRenderer = GameObject.Instantiate(Editor.GraphConnectionCreationRenderer, Editor.ConnectionsParent);
        mConnectionCreationRenderer.gameObject.SetActive(false);
    }

    public override void OnExit() {
        mConnectionCreationRenderer.gameObject.SetActive(false);
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            GraphEditorProcessor.Undo();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            GraphEditorProcessor.Redo();
        }
    }

    public override void UIGraphField_OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            Editor.CmdNode_Create(eventData.position);
        }
    }

    public override void UINode_OnBeginDrag(UIGraphNode node, PointerEventData eventData) {
        if (Input.GetKey(KeyCode.LeftShift)) {
            mConnectionCreationRenderer.gameObject.SetActive(true);
            mConnectionCreationRenderer.InitialNode = node;
            eventData.pointerDrag = mConnectionCreationRenderer.gameObject;
            mConnectionCreationRenderer.OnBeginDrag(eventData);
            return;
        }

        node.transform.SetAsFirstSibling();

        node.mOffset = eventData.position - new Vector2(node.transform.position.x, node.transform.position.y);

        node.mImage.raycastTarget = false;
    }

    public override void UINode_OnDrag(UIGraphNode node, PointerEventData eventData) {
        node.transform.position = eventData.position - node.mOffset;
    }

    public override void UINode_OnEndDrag(UIGraphNode node, PointerEventData eventData) {
        node.mImage.raycastTarget = true;

        Editor.CmdNode_Move(node.Id, node.transform.position);
    }

    public override void UINode_OnPointerClick(UIGraphNode node, PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            Editor.CmdNode_Remove(node.Id);
        }
    }

    public override void UINode_OnDrop(UIGraphNode node, PointerEventData eventData) {
        var go = eventData.pointerDrag;
        var connection = go.GetComponent<UIGraphConnectionCreationRenderer>();
        if (connection) {
            Editor.CmdNode_Connect(connection.InitialNode.Id, node.Id);
            mConnectionCreationRenderer.gameObject.SetActive(false);
        }
    }

    public override void UIConnection_OnPointerClick(UIGraphConnection connection, PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            Editor.CmdNode_Disconnect(connection.Node1.Id, connection.Node2.Id);
        }
    }
}