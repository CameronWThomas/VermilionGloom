using UnityEngine;

public class SunTrigger : MonoBehaviour
{
    WorldManager worldManager;
    public MeshRenderer floorMesh;
    public float maxFloorBrightness = 1;
    public MeshRenderer airMesh;
    public float maxAirBrightness = 1;

    public float minBrightness = 0.00001f;




    // Start is called before the first frame update
    void Start()
    {
        worldManager = WorldManager.Instance;

        Material flr = floorMesh.materials[0];
        maxFloorBrightness = flr.GetFloat("_alpha");

        Material air = airMesh.materials[0];
        maxAirBrightness = air.GetFloat("_alpha");
    }

    // Update is called once per frame
    void Update()
    {
        ModulateSunBrightness();
    }
    void ModulateSunBrightness()
    {
        //float floorBrightness = Mathf.Lerp(minBrightness, maxFloorBrightness, worldManager.brightness);
        float floorBrightness = WorldManager.Instance.Exprp(minBrightness, maxFloorBrightness, worldManager.brightness);
        floorMesh.materials[0].SetFloat("_alpha", floorBrightness);

        //float airBrightness = Mathf.Lerp(minBrightness, maxAirBrightness, worldManager.brightness);
        float airBrightness = WorldManager.Instance.Exprp(minBrightness, maxAirBrightness, worldManager.brightness);
        airMesh.materials[0].SetFloat("_alpha", airBrightness);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(worldManager.brightness <= 0.3)
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if(player != null)
        {
            player.inSun = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.inSun = false;
        }
    }
}
