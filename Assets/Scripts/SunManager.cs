using UnityEngine;

public class SunManager : GlobalSingleInstanceMonoBehaviour<SunManager>
{
    public RoomVisibilityManager roomVisibilityManager;
    [Range(0, 1)]
    public float brightness = 0.2f;
    public Light mainLight;
    public Vector2 lightVals = new Vector2(0f, 20f);
    public float exprpSteepness = 3;

    private Vector2 lowLightVals = new Vector2(0f, 0.333f);
    private Vector2 highLightVals = new Vector2(0.333f, 1f);
    private Vector2 highLightPeriodRange = new Vector2(5f, 20f); // in time ticks
    private Vector2 lowLightPeriodRange = new Vector2(20f, 60f); // in time ticks
    private float highLightPeriodChance = 0.2f;
    [SerializeField]
    private bool highLightPeriod = false;
    [SerializeField]
    private float lightPeriod = 0f;
    [SerializeField]
    private float newPeriodTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected override void Start()
    {
        base.Start();
        roomVisibilityManager = RoomVisibilityManager.Instance;
        newPeriodTime = Random.Range(lowLightPeriodRange.x, lowLightPeriodRange.y);
    }

    // Update is called once per frame
    void Update()
    {
        //mainLight.intensity= Mathf.Lerp(lightVals.x, lightVals.y, brightness);
        mainLight.intensity = Exprp(lightVals.x, lightVals.y, brightness);

        // fluctuate light periods
        lightPeriod += Time.deltaTime;
        if(lightPeriod >= newPeriodTime)
        {
            
            lightPeriod = 0f;
            if(shouldHighLight())
            {
                newPeriodTime = Random.Range(highLightPeriodRange.x, highLightPeriodRange.y);
                highLightPeriod = true;
                brightness = Random.Range(highLightVals.x, highLightVals.y);
            }
            else
            {
                newPeriodTime = Random.Range(lowLightPeriodRange.x, lowLightPeriodRange.y);
                highLightPeriod = false;
                brightness = Random.Range(lowLightVals.x, lowLightVals.y);
            }
        }

    }

    private bool shouldHighLight()
    {
        float rand = Random.Range(0f, 1f);
        return rand < highLightPeriodChance;
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
