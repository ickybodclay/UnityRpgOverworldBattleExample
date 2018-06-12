using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    private static bool instantiated = false;

	private void Awake () {
        if (!instantiated) {
            Instance = this;
            instantiated = true;
        }
        DontDestroyOnLoad(gameObject);
	}

    public void StartBattle() {
        SceneManager.LoadScene("Battle");
    }

    public void EndBattle() {
        SceneManager.LoadScene("Overworld");
    }
}
