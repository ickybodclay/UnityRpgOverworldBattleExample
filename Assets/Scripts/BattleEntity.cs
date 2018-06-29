using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BattleEntity : MonoBehaviour {
    private Animator animator;

	void Start () {
        animator = GetComponent<Animator>();
	}

    public void Attack() {
        animator.SetTrigger("Attack");
    }

    public void TakeDamage() {
        animator.SetTrigger("TakeDamage");
    }

    public void EndTurn() {
        GameManager.Instance.GetBattleManager().EndTurn();
    }
}
