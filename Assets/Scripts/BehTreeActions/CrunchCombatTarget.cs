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
            mvmntController.SetTarget(npcBrain.combatTarget.transform.position);
            if(mvmntController.distanceToTarget <= npcBrain.crunchDistance)
            {
                bool crunched = npcBrain.Crunch();
                if (crunched)
                {
                    return TaskStatus.Success;
                }
                
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
