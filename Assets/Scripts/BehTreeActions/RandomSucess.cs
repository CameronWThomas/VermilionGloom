using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.BehTreeActions
{
    [TaskCategory("Custom")]
    public class WeightedRandomSuccess : BehaviorDesigner.Runtime.Tasks.Action
    {

        public float successChance = 0.5f;

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            float randomVal = Random.Range(0f, 1f);
            if(randomVal <= successChance)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
    }
}
