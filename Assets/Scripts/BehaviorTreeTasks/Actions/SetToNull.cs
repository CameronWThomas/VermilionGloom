using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class SetToNull : Action
{
    public SharedVariable Object;

    public override TaskStatus OnUpdate()
    {
        Object.SetValue(null);
        return TaskStatus.Success;
    }
}