using UnityEngine;
using CommandSystem;

public class CCmdNodeConnect : AGraphVec2Command {
    private int mNodeId1 = -1;
    private int mNodeId2 = -1;
    private float mWeight = 0.0f;

    public CCmdNodeConnect(int mNodeId1, int mNodeId2, float mWeight) {
        this.mNodeId1 = mNodeId1;
        this.mNodeId2 = mNodeId2;
        this.mWeight = mWeight;
    }

    public override void Execute() {
        mGraph.ConnectNodes(mNodeId1, mNodeId2, mWeight);
    }

    public override void Undo() {
        mGraph.DisconnectNodes(mNodeId1, mNodeId2);
    }
}
