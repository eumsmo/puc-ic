using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    WAITING_TO_START,
    PLAYING,
    PAUSED,
    GAME_OVER
}

[RequireComponent(typeof(InfoLoader))]
public class GameManager : MonoBehaviour {
    public static GameManager instance;
    InfoLoader infoLoader;

    public GameState gameState = GameState.WAITING_TO_START;
    float tempo = 0;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        infoLoader = GetComponent<InfoLoader>();
    }

    void Start() {
        StartCoroutine(infoLoader.LoadTexto());
    }

    public void StartGame() {
        if (gameState == GameState.PLAYING) return;

        tempo = 0;

        gameState = GameState.PLAYING;
        UIController.instance.HandleGameStarted();
    }

    void FixedUpdate() {
        if (gameState != GameState.PLAYING) return;

        tempo += Time.fixedDeltaTime;
        UIController.instance.UpdateTempo(Mathf.RoundToInt(tempo));
    }
}
