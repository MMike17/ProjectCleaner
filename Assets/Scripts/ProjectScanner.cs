using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>Scans for Unity projects</summary>
public class ProjectScanner
{
	Action<string> SendResult;

	public IEnumerator FindProjectsPaths(string rootPath, Action<string> sendResult)
	{
		SendResult = sendResult;

		if (Directory.Exists(rootPath))
			yield return FindProjectsPathsRecursive(rootPath);
	}

	IEnumerator FindProjectsPathsRecursive(string root)
	{
		List<string> subFolders = new List<string>(Directory.GetDirectories(root));

		if (subFolders.Count > 0)
		{
			string settingsFolder = subFolders.Find(item => Folder.GetName(item) == UnityProjectPaths.V_SETTINGS_FOLDER_NAME);
			string assetsFolder = subFolders.Find(item => Folder.GetName(item) == UnityProjectPaths.V_ASSETS_FOLDER_NAME);
			bool versionFilePresent = false;

			if (settingsFolder != null)
				versionFilePresent = new FileInfo(Path.Combine(settingsFolder, UnityProjectPaths.V_VERSION_FILE_NAME)).Exists;

			if (settingsFolder != null && assetsFolder != null && versionFilePresent)
				SendResult(root);
			else
			{
				foreach (string subFolder in subFolders)
					yield return FindProjectsPathsRecursive(subFolder);
			}
		}
	}
}