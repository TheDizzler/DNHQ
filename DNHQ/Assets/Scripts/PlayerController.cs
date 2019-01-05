//#define DEBUG_RAYS
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
	public new Camera camera;

	List<Vector3> vertices = new List<Vector3>();
	public LineRenderer closeLine;
	public LineRenderer farLine;
#if DEBUG_RAYS
	private List<LineRenderer> rayLines = new List<LineRenderer>();
#endif

	public Sprite destinationNG;
	public Sprite destinationOK;
	public GameObject destinationMarker;

	private Transform selectedTarget;
	private HeroController selectedHero;

	public Material lineMat;


	void Start()
	{
		closeLine.startColor = Color.white;
		closeLine.endColor = Color.blue;
		closeLine.startWidth = .2f;
		closeLine.endWidth = .2f;

		farLine.startColor = Color.yellow;
		farLine.endColor = Color.red;
		farLine.startWidth = .02f;
		farLine.endWidth = .02f;

#if DEBUG_RAYS
		for (int i = 0; i < 27; ++i)
		{
			LineRenderer line = new GameObject().AddComponent<LineRenderer>();
			line.enabled = false;
			line.material = lineMat;
			line.startColor = Color.white;
			line.endColor = Color.white;
			line.transform.Rotate(new Vector3(1, 0, 0), 90);
			line.sortingLayerName = "Command";
			line.startWidth = .05f;
			line.endWidth = .05f;
			rayLines.Add(line);
		}
#endif
	}

	Transform lastTarget = null;

	void dont()
	{
		if (selectedHero != null)
		{
			//selectedHero.Command(camera);
			CommandHero();
		}
		else if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				Transform objectHit = hit.transform;
				//if (objectHit.CompareTag("Hero"))
				//{
				//	selectedTarget = objectHit;
				//	selectedHero = selectedTarget.GetComponent<HeroController>();
				//	selectedHero.SetSelected(true);

				//	closeLine.SetPosition(0, selectedTarget.position);
				//	closeLine.SetPosition(1, selectedTarget.position);
				//	closeLine.enabled = true;
				//	farLine.enabled = true;
				//	destinationMarker.GetComponent<SpriteRenderer>().sprite = destinationOK;
				//}
			}
		}
	}


	private void CommandHero()
	{
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			Transform objectFound = hit.transform;

			Vector3 heropos = selectedHero.transform.position;
			if (objectFound.CompareTag("Ground"))
			{
				CreateLineToTarget(hit.point, heropos, selectedHero.move);
				destinationMarker.transform.localPosition = hit.point;

				/*if (Input.GetMouseButtonDown(1))
				{ // deselect
					closeLine.enabled = false;
					farLine.enabled = false;
					selectedTarget = null;
					selectedHero.SetSelected(false);
					selectedHero = null;
				}*/
			}
			else if (objectFound != selectedTarget)
			{
				if (objectFound == lastTarget)
				{
					return;
				}

				CreateLineToTarget(objectFound.position, heropos, selectedHero.move);
				Vector3 markerPos = objectFound.position;
				markerPos.y += 2;
				destinationMarker.transform.localPosition = markerPos;

				// get corners of target's bounding box

				Matrix4x4 thisMatrix = objectFound.localToWorldMatrix;
				Quaternion storedRotation = objectFound.rotation;
				objectFound.rotation = Quaternion.identity;

				Vector3 extents = objectFound.GetComponent<BoxCollider>().bounds.extents * .9f;
				Vector3[] verts = new Vector3[9];
				verts[0] = thisMatrix.MultiplyPoint3x4(extents);
				verts[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));
				verts[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
				verts[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
				verts[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
				verts[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));
				verts[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
				verts[7] = thisMatrix.MultiplyPoint3x4(-extents);
				verts[8] = objectFound.transform.position;

				objectFound.rotation = storedRotation;

				string found = "";
				int next = 0;
				// raycast from hero to target
				int numHits = 0;
				Vector3[] raycastpoints = selectedHero.RaycastPoints;
				for (int i = 0; i < raycastpoints.Length; ++i)
				{
					Vector3 raycast = selectedTarget.TransformPoint(raycastpoints[i]);
					RaycastHit raycastHit;

					for (int j = 0; j < verts.Length; ++j)
					{
						if (Physics.Linecast(raycast, verts[j], out raycastHit))
						{

							found += raycastHit.transform.name + " - ";
							if (raycastHit.transform == objectFound)
							{
								++numHits;
#if DEBUG_RAYS
								rayLines[next].enabled = true;
								rayLines[next].SetPosition(0, raycast);
								rayLines[next].SetPosition(1, raycastHit.point);
								rayLines[next].startColor = Color.green;
								rayLines[next].endColor = Color.green;
							}
							else
							{
								rayLines[next].enabled = false;
								rayLines[next].startColor = Color.red;
								rayLines[next].endColor = Color.red;
#endif
							}
						}

						++next;
					}
				}

				Debug.Log(found);
				Debug.Log("numHits: " + numHits + " out of " + (verts.Length * raycastpoints.Length));
			}

			lastTarget = objectFound;
		}
	}



	public void CreateLineToTarget(Vector3 targetPos, Vector3 startPoint, float maxMove)
	{
		int nextPoint = 0;
		float distanceToPoint = (targetPos - startPoint).magnitude;
		if (distanceToPoint > maxMove)
		{
			nextPoint = (int)(distanceToPoint / maxMove);
			Vector3 A = startPoint;
			Vector3 B = targetPos;
			Vector3 P = (B - A) * (maxMove / distanceToPoint) + startPoint;
			closeLine.SetPosition(1, P);
			farLine.SetPosition(0, P);
			farLine.SetPosition(1, targetPos);
		}
		else
		{
			closeLine.SetPosition(1, targetPos);
			farLine.SetPosition(0, startPoint);
			farLine.SetPosition(1, startPoint);
		}
	}
}
