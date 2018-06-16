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

    [Header("Battle UI")]
    public GameObject battleUI;
    public GameObject interactPanel;
    public GameObject playerStatusPanel;
    public StatusBar[] playerStatusBars;
    public GameObject enemyStatusPanel;
    public StatusBar[] enemyStatusBars;
    public Text nameText;
    public Button attackButton;
    public GameObject endBattleUI;

    [Header("Battlefield Positions")]
    public GameObject[] playerSlots;
    public GameObject[] enemySlots;

    public void StartBattle(Player.Data playerData, Enemy.Data enemyData) {
        this.playerData = playerData;
        this.enemyData = enemyData;

        SetupUI();

        StartCoroutine(HandleTurn());
    }

    private void SetupUI() {
        int slotIndex = 0;
        foreach (BattleData data in playerData.battleDataList) {
            if (slotIndex >= playerSlots.Length) {
                break;
            }
            Debug.Log("Spawning " + data.Name);
            playerStatusBars[slotIndex].gameObject.SetActive(true);
            playerStatusBars[slotIndex].nameText.text = data.Name;
            playerStatusBars[slotIndex].healthBar.fillAmount = (float) data.CurrentHealth / data.MaxHealth;
            playerSlots[slotIndex].SetActive(data.CurrentHealth > 0);
            Instantiate(data.battlePrefab, playerSlots[slotIndex].transform);
            slotIndex++;
        }

        slotIndex = 0;
        foreach (BattleData data in enemyData.battleDataList) {
            if (slotIndex >= enemySlots.Length) {
                break;
            }
            Debug.Log("Spawning " + data.Name);
            enemyStatusBars[slotIndex].gameObject.SetActive(true);
            enemyStatusBars[slotIndex].nameText.text = data.Name;
            enemyStatusBars[slotIndex].healthBar.fillAmount = (float) data.CurrentHealth / data.MaxHealth;
            enemySlots[slotIndex].SetActive(data.CurrentHealth > 0);
            Instantiate(data.battlePrefab, enemySlots[slotIndex].transform);
            slotIndex++;
        }
    }

    private IEnumerator HandleTurn() {
        if (isPlayerTurn) {
            interactPanel.SetActive(true);

            foreach (BattleData player in playerData.battleDataList) {
                if (player.CurrentHealth > 0) {
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
                if (enemy.CurrentHealth > 0) {
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

        while (enemyData.battleDataList[targetEnemyId].CurrentHealth <= 0) {
            targetEnemyId = Random.Range(0, enemyData.battleDataList.Count);
        }

        Debug.Log(currentPlayerBattleData.Name + " attacking " + enemyData.battleDataList[targetEnemyId].Name + 
            " for " + currentPlayerBattleData.AttackDamage + " damage");
        enemyData.battleDataList[targetEnemyId].CurrentHealth -= currentPlayerBattleData.AttackDamage;

        Invoke("EndTurn", 0.5f); // fake delay to handle lack of animation
    }

    private void HandleEnemyTurn(BattleData enemy) {
        int targetPlayerId = Random.Range(0, playerData.battleDataList.Count);

        while (playerData.battleDataList[targetPlayerId].CurrentHealth <= 0) {
            targetPlayerId = Random.Range(0, playerData.battleDataList.Count);
        }

        Debug.Log(enemy.Name + " attacking " + playerData.battleDataList[targetPlayerId].Name +
            " for " + enemy.AttackDamage + " damage");
        playerData.battleDataList[targetPlayerId].CurrentHealth -= enemy.AttackDamage;

        Invoke("EndTurn", 0.5f); // fake delay to handle lack of animation
    }

    private void EndTurn() {
        UpdateUI();

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

    private void UpdateUI() {
        int slotIndex = 0;
        foreach (BattleData data in playerData.battleDataList) {
            if (slotIndex >= playerSlots.Length) {
                break;
            }
            playerSlots[slotIndex].SetActive(data.CurrentHealth > 0);
            playerStatusBars[slotIndex].healthBar.fillAmount = (float) data.CurrentHealth / data.MaxHealth;
            slotIndex++;
        }

        slotIndex = 0;
        foreach (BattleData data in enemyData.battleDataList) {
            if (slotIndex >= enemySlots.Length) {
                break;
            }
            enemySlots[slotIndex].SetActive(data.CurrentHealth > 0);
            enemyStatusBars[slotIndex].healthBar.fillAmount = (float) data.CurrentHealth / data.MaxHealth;
            slotIndex++;
        }
    }

    private bool AreAllEnemiesDead() {
        foreach (BattleData enemy in enemyData.battleDataList) {
            if (enemy.CurrentHealth > 0) {
                return false;
            }
        }
        return true;
    }

    private bool AreAllPlayersDead() {
        foreach (BattleData player in playerData.battleDataList) {
            if (player.CurrentHealth > 0) {
                return false;
            }
        }
        return true;
    }

    public void ShowEndBattleUI(bool success) {
        Debug.Log("ShowEndBattleUI> success? " + success);

        endBattleUI.SetActive(true);

        StopCoroutine(HandleTurn());
    }

    public void EndBattle() {
        enemyData.isActive = false;
        GameManager.Instance.EndBattle();
    }
}
