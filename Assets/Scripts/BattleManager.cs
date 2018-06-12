using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    public void EndBattle() {
        GameManager.Instance.EndBattle();
    }
}
