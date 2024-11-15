using UnityEngine;
using UnityEngine.UI;

public class UISecretTile : MonoBehaviour
{
    private Secret _secret;

    internal void Initialize(Secret secret)
    {
        _secret = secret;

        GetComponent<RawImage>().texture = secret.IconTexture;
    }
}
