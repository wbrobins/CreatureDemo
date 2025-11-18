using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] string playerCreatureId;
    [SerializeField] string enemyCreatureId;

    [SerializeField] GameObject moveButtonPrefab;
    [SerializeField] GameObject creatureButtonPrefab;
    [SerializeField] Transform movesPanel;
    [SerializeField] Transform partyPanel;

    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips;

    [SerializeField] List<Move> struggleMoves;

    private GameController gameController;
    private bool winOrLoss;

    public event Action<bool> OnBattleOver;

    BattleState state;

    private void OnDisable()
    {
        OnBattleOver -= EndBattle;
    }

    public void StartBattle(int level, List<Creature> cPartyList, CreatureBase cEnemyBase)
    {
        state = BattleState.Start;
        movesPanel = GameObject.Find("MovesPanel").GetComponent<Transform>();
        partyPanel = GameObject.Find("PartyPanel").GetComponent<Transform>();
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        playerUnit = GameObject.Find("PlayerUnit").GetComponent<BattleUnit>();
        enemyUnit = GameObject.Find("EnemyUnit").GetComponent<BattleUnit>();
        playerHUD = GameObject.Find("PlayerUnitHUD").GetComponent<BattleHUD>();
        enemyHUD = GameObject.Find("EnemyUnitHUD").GetComponent<BattleHUD>();

        gameController.dialogueBox.dialogueEmpty.RemoveAllListeners();
        gameController.dialogueBox.dialogueEmpty.AddListener(OnEndOfBattleDialogueEmpty);

        //clone party list passed from game controller 
        partyList = new List<Creature>();
        ClonePartyList(cPartyList);



        playerBase = partyList[0].Base;

        CreateCreatureButtons(partyList);
        CreateMoveButtons(partyList[0].Moves);

        enemyBase = cEnemyBase;

        playerCreatureId = partyList[0].CreatureId;
        enemyCreatureId = enemyBase.name;

        enemyLevel = level;

        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        foreach (Creature c in partyList)
        {
            if (c.HP > 0)
            {
                playerUnit.Setup(c);
                playerHUD.SetData(playerUnit.Creature);
                break;
            }
        }

        enemyUnit.Setup(new Creature(enemyCreatureId, enemyLevel, -1, 0, null));
        enemyHUD.SetData(enemyUnit.Creature);

        CheckForUsableMoves();

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
                Debug.Log(playerWon);
                OnBattleOver?.Invoke(playerWon);
            }
        }
        
    }

    public void CreateMoveButtons(List<Move> moves)
    {
        foreach (Transform child in movesPanel)
        {
            if(child.GetComponent<Button>() != null) //only clear buttons, not the label
            {
               Destroy(child.gameObject); 
            }
        }

        foreach (var move in moves)
        {
            GameObject buttonObj = Instantiate(moveButtonPrefab, movesPanel);
            MoveButton moveButton = buttonObj.GetComponent<MoveButton>();
            moveButton.SetData(move);
            moveButton.moveSelected.AddListener(OnMoveSelected);
        }
    }

    public void CreateCreatureButtons(List<Creature> creatures)
    {
        foreach (Transform child in partyPanel)
        {
            if(child.GetComponent<Button>() != null)  //only clear buttons, not the label
            {
               Destroy(child.gameObject); 
            }
        }

        foreach (var creature in creatures)
        {
            GameObject buttonObj = Instantiate(creatureButtonPrefab, partyPanel);
            CreatureButton cButton = buttonObj.GetComponent<CreatureButton>();
            cButton.SetData(creature);
            cButton.creatureSelected.AddListener(OnCreatureSwapSelected);
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
            
            Creature clone = new Creature(c.CreatureId, c.Level, c.HP, c.Experience, moves);
            partyList.Add(clone);
        }
    }


    //move chain
    void OnMoveSelected(Move move)
    {
        if (state == BattleState.PlayerAction && move.PP > 0)
        {
            Debug.Log("Move executed: " + move.Base.moveName);
            StartCoroutine(PlayerMove(move.Base.power));
            movesPanel.gameObject.SetActive(false);
            partyPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot execute move. Not player turn");
        }
    }

    IEnumerator PlayerMove(int power)
    {
        state = BattleState.PlayerMove;
        Debug.Log(state);

        playerUnit.PlayAttackAnimation();
        enemyUnit.PlayHitAnimation();

        enemyUnit.Creature.TakeDamage((playerUnit.Creature.Attack * power)/2); //damage enemy
        Debug.Log("Player attacked for: " + (playerUnit.Creature.Attack * power)/2);
        enemyHUD.SetData(enemyUnit.Creature);  //update enemy HUD

        yield return new WaitForSeconds(2);
        
        if(enemyUnit.Creature.HP == 0)
        {
            bool playerWon = true;
            enemyUnit.PlayFaintAnimation();
            Debug.Log("Player wins!");
            EndBattle(playerWon);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        Debug.Log(state);
        Move move = enemyUnit.Creature.GetRandomMove(); //get random move from enemy move pool

        enemyUnit.PlayAttackAnimation();
        playerUnit.PlayHitAnimation();

        playerUnit.Creature.TakeDamage((enemyUnit.Creature.Attack * move.Base.power) / 2); //damage player
        Debug.Log("Enemy uses: " + move.Base.moveName);
        playerHUD.SetData(playerUnit.Creature);   //update player HUD
        CreateCreatureButtons(partyList); //update creature info in party list

        yield return new WaitForSeconds(2);

        if (playerUnit.Creature.HP == 0)
        {
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2);
            bool remaining = false;
            foreach (Creature c in partyList)
            {
                if (c.HP > 0)
                {
                    playerUnit.Setup(c);
                    playerHUD.SetData(playerUnit.Creature);  //set HUD data of selected creature
                    CreateCreatureButtons(partyList);       //reset creature buttons to update data
                    CreateMoveButtons(c.Moves);
                    remaining = true;
                    break;
                }
                else
                {
                    remaining = false;
                }
            }

            if (!remaining)
            {
                Debug.Log("Player loses!");
                bool playerWon = false;
                EndBattle(playerWon);
            }
            else
            {
                state = BattleState.PlayerAction;
                Debug.Log(state);
                Debug.Log("Player turn!");
                movesPanel.gameObject.SetActive(true);
                partyPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            state = BattleState.PlayerAction;
            Debug.Log(state);
            Debug.Log("Player turn!");
            movesPanel.gameObject.SetActive(true);
            partyPanel.gameObject.SetActive(true);
        }

        CheckForUsableMoves();
    }

    void EndBattle(bool playerWon)
    {
        state = BattleState.Busy;

        winOrLoss = playerWon;
        DialogueBox dBox = gameController.dialogueBox;

        if (playerWon)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
            int prevLevel = playerUnit.Creature.Level;
            dBox.AddToQueue("System", "You have won!");
            playerUnit.Creature.GetExperience(enemyUnit.Creature.Experience / 7);
            dBox.AddToQueue("System", playerUnit.Creature.Base.Name + " has gained " + (enemyUnit.Creature.Experience / 7) + " exp!");
            if(playerUnit.Creature.Level > prevLevel)
            {
                dBox.AddToQueue("System", playerUnit.Creature.Base.Name + " has leveled up from " + prevLevel + " to " + playerUnit.Creature.Level + "!");
            }
        }
        else
        {
            audioSource.clip = audioClips[1];
            audioSource.Play();
            dBox.AddToQueue("System", "You have lost...");
            dBox.AddToQueue("System", "Loading from last save...");
        }

        dBox.PlayNextInQueue();
    }

    void OnEndOfBattleDialogueEmpty()
    {
        OnBattleOver?.Invoke(winOrLoss);
    }

    void OnCreatureSwapSelected(Creature c)
    {
        if (state == BattleState.PlayerAction && c != playerUnit.Creature)
        {
            state = BattleState.PlayerMove;
            StartCoroutine(SwapCreature(c));
        }
    }

    IEnumerator SwapCreature(Creature c)
    {
        playerUnit.Setup(c);
        playerHUD.SetData(playerUnit.Creature);  //set HUD data of selected creature
        CreateCreatureButtons(partyList);       //reset creature buttons to update data
        CreateMoveButtons(c.Moves);             //reset move buttons
        movesPanel.gameObject.SetActive(false);
        partyPanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        StartCoroutine(EnemyMove());
    }
    
    void CheckForUsableMoves()
    {
        bool hasUsableMoves = false;

        foreach (Move m in playerUnit.Creature.Moves)
        {
            if (m.HasPPLeft)
            {
                hasUsableMoves = true;
                break;
            }
        }

        if (!hasUsableMoves)
        {
            CreateMoveButtons(struggleMoves);
        }
    }
}
