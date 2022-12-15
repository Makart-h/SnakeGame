using SnakeGameEngine;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Numerics;
using System.Collections.Generic;
using SnakeGameEngine.Entities;
using SnakeGameEngine.Snake;
using SnakeGameEngine.Utility;
using SnakeGameEngine.Services;
using System.Media;

namespace SnakeGameGUI;

internal sealed class WPFSnakeGame : IWPFSnakeGame
{  
    private IGame _game;
    private ISnakeController _snakeController;
    private readonly IWorldInfo _worldInfo;
    private readonly SoundPlayer _soundPlayer;
    private bool _directionUpdatedThisFrame;
    private Direction? _queuedDirection;
    private bool _isRunning;
    private TextBlock? _alert;

    public WPFSnakeGame(IWorldInfo worldInfo)
    { 
        _worldInfo = worldInfo;
        _soundPlayer = new(Properties.Resources.BeepSound);
        try
        {
            _soundPlayer.LoadAsync();
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message, "Error loading sound file");
        }
        Initialize();
    }

    public int Score { get => _game.Score; }

    private void Initialize()
    {
        _game = GameEngineFactory.CreateGame(_worldInfo);
        _snakeController = GameEngineFactory.CreateSnakeController(_game);
        Vector2 defaultPosition = new(_game.WorldInfo.CellWidth, _game.WorldInfo.CellHeight);
        ISnakeFragment snakeHead = GameEngineFactory.CreateSnakeFragment(_snakeController, null, defaultPosition);
        _game.AddEntity(snakeHead);
        _alert = null;
    }
    public void Run()
    {
        if (!_isRunning)
        {
            _game.AddGameService(new AppleSpawnerService(_game));
            _isRunning = true;
        }
    }

    public void Restart()
    {
        if (_isRunning)
        {
            _game.Exit();
            _isRunning = false;
            Initialize();
            Run();
        }
    }

    public void SubscribeToGameEnded(EventHandler<GameEndedEventArgs> action) => _game.GameEnded += action;

    public void Update()
    {
        CheckQueuedDirection();
        int previousScore = _game.Score;
        _game.Update();
        if (previousScore != _game.Score)
            PlaySound();
    }

    private void PlaySound()
    {
        try
        {
            _soundPlayer.Play();
        }
        catch (Exception ex)
        {
            _alert = new TextBlock()
            {
                Text = ex.Message,
                FontSize = 20,
            };
        }
    }

    public List<(UIElement element, Vector2 position)> GetUIElements()
    {
        List<(UIElement, Vector2)> elements = new();
        IEntity[] entities = _game.Entities.ToArray();

        foreach (IEntity entity in entities)
        {
            try
            {
                UIElement element = UIElementFactory.CreateUIElement(entity);
                elements.Add((element, entity.Position));
            }
            catch (NotImplementedException)
            {
                continue;
            }
        }

        if(_game.IsPaused)
            elements.Add((GetPauseInfo(), Vector2.Zero));

        if(_alert != null)
            elements.Add((_alert, Vector2.Zero));

        return elements;
    }

    private static UIElement GetPauseInfo()
    {
        return new TextBlock() {
            Text = "Game Paused",
            FontSize = 40,
        };
    }

    private void CheckQueuedDirection()
    {
        if (!_directionUpdatedThisFrame && _queuedDirection.HasValue)
        {
            _snakeController.Direction = _queuedDirection.Value;
            _queuedDirection = null;
        }
        _directionUpdatedThisFrame = false;
    }
    
    public void Exit() => _game.Exit();

    public void ReadKeyInput(Key key)
    {
        switch (key)
        {
            case Key.Up:
                HandleDirection(Direction.Up);
                break;
            case Key.Down:
                HandleDirection(Direction.Down);
                break;
            case Key.Left:
                HandleDirection(Direction.Left);
                break;
            case Key.Right:
                HandleDirection(Direction.Right);
                break;
            case Key.Space:
                TogglePause();
                break;
            case Key.Escape:
                Exit();
                break;
        }
    }

    private void HandleDirection(Direction direction)
    {
        if (_directionUpdatedThisFrame)
        {
            _queuedDirection = direction;
        }
        else
        {
            _directionUpdatedThisFrame = true;
            _queuedDirection = null;
            _snakeController.Direction = direction;
        }
    }

    private void TogglePause()
    {
        if (_game.IsPaused)
            _game.Resume();
        else
            _game.Pause();
    }
}
