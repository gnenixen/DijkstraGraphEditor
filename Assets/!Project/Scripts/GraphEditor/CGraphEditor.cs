using System.Collections.Generic;
using CommandSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class CGraphEditor : MonoBehaviour, IPointerClickHandler {
    #region Inspector variables

    [SerializeField]
    private UIGraphNode mGraphNodePrefab;

    [SerializeField]
    private UIGraphConnection mGraphConnectionPrefab;

    [SerializeField]
    private UIGraphConnectionCreationRenderer mGraphConnectionCreationRendererPrefab;

    [SerializeField]
    private Transform mNodesParent;

    [SerializeField]
    private Transform mConnectionsParent;

    [SerializeField]
    private TextMeshProUGUI mModeText;

    [SerializeField]
    private TextMeshProUGUI mModeInputDescriptionText;

    #endregion

    #region Internal variables

    public CGraph<Vector2> mGraph;
    public CGraphEditorProcessor<Vector2> mGraphEditorProcessor { get; private set; }
    public CFSM<CGraphEditorState, EGraphEditorStates> mFSM { get; private set;}

    public CSlotsList<UIGraphNode> mUINodes;
    public List<UIGraphConnection> mUIConnections;

    #endregion

    #region Properties

    public UIGraphConnectionCreationRenderer GraphConnectionCreationRenderer { get { return mGraphConnectionCreationRendererPrefab; } }
    public Transform NodesParent { get { return mNodesParent; } }
    public Transform ConnectionsParent { get { return mConnectionsParent; } }

    #endregion

    public void Start() {
        // Setup graph related things
        mGraph = new CGraph<Vector2>();
        mGraphEditorProcessor = new CGraphEditorProcessor<Vector2>(mGraph);
        mFSM = new CFSM<CGraphEditorState, EGraphEditorStates>();

        mFSM.OnTransitionEvent += (state) => {
            mModeText.text = state.StateName;
            mModeInputDescriptionText.text = state.StateInputDescription;
        };

        UIGraphNode.OnBeginDragEvent += (node, eventData) => { mFSM.CurrentState.UINode_OnBeginDrag(node, eventData); };
        UIGraphNode.OnDragEvent += (node, eventData) => { mFSM.CurrentState.UINode_OnDrag(node, eventData); };
        UIGraphNode.OnEndDragEvent += (node, eventData) => { mFSM.CurrentState.UINode_OnEndDrag(node, eventData); };
        UIGraphNode.OnPointerClickEvent += (node, eventData) => { mFSM.CurrentState.UINode_OnPointerClick(node, eventData); };
        UIGraphNode.OnDropEvent += (node, eventData) => { mFSM.CurrentState.UINode_OnDrop(node, eventData); };

        mUINodes = new CSlotsList<UIGraphNode>();
        mUIConnections = new List<UIGraphConnection>();

        mGraph.OnNodeAdded += Graph_OnNodeCreated;
        mGraph.OnNodeRemove += Graph_OnNodeRemoved;
        mGraph.OnNodeConnected += Graph_OnNodeConnected;
        mGraph.OnNodeDisconnected += Graph_OnNodeDisconnected;
        mGraph.OnNodeValueUpdated += Graph_OnNodeValueUpdated;

        // Setup FSM states
        mFSM.RegisterState(EGraphEditorStates.EDIT, new CGraphEditorState_Edit() { Editor = this });
        mFSM.RegisterState(EGraphEditorStates.PATHFINDING, new CGraphEditorState_Pathfinding() { Editor = this });

        mFSM.Transition(EGraphEditorStates.PATHFINDING);

        // Spawn default initial graph
        {
            int v = 150;

            CmdNode_Create(new Vector2(v+ 100, 500));
            CmdNode_Create(new Vector2(v+ 300, 300));
            CmdNode_Create(new Vector2(v+ 446, 437));
            CmdNode_Create(new Vector2(v+ 471, 272));
            CmdNode_Create(new Vector2(v+ 641, 254));
            CmdNode_Create(new Vector2(v+ 480, 105));
            CmdNode_Connect(0, 1);
            CmdNode_Connect(1, 2);
            CmdNode_Connect(1, 3);
            CmdNode_Connect(1, 5);
            CmdNode_Connect(2, 3);
            CmdNode_Connect(2, 4);
            CmdNode_Connect(3, 4);
            CmdNode_Connect(3, 5);
            CmdNode_Connect(4, 5);
            CmdNode_Connect(0, 5);
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (mFSM.CurrentState is CGraphEditorState_Edit) {
                mFSM.Transition(EGraphEditorStates.PATHFINDING);
            } else {
                mFSM.Transition(EGraphEditorStates.EDIT);
            }
        } else {
            mFSM.CurrentState.Update();
        }
    }

    #region Commands

    public void PushCommand(AGraphVec2Command command) {
        mGraphEditorProcessor.PushCommand(command);
    }

    public void CmdNode_Create(Vector2 position) {
        PushCommand(new CCmdNodeCreate(position));
    }

    public void CmdNode_Remove(int id) {
        PushCommand(new CCmdNodeRemove(id, mGraph[id]));
    }

    public void CmdNode_Move(int id, Vector2 newPosition) {
        PushCommand(new CCmdNodeMove(id, newPosition, mGraph[id]));
    }

    public void CmdNode_Connect(int node1, int node2, float weight = -1.0f) {
        if (weight == -1.0f) {
            weight = GetConnectionWeight(node1, node2);
        }

        PushCommand(new CCmdNodeConnect(node1, node2, weight));
    }

    public void CmdNode_Disconnect(int node1, int node2) {
        PushCommand(new CCmdNodeDisconnect(node1, node2));
    }

    #endregion

    #region GUI Observers

    public void OnPointerClick(PointerEventData eventData) => mFSM.CurrentState.UIGraphField_OnPointerClick(eventData);

    #endregion

    #region Graph Observers

    private void Graph_OnNodeCreated(int id, Vector2 position) {
        var newNode = Instantiate(mGraphNodePrefab, mNodesParent);

        newNode.Id = id;
        newNode.transform.position = position;

        mUINodes.Add(newNode);
    }

    private void Graph_OnNodeRemoved(int id) {
        mUINodes[id].DestroyWithAnimation();
        mUINodes.Remove(id);
    }

    private void Graph_OnNodeConnected(int node1, int node2, float weight) {
        var connection = Instantiate(mGraphConnectionPrefab, mConnectionsParent);

        connection.SetNodes(mUINodes[node1], mUINodes[node2]);

        mUIConnections.Add(connection);
    }

    private void Graph_OnNodeDisconnected(int node1, int node2) {
        foreach (var connection in mUIConnections) {
            if (connection.IsConnectedToNodesId(node1, node2)) {
                connection.DestroyWithAnimation();
                mUIConnections.Remove(connection);
                return;
            }
        }
    }

    private void Graph_OnNodeValueUpdated(int id, Vector2 value) {
        mUINodes[id].transform.position = value;

        // Force update of all connections weight
        foreach (var connection in mGraph.GetAllConnectionsWithNode(id)) {
            mGraph.SetConnectionWeight(id, connection.Item1, GetConnectionWeight(id, connection.Item1));
        }
    }

    #endregion

    #region Utils

    public float GetConnectionWeight(int node1, int node2) {
        Assert.IsTrue(mGraph.IsNodeValid(node1) && mGraph.IsNodeValid(node2), "Canno't auto calculate connection weight!");

        return (mGraph[node2] - mGraph[node1]).magnitude;
    }

    #endregion

}
