using UnityEngine;

public class DestinationMarkerController : MonoBehaviour
{
	[SerializeField] private Sprite destinationNG = null;
	[SerializeField] private Sprite destinationOK = null;
	[SerializeField] private SpriteRenderer spriteRenderer = null;

	public void SetDestination(Vector3 destination, bool isValidDestination)
	{
		transform.localPosition = destination;
		if (isValidDestination)
		{
			spriteRenderer.sprite = destinationOK;
		}
		else
		{
			spriteRenderer.sprite = destinationNG;
		}
	}

	public void Activate(bool show)
	{
		gameObject.SetActive(show);
	}
}
