using UnityEngine;
using CommandSystem;
using System.Collections.Generic;

public class CCmdNodeRemove : AGraphVec2Command {
    private int mNodeId;
    private Vector2 mPosition;
    private List<(int, float)> mConnections;

    public CCmdNodeRemove(int mNodeId, Vector2 mPosition) {
        this.mNodeId = mNodeId;
        this.mPosition = mPosition;
    }

    public override void Execute() {
        mConnections = mGraph.GetAllConnectionsWithNode(mNodeId);
        mGraph.RemoveNode(mNodeId);
    }

    public override void Undo() {
        mNodeId = mGraph.AddNewNode(mPosition);
        foreach ((int node, float weight) in mConnections) {
            mGraph.ConnectNodes(mNodeId, node, weight);
        }
    }
}