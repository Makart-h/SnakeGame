namespace SnakeGameEngine.Utility;

internal sealed class WorldInfo : IWorldInfo
{
    public int WorldWidth { get; private set; }

    public int WorldHeight { get; private set; }

    public int CellWidth { get; private set; }

    public int CellHeight { get; private set; }

    public WorldInfo(int worldWidth, int worldHeight, int cellWidth, int cellHeight)
    {
        WorldWidth = worldWidth;
        WorldHeight = worldHeight;
        CellWidth = cellWidth;
        CellHeight = cellHeight;
    }
}
