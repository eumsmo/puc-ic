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
    public ArtigoInfo info;

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

    public void OnLoadedInfo(ArtigoInfo info) {
        this.info = info;
        quantasPerguntas = info.dados.Length;
    }

    public void StartGame() {
        if (gameState == GameState.PLAYING) return;

        tempo = 0;
        qualPergunta = 0;

        gameState = GameState.PLAYING;
        UIController.instance.HandleGameStarted();
        UIController.game.UpdateQual(qualPergunta, quantasPerguntas);

        LoadPergunta(info.dados[0]);
    }

    public void ProximaPergunta() {
        qualPergunta++;
        if (qualPergunta > quantasPerguntas - 1) qualPergunta = 0;

        UIController.game.UpdateQual(qualPergunta, quantasPerguntas);

        LoadPergunta(info.dados[qualPergunta]);
    }

    public void LoadPergunta(Dados dado) {
        string[] tipos = new string[] {"Boleano", "Porcentagem", "Grafico"};
        int id = -1;
        for (int i = 0; i < tipos.Length; i++) {
            if (tipos[i] == dado.tipo) {
                id = i;
                break;
            }
        }

        if (id==-1) {
            Debug.LogError("Nenhum tipo encontrado para [" + dado.tipo + "]");
        }

        UIController.game.LoadSecao(id, dado);
    }

    public void EndGame(bool vitoria = true) {
        gameState = GameState.GAME_OVER;


        UIController.instance.HandleGameEnded(vitoria);
    }

    public void SetError(string error) {
        UIController.instance.HandleGameError(error);
    }

    public void SetError() {
        SetError("Ocorreu um erro ao carregar o texto");
    }

    void FixedUpdate() {
        if (gameState != GameState.PLAYING) return;

        tempo += Time.fixedDeltaTime;
        UIController.game.UpdateTempo(Mathf.RoundToInt(tempo));
    }

    public ArtigoInfo GetArtigoInfo() {
        return infoLoader.info;
    }

    public void IrAoArtigo() {
        Application.OpenURL(GetArtigoInfo().url);
    }
}
