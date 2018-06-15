using System;
using UnityEngine;

[Serializable]
public class BattleData {
    public String Name;
    public int CurrentHealth;
    public int MaxHealth;
    public int AttackDamage;
    public GameObject battlePrefab;
}
