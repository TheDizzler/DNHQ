using System;
using UnityEngine;

public class MoveHelperController : MonoBehaviour
{
	[SerializeField] private LineRenderer closeLine = null;
	[SerializeField] private LineRenderer farLine = null;
	[SerializeField] private DestinationMarkerController destinationMarker = null;


	public void Activate(bool show, Vector3 position)
	{
		closeLine.enabled = show;
		farLine.enabled = show;
		destinationMarker.Activate(show);
		destinationMarker.SetDestination(position, false);
		closeLine.SetPosition(0, position);
		closeLine.SetPosition(1, position);
		farLine.SetPosition(0, position);
		farLine.SetPosition(1, position);

	}

	public void CreateLineToTarget(Vector3 targetPos, Vector3 startPoint, 
		float maxMove,bool isTargetValidMove)
	{
		float distanceToPoint = (targetPos - startPoint).magnitude;
		if (distanceToPoint > maxMove)
		{
			Vector3 A = startPoint;
			Vector3 B = targetPos;
			Vector3 P = (B - A) * (maxMove / distanceToPoint) + A;
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

		destinationMarker.SetDestination(targetPos, isTargetValidMove);
	}

	public void UpdatePosition(Vector3 position)
	{
		closeLine.SetPosition(0, position);
	}
}
