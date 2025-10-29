using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Moves/Create New Move")]
public class MoveBase : ScriptableObject
{
    public string moveName;
    public string description;

    public int power;
    public int accuracy;
    public int pp;

    public bool isSpecial;
}