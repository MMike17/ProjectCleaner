/// <summary>Opens window's file explorer</summary>
public static class WindowsExplorer
{
	public static void ShowExplorer(string itemPath)
	{
		itemPath = itemPath.Replace(@"/", @"\"); // explorer doesn't like front slashes
		System.Diagnostics.Process.Start("explorer.exe", "/root," + itemPath);
	}
}