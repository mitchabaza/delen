namespace Delen.Core.Tasks
{
    public interface IRunnable
    {
        string Runner { get; }
        string Arguments { get; }
        byte[] WorkDirectoryArchive { get; }
        string ArtifactSearchFilter { get; set; }
       
    }
}