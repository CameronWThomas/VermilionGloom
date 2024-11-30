using System;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] Vector2 _intensityRange = new Vector2(.5f, 3.5f);
    [SerializeField] float _maxStep = .1f;
 
    private float _lastIntensity = -1f;

    private Light Light => GetComponent<Light>();


    private void Start()
    {
        _lastIntensity = Light.intensity;
        UnityEngine.Random.InitState((int)DateTime.UtcNow.Ticks);
    }

    private void Update()
    {
        var step = UnityEngine.Random.Range(0f, _maxStep);
        var sign = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;

        var newIntensity = Mathf.Clamp(_lastIntensity + step * sign, _intensityRange.x, _intensityRange.y);

        Light.intensity = newIntensity;
        _lastIntensity = newIntensity;


    }

}