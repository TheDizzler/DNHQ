public interface ICommand
{
	string Name();
	void Act();
}

public class MoveCommand : ICommand
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

public class AttackCommand : ICommand
{
	const string name = "Attack";
	ActorController actor;

	public AttackCommand(ActorController actr)
	{
		actor = actr;
	}

	public void Act()
	{
		actor.AttackAction();
	}

	public string Name()
	{
		return name;
	}
}

public class DefendCommand : ICommand
{
	string name = "DefendType";
	public ActorController actor;
	public double SuccessRate;
	private CombatManager combatManager;


	public DefendCommand(ActorController actr, string nm)
	{
		actor = actr;
		name = nm;
	}

	public void Act()
	{
		combatManager.OptionSelected(this);
	}

	public string Name()
	{
		return name;
	}

	public void SetCallback(CombatManager cmbtMang)
	{
		combatManager = cmbtMang;
	}

	public void SetSuccessRate(double chance)
	{
		SuccessRate = chance;
	}
}
