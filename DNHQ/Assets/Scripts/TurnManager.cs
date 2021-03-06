﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
	[SerializeField] private Text nameText = null;
	[SerializeField] private Text moveText = null;
	[SerializeField] private CommandListController commandList = null;
	[SerializeField] private GameObject commandPanel = null;
	[SerializeField] private Canvas canvas = null;

	[SerializeField] private CombatManager combatManager = null;

	private List<ActorController> actors = new List<ActorController>();
	private int currentActorIndex = -1;
	private ActorController currentActor;


	public void Start()
	{
		canvas.gameObject.SetActive(true);
		if (actors.Count == 0)
		{
			Debug.Log("There's no one here :(");
		}
		else
		{
			GetNextActor();
		}
	}


	/// <summary>
	/// Place this Actor in the turn queue.
	/// </summary>
	/// <param name="actorController"></param>
	public void Register(ActorController actorController)
	{
		actors.Add(actorController);
	}


	public void TurnDone()
	{
		actors[currentActorIndex].FinishTurn();
		GetNextActor();
	}

	private void GetNextActor()
	{
		if (++currentActorIndex >= actors.Count)
		{
			currentActorIndex = 0;
		}

		currentActor = actors[currentActorIndex];
		commandList.SetCommands(currentActor.commands);
		currentActor.TakeTurn();
	}


	public void UpdateCurrentActorHUD()
	{
		nameText.text = currentActor.name;
		moveText.text = currentActor.moveRemaining + " / " + currentActor.maxMove;
	}

	public void DisablePlayerInput()
	{
		commandPanel.SetActive(false);
	}

	public void EnablePlayerInput()
	{
		commandPanel.SetActive(true);
	}

	public void DisableCommand(ICommand command)
	{
		commandList.Disable(command);
	}

	public void CommenceAttackSequence(ActorController attacker, ActorController defender)
	{
		commandPanel.SetActive(false);
		combatManager.CommenceAttackSequence(attacker, defender);
	}

	public void CombatResolved()
	{
		commandPanel.SetActive(true);
	}
}