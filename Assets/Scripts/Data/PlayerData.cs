using System;
using System.Collections.Generic;


[Serializable]
public class MoveData
{
    public string MoveId;
    public int CurrentPP;
}

[Serializable]
public class CreatureData
{
    //TODO: ADD HP
    public string CreatureId;
    public int Level;
    public List<MoveData> Moves;
}

[Serializable]
public class PlayerData
{
    public string playerName;

    public List<CreatureData> partyList;
}
