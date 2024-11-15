using System;
using UnityEngine;

public class UI_SecretTile : MonoBehaviour
{
    public const float SIDE_LENGTH = 80F;
    public const float BUFFER = 10F;
    public const float TOTAL = SIDE_LENGTH + BUFFER;

    private Secret _secret;

    internal void Initialize(Secret secret)
    {
        _secret = secret;
    }
}
