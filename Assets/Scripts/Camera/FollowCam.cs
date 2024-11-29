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
	public float panningThreshold = 5f; // How far the mouse must be from the player to start panning
	public float panningSpeed = 3f;     // Speed at which the camera pans

	// Start is called before the first frame update
	void Start()
	{
		InitialOffset = transform.position - followTarget.position;
		zoomDirection = followTarget.position - transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		// Linear Interpolation for Smooth Camera Follow
		Vector3 targetPosition = followTarget.position + InitialOffset;
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

		// Mouse Panning Logic with Deadzone
		Vector3 mousePosition = Input.mousePosition;
		Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
		Vector3 mouseOffset = mousePosition - screenCenter;

		if (Mathf.Abs(mouseOffset.x) > panningThreshold || Mathf.Abs(mouseOffset.y) > panningThreshold)
		{
			// Calculate the amount to pan based on the mouse offset
			Vector3 panDirection = new Vector3(mouseOffset.x / Screen.width, 0, mouseOffset.y / Screen.height);
			transform.position += panDirection * panningSpeed * Time.deltaTime;
		}
	}
}
