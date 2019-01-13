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
	private TargetArrowController targetArrow = null;

	private Vector3 height;
	private HeroTurnState turnState = HeroTurnState.NotMyTurn;

	public List<Command> commands = new List<Command>();
	private Command moveCommand;

	private List<GameObject> targets = new List<GameObject>();
	private RaycastHit hit;
	private Ray ray;
	private GameObject currentTarget;


	public void Awake()
	{
		mainCamera = FindObjectOfType<Camera>();
		turnManager = FindObjectOfType<TurnManager>();
		turnManager.Register(this);
		moveHelper = FindObjectOfType<MoveHelperController>();
		targetMarker = FindObjectOfType<TargetMarkerController>();
		targetArrow = FindObjectOfType<TargetArrowController>();

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
					break;
				}

				ray = mainCamera.ScreenPointToRay(Input.mousePosition);

				RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

				for (int i = 0; i < hits.Length; i++)
				{
					Transform objectFound = hits[i].transform;
					if (targets.Contains(objectFound.gameObject))
					{
						if (objectFound.gameObject != currentTarget)
						{
							currentTarget = objectFound.gameObject;
							Debug.Log("I found " + objectFound.name);
							targetArrow.Show(currentTarget);
						}
					}
				}

				break;
			case HeroTurnState.MoveCommand:
				ray = mainCamera.ScreenPointToRay(Input.mousePosition);

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

		// find targets in hit area
		CapsuleCollider capsule = targetMarker.GetComponent<CapsuleCollider>();
		Collider[] colliders = OverlapCollider(capsule);
		foreach (Collider coll in colliders)
		{
			if (coll.gameObject.layer == Layers.Actors && coll.gameObject != this.gameObject)
			{
				coll.gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Color.blue;
				targets.Add(coll.gameObject);
			}
		}
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
		if (targets.Count != 0)
		{
			foreach (GameObject target in targets)
			{
				target.GetComponent<MeshRenderer>().sharedMaterial.color = Color.white;
			}

			targets.Clear();
			targetArrow.Hide();
			currentTarget = null;
		}


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


	private Collider[] OverlapCollider(CapsuleCollider capsule,
		int layerMask = Physics.DefaultRaycastLayers,
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		var center = capsule.transform.TransformPoint(capsule.center);
		float radius = 0f;
		float height = 0f;
		Vector3 lossyScale = AbsVec3(capsule.transform.lossyScale);
		Vector3 dir = Vector3.zero;

		switch (capsule.direction)
		{
			case 0: // x
				radius = Mathf.Max(lossyScale.y, lossyScale.z) * capsule.radius;
				height = lossyScale.x * capsule.height;
				dir = capsule.transform.TransformDirection(Vector3.right);
				break;
			case 1: // y
				radius = Mathf.Max(lossyScale.x, lossyScale.z) * capsule.radius;
				height = lossyScale.y * capsule.height;
				dir = capsule.transform.TransformDirection(Vector3.up);
				break;
			case 2: // z
				radius = Mathf.Max(lossyScale.x, lossyScale.y) * capsule.radius;
				height = lossyScale.z * capsule.height;
				dir = capsule.transform.TransformDirection(Vector3.forward);
				break;
		}

		if (height < radius * 2f)
		{
			dir = Vector3.zero;
		}

		Vector3 point0 = center + dir * (height * 0.5f - radius);
		Vector3 point1 = center - dir * (height * 0.5f - radius);

		return Physics.OverlapCapsule(point0, point1, radius, layerMask, queryTriggerInteraction);
	}

	private static Vector3 AbsVec3(Vector3 v)
	{
		return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
	}
}