using UnityEngine;

public class UI_CharacterInfo : MonoBehaviour
{
    private CharacterInfo _characterInfo;

    public void Initialize(CharacterInfo characterInfo)
    {
        var statZone = GetComponentInChildren<UI_StatZone>(true);
        statZone.SetCharacter(characterInfo);
    }
}