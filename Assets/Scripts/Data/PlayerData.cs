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
    //TODO: ADD EXPERIENCE
    public string CreatureId;
    public int Level;
    public int HP;
    public List<MoveData> Moves;
}

[Serializable]
public class PlayerData
{
    public string playerName;

    public List<CreatureData> partyList;
}
