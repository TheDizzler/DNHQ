using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RaycastHelper : MonoBehaviour
{
	[SerializeField] private HeroController hero = null;

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		for (int i = 0; i < hero.RaycastPoints.Length; ++i)
		{
			Gizmos.DrawSphere(transform.TransformPoint(hero.RaycastPoints[i]), .1f);
		}
	}
}
