using System.Numerics;

namespace SnakeGameEngine.Snake;

internal sealed class SnakeController : ISnakeController
{
    private readonly IGame _game;
    private Direction _direction;

    /// <summary>
    /// Before assigning the value, checks if the direction is
    /// not opposite to the current direction, to make sure
    /// that the snake will never move inside itself.
    /// </summary>
    public Direction Direction
    {
        get => _direction;
        set
        {
            if (value == Direction.Left && _direction != Direction.Right
                || value == Direction.Right && _direction != Direction.Left
                || value == Direction.Up && _direction != Direction.Down
                || value == Direction.Down && _direction != Direction.Up)
            {
                _direction = value;
            }
        }
    }

    public SnakeController(IGame game, Direction initialDirection)
    {
        _game = game;
        _direction = initialDirection;
    }

    public (int width, int height) GetSnakeFragmentSize() => (_game.WorldInfo.CellWidth, _game.WorldInfo.CellHeight);

    public void RegisterSnakeFragment(ISnakeFragment fragment) => _game.AddEntity(fragment);

    /// <summary>
    /// Finds the next position for the given fragment.
    /// If the fragment is the head of the snake then
    /// it's position is moved by it's size in the current direction.
    /// If the fragment is any other part of the snake
    /// then the next position is the position of the
    /// next fragment.
    /// </summary>
    /// <param name="fragment"></param>
    /// <returns>Next position for the snake fragment.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public Vector2 FindSnakeFragmentNextPosition(ISnakeFragment fragment)
    {
        if (fragment == null)
        {
            throw new ArgumentNullException(nameof(fragment), "Snake fragment must exist to have the next position!");
        }
        else if (fragment.NextFragment != null)
        {
            return fragment.NextFragment.Position;
        }
        else
        {
            Vector2 localNextPosition = GetLocalNextPosition(fragment);
            return GetWorldPosition(localNextPosition);
        }
    }

    /// <summary>
    /// Moves the fragment by it's width/height towards the current direction.
    /// </summary>
    /// <param name="fragment"></param>
    /// <returns>Position that is a raw increment, without considering the world bounds.</returns>
    /// <exception cref="NotImplementedException"></exception>
    private Vector2 GetLocalNextPosition(ISnakeFragment fragment)
    {
        return Direction switch
        {
            Direction.Left => new Vector2(fragment.Position.X - fragment.Width, fragment.Position.Y),
            Direction.Right => new Vector2(fragment.Position.X + fragment.Width, fragment.Position.Y),
            Direction.Up => new Vector2(fragment.Position.X, fragment.Position.Y - fragment.Height),
            Direction.Down => new Vector2(fragment.Position.X, fragment.Position.Y + fragment.Height),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Checks the local position against the world size
    /// and adjusts it so it is always inside the bounds.
    /// E.g. Snake fragment going of the screen on the right side
    /// will reappear on the left.
    /// </summary>
    /// <param name="localPosition"></param>
    /// <returns>Position that is inside the world bounds.</returns>
    private Vector2 GetWorldPosition(Vector2 localPosition)
    {
        Vector2 worldPosition = localPosition;

        if (worldPosition.X >= _game.WorldInfo.WorldWidth)
            worldPosition.X %= _game.WorldInfo.WorldWidth;
        else if (worldPosition.X < 0)
            worldPosition.X = _game.WorldInfo.WorldWidth - Math.Abs(worldPosition.X % _game.WorldInfo.WorldWidth);
        if (worldPosition.Y >= _game.WorldInfo.WorldHeight)
            worldPosition.Y %= _game.WorldInfo.WorldHeight;
        else if (worldPosition.Y < 0)
            worldPosition.Y = _game.WorldInfo.WorldHeight - Math.Abs(worldPosition.Y % _game.WorldInfo.WorldHeight);

        return worldPosition;
    }
}
