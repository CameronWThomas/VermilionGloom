using System;
using UnityEngine;

public class TalkingMenu : MonoBehaviour
{
    [SerializeField] private GameObject _talkingMenuGameObject;

    private NpcBrain _npcBrain = null;

    public void Close()
    {
        _talkingMenuGameObject.SetActive(false);
    }

    public void Open()
    {
        _talkingMenuGameObject.SetActive(true);
    }

    internal void SetNpc(NpcBrain brain)
    {
        _npcBrain = brain;
    }
}
