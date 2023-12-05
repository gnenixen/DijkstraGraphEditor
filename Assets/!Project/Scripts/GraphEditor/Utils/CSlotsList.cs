using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

public class CSlotsList<T> {
    private class CElement {
        public int mId;
        public int mNextFreeSlot;
        public bool bActive;
        public T mValue;

        public CElement(int mId, int mNextFreeSlot, bool bActive, T mValue) {
            this.mId = mId;
            this.mNextFreeSlot = mNextFreeSlot;
            this.bActive = bActive;
            this.mValue = mValue;
        }
    }

    public event Action<int, T> OnSlotValueUpdated = delegate {};

    private List<CElement> mElements;
    private int mSlotsCount = 0;

    public int Count => mSlotsCount;

    public T this[int i] {
        get { return GetSlotValue(i); }
        set { SetSlotValue(i, value); }
    }

    public CSlotsList(int preinitialNodesPoolSize = 32) {
        mElements = new List<CElement>();

        for (int i = 0; i < preinitialNodesPoolSize; i++) {
            mElements.Add(new CElement(i, i, false, default));
        }
    }

    public int Add(T value) {
        // Reserve more space if needed
        if (mSlotsCount == mElements.Count) {
            int count = mElements.Count;

            for (int i = 0; i < 32; i++) {
                mElements.Add(new CElement(count + i, count, false, default));
            }
        }

        // Add new element to first valid slot
        int slot = mElements[mSlotsCount].mNextFreeSlot;
        Assert.IsFalse(mElements[slot].bActive);

        mElements[slot].bActive = true;
        mElements[slot].mValue = value;

        mSlotsCount++;

        return slot;
    }

    public void Remove(int id) {
        Assert.IsTrue(IsSlotActive(id));

        // Make node free
        mSlotsCount--;
        mElements[mSlotsCount].mNextFreeSlot = id;
        mElements[id].bActive = false;
    }

    public void SetSlotValue(int id, T value) {
        Assert.IsTrue(IsSlotActive(id));

        mElements[id].mValue = value;

        OnSlotValueUpdated.Invoke(id, value);
    }

    public T GetSlotValue(int id) {
        Assert.IsTrue(IsSlotActive(id));

        return mElements[id].mValue;
    }

    public List<int> GetActiveSlots() {
        return mElements
            .Select((item, index) => new {item = item, index = index})
            .Where(e => e.item.bActive)
            .Select(e => e.index)
        .ToList();
    }

    public bool IsSlotActive(int id) {
        if (id < 0 || id >= mElements.Count)
            return false;
        
        return mElements[id].bActive;
    }
}