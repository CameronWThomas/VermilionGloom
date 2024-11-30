using UnityEngine;

public class FollowCam : MonoBehaviour
{
	public Transform followTarget;
	private Vector3 zoomDirection;

	[Header("Zoom Settings")]
	private float targetDistance;
	public Vector2 zoomLimits = new Vector2(5, 20);

	public Vector3 InitialOffset { get; private set; }

	// Lerp settings for smooth follow
	[Header("Follow Settings")]
	public float followSpeed = 5f;  // Adjust this to control how quickly the camera follows the player

	// Mouse panning settings
	[Header("Mouse Panning Settings")]
	[SerializeField]
	Vector3 mouseOffsetPct;
	[SerializeField]
	Vector2 panMultiplier = new Vector2(1, 1.25f);
	[SerializeField]
	Vector3 camFwd;
	public bool enablePanning = true;
	//public float panningThreshold = 5f; // How far the mouse must be from the player to start panning
	public Vector2 panningThreshold = new Vector2(0.5f, 0.2f); // How far the mouse must be from the player to start panning
	public float panningIntensity = 3f;     // Speed at which the camera pans
	public float panningSpeed = 1f;     // Speed at which the camera pans

    // Start is called before the first frame update
    void Start()
	{
		camFwd = transform.forward;
		InitialOffset = transform.position - followTarget.position;
		zoomDirection = followTarget.position - transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		// Linear Interpolation for Smooth Camera Follow
		Vector3 targetPosition = followTarget.position + InitialOffset;
		if (!enablePanning)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
			return;
        }

		//transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

		// Mouse Panning Logic with Deadzone
		Vector3 mousePosition = Input.mousePosition;
		Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
		Vector3 mouseOffset = mousePosition - screenCenter;
		mouseOffsetPct = new Vector3(mouseOffset.x / (Screen.width / 2), mouseOffset.y / (Screen.height / 2), 0f);

		if (Mathf.Abs(mouseOffsetPct.x) > panningThreshold.x || Mathf.Abs(mouseOffsetPct.y) > panningThreshold.y)
		{
			// Calculate the amount to pan based on the mouse offset
			Vector3 panDirection = new Vector3(
				(mouseOffset.x * panMultiplier.x) / Screen.width,
                (mouseOffset.y * panMultiplier.y) / Screen.height,
				0f
                );

            Vector3 camTranslatedDir = transform.TransformDirection(panDirection);
            //transform.position += panDirection * panningSpeed * Time.deltaTime;
            targetPosition = targetPosition + (camTranslatedDir * panningIntensity);

        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, panningSpeed * Time.deltaTime);
    }
}
