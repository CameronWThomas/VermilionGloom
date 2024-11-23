using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class IsNull : Conditional
{
    public SharedVariable Object;
    public SharedBool InvertCondition;
    public SharedFloat Priority;

    public override TaskStatus OnUpdate()
    {
        var condition = Object.GetValue() == null;
        condition = InvertCondition.Value ? !condition : condition;

        return condition ? TaskStatus.Success : TaskStatus.Failure;
    }

    public override float GetPriority() => Priority.Value;
}