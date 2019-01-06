public interface Command
{
	string Name();
	void Act();
}

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

public class AttackCommand : Command
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
