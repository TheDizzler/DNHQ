#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class FloorTileMapController : MonoBehaviour
{
	public Renderer materialRenderer;


	void Update()
	{
		Vector2 scale = transform.localScale;
		materialRenderer.sharedMaterial.SetTextureScale("_MainTex", scale);
	}
}
#endif