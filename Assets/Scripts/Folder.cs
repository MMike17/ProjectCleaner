using System;
using System.IO;
using UnityEngine;

/// <summary>Utility class for Directories (folders)</summary>
public static class Folder
{
	public static string GetName(string path)
	{
		return new DirectoryInfo(path).Name;
	}

	public static long GetFolderSize(string path)
	{
		long folderSize = 0;

		try
		{
			foreach (string file in Directory.GetFiles(path))
				folderSize += new FileInfo(file).Length;

			foreach (string folder in Directory.GetDirectories(path))
				folderSize += GetFolderSize(folder);
		}
		catch (Exception exception)
		{
			Debug.Log(exception);
		}

		return folderSize;
	}

	public static string ToLowerCase(string name)
	{
		return name.Substring(0, 1).ToLower() + name.Substring(1, name.Length - 1);
	}
}