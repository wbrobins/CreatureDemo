using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] private int enemyLevel;
    [SerializeField] public List<Creature> partyList;
    [SerializeField] CreatureBase playerBase;
    [SerializeField] CreatureBase enemyBase;
    [SerializeField] PartyHUD partyHUD;

    [SerializeField] string playerCreatureId;
    [SerializeField] string enemyCreatureId;

    [SerializeField] GameObject moveButtonPrefab;
    [SerializeField] Transform movesPanel;

    private GameController gameController;

    public event Action<bool> OnBattleOver;

    BattleState state;

    public void StartBattle(int level, List<Creature> cPartyList, CreatureBase cEnemyBase)
    {
        state = BattleState.Start;
        movesPanel = GameObject.Find("MovesPanel").GetComponent<Transform>();
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        playerUnit = GameObject.Find("PlayerUnit").GetComponent<BattleUnit>();
        enemyUnit = GameObject.Find("EnemyUnit").GetComponent<BattleUnit>();
        playerHUD = GameObject.Find("PlayerUnitHUD").GetComponent<BattleHUD>();
        enemyHUD = GameObject.Find("EnemyUnitHUD").GetComponent<BattleHUD>();
        partyHUD = GameObject.Find("PartyList").GetComponent<PartyHUD>();

        //clone party list passed from game controller 
        partyList = new List<Creature>();
        ClonePartyList(cPartyList);

        playerBase = partyList[0].Base;
        CreateMoveButtons(partyList[0].Moves);

        enemyBase = cEnemyBase;

        playerCreatureId = partyList[0].CreatureId;
        enemyCreatureId = enemyBase.name;

        enemyLevel = level;

        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerUnit.Setup(partyList[0]);
        enemyUnit.Setup(new Creature(enemyCreatureId, enemyLevel, null));

        playerHUD.SetData(playerUnit.Creature);
        enemyHUD.SetData(enemyUnit.Creature);

        partyHUD.SetData(gameController.partyList);

        state = BattleState.PlayerAction;
        Debug.Log("Battle! Level: " + enemyLevel);
        yield return null;
    }

    public void HandleUpdate()
    {
        if(state == BattleState.PlayerAction)
        {
            if (Input.GetKeyDown(KeyCode.Space)) //simulate win
            {
                bool playerWon = true;
                OnBattleOver?.Invoke(playerWon);
            } else if (Input.GetKeyDown(KeyCode.L)) //simulate loss
            {
                bool playerWon = false;
                OnBattleOver?.Invoke(playerWon);
            }
        }
        
    }

    public void CreateMoveButtons(List<Move> moves)
    {
        foreach (Transform child in movesPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var move in moves)
        {
            GameObject buttonObj = Instantiate(moveButtonPrefab, movesPanel);
            MoveButton moveButton = buttonObj.GetComponent<MoveButton>();
            moveButton.SetData(move);
            moveButton.moveSelected.AddListener(OnMoveSelected);
        }
    }

    void ClonePartyList(List<Creature> cPartyList)
    {
        foreach(var c in cPartyList)
        {
            List<Move> moves = new List<Move>();

            if (c.Moves != null)
            {
                foreach (var m in c.Moves)
                {
                    MoveBase moveBase = Resources.Load<MoveBase>("Data/MoveBases/" + m.MoveId);

                    if (moveBase == null)
                    {
                        Debug.LogWarning($"MoveBase not found: {m.MoveId}");
                        continue;
                    }

                    Move move = new Move(m.MoveId);
                    move.SetPP(m.PP);

                    moves.Add(move);
                }
            }
            
            Creature clone = new Creature(c.CreatureId, c.Level, moves);
            partyList.Add(clone);
        }
    }


    //move chain
    void OnMoveSelected(Move move)
    {
        if (state == BattleState.PlayerAction && move.PP > 0)
        {
            Debug.Log("Move executed: " + move.Base.moveName);
            StartCoroutine(PlayerMove());
        }
        else
        {
            Debug.Log("Cannot execute move. Not player turn");
        }
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PlayerMove;
        yield return new WaitForSeconds(2);
        StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        Debug.Log("Enemy uses: " + enemyUnit.Creature.GetRandomMove().Base.moveName);
        yield return new WaitForSeconds(2);
        state = BattleState.PlayerAction;
        Debug.Log("Player turn!");
    }
}
