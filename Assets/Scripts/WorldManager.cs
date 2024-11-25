using UnityEngine;

public class WorldManager : GlobalSingleInstanceMonoBehaviour<WorldManager>
{
    public RoomVisibilityManager roomVisibilityManager;
    [Range(0, 1)]
    public float brightness = 0.2f;
    public Light mainLight;
    public Vector2 lightVals = new Vector2(0f, 20f);
    public float exprpSteepness = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected override void Start()
    {
        base.Start();
        roomVisibilityManager = RoomVisibilityManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //mainLight.intensity= Mathf.Lerp(lightVals.x, lightVals.y, brightness);
        mainLight.intensity = Exprp(lightVals.x, lightVals.y, brightness);
    }

    public float Exprp(float a, float b, float t, float? steepness= null)
    {
        if(steepness == null)
        {
            steepness = exprpSteepness;
        }
        return a + (b - a) * Mathf.Pow(t, steepness.Value) / (Mathf.Pow(t, steepness.Value) + Mathf.Pow(1 - t, steepness.Value));
    }
}
