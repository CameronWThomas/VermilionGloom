using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class UI_Portrait : MonoBehaviour
{
    [SerializeField] private RawImage _content;

    private CharacterID _characterID;

    public void SetCharacter(CharacterID characterId)
    {
        _characterID = characterId;
        
        //SetContent(characterId.PortraitContent);
        SetContent(characterId.PortraitColor);
    }

    // TODO use eventually
    public void SetContent(Texture2D contentTexture)
    {
        _content.texture = contentTexture;
    }

    //TODO remove eventually
    public void SetContent(Color contentColor)
    {
        _content.color = contentColor;
    }
}