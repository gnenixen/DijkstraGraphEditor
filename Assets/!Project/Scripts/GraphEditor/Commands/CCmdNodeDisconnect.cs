using UnityEngine;
using CommandSystem;

public class CCmdNodeDisconnect : AGraphVec2Command {
    private int mNodeId1 = -1;
    private int mNodeId2 = -1;
    private float mWeight = -1.0f;

    public CCmdNodeDisconnect(int mNodeId1, int mNodeId2) {
        this.mNodeId1 = mNodeId1;
        this.mNodeId2 = mNodeId2;
    }

    public override void Execute() {
        mWeight = mGraph.GetConnectionWeight(mNodeId1, mNodeId2);
        mGraph.DisconnectNodes(mNodeId1, mNodeId2);
    }

    public override void Undo() {
        mGraph.ConnectNodes(mNodeId1, mNodeId2, mWeight);
    }
}
