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

        SetContent(characterId.PortraitContent);
    }

    private void SetContent(Texture2D contentTexture)
    {
        _content.texture = contentTexture;
    }
}