using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    public UnityEvent<Move> moveSelected;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI atkText;
    [SerializeField] TextMeshProUGUI accText;
    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] Move pMove;

    void Start()
    {
        if(moveSelected == null)
        {
            moveSelected = new UnityEvent<Move>();
        }
    }

    public void SetData(Move move)
    {
        pMove = move;
        nameText.text = move.Base.moveName;
        atkText.text = "Atk: " + move.Base.power.ToString();
        accText.text = "Acc: " + move.Base.accuracy.ToString();
        ppText.text = "PP: " + move.PP.ToString();
    }

    public void UseMove()
    {
        if(pMove != null)
        {
            pMove.UseMove();
            SetData(pMove);
            moveSelected.Invoke(pMove);
        }
    }
}
