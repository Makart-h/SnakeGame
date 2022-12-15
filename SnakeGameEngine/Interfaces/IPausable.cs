namespace SnakeGameEngine.Interfaces;

internal interface IPausable
{
    public bool IsPaused { get; }
    public void Pause();
    public void Resume();
}
