using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
	public ActorController attacker, defender;

	public enum CombatState
	{
		DefenderChoosing, ResolvingCombat, Done
	}

	public CombatState combatState;
	public float MinToHitChance = .5f;
	public float MaxToHitChance = .95f;

	[SerializeField] private Text dialogText = null;
	[SerializeField] private GameObject defendCommandPanel = null;
	[SerializeField] private DefendCommandListController defendList = null;


	public void CommenceAttackSequence(ActorController atkr, ActorController dfndr)
	{
		attacker = atkr;
		defender = dfndr;
		dialogText.text += "\n" + atkr.name + " attacks " + defender.name + "!";

		List<DefendCommand> commands = defender.GetDefenceOptions();
		float rate = .4f;
		foreach (DefendCommand command in commands)
		{
			command.SetCallback(this);
			// calculate odds of success here
			command.SetSuccessRate(rate += .1f);
		}

		defendList.SetCommands(commands);
		combatState = CombatState.DefenderChoosing;

		defendCommandPanel.SetActive(true);
	}

	public void OptionSelected(DefendCommand defendCommand)
	{
		combatState = CombatState.ResolvingCombat;
		defendCommandPanel.SetActive(false);
		dialogText.text += "\n" + defender.name + " tries to " + defendCommand.Name() + "!";

		// do calculations
		float baseToHit = .75f;
		float baseToDodge = defendCommand.SuccessRate;
		Debug.Log(baseToHit - baseToDodge);
		float toHitAdjusted = Mathf.Clamp(baseToHit - baseToDodge, MinToHitChance, MaxToHitChance);
		dialogText.text += "\n Total chance to hit is " + toHitAdjusted;
		float rand = Random.Range(0f, 1);
		dialogText.text += "\n " + attacker.name + " rolled a " + rand;
		if (rand <= toHitAdjusted)
		{
			dialogText.text += "\n " + attacker.name + " Hit!";
		}
		else
		{
			dialogText.text += "\n " + defender.name + " " + defendCommand.Verbed() + " the attack!";
		}
		// resolve counter attacks

		// calculate damage, if any

		// resolve final results (forced movements, saving throws, status inflictions, etc)

		// continue with turn
		attacker.CombatResolved();
		combatState = CombatState.Done;
	}
}