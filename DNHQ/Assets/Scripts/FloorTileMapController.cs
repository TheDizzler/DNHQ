#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class FloorTileMapController : MonoBehaviour
{
	public Renderer renderer;


	void Update()
	{
		Vector2 scale = transform.localScale;
		renderer.sharedMaterial.SetTextureScale("_MainTex", scale);
	}
}
#endif