using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Secret Processing")]
public class FaceLastSecretEventTarget : Action
{
    protected NpcBrain _brain;

    public override void OnStart()
    {
        _brain = GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_brain.LastSecretEventResponse != null)
        {
            var characterTransform = _brain.LastSecretEventResponse.SecretResponseTarget;
            GetComponent<MvmntController>().FaceTarget(characterTransform.position);
        }

        return TaskStatus.Success;
    }
}