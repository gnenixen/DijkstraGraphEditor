using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CGraphEditorState_Pathfinding : CGraphEditorState {
    private CDijkstra mDijkstra;
    private UIGraphNode mFromNode = null;
    private UIGraphNode mToNode = null;

    public override string StateName { get { return "Pathfinding"; } }
    public override string StateInputDescription { get{ return @"<b>Pathfinding mode:</b>
● <indent=10%>LMB - select TO node</indent>
● <indent=10%>RMB - select FROM node</indent>"; } }

    public override void OnRegistered() {
        mDijkstra = new CDijkstra();
    }

    public override void OnExit() {
        ResetPathDraw();

        if (mToNode) {
            mToNode.MarkAsDefault();
            mToNode = null;
        }

        if (mFromNode) {
            mFromNode.MarkAsDefault();
            mFromNode = null;
        }
    }

    public override void UINode_OnPointerClick(UIGraphNode node, PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            UpdateFromToNodes(node, ref mToNode, ref mFromNode);
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            UpdateFromToNodes(node, ref mFromNode, ref mToNode);
        }

        if (mFromNode != null && mToNode != null) {
            var path = mDijkstra.Search(Editor.mGraph, mFromNode.Id, mToNode.Id);

            ResetPathDraw();

            if (path != null) {
                int last = path[0];
                for (int i = 1; i < path.Count; i++) {
                    GetUIConnectionBetweenNodes(last, path[i]).SetIsPath(true);
                    last = path[i];
                }
            }
        }
    }

    private void UpdateFromToNodes(UIGraphNode newNode, ref UIGraphNode nodeToUpdate, ref UIGraphNode otherNode) {
        if (newNode == nodeToUpdate)
            return;
        
        if (newNode == otherNode)
            return;

        if (nodeToUpdate != null) {
            nodeToUpdate.MarkAsDefault();
        }

        nodeToUpdate = newNode;

        if (nodeToUpdate == mFromNode) {
            nodeToUpdate.MarkAsFrom();
        } else {
            nodeToUpdate.MarkAsTo();
        }
    }

    private void ResetPathDraw() {
        foreach (var connection in Editor.mUIConnections) {
            connection.SetIsPath(false);
        }
    }

    private UIGraphConnection GetUIConnectionBetweenNodes(int node1, int node2) {
        foreach (var connection in Editor.mUIConnections) {
            if (connection.IsConnectedToNodesId(node1, node2)) {
                return connection;
            }
        }

        return null;
    }

}
