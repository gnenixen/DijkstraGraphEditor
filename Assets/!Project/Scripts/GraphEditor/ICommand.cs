using UnityEngine;

namespace CommandSystem {
    public interface IBaseCommand {
        public void Execute();

        public void Undo();
    }

    public abstract class ABaseGraphCommand<T> : IBaseCommand {
        public CGraph<T> mGraph;

        public virtual void Execute() {}
        public virtual void Undo() {}
    }

    public abstract class AGraphVec2Command : ABaseGraphCommand<Vector2> {}
}