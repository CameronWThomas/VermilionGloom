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
        if (_brain.SecretFromLastProcessedSecretEvent.HasSecretTarget)
        {
            var characterId = _brain.SecretFromLastProcessedSecretEvent.SecretTarget;
            var characterTransform = CharacterInfoBB.Instance.GetCharacterInfo(characterId).transform;
            GetComponent<MvmntController>().FaceTarget(characterTransform.position);
        }

        return TaskStatus.Success;
    }
}