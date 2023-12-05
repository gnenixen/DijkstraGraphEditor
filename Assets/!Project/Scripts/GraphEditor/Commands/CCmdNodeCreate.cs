using UnityEngine;
using CommandSystem;

public class CCmdNodeCreate : AGraphVec2Command {
    private Vector2 mPosition;
    private int mNodeId = -1;

    public CCmdNodeCreate(Vector2 mPosition) {
        this.mPosition = mPosition;
    }

    public override void Execute() {
        mNodeId = mGraph.AddNewNode(mPosition);
    }

    public override void Undo() {
        mGraph.RemoveNode(mNodeId);
    }
}