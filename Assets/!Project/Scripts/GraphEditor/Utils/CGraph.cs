using System;
using System.Collections.Generic;

public class CGraph<T> {
    public class CNode {
        public int mId;
        public int mNextFreeSlot;
        public bool bActive;
        public T mValue;

        public CNode(int mId, int mNextFreeSlot, bool bActive, T mValue) {
            this.mId = mId;
            this.mNextFreeSlot = mNextFreeSlot;
            this.bActive = bActive;
            this.mValue = mValue;
        }
    }

    public class CConnection {
        public int mNode1;
        public int mNode2;
        public float mWeigth;

        public CConnection(int mNode1, int mNode2, float mWeight) {
            this.mNode1 = mNode1;
            this.mNode2 = mNode2;
            this.mWeigth = mWeight;
        }

        public bool IsGivenNodesConnected(int node1, int node2) {
            return
                (mNode1 == node1 && mNode2 == node2) ||
                (mNode1 == node2 && mNode2 == node1);
        }
    }

    public event Action<int, T> OnNodeAdded = delegate {};
    public event Action<int> OnNodeRemove = delegate {};
    public event Action<int, int, float> OnNodeConnected = delegate {};
    public event Action<int, int> OnNodeDisconnected = delegate {};

    public event Action<int, T> OnNodeValueUpdated {
        add { mNodes.OnSlotValueUpdated += value; }
        remove { mNodes.OnSlotValueUpdated -= value; }
    }

    #region Variables

    private CSlotsList<T> mNodes;
    private List<CConnection> mConnections;

    #endregion

    public int NodesCount => mNodes.Count;

    public T this[int i] {
        get { return mNodes.GetSlotValue(i); }
        set { mNodes.SetSlotValue(i, value); }
    }

    public CGraph(int preinitialNodesPoolSize = 32) {
        mNodes = new CSlotsList<T>(preinitialNodesPoolSize);
        mConnections = new List<CConnection>();
    }

    public int AddNewNode(T value) {
        var slot = mNodes.Add(value);

        OnNodeAdded.Invoke(slot, value);

        return slot;
    }

    public void RemoveNode(int id) {
        var connectionsToNodes = mConnections.FindAll((connection) => connection.mNode1 == id || connection.mNode2 == id);

        foreach (var connection in connectionsToNodes) {
            mConnections.Remove(connection);

            OnNodeDisconnected.Invoke(connection.mNode1, connection.mNode2);
        }

        mConnections.RemoveAll((connection) => connection.mNode1 == id || connection.mNode2 == id);

        mNodes.Remove(id);

        OnNodeRemove.Invoke(id);
    }

    public bool IsNodeValid(int id) {
        return mNodes.IsSlotActive(id);
    }

    public bool ConnectNodes(int n1, int n2, float weight) {
        if (n1 == n2)
            return false;

        if (!IsNodeValid(n1) || !IsNodeValid(n2))
            return false;
        
        if (IsNodesConnected(n1, n2))
            return false;
        
        mConnections.Add(new CConnection(n1, n2, weight));

        OnNodeConnected.Invoke(n1, n2, weight);

        return true;
    }

    public void DisconnectNodes(int n1, int n2) {
        if (n1 == n2)
            return;

        if (!IsNodeValid(n1) || !IsNodeValid(n2))
            return;

        if (!IsNodesConnected(n1, n2))
            return;
        
        foreach (var connection in mConnections) {
            if (connection.IsGivenNodesConnected(n1, n2)) {
                OnNodeDisconnected.Invoke(n1, n2);
                mConnections.Remove(connection);
                return;
            }
        }
    }

    public void SetConnectionWeight(int n1, int n2, float weigth) {
        if (n1 == n2)
            return;

        if (!IsNodeValid(n1) || !IsNodeValid(n2))
            return;

        if (!IsNodesConnected(n1, n2))
            return;
        
        foreach (var connection in mConnections) {
            if (connection.IsGivenNodesConnected(n1, n2)) {
                connection.mWeigth = weigth;
                return;
            }
        }
    }

    public float GetConnectionWeight(int n1, int n2) {
        if (n1 == n2)
            return -1.0f;

        if (!IsNodeValid(n1) || !IsNodeValid(n2))
            return -1.0f;

        foreach (var connection in mConnections) {
            if (connection.IsGivenNodesConnected(n1, n2)) {
                return connection.mWeigth;
            }
        }

        return -1.0f;
    }

    public List<(int, float)> GetAllConnectionsWithNode(int id) {
        var ret = new List<(int, float)>();
        foreach (var connection in mConnections) {
            if (connection.mNode1 != id && connection.mNode2 != id)
                continue;
            
            if (connection.mNode1 == id) {
                ret.Add((connection.mNode2, connection.mWeigth));
            } else {
                ret.Add((connection.mNode1, connection.mWeigth));
            }
        }

        return ret;
    }

    public bool IsNodesConnected(int n1, int n2) {
        if (!IsNodeValid(n1) || !IsNodeValid(n2))
            return false;
        
        foreach (var connection in mConnections) {
            if (connection.IsGivenNodesConnected(n1, n2))
                return true;
        }

        return false;
    }

    public int[,] AdjacencyMatrix() {
        var ret = new int[mNodes.Count, mNodes.Count];

        for (int i = 0; i < mNodes.Count; i++) {
            for (int j = 0; j < mNodes.Count; j++) {
                if (IsNodesConnected(i, j)) {
                    ret[i, j] = 1;
                }
            }
        }

        return ret;
    }

    public List<int> GetActiveNodes() => mNodes.GetActiveSlots();
}