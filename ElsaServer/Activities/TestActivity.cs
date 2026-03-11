using Elsa.Workflows;

namespace ElsaServer.Activities;

public class TestActivity : CodeActivity<string>
{
    protected override void Execute(ActivityExecutionContext context)
    {
        context.Set(Result, "awd");
        Console.WriteLine("Executing TestActivity");
    }
}
