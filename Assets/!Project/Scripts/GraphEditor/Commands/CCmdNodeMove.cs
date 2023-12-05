using UnityEngine;
using CommandSystem;

public class CCmdNodeMove : AGraphVec2Command {
    private int mNodeId;
    private Vector2 mNewPosition;
    private Vector2 mOldPosition;

    public CCmdNodeMove(int mNodeId, Vector2 mNewPosition, Vector2 mOldPosition) {
        this.mNodeId = mNodeId;
        this.mNewPosition = mNewPosition;
        this.mOldPosition = mOldPosition;
    }

    public override void Execute() {
        mGraph[mNodeId] = mNewPosition;
    }

    public override void Undo() {
        mGraph[mNodeId] = mOldPosition;
    }
}