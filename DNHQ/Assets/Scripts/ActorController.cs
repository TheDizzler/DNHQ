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
		NotMyTurn = -1,
		WaitingForCommand, MoveCommand, AttackCommand,
		Moving, EndingTurn,
	}

	public float maxMove = 5;
	[HideInInspector]
	public float moveRemaining;

	private Camera mainCamera = null;
	private TurnManager turnManager = null;
	[SerializeField] private Renderer meshRenderer = null;
	private TargetMarkerController targetMarker = null;
	private MoveHelperController moveHelper = null;

	private Vector3 height;
	private HeroTurnState turnState = HeroTurnState.NotMyTurn;

	public List<Command> commands = new List<Command>();
	private Command moveCommand;


	public void Awake()
	{
		mainCamera = FindObjectOfType<Camera>();
		turnManager = FindObjectOfType<TurnManager>();
		turnManager.Register(this);
		moveHelper = FindObjectOfType<MoveHelperController>();
		targetMarker = FindObjectOfType<TargetMarkerController>();
		
		moveCommand = new MoveCommand(this);
		commands.Add(moveCommand);
		commands.Add(new AttackCommand(this));

		height = new Vector3(0, transform.position.y, 0);
	}

	public void OnDestroy()
	{
		SetSelected(false);
	}

	void Update()
	{
		switch (turnState)
		{
			case HeroTurnState.AttackCommand:
				if (Input.GetMouseButtonDown(1))
				{
					CancelCommand();
				}
				break;
			case HeroTurnState.MoveCommand:
				RaycastHit hit;
				Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

				if (Input.GetMouseButtonDown(1))
				{
					CancelCommand();
				}
				else if (Physics.Raycast(ray, out hit))
				{
					Transform objectFound = hit.transform;
					if (objectFound.CompareTag("Ground"))
					{
						moveHelper.CreateLineToTarget(hit.point,
							transform.position - height, moveRemaining, true);

						if (Input.GetMouseButtonDown(0))
						{
							if (moveRemaining > 0)
							{
								// move
								StartCoroutine(MoveTo(hit.point));
							}
						}
					}
					else
					{
						moveHelper.CreateLineToTarget(hit.point,
							transform.position - height, moveRemaining, false);
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

	public void TakeTurn()
	{
		SetSelected(true);
		turnState = HeroTurnState.WaitingForCommand;
		moveRemaining = maxMove;

		turnManager.EnablePlayerInput();
		turnManager.UpdateCurrentActorHUD();
	}

	public void AttackAction()
	{
		if (turnState != HeroTurnState.WaitingForCommand)
		{
			CancelCommand();
		}

		turnState = HeroTurnState.AttackCommand;

		targetMarker.Activate(true);
		targetMarker.SetPosition(transform.position - height);
	}

	public void MoveAction()
	{
		if (turnState != HeroTurnState.WaitingForCommand)
		{
			CancelCommand();
		}

		turnState = HeroTurnState.MoveCommand;
		moveHelper.Activate(true, transform.position - height);
	}

	public void FinishTurn()
	{
		if (turnState != HeroTurnState.WaitingForCommand)
		{
			CancelCommand();
		}

		SetSelected(false);
		turnState = HeroTurnState.NotMyTurn;

		moveHelper.Activate(false, Vector3.zero);

		turnManager.DisablePlayerInput();
	}


	private IEnumerator MoveTo(Vector3 point)
	{
		turnManager.DisablePlayerInput();
		turnState = HeroTurnState.Moving;

		float t = 0;
		Vector3 startpos = transform.localPosition;
		Vector3 endpoint;
		float distanceToPoint = (point - startpos).magnitude;
		if (distanceToPoint > moveRemaining)
		{
			Vector3 A = startpos;
			Vector3 B = point;
			endpoint = (B - A) * (moveRemaining / distanceToPoint) + A;
		}
		else
		{
			endpoint = point;
		}

		endpoint.y = startpos.y;

		while (t < 1)
		{
			transform.localPosition = Vector3.MoveTowards(startpos, endpoint, t * maxMove);
			moveHelper.UpdatePosition(transform.position - height);
			t += Time.deltaTime;
			yield return null;
		}

		moveRemaining -= distanceToPoint > moveRemaining ? moveRemaining : distanceToPoint;
		if (moveRemaining <= 0)
		{
			turnManager.DisableCommand(moveCommand);
		}

		transform.localPosition = endpoint;

		CancelCommand();
	}

	private void CancelCommand()
	{
		moveHelper.Activate(false, Vector3.zero);
		targetMarker.Activate(false);

		turnState = HeroTurnState.WaitingForCommand;

		turnManager.EnablePlayerInput();
		turnManager.UpdateCurrentActorHUD();
	}

	private void SetSelected(bool selected)
	{
		if (selected)
		{
			meshRenderer.sharedMaterial.color = Color.red;
		}
		else
		{
			meshRenderer.sharedMaterial.color = Color.white;
		}
	}
}