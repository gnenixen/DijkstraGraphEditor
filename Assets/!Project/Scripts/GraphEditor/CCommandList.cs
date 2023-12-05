using System.Collections.Generic;

namespace CommandSystem {
    public sealed class CCommandList {
        private List<IBaseCommand> mCommandList = new List<IBaseCommand>();
        private int mCurrentIndex = 0;

        public void AddAndExecuteCommand(IBaseCommand command) {
            // Clear all commands that above current, we override top of list
            if (mCurrentIndex < mCommandList.Count) {
                mCommandList.RemoveRange(mCurrentIndex, mCommandList.Count - mCurrentIndex);
            }

            mCommandList.Add(command);
            command.Execute();
            mCurrentIndex++;
        }

        public void Undo() {
            if (mCommandList.Count == 0)
                return;
            
            if (mCurrentIndex > 0) {
                mCommandList[mCurrentIndex - 1].Undo();
                mCurrentIndex--;
            }
        }

        public void Redo() {
            if (mCommandList.Count == 0)
                return;

            if (mCurrentIndex < mCommandList.Count) {
                mCurrentIndex++;
                mCommandList[mCurrentIndex - 1].Execute();
            }
        }

        public void ClearHistory() {
            mCommandList = new List<IBaseCommand>();
            mCurrentIndex = 0;
        }
    }
}