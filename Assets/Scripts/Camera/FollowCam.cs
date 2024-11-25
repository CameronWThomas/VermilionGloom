using UnityEngine;

public class FollowCam : MonoBehaviour
{
	public Transform player;
	public float followSpeed = 1.25f;
	public float mousePanLimit = 10f; 
	public float panSpeed = 3f;

	private Vector3 initialoffset;

	void Start()
	{
		// Set initial offset between camera and player
		initialoffset = transform.position - player.position;
	}

	void Update()
	{
		FollowPlayerWithMousePan();
	}

	void FollowPlayerWithMousePan()
	{
		// Get mouse position relative to screen center
		Vector3 mousePosition = Input.mousePosition;
		Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
		Vector3 mouseOffset = mousePosition - screenCenter;

		// Normalize the offset and apply limit to control panning amount
		Vector3 panMovement = new Vector3(mouseOffset.x / Screen.width, 0, mouseOffset.y / Screen.height) * panSpeed;
		panMovement = Vector3.ClampMagnitude(panMovement, mousePanLimit);

		// Target position for the camera based on player's position, initial offset, and pan movement
		Vector3 targetPosition = player.position + initialoffset + panMovement;

		// Smoothly interpolate camera's position to follow the player and apply panning
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
	}
}

