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


	[SerializeField] private Text dialogText = null;
	[SerializeField] private GameObject defendCommandPanel = null;
	[SerializeField] private DefendCommandListController defendList = null;


	public void CommenceAttackSequence(ActorController atkr, ActorController dfndr)
	{
		attacker = atkr;
		defender = dfndr;
		dialogText.text += atkr.name + " attacks " + defender.name + "!";

		List<DefendCommand> commands = defender.GetDefenceOptions();
		double rate = .4f;
		foreach (DefendCommand command in commands)
		{
			command.SetCallback(this);
			// calculate odds of success here
			command.SetSuccessRate(rate += .1);
		}

		defendList.SetCommands(commands);
		combatState = CombatState.DefenderChoosing;

		defendCommandPanel.SetActive(true);
	}

	public void OptionSelected(DefendCommand defendCommand)
	{
		combatState = CombatState.ResolvingCombat;
		defendCommandPanel.SetActive(false);
		dialogText.text += defender.name + " tries to " + defendCommand.Name() + "!";

		// do calculations

		// resolve counter attacks

		// calculate damage, if any

		// resolve final results (forced movements, saving throws, status inflictions, etc)

		// continue with turn
		attacker.CombatResolved();
		combatState = CombatState.Done;
	}
}