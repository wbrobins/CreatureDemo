using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CreatureButton : MonoBehaviour
{
    public UnityEvent<Creature> creatureSelected;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Creature pCreature;

    void Start()
    {
        if (creatureSelected == null)
        {
            creatureSelected = new UnityEvent<Creature>();
        }
    }

    public void SetData(Creature creature)
    {
        pCreature = creature;
        nameText.text = creature.Base.Name;
        hpText.text = "HP: " + creature.HP;
        levelText.text = "Level " + creature.Level;
    }
    
    public void SwapToCreature()
    {
        if(pCreature != null)
        {
            creatureSelected.Invoke(pCreature);
        }
    }
}
