using SFB;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectInfos;

/// <summary>Manages all the UI for this project</summary>
public class ProcessInterface : MonoBehaviour
{
	const string OUTPUT_FORMAT = "Deleted {0} folders ({1})";

	[Header("Assign in Inspector")]
	public TMP_InputField rootFolderPath;
	public Button rootFolderPick;
	public Button loadProjectsButton;
	public Toggle allSelectionToggle;
	public Button cleanProjectsButton;
	public TextMeshProUGUI outputDisplay;
	public ScrollRect outputScroll;
	public OutputTicket ticketPrefab;

	List<OutputTicket> tickets;

	void Awake()
	{
		tickets = new List<OutputTicket>();

		rootFolderPath.onValueChanged.AddListener(text => loadProjectsButton.interactable = !string.IsNullOrWhiteSpace(text));

		rootFolderPick.onClick.AddListener(() =>
		{
			string[] paths = StandaloneFileBrowser.OpenFolderPanel("Pick root folder to scan", "", false);

			if (paths.Length > 0)
			{
				string folderPath = paths[0];
				rootFolderPath.text = folderPath;
			}
		});

		allSelectionToggle.onValueChanged.AddListener(state =>
		{
			if (tickets != null)
				tickets.ForEach(ticket => ticket.SetToggleState(state));
		});

		loadProjectsButton.interactable = false;
		cleanProjectsButton.interactable = false;

		outputScroll.normalizedPosition = new Vector2(0, 1);
	}

	public void LoadProjectsInfos(ProjectInfos[] infos)
	{
		// generate new tickets list
		foreach (ProjectInfos info in infos)
			CreateTicket(info);
	}

	void CleanProjectsInfos()
	{
		if (tickets == null)
			tickets = new List<OutputTicket>();
		else
		{
			tickets.ForEach(item => Destroy(item.gameObject));
			tickets.Clear();
		}
	}

	public void UpdateInfo(ProjectInfos info)
	{
		OutputTicket selected = tickets.Find(item => item.IsSameProject(info.projectName));

		if (selected != null)
			selected.UpdateState(info.state);
		else
			CreateTicket(info);
	}

	void CreateTicket(ProjectInfos info)
	{
		OutputTicket ticket = Instantiate(ticketPrefab, outputScroll.content);
		ticket.Init(info);

		tickets.Add(ticket);
	}

	public void SetLoadProjectsAction(Action<string> Process)
	{
		loadProjectsButton.onClick.RemoveAllListeners();
		loadProjectsButton.onClick.AddListener(() =>
		{
			CleanProjectsInfos();
			outputDisplay.text = string.Empty;

			Process(rootFolderPath.text);

			allSelectionToggle.isOn = true;
			SetCanInterract(false);
		});
	}

	public void SetCleanProjectsAction(Action<List<ProjectInfos>> Process)
	{
		cleanProjectsButton.onClick.RemoveAllListeners();
		cleanProjectsButton.onClick.AddListener(() =>
		{
			List<ProjectInfos> projectsInfos = new List<ProjectInfos>();
			tickets.ForEach(ticket =>
			{
				if (ticket.isSelected)
					projectsInfos.Add(ticket.GetProjectInfos());
			});

			tickets.ForEach(ticket =>
			{
				if (ticket.isSelected)
					ticket.UpdateState(State.PENDING);
			});

			Process(projectsInfos);
			SetCanInterract(false);
		});
	}

	public void SetCanInterract(bool state)
	{
		rootFolderPath.interactable = state;
		rootFolderPick.interactable = state;

		loadProjectsButton.interactable = state;
		allSelectionToggle.interactable = state;

		cleanProjectsButton.interactable = state ? tickets != null && tickets.Count > 0 : false;
	}

	public void SetOutputInfos(int foldersCount, float size)
	{
		string suffix = "bytes";
		string sizeString;

		if (size >= 1048576)
		{
			suffix = "MB";
			size /= 1048576;
		}
		else if (size >= 1024)
		{
			suffix = "KB";
			size /= 1024;
		}

		string[] fragments = size.ToString().Split(',');

		if (fragments.Length > 1 && fragments[1].Length > 2)
			sizeString = fragments[0] + "," + fragments[1].Substring(0, 1);
		else
			sizeString = size.ToString();

		string textSize = sizeString + " " + suffix;
		outputDisplay.text = string.Format(OUTPUT_FORMAT, foldersCount, textSize);
	}
}