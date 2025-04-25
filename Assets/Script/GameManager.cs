using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public enum PlaygroundData
{
    Empty = 0,
    Player1 = 1,
    Player2 = 2,
    Player3 = 3,
    Player4 = 4,
    Player5 = 5,
    Ship = 10,
    ShipTouched = 11,
    ShipDestroyed = 12,
}


public class GameManager
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
                _instance.playground = new PlaygroundData[0,0,0];
                _instance.ships1D = new();
                _instance.ships2D = new();
                _instance.ships3D = new();
            }
            return _instance;
        } 
    }

    public PlaygroundData[,,] playground;
    public int playgroundSize;

    public List<Ship1D> ships1D;
    public List<Ship2D> ships2D;
    public List<Ship3D> ships3D;

    public int IdPartie;



}
