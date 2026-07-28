namespace CmdSim.Engine.State;

public interface IBackgroundIndexer
{
    void StartIndexing();
    bool IsIndexReady();
}

public class DummyFilesystemIndexer : IBackgroundIndexer
{
    public void StartIndexing()
    {
        // Simulation: spawn background thread to traverse local C: drive map
    }

    public bool IsIndexReady() => true; 
}
