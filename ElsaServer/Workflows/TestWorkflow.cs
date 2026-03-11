using Elsa.Workflows;
using Elsa.Workflows.Activities;
using ElsaServer.Activities;

namespace ElsaServer.Workflows;

public class TestWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new Sequence { Activities = { new TestActivity(), new Complete() } };
    }
}
