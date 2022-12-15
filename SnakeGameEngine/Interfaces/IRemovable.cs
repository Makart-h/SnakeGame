namespace SnakeGameEngine.Interfaces;

internal interface IRemovable
{
    public bool IsRemoved { get; }
    public void Remove();
}
