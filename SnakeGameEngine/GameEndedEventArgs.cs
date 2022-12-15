namespace SnakeGameEngine;

internal sealed class GameEndedEventArgs : EventArgs
{
    public string Message { get; }
    public int Score { get; }

    public GameEndedEventArgs(string message, int score)
    {
        Message = message;
        Score = score;
    }
}
