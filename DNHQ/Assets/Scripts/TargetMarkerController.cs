using UnityEngine;

public class TargetMarkerController : MonoBehaviour
{
	public void SetPosition(Vector3 position)
	{
		transform.localPosition = position;
	}

	public void Activate(bool show)
	{
		gameObject.SetActive(show);
	}
}
