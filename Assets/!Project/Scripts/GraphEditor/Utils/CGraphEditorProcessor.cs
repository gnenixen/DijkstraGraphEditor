using CommandSystem;

public class CGraphEditorProcessor<T> {
    private CGraph<T> mGraph;
    private CCommandList mGraphCommandList = new();

    public CGraphEditorProcessor(CGraph<T> mGraph) {
        this.mGraph = mGraph;
    }

    public void PushCommand(ABaseGraphCommand<T> command) {
        command.mGraph = mGraph;
        mGraphCommandList.AddAndExecuteCommand(command);
    }

    public void Undo() {
        mGraphCommandList.Undo();
    }

    public void Redo() {
        mGraphCommandList.Redo();
    }
}