using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class CrunchCombatTarget : Action
{
    NpcBrain npcBrain;
    MvmntController mvmntController;
    public override void OnStart()
    {
        npcBrain = GetComponent<NpcBrain>();
        mvmntController = GetComponent<MvmntController>();
        mvmntController.SetRunning(true);

    }

    public override TaskStatus OnUpdate()
    {
        if (npcBrain.combatTarget != null)
        {
            //could maybe make them wait for you to approach instead?
            //think i might do this ^^^
            //uncomment this line to change it
            mvmntController.GoToTarget(npcBrain.combatTarget.transform.position);
            if(mvmntController.distanceToTarget <= npcBrain.crunchDistance)
            {
                npcBrain.Crunch();
            }
            return TaskStatus.Running;
        }
        else
        {
            mvmntController.SetRunning(false);
            return TaskStatus.Failure;
        }
    }

    

}
