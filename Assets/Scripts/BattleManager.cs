using System.Collections;
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

    private BattleEntity[] playerEntities;
    private BattleEntity[] enemyEntities;

    public void StartBattle(Player.Data playerData, Enemy.Data enemyData) {
        this.playerData = playerData;
        this.enemyData = enemyData;

        SetupUI();

        StartCoroutine(HandleTurn());
    }

    private void SetupUI() {
        int slotIndex = 0;
        playerEntities = new BattleEntity[playerData.battleDataList.Count];
        foreach (BattleData data in playerData.battleDataList) {
            if (slotIndex >= playerSlots.Length) {
                break;
            }
            Debug.Log("Spawning " + data.Name);
            playerStatusBars[slotIndex].gameObject.SetActive(true);
            playerStatusBars[slotIndex].nameText.text = data.Name;
            playerStatusBars[slotIndex].SetHealthFillAmount((float) data.CurrentHealth / data.MaxHealth);
            playerSlots[slotIndex].SetActive(data.CurrentHealth > 0);
            playerEntities[slotIndex] = Instantiate(data.battlePrefab, playerSlots[slotIndex].transform).GetComponent<BattleEntity>();
            slotIndex++;
        }

        slotIndex = 0;
        enemyEntities = new BattleEntity[enemyData.battleDataList.Count];
        foreach (BattleData data in enemyData.battleDataList) {
            if (slotIndex >= enemySlots.Length) {
                break;
            }
            Debug.Log("Spawning " + data.Name);
            enemyStatusBars[slotIndex].gameObject.SetActive(true);
            enemyStatusBars[slotIndex].nameText.text = data.Name;
            enemyStatusBars[slotIndex].SetHealthFillAmount((float) data.CurrentHealth / data.MaxHealth);
            enemySlots[slotIndex].SetActive(data.CurrentHealth > 0);
            enemyEntities[slotIndex] = Instantiate(data.battlePrefab, enemySlots[slotIndex].transform).GetComponent<BattleEntity>();
            slotIndex++;
        }
    }

    private int currentPlayerSlotIndex = 0;
    private int currentEnemySlotIndex = 0;

    private IEnumerator HandleTurn() {
        if (isPlayerTurn) {
            interactPanel.SetActive(true);

            currentPlayerSlotIndex = 0;
            foreach (BattleData player in playerData.battleDataList) {
                if (player.CurrentHealth > 0) {
                    Debug.Log("HandleTurn> player " + player.Name);
                    isCurrentTurnOver = false;
                    nameText.text = player.Name;
                    currentPlayerBattleData = player;
                    yield return new WaitUntil(() => isCurrentTurnOver);
                }
                currentPlayerSlotIndex++;
            }
        }
        else {
            interactPanel.SetActive(false);

            currentEnemySlotIndex = 0;
            foreach (BattleData enemy in enemyData.battleDataList) {
                if (enemy.CurrentHealth > 0) {
                    Debug.Log("HandleTurn> enemy " + enemy.Name);
                    isCurrentTurnOver = false;
                    HandleEnemyTurn(enemy);
                    yield return new WaitUntil(() => isCurrentTurnOver);
                }
                currentEnemySlotIndex++;
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

        UpdateEnemyStatusUI(targetEnemyId);

        playerEntities[currentPlayerSlotIndex].Attack();
        enemyEntities[targetEnemyId].TakeDamage();
    }

    private void HandleEnemyTurn(BattleData enemy) {
        int targetPlayerId = Random.Range(0, playerData.battleDataList.Count);

        while (playerData.battleDataList[targetPlayerId].CurrentHealth <= 0) {
            targetPlayerId = Random.Range(0, playerData.battleDataList.Count);
        }

        Debug.Log(enemy.Name + " attacking " + playerData.battleDataList[targetPlayerId].Name +
            " for " + enemy.AttackDamage + " damage");
        playerData.battleDataList[targetPlayerId].CurrentHealth -= enemy.AttackDamage;

        UpdatePlayerStatusUI(targetPlayerId);

        enemyEntities[currentEnemySlotIndex].Attack();
        playerEntities[targetPlayerId].TakeDamage();
    }

    private void UpdateEnemyStatusUI(int targetEnemyId) {
        enemySlots[targetEnemyId].SetActive(enemyData.battleDataList[targetEnemyId].CurrentHealth > 0);
        enemyStatusBars[targetEnemyId].AnimateHeathFillAmount((float)enemyData.battleDataList[targetEnemyId].CurrentHealth / enemyData.battleDataList[targetEnemyId].MaxHealth);
    }

    private void UpdatePlayerStatusUI(int targetPlayerId) {
        playerSlots[targetPlayerId].SetActive(playerData.battleDataList[targetPlayerId].CurrentHealth > 0);
        playerStatusBars[targetPlayerId].AnimateHeathFillAmount((float)playerData.battleDataList[targetPlayerId].CurrentHealth / playerData.battleDataList[targetPlayerId].MaxHealth);
    }

    public void EndTurn() {
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
