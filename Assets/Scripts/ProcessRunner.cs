using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProjectInfos;

/// <summary>Runs the main process of the project</summary>
class ProcessRunner : MonoBehaviour
{
	[Header("Assign in Inspector")]
	public ProcessInterface processUI;

	ProjectScanner scanner;
	ProjectCleaner cleaner;

	void Awake()
	{
		scanner = new ProjectScanner();
		cleaner = new ProjectCleaner();

		processUI.SetLoadProjectsAction(folderPath => StartCoroutine(ScanProjects(folderPath)));
		processUI.SetCleanProjectsAction(projectsInfos => StartCoroutine(CleanProjects(projectsInfos)));
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}

	IEnumerator ScanProjects(string folderPath)
	{
		yield return scanner.FindProjectsPaths(folderPath, (result) =>
		{
			ProjectInfos info = new ProjectInfos(result);
			processUI.UpdateInfo(info);
		});

		processUI.SetCanInterract(true);
	}

	IEnumerator CleanProjects(List<ProjectInfos> infos)
	{
		cleaner.ResetCleaner();

		foreach (ProjectInfos project in infos)
		{
			yield return cleaner.CleanProject(project.projectPath, (result) =>
			{
				if (result)
					project.state = State.DONE;
				else
					project.state = State.CANCELED;
			});

			processUI.UpdateInfo(project);
			yield return null;
		}

		processUI.SetOutputInfos(cleaner.GetDeleteCount(), cleaner.GetDeleteSize());
		processUI.SetCanInterract(true);
	}
}