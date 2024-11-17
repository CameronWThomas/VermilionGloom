using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.BehTreeActions
{
    [TaskCategory("Custom")]
    public class SetActiveRoomToRandomRoom: BehaviorDesigner.Runtime.Tasks.Action
    {
        NpcBrain npcBrain;

        public override void OnStart()
        {
            npcBrain = GetComponent<NpcBrain>();
            GenerateNewActiveRoom();
        }

        public override TaskStatus OnUpdate()
        {
            if(npcBrain.activeRoom != null)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
        private void GenerateNewActiveRoom()
        {
            int shortenedListLength = Random.Range(0, npcBrain.allRooms.Length);
            int randomRoomIndex = Random.Range(0, shortenedListLength);
            npcBrain.activeRoom = npcBrain.allRooms[randomRoomIndex];
        }
    }
}
