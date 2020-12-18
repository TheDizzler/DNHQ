using UnityEngine;

public class HeroController : MonoBehaviour
{
	public float move = 5;
	public Vector3[] RaycastPoints;

	public float losConeSize = 10;

	[SerializeField] private PlayerController playerCon = null;


}
