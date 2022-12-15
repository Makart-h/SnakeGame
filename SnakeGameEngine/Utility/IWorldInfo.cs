namespace SnakeGameEngine.Utility;

internal interface IWorldInfo
{
    public int WorldWidth { get; }
    public int WorldHeight { get; }
    public int CellWidth { get; }
    public int CellHeight { get; }
}
