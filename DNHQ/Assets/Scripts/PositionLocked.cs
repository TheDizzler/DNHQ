using UnityEngine;

/// <summary>
/// Add to an object to prevent it from being moved in the editor (such as Managers).
/// </summary>
[ExecuteInEditMode]
public class PositionLocked : MonoBehaviour
{
#if UNITY_EDITOR
	void Update()
	{
		transform.position = new Vector3(0, 0, 0);
		transform.localScale = Vector3.one;
		transform.localEulerAngles = Vector3.zero;
	}
#endif
}