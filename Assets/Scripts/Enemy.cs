using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IPersistable<Enemy.Data> {
    [Serializable]
    public class Data {
        public int id;
        public bool isActive;
        public OverworldData overworldData;
        public List<BattleData> battleDataList;
    }

    public Data data;
    private bool hasLoadedState = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!hasLoadedState) {
            return;
        }

        if (collision.tag == "Player") {
            Player player = collision.transform.GetComponent<Player>();
            player.SaveData();
            SaveData();
            GameManager.Instance.StartBattle(player.data, data);
        }
    }

    public void SaveData() {
        data.overworldData.Position = transform.position;
    }

    public void LoadData(Data data) {
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
        transform.position = data.overworldData.Position;
        if (!data.isActive) {
            Destroy(gameObject);
        }
    }
}
