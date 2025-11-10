using TMPro;
using UnityEngine;

public class CreatureOverworldPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI levelText;

    public void SetData(Creature c)
    {
        nameText.text = c.Base.Name;
        hpText.text = "HP: " + c.HP;
        levelText.text = "Level: " + c.Level; 
    }
}
