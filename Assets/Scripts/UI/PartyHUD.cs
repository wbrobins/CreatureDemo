using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI partyText;
   public void SetData(List<Creature> creatureList)
    {
        foreach(Creature c in creatureList)
        {
            partyText.text += c.Base.name + "\n";
        }
    }
}
