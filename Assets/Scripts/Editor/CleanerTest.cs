using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>Generates test folders to test project</summary>
class CleanerTest : EditorWindow
{
	const string CONTAINER_FOLDER_NAME = "Tests";
	const string ROOT_FOLDER_NAME = "CleanTest";

	const string FILE_NAME = "TestFile.txt";

	string desktopUserPath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
	int amount;
	int fileAmount;
	bool withProjectVersionFile;

	[MenuItem("Tools/CleanerTest")]
	static void ShowWindow()
	{
		var window = GetWindow<CleanerTest>();
		window.titleContent = new GUIContent("CleanerTest");
		window.Show();
	}

	void OnGUI()
	{
		amount = EditorGUILayout.IntField("Folders count", amount);
		fileAmount = EditorGUILayout.IntField("Files per project", fileAmount);
		withProjectVersionFile = EditorGUILayout.Toggle("With ProjectVersion file", withProjectVersionFile);

		EditorGUILayout.Space();

		if (GUILayout.Button("Generate test folders"))
			GenerateTestFolder(withProjectVersionFile);
	}

	void GenerateTestFolder(bool withProjectVersionFile)
	{
		string rootPath = Path.Combine(desktopUserPath, CONTAINER_FOLDER_NAME);
		Directory.CreateDirectory(rootPath);

		long totalGarbageSize = 0;

		for (int i = 0; i < amount; i++)
		{
			rootPath = Path.Combine(desktopUserPath, CONTAINER_FOLDER_NAME, ROOT_FOLDER_NAME + i);
			Directory.CreateDirectory(rootPath);

			// generate files and folders that should be kept
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.V_ASSETS_FOLDER_NAME));
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.V_SETTINGS_FOLDER_NAME));

			FileStream stream = File.Create(Path.Combine(rootPath, UnityProjectPaths.V_SETTINGS_FOLDER_NAME, UnityProjectPaths.V_VERSION_FILE_NAME));
			stream.Close();
			stream.Dispose();

			// generate files and folders that should be deleted
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.X_LIBRARY_FOLDER_NAME));
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.X_TEMP_FOLDER_NAME));
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.X_DEBUG_FOLDER_NAME));
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.X_LOGS_FOLDER_NAME));
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.X_OBJ_FOLDER_NAME));
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.X_GENERATED_FOLDER_NAME));
			Directory.CreateDirectory(Path.Combine(rootPath, UnityProjectPaths.X_EXPORTED_FOLDER_NAME));

			stream = File.Create(Path.Combine(rootPath, UnityProjectPaths.X_CRASH_REPORT_FILE_NAME));
			stream.Close();
			stream.Dispose();

			string content = string.Empty;
			for (int j = 0; j < fileAmount; j++)
				content += "This is a test file made to have a random weight\n";

			File.WriteAllText(Path.Combine(rootPath, UnityProjectPaths.X_LIBRARY_FOLDER_NAME, FILE_NAME), content);
			totalGarbageSize += new FileInfo(Path.Combine(rootPath, UnityProjectPaths.X_LIBRARY_FOLDER_NAME, FILE_NAME)).Length;
		}

		Debug.Log("Total garbage size : " + totalGarbageSize);
	}
}