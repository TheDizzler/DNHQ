using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefendCommandListController : MonoBehaviour
{
	[SerializeField] private GameObject defendListPrefab = null;
	[SerializeField] private List<GameObject> commandObjects = new List<GameObject>();


	public void SetCommands(List<DefendCommand> commands)
	{
		for (int i = commandObjects.Count - 1; i >= 0; --i)
		{
			Destroy(commandObjects[i]);
		}

		commandObjects.Clear();

		GameObject clone;
		for (int i = 0; i < commands.Count; ++i)
		{
			clone = Instantiate(defendListPrefab);
			clone.transform.SetParent(transform, false);
			clone.name = commands[i].Name();
			Text[] texts = clone.GetComponentsInChildren<Text>();
			texts[0].text = clone.name;
			texts[1].text = (commands[i].SuccessRate * 100).ToString("00.0") + "%";
			Button button = clone.GetComponent<Button>();
			button.onClick.AddListener(commands[i].Act);
			commandObjects.Add(clone);
		}
	}
}

