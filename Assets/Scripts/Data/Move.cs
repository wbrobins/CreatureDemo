using UnityEngine;

[System.Serializable]
public class Move
{
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private string move_id;
    [SerializeField] private int pp;

    // Public read-only properties
    public MoveBase Base => moveBase;
    public string MoveId => move_id;
    public int PP => pp;
    public bool HasPPLeft => pp > 0;

    // Constructor for runtime initialization
    public Move(string id)
    {
        move_id = id;
        moveBase = Resources.Load<MoveBase>($"Data/MoveBases/" + id);
        Debug.Log(moveBase);
        pp = moveBase.pp;
    }

    public void UseMove()
    {
        pp = Mathf.Max(pp - 1, 0);
    }

    public void SetPP(int newPP)
    {
        pp = newPP;
    }
}
