using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    public void Spawn(Player.Data playerData, Enemy.Data enemyData) {
        foreach (BattleData data in playerData.battleDataList) {
            Debug.Log("Spawning " + data.Name);
            // TODO spawn players using battle data
            // TODO place character
            // TODO update ui
            // TODO in-order fill based on available battle positions
        }

        foreach (BattleData data in enemyData.battleDataList) {
            Debug.Log("Spawning " + data.Name);
        }
    }

    public void EndBattle() {
        GameManager.Instance.GetEnemyData().isAlive = false;
        GameManager.Instance.EndBattle();
    }
}
