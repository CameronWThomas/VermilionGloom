using System;
using UnityEngine;

[RequireComponent(typeof(NPCCharacterCreator))]
public class SceneInitializer : GlobalSingleInstanceMonoBehaviour<SceneInitializer>
{
    [SerializeField, Range(1, 500)] private int _npcHumanCharacterCount = 10;

    protected override void Start()
    {
        base.Start();
        CreateCharacters();
    }

    private void CreateCharacters()
    {
        var characterCreator = GetComponent<NPCCharacterCreator>();

        var characterInfos = characterCreator.CreateCharacters(_npcHumanCharacterCount);
    }
}