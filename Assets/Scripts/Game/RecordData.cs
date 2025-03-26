using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons;
using System;

[System.Serializable]
public class RecordData
{
    public string Nickname;
    public string Grade;
    public string Date;
    public string Result;
    public List<PlayerMove> Moves = new List<PlayerMove>();
}

[Serializable]
public class PlayerMove
{
    public Constants.PlayerType player;
    public int x;
    public int y;

    public PlayerMove(Constants.PlayerType player, int x, int y)
    {
        this.player = player;
        this.x = x;
        this.y = y;
    }
}