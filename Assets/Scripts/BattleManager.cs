using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {
    private Player.Data playerData;
    private Enemy.Data enemyData;

    private bool isPlayerTurn = true;
    private bool isCurrentTurnOver = false;
    private BattleData currentPlayerBattleData;

    public GameObject battleUI;
    public GameObject interactPanel;
    public GameObject playerStatusPanel;
    public GameObject enemyStatusPanel;
    public Text nameText;
    public Button attackButton;

    public GameObject endBattleUI;

    public void StartBattle(Player.Data playerData, Enemy.Data enemyData) {
        this.playerData = playerData;
        this.enemyData = enemyData;

        SetupUI();

        StartCoroutine(HandleTurn());
    }

    private void SetupUI() {
        foreach (BattleData data in playerData.battleDataList) {
            Debug.Log("Spawning " + data.Name);
            // TODO add status line to player status panel
            // TODO add player battle prefab to battlefield
        }

        foreach (BattleData data in enemyData.battleDataList) {
            Debug.Log("Spawning " + data.Name);
            // TODO add status line to enemy status panel
            // TODO add enemy battle prefab to battlefield
        }
    }

    private IEnumerator HandleTurn() {
        if (isPlayerTurn) {
            interactPanel.SetActive(true);

            foreach (BattleData player in playerData.battleDataList) {
                if (player.Health > 0) {
                    Debug.Log("HandleTurn> player " + player.Name);
                    isCurrentTurnOver = false;
                    nameText.text = player.Name;
                    currentPlayerBattleData = player;
                    yield return new WaitUntil(() => isCurrentTurnOver);
                }
            }
        }
        else {
            interactPanel.SetActive(false);

            foreach (BattleData enemy in enemyData.battleDataList) {
                if (enemy.Health > 0) {
                    Debug.Log("HandleTurn> enemy " + enemy.Name);
                    isCurrentTurnOver = false;
                    HandleEnemyTurn(enemy);
                    yield return new WaitUntil(() => isCurrentTurnOver);
                }
            }
        }

        isPlayerTurn = !isPlayerTurn;
        StartCoroutine(HandleTurn());
    }

    public void HandleAttackClicked() {
        int targetEnemyId = Random.Range(0, enemyData.battleDataList.Count);

        while (enemyData.battleDataList[targetEnemyId].Health <= 0) {
            targetEnemyId = Random.Range(0, enemyData.battleDataList.Count);
        }

        Debug.Log(currentPlayerBattleData.Name + " attacking " + enemyData.battleDataList[targetEnemyId].Name + 
            " for " + currentPlayerBattleData.AttackDamage + " damage");
        enemyData.battleDataList[targetEnemyId].Health -= currentPlayerBattleData.AttackDamage;

        EndTurn();
    }

    private void HandleEnemyTurn(BattleData enemy) {
        int targetPlayerId = Random.Range(0, playerData.battleDataList.Count);

        while (playerData.battleDataList[targetPlayerId].Health <= 0) {
            targetPlayerId = Random.Range(0, playerData.battleDataList.Count);
        }

        Debug.Log(enemy.Name + " attacking " + playerData.battleDataList[targetPlayerId].Name +
            " for " + enemy.AttackDamage + " damage");
        playerData.battleDataList[targetPlayerId].Health -= enemy.AttackDamage;

        Invoke("EndTurn", 1f); // fake delay to handle lack of animation
    }

    private void EndTurn() {
        isCurrentTurnOver = true;

        if (isPlayerTurn) {
            if (AreAllEnemiesDead()) {
                ShowEndBattleUI(true);
            }
        }
        else {
            if (AreAllPlayersDead()) {
                ShowEndBattleUI(false);
            }
        }
    }

    private bool AreAllEnemiesDead() {
        foreach (BattleData enemy in enemyData.battleDataList) {
            if (enemy.Health > 0) {
                return false;
            }
        }
        return true;
    }

    private bool AreAllPlayersDead() {
        foreach (BattleData player in playerData.battleDataList) {
            if (player.Health > 0) {
                return false;
            }
        }
        return true;
    }

    public void ShowEndBattleUI(bool success) {
        Debug.Log("ShowEndBattleUI> success? " + success);

        battleUI.SetActive(false);
        endBattleUI.SetActive(true);

        StopCoroutine(HandleTurn());
    }

    public void EndBattle() {
        enemyData.isActive = false;
        GameManager.Instance.EndBattle();
    }
}
