using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    private static bool instantiated = false;

    private Player.Data currentPlayerData = null;
    private Enemy.Data currentEnemyData = null;
    private bool hasBattleEnded = false;

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
        if (scene.name == "Overworld") {
            LoadOverworld();
        }
        else if (scene.name == "Battle") {
            LoadBattle();
        }
    }

    private void LoadOverworld() {
        Debug.Log(">> Loading Overworld...");
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        if (enemies != null) {
            foreach (Enemy enemy in enemies) {
                enemy.LoadOverworldData(currentEnemyData);
            }
            currentEnemyData = null;
        }

        if (hasBattleEnded) {
            Debug.Log(">>> Restoring Overworld State After Battle...");
            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null) {
                player.LoadOverworldData(currentPlayerData);
                currentPlayerData = null;
            }
            hasBattleEnded = false;
        }
    }

    private void LoadBattle() {
        Debug.Log(">> Loading Battle...");
        BattleManager battleManager = GameObject.FindObjectOfType<BattleManager>();
        battleManager.Spawn(currentPlayerData, currentEnemyData);
    }

    public void StartBattle(Player.Data playerData, Enemy.Data enemyData) {
        currentPlayerData = playerData;
        currentEnemyData = enemyData;
        SceneManager.LoadScene("Battle");
    }
    
    public void EndBattle() {
        hasBattleEnded = true;
        SceneManager.LoadScene("Overworld");
    }

    public Player.Data GetPlayerData() {
        return currentPlayerData;
    }

    public Enemy.Data GetEnemyData() {
        return currentEnemyData;
    }
}
