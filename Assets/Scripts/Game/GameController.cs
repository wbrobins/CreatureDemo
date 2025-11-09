using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState { FreeRoam, Battle, Dialog }

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] public DialogueBox dialogueBox;
    [SerializeField] PlayerController playerController;
    [SerializeField] Vector3 playerPos;
    [SerializeField] public List<Creature> partyList;

    GameState state;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadGame();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        state = GameState.FreeRoam;
    }

    void Update()
    {
        //Debug.Log(state);
        if (state == GameState.FreeRoam && playerController != null)
            playerController.HandleUpdate();
        else if (state == GameState.Battle && battleSystem != null)
            battleSystem.HandleUpdate();
        
        if (state == GameState.FreeRoam && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(SaveRoutine(dialogueBox.dialogueEmpty));
        }
    }

    public void StartBattle(int level, CreatureBase enemyCreature)
    {
        playerPos = playerController.transform.position;

        pendingLevel = level;
        pendingEnemy = enemyCreature;

        state = GameState.Battle;
        SceneManager.LoadScene("Battle");
    }

    void EndBattle(bool won)
    {
        //if battle is won, save the updated creature data here
        if(won == true)
        {
            SavePartyFromWonBattle();
        }

        SceneManager.LoadScene("Overworld");
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        playerController = FindFirstObjectByType<PlayerController>();
    }

    int pendingLevel;
    CreatureBase pendingEnemy;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //reassign player controller to game controller
        playerController = FindFirstObjectByType<PlayerController>();

        battleSystem.OnBattleOver += EndBattle;

        //return player to last position after a battle
        if (scene.name == "Overworld" && playerController != null)
        {
            playerController.transform.position = playerPos;
        }
        else if (scene.name == "Battle" && battleSystem != null)
        {
            battleSystem.gameObject.SetActive(true);
            battleSystem.StartBattle(pendingLevel, partyList, pendingEnemy);
        }
    }

    public void SaveGame()
    {
        PlayerData data = new PlayerData();
        data.playerName = "Morgan";
        data.partyList = new List<CreatureData>();

        foreach (var creature in partyList)
        {
            CreatureData cData = new CreatureData();
            cData.CreatureId = creature.Base.name;
            cData.Level = creature.Level;
            cData.HP = creature.HP;
            cData.Moves = new List<MoveData>();

            foreach (var move in creature.Moves)
            {
                MoveData mData = new MoveData();
                mData.MoveId = move.MoveId;
                mData.CurrentPP = move.PP;
                cData.Moves.Add(mData);
            }

            data.partyList.Add(cData);
        }
    
        string json = JsonUtility.ToJson(data);
        string path = Application.persistentDataPath + "/savefile.json";

        File.WriteAllText(path, JsonUtility.ToJson(data, true));
        Debug.Log("Game saved to " + path);
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (!File.Exists(path))
        {
            Debug.Log("Save file not found in " + path);
            return;
        }

        string json = File.ReadAllText(path);
        PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);

        partyList.Clear();

        foreach (var c in loadedData.partyList)
        {
            //rebuild the move list for creature
            List<Move> moves = new List<Move>();

            if (c.Moves != null)
            {
                foreach (var m in c.Moves)
                {
                    //load the MoveBase asset by name
                    MoveBase moveBase = Resources.Load<MoveBase>("Data/MoveBases/" + m.MoveId);

                    if (moveBase == null)
                    {
                        Debug.LogWarning($"MoveBase not found: {m.MoveId}");
                        continue;
                    }

                    //rebuild the Move instance
                    Move move = new Move(m.MoveId);
                    move.SetPP(m.CurrentPP);
                    moves.Add(move);
                }
            }

            //rebuild the Creature
            Creature creature = new Creature(c.CreatureId, c.Level, c.HP, c.Experience, moves);

            //add to player's party
            partyList.Add(creature);

            Debug.Log($"Loaded creature: {c.CreatureId} (Level {c.Level})");
        }

        Debug.Log("Game loaded. Player Name: " + loadedData.playerName);
        Debug.Log("Party List Count: " + partyList.Count);
    }

    void SavePartyFromWonBattle() //stupid function name
    {
        partyList.Clear(); //clear list 
        foreach (var c in battleSystem.partyList)
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

            Creature creature = new Creature(c.CreatureId, c.Level, c.HP, c.Experience, moves);

            partyList.Add(creature);
        }
    }
    
    IEnumerator SaveRoutine(UnityEvent unityEvent)
    {
        state = GameState.Dialog;

        unityEvent.RemoveAllListeners();

        var trigger = false;
        Action action = () => trigger = true;
        unityEvent.AddListener(action.Invoke);

        dialogueBox.AddToQueue("System", "Would you like to save your game?");
        dialogueBox.SetChoiceButtons("Yes", "No");
        dialogueBox.PlayNextInQueue();

        yield return new WaitUntil(() => trigger);

        if (dialogueBox.choice)
        {
            var trigger2 = false;
            Action action2 = () => trigger2 = true;
            unityEvent.AddListener(action2.Invoke);

            dialogueBox.AddToQueue("System", "Game saved!");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            SaveGame();
            sw.Stop();
            Debug.Log($"SaveGame took {sw.ElapsedMilliseconds} ms");
            dialogueBox.PlayNextInQueue();

            yield return new WaitUntil(() => trigger2);

            unityEvent.RemoveListener(action2.Invoke);
        }

        unityEvent.RemoveListener(action.Invoke);
        state = GameState.FreeRoam;
    }
}