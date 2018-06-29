using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    private static bool instantiated = false;

    private Player.Data currentPlayerData = null;
    private int currentEnemyId = -1;
    private readonly Dictionary<int, Enemy.Data> enemyDataDictionary = new Dictionary<int, Enemy.Data>();

    private bool hasBattleEnded = false;

    private SceneTransitionManager sceneTransitionManager;
    private BattleManager battleManager;

    private void Awake () {
        if (!instantiated) {
            Instance = this;
            instantiated = true;
        }
        else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
	}

    private void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        Debug.Log("> OnLevelFinishedLoading scene=" + scene.name + " mode=" + mode.ToString());
        sceneTransitionManager = null; // ensure reference to previous scene manager is cleared
        battleManager = null;

        if (scene.name == "Overworld") {
            LoadOverworld();
        }
        else if (scene.name == "Battle") {
            LoadBattle();
        }
    }

    private void LoadOverworld() {
        Debug.Log(">> Loading Overworld...");
        sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();

        if (sceneTransitionManager == null) {
            Debug.LogError("Scene Transition Manger missing from Current Overworld Scene");
        }

        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        if (enemies != null) {
            foreach (Enemy enemy in enemies) {
                Enemy.Data enemyData = enemyDataDictionary.ContainsKey(enemy.data.id)
                    ? enemyDataDictionary[enemy.data.id]
                    : null;
                enemy.LoadData(enemyData);
            }
        }

        if (hasBattleEnded) {
            Debug.Log(">>> Restoring Overworld State After Battle...");

            sceneTransitionManager.FadeIn(() => Debug.Log("FadeIn Complete"));

            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null) {
                player.LoadData(currentPlayerData);
                currentPlayerData = null;
            }
            hasBattleEnded = false;
        }
    }

    private void LoadBattle() {
        Debug.Log(">> Loading Battle...");
        sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();

        if (sceneTransitionManager == null) {
            Debug.LogError("Scene Transition Manger missing from Current Battle Scene");
        }

        battleManager = FindObjectOfType<BattleManager>();

        if (battleManager == null) {
            Debug.LogError("Battle Manger missing from Current Battle Scene");
        }

        battleManager.StartBattle(currentPlayerData, enemyDataDictionary[currentEnemyId]);
        sceneTransitionManager.FadeIn(() => Debug.Log("FadeIn Complete"));
    }

    public void StartBattle(Player.Data playerData, Enemy.Data enemyData) {
        currentPlayerData = playerData;
        currentEnemyId = enemyData.id;
        enemyDataDictionary[currentEnemyId] = enemyData;
        sceneTransitionManager.FadeOut(() => SceneManager.LoadScene("Battle"));
    }
    
    public void EndBattle() {
        hasBattleEnded = true;
        sceneTransitionManager.FadeOut(() => SceneManager.LoadScene("Overworld"));
    }

    public Player.Data GetPlayerData() {
        return currentPlayerData;
    }

    public Enemy.Data GetEnemyData() {
        return enemyDataDictionary.ContainsKey(currentEnemyId) 
            ? enemyDataDictionary[currentEnemyId]
            : null;
    }

    public BattleManager GetBattleManager() {
        return battleManager;
    }
}
