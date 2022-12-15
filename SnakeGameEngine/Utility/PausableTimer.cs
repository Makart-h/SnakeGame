using System.Timers;
using SnakeGameEngine.Interfaces;

namespace SnakeGameEngine.Utility;

internal sealed class PausableTimer : IPausable, IDisposable
{
    private readonly System.Timers.Timer _mainTimer;
    private readonly System.Diagnostics.Stopwatch _pauseStopwatch;
    private readonly double _initialInterval;
    private bool _isDisposed;
    public bool IsPaused { get; private set; }

    public PausableTimer(double intervalInMilliseconds, ElapsedEventHandler action, bool autoReset)
    {
        _mainTimer = new System.Timers.Timer
        {
            AutoReset = autoReset,
            Interval = intervalInMilliseconds,
        };
        _initialInterval = intervalInMilliseconds;
        _mainTimer.Elapsed += action;
        if (autoReset)
            _mainTimer.Elapsed += RestoreInitialInterval;
        _pauseStopwatch = new System.Diagnostics.Stopwatch();
    }

    private void RestoreInitialInterval(object? sender, ElapsedEventArgs e)
    {
        _mainTimer.Interval = _initialInterval;
        _pauseStopwatch.Restart();
    }

    public void Start()
    {
        _mainTimer.Start();
        _pauseStopwatch.Start();
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _mainTimer.Dispose();
            _isDisposed = true;
        }
    }

    public void Pause()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            _pauseStopwatch.Stop();
            _mainTimer.Stop();
        }
    }

    public void Resume()
    {
        if (IsPaused)
        {
            IsPaused = false;
            double newInterval = _mainTimer.Interval - _pauseStopwatch.ElapsedMilliseconds;
            _mainTimer.Interval = newInterval > 0 ? newInterval : 0.1;
            _mainTimer.Start();
            _pauseStopwatch.Restart();
        }
    }
}
