using SnakeGameEngine;
using SnakeGameEngine.Utility;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SnakeGameGUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IWPFSnakeGame _game;
    private readonly DispatcherTimer _gameLoopTimer;
    private static readonly double _updateRateInSeconds = 0.1;
    private bool _checkForRestart;
    private bool _checkForStart;
    public MainWindow()
    {
        InitializeComponent();
        _gameLoopTimer = new()
        {
            Interval = TimeSpan.FromSeconds(_updateRateInSeconds)
        };
        _gameLoopTimer.Tick += OnFrameUpdate;
        IWorldInfo worldInfo = GameEngineFactory.CreateWorldInfo((int)PlayableArea.Width, (int)PlayableArea.Height);
        _game = WPFSnakeGameFactory.Create(worldInfo);
        _game.SubscribeToGameEnded(OnGameEnded);
    }
    
    private void OnFrameUpdate(object? sender, EventArgs e)
    {
        PlayableArea.Children.Clear();
        _game.Update();      
        ScoreDisplay.Text = _game.Score.ToString();
        foreach (var (element, position) in _game.GetUIElements())
        {
            PlayableArea.Children.Add(element);
            Canvas.SetTop(element, position.Y);
            Canvas.SetLeft(element, position.X);
        }
    }

    private void OnGameEnded(object? sender, GameEndedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            UIElement endgameMessage = CreateMessage($"{e.Message}\nYour score: {e.Score}\nEnter - new game\nEsc - exit", 40);
            AddEndgameMessegeToCanvas(endgameMessage);
            SetStateToEndgame();
        }
        );
    }

    private static UIElement CreateMessage(string message, int fontSize)
    {
        return new TextBlock()
        {
            Text = message,
            FontSize = fontSize,
            FontWeight = FontWeights.Bold,
        };
    }

    private void AddEndgameMessegeToCanvas(UIElement endgameMessage)
    {       
        PlayableArea.Children.Add(endgameMessage);
        Panel.SetZIndex(endgameMessage, int.MaxValue);
        Canvas.SetLeft(endgameMessage, 0);
        Canvas.SetTop(endgameMessage, 0);
    }

    private void SetStateToEndgame()
    {
        _gameLoopTimer.Stop();
        _checkForRestart = true;
    }

    private void WindowOnContentRendered(object sender, EventArgs e)
    {
        _checkForStart = true;
        string message;
        try
        {
            message = File.ReadAllText(Properties.Resources.WelcomeMessage);
        }
        catch(Exception)
        {
            message = "Press enter.";
        }
        UIElement welcomeMessage = CreateMessage(message, 20);
        PlayableArea.Children.Add(welcomeMessage);
        Canvas.SetLeft(welcomeMessage, 0);
        Canvas.SetTop(welcomeMessage, 0);
    }

    private void WindowOnKeyDown(object sender, KeyEventArgs e)
    {
        _game.ReadKeyInput(e.Key);
        if (e.Key == Key.Escape)
        {
            Exit();
        }
        else if (e.Key == Key.Enter)
        {
            if (_checkForRestart)
                RestartGame();
            else if (_checkForStart)
                StartGame();
        }
    }
    private void StartGame()
    {
        _game.Run();
        _gameLoopTimer.Start();
        _checkForStart = false;
    }
    private void RestartGame()
    {
        _gameLoopTimer.Start();
        _game.Restart();
        _game.SubscribeToGameEnded(OnGameEnded);
    }

    private void Exit()
    {
        _game.Exit();
        Close();
    }

    private void WindowOnMouseDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void ButtonCloseOnClick(object sender, RoutedEventArgs e) => Exit();
}
