using SnakeGameEngine;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Input;

namespace SnakeGameGUI;

internal interface IWPFSnakeGame
{
    public void Run();
    public void Restart();
    public void Update();
    public void SubscribeToGameEnded(EventHandler<GameEndedEventArgs> action);
    public List<(UIElement element, Vector2 position)> GetUIElements();
    public int Score { get; }
    public void ReadKeyInput(Key key);
    public void Exit();
}
