using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons;
using System;

[Serializable]
public class RecordData
{
    public string Nickname;
    public string Grade;
    public string Date;
    public string Result;
    public List<(Constants.PlayerType player, int x, int y)> Moves;



}
