using System;
using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>Cleans Unity projects of useless folders</summary>
public class ProjectCleaner
{
	long deleteSize;
	int deleteCount;
	bool hadError;

	public void ResetCleaner()
	{
		deleteSize = 0;
		deleteCount = 0;
	}

	public IEnumerator CleanProject(string root, Action<bool> SendResult)
	{
		hadError = false;
		string[] foldersPaths = Directory.GetDirectories(root);
		string[] filesPaths = Directory.GetFiles(root);

		foreach (string folderPath in foldersPaths)
		{
			string folderName = Folder.GetName(folderPath);

			if (ShouldBeDeleted(folderName))
			{
				long folderSize = Folder.GetFolderSize(folderPath);

				yield return DeleteDirectoryRecursive(folderPath);

				if (!hadError)
				{
					deleteSize += folderSize;
					deleteCount++;
				}
				else
					break;
			}

			yield return null;
		}

		foreach (string filePath in filesPaths)
		{
			FileInfo fileInfo = new FileInfo(filePath);

			if (ShouldBeDeleted(fileInfo.Name))
			{
				long fileSize = fileInfo.Length;
				DeleteFile(filePath);

				if (!hadError)
					deleteSize += fileSize;
				else
					break;
			}

			yield return null;
		}

		SendResult(!hadError);
	}

	IEnumerator DeleteDirectoryRecursive(string root)
	{
		string[] filesPaths = Directory.GetFiles(root);
		string[] foldersPaths = Directory.GetDirectories(root);

		foreach (string filePath in filesPaths)
		{
			try
			{
				File.SetAttributes(filePath, FileAttributes.Normal);
				File.Delete(filePath);
			}
			catch (Exception e)
			{
				Debug.Log(e);
				hadError = true;
				yield break;
			}

			yield return null;
		}

		// so that it doesn't lag when exploring empty folders
		yield return null;

		foreach (string folderPath in foldersPaths)
			yield return DeleteDirectoryRecursive(folderPath);

		try
		{
			Directory.Delete(root);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			hadError = true;
		}
	}

	void DeleteFile(string path)
	{
		try
		{
			File.SetAttributes(path, FileAttributes.Normal);
			File.Delete(path);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			hadError = true;
		}
	}

	bool ShouldBeDeleted(string path)
	{
		string[] pathsToCheck = new string[]
		{
			UnityProjectPaths.X_LIBRARY_FOLDER_NAME,
			UnityProjectPaths.X_TEMP_FOLDER_NAME,
			UnityProjectPaths.X_DEBUG_FOLDER_NAME,
			UnityProjectPaths.X_LOGS_FOLDER_NAME,
			UnityProjectPaths.X_OBJ_FOLDER_NAME,
			UnityProjectPaths.X_GENERATED_FOLDER_NAME,
			UnityProjectPaths.X_EXPORTED_FOLDER_NAME,
			UnityProjectPaths.X_CRASH_REPORT_FILE_NAME,

			Folder.ToLowerCase(UnityProjectPaths.X_LIBRARY_FOLDER_NAME),
			Folder.ToLowerCase(UnityProjectPaths.X_TEMP_FOLDER_NAME),
			Folder.ToLowerCase(UnityProjectPaths.X_DEBUG_FOLDER_NAME),
			Folder.ToLowerCase(UnityProjectPaths.X_LOGS_FOLDER_NAME),
			Folder.ToLowerCase(UnityProjectPaths.X_OBJ_FOLDER_NAME),
			Folder.ToLowerCase(UnityProjectPaths.X_GENERATED_FOLDER_NAME),
			Folder.ToLowerCase(UnityProjectPaths.X_EXPORTED_FOLDER_NAME)
		};

		foreach (string pathCheck in pathsToCheck)
		{
			if (path == pathCheck)
				return true;
		}

		return false;
	}

	public int GetDeleteCount()
	{
		return deleteCount;
	}

	public long GetDeleteSize()
	{
		return deleteSize;
	}
}