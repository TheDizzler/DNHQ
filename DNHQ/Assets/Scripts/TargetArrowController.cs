using UnityEngine;

public class TargetArrowController : MonoBehaviour
{
	[SerializeField] private float speed = 0;
	[SerializeField] private float bounceHeight = 0;
	private float heightAdj;
	private float t = 0;
	private float startHeight;


	public void Start()
	{
		heightAdj = GetComponent<SpriteRenderer>().bounds.size.y;
		gameObject.SetActive(false);
	}

	public void Show(GameObject target)
	{
		gameObject.SetActive(true);
		Vector3 newpos = target.transform.position;
		MeshRenderer renderer = target.GetComponent<MeshRenderer>();
		startHeight = renderer.bounds.size.y + heightAdj;
		newpos.y += startHeight;
		transform.localPosition = newpos;
		t = 0;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Update()
	{
		t += Time.deltaTime;
		Vector3 newpos = transform.localPosition;
		newpos.y = startHeight + (bounceHeight * Mathf.Sin(speed * t));
		transform.localPosition = newpos;
	}
}
