using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Slider hpSlider;

    public void SetData(Creature creature)
    {
        nameText.text = creature.Base.Name;
        hpSlider.maxValue = creature.MaxHP;
        hpSlider.value = creature.HP;
    }
}
