using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class MoveToRandomPos : Action
{
    public SharedFloat posRadius = 5;
    public SharedVector2 lingerTimeRange = new Vector2(5, 20);
    public float lingerTime;
    public float lingerCounter;

    public float restartAnywayTime = 20f;
    public float restartCounter;

    MvmntController mvmntController;
    public override void OnStart()
    {
        mvmntController = GetComponent<MvmntController>(); 
        //if(mvmntController == null)
        //{
        //    return;
        //}

        if(posRadius == null)
        {
            posRadius = 5;
        }
        if(lingerTimeRange == null)
        {
            lingerTimeRange = new Vector2(5, 20);
        }
        
        lingerTime = Random.Range(lingerTimeRange.Value.x, lingerTimeRange.Value.y);

        lingerCounter = 0;
        restartCounter = 0;

        SetRandomPositionInRadius();
    }

    public override TaskStatus OnUpdate()
    {
        if (mvmntController.IsAtDestination())
        {
            lingerCounter += Time.deltaTime;
            if(lingerCounter >= lingerTime)
            {
                return TaskStatus.Success;
            }
        }

        if(restartCounter >= restartAnywayTime)
        {
            return TaskStatus.Failure;
        }
        else
        {
            restartCounter += Time.deltaTime;
        }

        return TaskStatus.Running;
    }

    private void SetRandomPositionInRadius()
    {
        Vector3 offset = new Vector3(Random.Range(-posRadius.Value, posRadius.Value), 0, Random.Range(-posRadius.Value, posRadius.Value));
        Vector3 randomPos = transform.position + offset;

        if(mvmntController.CanReachTarget(randomPos))
        {
            mvmntController.SetTarget(randomPos);
        }
        else
        {
            SetRandomPositionInRadius();
        }

    }

}
