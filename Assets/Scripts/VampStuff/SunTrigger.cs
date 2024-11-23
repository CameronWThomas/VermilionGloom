using UnityEngine;

public class SunTrigger : MonoBehaviour
{
    public bool sunIsUp = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (sunIsUp)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
