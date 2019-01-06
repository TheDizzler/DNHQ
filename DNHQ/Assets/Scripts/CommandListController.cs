using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandListController : MonoBehaviour
{
	[SerializeField] private GameObject commandListPrefab = null;
	[SerializeField] private List<GameObject> commandObjects = new List<GameObject>();

	[SerializeField] private GameObject turnDoneButton = null;


	public void SetCommands(List<Command> commands)
	{
		for (int i = commandObjects.Count - 2; i >= 0; --i)
		{ // -2 to prevent from destroying the Universal End Turn command.
			Destroy(commandObjects[i]);
		}

		commandObjects.Clear();

		GameObject clone;
		for (int i = 0; i < commands.Count; ++i)
		{
			clone = Instantiate(commandListPrefab);
			clone.transform.SetParent(transform, false);
			clone.name = commands[i].Name();
			clone.GetComponentInChildren<Text>().text = clone.name;
			Button button = clone.GetComponent<Button>();
			button.onClick.AddListener(commands[i].Act);
			commandObjects.Add(clone);
		}

		commandObjects.Add(turnDoneButton);
		turnDoneButton.transform.SetSiblingIndex(commandObjects.Count + 1);
	}

	public void Disable(Command command)
	{
		foreach (GameObject c in commandObjects)
		{
			if (c.name == command.Name())
			{
				c.GetComponent<Button>().interactable = false;
			}
		}
	}
}