using System;
using UnityEngine;

public class TalkingMenu : MonoBehaviour
{
    private const float WIDTH_BUFFER = 10f;

    [SerializeField] private GameObject _talkingMenuGameObject;
    [SerializeField] private RectTransform _othersSecretContent;

    [SerializeField] private GameObject _otherCharacterUIPrefab;

    private NpcBrain _npcBrain = null;

    public void Close()
    {
        _talkingMenuGameObject.SetActive(false);

        for (var i = 0; i < _othersSecretContent.childCount; i++)
        {
            Destroy(_othersSecretContent.GetChild(i).gameObject);
        }
    }

    public void Open()
    {
        var totalWidth = 0f;
        foreach (var othersSecrets in _npcBrain.OthersSecretsCollection)
        {
            var otherSecretInstance = Instantiate(_otherCharacterUIPrefab, _othersSecretContent);
            
            var othersSecretsControllerUI = otherSecretInstance.GetComponent<OthersSecretsControllerUI>();
            othersSecretsControllerUI.Initialize(othersSecrets);

            var othersSecretsRectTransform = otherSecretInstance.GetComponent<RectTransform>();
            var width = othersSecretsRectTransform.rect.width;
            othersSecretsRectTransform.anchoredPosition = new Vector2(totalWidth + width / 2f, othersSecretsRectTransform.anchoredPosition.y);

            totalWidth += width + WIDTH_BUFFER;
        }
        
        _othersSecretContent.sizeDelta = new Vector2(totalWidth, _othersSecretContent.sizeDelta.y);

        _talkingMenuGameObject.SetActive(true);


    }

    internal void SetNpc(NpcBrain brain)
    {
        _npcBrain = brain;
    }
}
