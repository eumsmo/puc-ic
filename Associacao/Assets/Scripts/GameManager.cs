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
    public AssociacaoInfo info;

    public GameState gameState = GameState.WAITING_TO_START;

    public Controls controls;

    public int qualPergunta = 0;
    public int quantasPerguntas = 0;

    float tempo = 0;
    public float TempoPartida {
        get { return tempo; }
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        infoLoader = GetComponent<InfoLoader>();
        controls = new Controls();
        controls.Game.Enable();
    }

    void Start() {
        StartCoroutine(infoLoader.LoadTexto());
    }

    void FixedUpdate() {
        if (gameState != GameState.PLAYING) return;

        tempo += Time.fixedDeltaTime;
        UIController.game.UpdateTempo(Mathf.RoundToInt(tempo));
    }

    public void OnLoadedInfo(AssociacaoInfo info) {
        this.info = info;
    }

    public void StartGame() {
        if (gameState == GameState.PLAYING) return;

        tempo = 0;
        qualPergunta = 0;

        gameState = GameState.PLAYING;
        UIController.instance.HandleGameStarted();
        UIController.game.ResetGame();
        UIController.game.GerarAssociacoes(info);
    }

    public void ProximaPergunta() {
        qualPergunta++;
        if (qualPergunta > quantasPerguntas - 1) {
            int acertos = UIController.game.acertos;
            int erros = UIController.game.erros;

            EndGame(acertos >= erros);
            return;
        }
    }

    public void EndGame(bool vitoria = true) {
        gameState = GameState.GAME_OVER;


        UIController.instance.HandleGameEnded(vitoria);
    }

    public void VoltarParaInicio() {
        gameState = GameState.WAITING_TO_START;
        UIController.instance.HandleGameRestarted();
    }

    public void SetError(string error) {
        UIController.instance.HandleGameError(error);
    }

    public void SetError() {
        SetError("Ocorreu um erro ao carregar o texto");
    }

    public AssociacaoInfo GetInfo() {
        return infoLoader.info;
    }

    public void IrAoArtigo() {
        Application.OpenURL(GetInfo().url);
    }
}
