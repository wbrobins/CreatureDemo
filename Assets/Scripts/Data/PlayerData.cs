using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MoveData
{
    public string MoveId;
    public int CurrentPP;
}

[Serializable]
public class CreatureData
{
    public string CreatureId;
    public int Level;
    public int HP;
    public int Experience;
    public List<MoveData> Moves;
}

[Serializable]
public class PlayerData
{
    public string playerName;

    public Vector3 playerPos;

    public List<CreatureData> partyList;
}
