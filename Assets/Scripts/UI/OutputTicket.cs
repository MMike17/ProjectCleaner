using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectInfos;

/// <summary>Displays informations about project folders</summary>
public class OutputTicket : MonoBehaviour
{
	[Header("Settings")]
	public float pendingRotationSpeed;
	public List<StateSprite> stateSprites;

	[Header("Assign in Inspector")]
	public Toggle projectSelectionToggle;
	public TextMeshProUGUI projectNameDisplay;
	public Image stateDisplay;
	public Button openFolderButton;

	public bool isSelected => projectSelectionToggle.isOn;

	bool init = false;
	ProjectInfos projectInfos;

	void Update()
	{
		if (!init)
			return;

		if (projectInfos.state == State.PENDING)
			stateDisplay.transform.Rotate(0, 0, pendingRotationSpeed * Time.deltaTime);
		else
			stateDisplay.transform.eulerAngles = Vector3.zero;
	}

	public void Init(ProjectInfos projectInfos)
	{
		init = true;

		this.projectInfos = projectInfos;
		projectNameDisplay.text = projectInfos.projectName;

		projectSelectionToggle.isOn = true;
		stateDisplay.gameObject.SetActive(false);

		openFolderButton.onClick.RemoveAllListeners();
		openFolderButton.onClick.AddListener(() => WindowsExplorer.ShowExplorer(projectInfos.projectPath));
	}

	public void UpdateState(State state)
	{
		StateSprite selected = stateSprites.Find(item => item.state == state);

		if (selected != null)
		{
			selected.ConfigureSprite(stateDisplay);
			stateDisplay.gameObject.SetActive(true);
		}

		projectInfos.state = state;
	}

	public bool IsSameProject(string projectName) => projectNameDisplay.text == projectName;

	public ProjectInfos GetProjectInfos() => projectInfos;

	public void SetToggleState(bool state) => projectSelectionToggle.isOn = state;

	[Serializable]
	public class StateSprite
	{
		public State state;
		public Sprite sprite;
		public Color color;

		public void ConfigureSprite(Image image)
		{
			image.sprite = sprite;
			image.color = color;
		}
	}
}