using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Conversation")]
public class HaveConversation_Old : Action
{
    NpcBrain npcBrain;
    MvmntController mvmntController;
    public override void OnStart()
    {
        npcBrain = GetComponent<NpcBrain>();
        mvmntController = GetComponent<MvmntController>();

        mvmntController.SetRunning(false);
        mvmntController.GoToTarget(transform.position);

    }

    public override TaskStatus OnUpdate()
    {
        if(npcBrain.ConvoTarget != null)
        {
            //could maybe make them wait for you to approach instead?
            //think i might do this ^^^
            //uncomment this line to change it
            //mvmntController.SetTarget(npcBrain.convoTarget.transform.position);
            //if(mvmntController.IsAtDestination())
            //{
                mvmntController.FaceTarget(npcBrain.ConvoTarget.transform.position);
            //}
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }

}
