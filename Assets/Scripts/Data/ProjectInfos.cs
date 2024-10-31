/// <summary>Represents the state of a Unity project folder</summary>
public class ProjectInfos
{
	public enum State
	{
		PENDING,
		DONE,
		CANCELED
	}

	public string projectPath;
	public string projectName;
	public State state;

	public ProjectInfos(string path)
	{
		projectPath = path;
		projectName = Folder.GetName(path);
		state = State.PENDING;
	}
}