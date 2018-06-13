using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Serializable]
    public class Data {
        public int id;
        public bool isAlive;
        public List<BattleData> battleDataList;
    }

    public Data data;
    private bool hasLoadedState = false;

    public void LoadOverworldData(Data data) {
        if (data != null) {
            Debug.Log("LoadOverworldData> data " + data.id + " >> " + this.data.id);
            if (this.data.id == data.id) {
                this.data = data;
                RestoreOverworldState();
            }
        }
        hasLoadedState = true;
    }

    private void RestoreOverworldState() {
        if (!data.isAlive) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!hasLoadedState) {
            return;
        }

        if (collision.tag == "Player") {
            Player player = collision.transform.GetComponent<Player>();
            player.SaveOverworldData();
            GameManager.Instance.StartBattle(player.data, data);
        }
    }
}
