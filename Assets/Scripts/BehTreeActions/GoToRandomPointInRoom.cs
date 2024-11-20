using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class GoToRandomPointInRoom : Action
{
    public SharedFloat posRadius = 5;
    public SharedVector2 lingerTimeRange = new Vector2(5, 20);
    public float lingerTime;
    public float lingerCounter;

    public float restartAnywayTime = 20f;
    public float restartCounter;

    MvmntController mvmntController;
    NpcBrain npcBrain;

    Vector3 targetPosition;
    public override void OnStart()
    {
        mvmntController = GetComponent<MvmntController>();
        npcBrain = GetComponent<NpcBrain>();

        mvmntController.SetRunning(false);
        //if(mvmntController == null)
        //{
        //    return;
        //}


        

        if (posRadius == null)
        {
            posRadius = 5;
        }
        if (lingerTimeRange == null)
        {
            lingerTimeRange = new Vector2(5, 20);
        }

        lingerTime = Random.Range(lingerTimeRange.Value.x, lingerTimeRange.Value.y);

        lingerCounter = 0;
        restartCounter = 0;

        if (mvmntController.enabled)
        {
            SetTargetToRandomPointInRoom();
        }
    }

    public override TaskStatus OnUpdate()
    {
        if(npcBrain.activeRoom == null)
        {
            return TaskStatus.Failure;
        }
        


        if (mvmntController.IsAtDestination)
        {
            lingerCounter += Time.deltaTime;
            if (lingerCounter >= lingerTime)
            {
                return TaskStatus.Success;
            }
        }

        if (restartCounter >= restartAnywayTime)
        {
            return TaskStatus.Failure;
        }
        else
        {
            restartCounter += Time.deltaTime;
        }

        return TaskStatus.Running;
    }

    private void SetTargetToRandomPointInRoom()
    {
        if(npcBrain.activeRoom == null)
        {
            return;
        }

        targetPosition = npcBrain.activeRoom.GetRandomPointInRoom();

        if (mvmntController.CanReachTarget(targetPosition))
        {
            mvmntController.GoToTarget(targetPosition);
        }
        else
        {
            SetTargetToRandomPointInRoom();
        }

    }

}
