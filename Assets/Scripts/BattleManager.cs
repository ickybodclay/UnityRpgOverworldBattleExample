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
    public GameObject battleCursor;

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
            currentState = InputState.START;

            currentPlayerSlotIndex = 0;
            foreach (BattleData player in playerData.battleDataList) {
                if (player.CurrentHealth > 0) {
                    Debug.Log("HandleTurn> player " + player.Name);
                    interactPanel.SetActive(true);
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
            currentState = InputState.WAIT;

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

    enum InputState {
        START,
        ATTACK,
        WAIT
    }
    private InputState currentState;
    private int battleCursorPosition;

    public void HandleAttackClicked() {
        currentState = InputState.ATTACK;

        battleCursorPosition = 0;
        battleCursor.transform.position = enemySlots[battleCursorPosition].transform.position + (Vector3.right * 1.5f);
        battleCursor.SetActive(true);
        interactPanel.SetActive(false);
    }

    private void HandleTargetSelected(int targetId) {
        switch (currentState) {
            case InputState.ATTACK:
                Debug.Log(currentPlayerBattleData.Name + " attacking " + enemyData.battleDataList[targetId].Name +
                    " for " + currentPlayerBattleData.AttackDamage + " damage");
                enemyData.battleDataList[targetId].CurrentHealth -= currentPlayerBattleData.AttackDamage;

                UpdateEnemyStatusUI(targetId);

                playerEntities[currentPlayerSlotIndex].Attack();
                enemyEntities[targetId].TakeDamage();
                break;
        }

        currentState = InputState.START;
        battleCursor.SetActive(false);
    }

    private void Update() {
        switch (currentState) {
            case InputState.ATTACK:
                HandleAttackInput();
                break;
        }
    }

    private bool verticalButtonPressed = false;
    private void HandleAttackInput() {
        float y = Input.GetAxis("Vertical");
        if (!verticalButtonPressed && Mathf.Abs(y) > float.Epsilon) {

            battleCursorPosition = (battleCursorPosition + (int)-Mathf.Sign(y)) % enemyData.battleDataList.Count;
            if (battleCursorPosition < 0) {
                battleCursorPosition += enemyData.battleDataList.Count;
            }

            while (enemyData.battleDataList[battleCursorPosition].CurrentHealth <= 0) {
                battleCursorPosition = (battleCursorPosition + 1) % enemyData.battleDataList.Count;
            }

            battleCursor.transform.position = enemySlots[battleCursorPosition].transform.position + (Vector3.right * 1.5f);

            verticalButtonPressed = true;
            // TODO play sfx
        }

        if (Mathf.Abs(y) <= float.Epsilon) {
            verticalButtonPressed = false;
        }

        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit")) {
            HandleTargetSelected(battleCursorPosition);
        }

        if (Input.GetButtonDown("Cancel")) {
            currentState = InputState.START;
            battleCursor.SetActive(false);
            interactPanel.SetActive(true);
        }
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

        StopAllCoroutines();

        endBattleUI.SetActive(true);
        interactPanel.SetActive(false);
    }

    public void EndBattle() {
        enemyData.isActive = false;
        GameManager.Instance.EndBattle();
    }
}
