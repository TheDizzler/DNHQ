using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Every object that gets a turn must have this.
/// </summary>
public class ActorController : MonoBehaviour
{
	private enum HeroTurnState
	{
		NotMyTurn = -1, WaitingForCommand, MoveCommand, AttackCommand, EndingTurn
	}

	public new Camera camera;
	public TurnManager turnManager;
	public bool myTurn = false;

	public float maxMove = 5;
	[HideInInspector]
	public float moveLeft;

	[SerializeField] private Material material = null;
	public LineRenderer closeLine;
	public LineRenderer farLine;
	public GameObject destinationMarker;

	private HeroTurnState turnState = HeroTurnState.NotMyTurn;


	public List<Command> commands = new List<Command>();


	public class MoveCommand : Command
	{
		const string name = "Move";
		ActorController actor;
		public MoveCommand(ActorController actr)
		{
			actor = actr;
		}

		public string Name()
		{
			return name;
		}

		public void Act()
		{
			actor.MoveAction();
		}
	}


	public void Awake()
	{
		turnManager.Register(this);
		commands.Add(new MoveCommand(this));
	}

	public void OnDestroy()
	{
		SetSelected(false);
	}

	void Update()
	{
		switch (turnState)
		{
			case HeroTurnState.MoveCommand:
				RaycastHit hit;
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit))
				{
					Transform objectFound = hit.transform;
					if (objectFound.CompareTag("Ground"))
					{
						CreateLineToTarget(hit.point, transform.position, moveLeft);
						destinationMarker.transform.localPosition = hit.point;

						if (Input.GetMouseButtonDown(0))
						{
							if (moveLeft > 0)
							{
								// move
								StartCoroutine(MoveTo(hit.point));
							}
						}
					}
				}

				break;
			case HeroTurnState.NotMyTurn:
			default:
				break;
		}
		//if (Input.GetMouseButtonDown(0))
		//{
		//	RaycastHit hit;
		//	Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		//	if (Physics.Raycast(ray, out hit))
		//	{
		//		Transform objectHit = hit.transform;
		//		if (objectHit == transform)
		//		{
		//			Debug.Log("That's me!");
		//		}
		//		else if (objectHit.CompareTag("Ground") && !moving && moveLeft > 0)
		//		{
		//			// move
		//			StartCoroutine(MoveTo(hit.point));
		//		}
		//		else
		//		{
		//			Debug.Log("That's " + objectHit.name + " *spit*");
		//		}
		//	}
		//}
		//else if (!moving)
		//{
		//	RaycastHit hit;
		//	Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		//	if (Physics.Raycast(ray, out hit))
		//	{
		//		Transform objectFound = hit.transform;
		//		if (objectFound.CompareTag("Ground"))
		//		{
		//			CreateLineToTarget(hit.point, transform.position, moveLeft);
		//			destinationMarker.transform.localPosition = hit.point;

		//		}
		//	}
		//}
	}


	public void MoveAction()
	{
		turnState = HeroTurnState.MoveCommand;
		closeLine.enabled = true;
		farLine.enabled = true;
		destinationMarker.SetActive(true);
	}

	private IEnumerator MoveTo(Vector3 point)
	{
		turnManager.DisablePlayerInput();

		float t = 0;
		Vector3 startpos = transform.localPosition;
		Vector3 endpoint;
		float distanceToPoint = (point - startpos).magnitude;
		if (distanceToPoint > moveLeft)
		{
			Vector3 A = startpos;
			Vector3 B = point;
			endpoint = (B - A) * (moveLeft / distanceToPoint) + A;
		}
		else
		{
			endpoint = point;
		}

		endpoint.y = startpos.y;

		while (t < 1)
		{
			transform.localPosition = Vector3.MoveTowards(startpos, endpoint, t * maxMove);
			closeLine.SetPosition(0, transform.position);
			t += Time.deltaTime;
			yield return null;
		}


		moveLeft -= distanceToPoint > moveLeft ? moveLeft : distanceToPoint;
		transform.localPosition = endpoint;
		closeLine.SetPosition(0, transform.position);
		closeLine.enabled = false;
		farLine.enabled = false;
		destinationMarker.SetActive(false);

		turnState = HeroTurnState.WaitingForCommand;

		turnManager.EnablePlayerInput();
		turnManager.UpdateCurrentActorHUD();
	}

	public void TakeTurn()
	{
		SetSelected(true);
		turnState = HeroTurnState.WaitingForCommand;
		moveLeft = maxMove;

		closeLine.SetPosition(0, transform.position);

		turnManager.EnablePlayerInput();
		turnManager.UpdateCurrentActorHUD();
	}

	public void FinishTurn()
	{
		SetSelected(false);
		turnState = HeroTurnState.NotMyTurn;
		
		closeLine.enabled = false;
		farLine.enabled = false;
		destinationMarker.SetActive(false);

		turnManager.DisablePlayerInput();
	}


	private void SetSelected(bool selected)
	{
		myTurn = selected;
		if (myTurn)
		{
			material.color = Color.red;
		}
		else
		{
			material.color = Color.white;
		}
	}


	public void CreateLineToTarget(Vector3 targetPos, Vector3 startPoint, float maxMove)
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
	}
}
