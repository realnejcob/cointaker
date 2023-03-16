using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
