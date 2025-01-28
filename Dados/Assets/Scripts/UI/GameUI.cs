using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public interface SecaoDoJogo {
    void Inicializar(GameUI game);
    void Comecar(Dados dados);
    void Finalizar();
    bool GetResposta();
}



public class GameUI : MonoBehaviour {
    public enum CurrentGameState {
        WaitingStart,
        Playing,
        ShowingStatus,
        Ended
    }

    public CurrentGameState state = CurrentGameState.WaitingStart;

    public VisualElement root;
    Label tempoLabel, perguntasQuantLabel;

    public GameObject[] secoes;
    SecaoDoJogo[] _secoes;
    public SecaoDoJogo secaoAtual;

    public VisualElement statusHolder;
    public Label statusLabel, statusDescricao;
    public Button nextButton;

    Dados dadosAtuais;


    void Awake() {
        root = GetComponent<UIDocument>().rootVisualElement;
        tempoLabel = root.Q<Label>("Tempo");
        perguntasQuantLabel = root.Q<Label>("Rodadas");

        statusHolder = root.Q<VisualElement>("Status");
        statusLabel = root.Q<Label>("InformaStatus");
        statusDescricao = root.Q<Label>("InformaMotivo");
        nextButton = root.Q<Button>("ContinuarInformativo");
        nextButton.clicked += GoToNext;


        _secoes = new SecaoDoJogo[secoes.Length];
        int i = 0;
        foreach (GameObject secaoObj in secoes) {
            SecaoDoJogo secao = secaoObj.GetComponent<SecaoDoJogo>();
            secao.Inicializar(this);
            _secoes[i] = secao;
            i++;
        }
    }

    void Start() {
        GameManager.instance.controls.Game.Submit.performed += ctx => OnAttemptButtonClicked();
    }

    public void UpdateQual(int qual, int quantas) {
        perguntasQuantLabel.text = (qual+1) + "/" + quantas;
    }

    public void LoadSecao(int secaoId, Dados dados) {
        if (secaoAtual != null) secaoAtual.Finalizar();
        secaoAtual = _secoes[secaoId];
        dadosAtuais = dados;
        state = CurrentGameState.Playing;
        secaoAtual.Comecar(dados);
    }

    public void UpdateTempo(int tempoSeconds) {
        int min = tempoSeconds / 60;
        int sec = tempoSeconds % 60;
        tempoLabel.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    public void OnAttemptButtonClicked() {
        if (state == CurrentGameState.ShowingStatus) return;
        state = CurrentGameState.ShowingStatus;

        bool resposta = secaoAtual.GetResposta();
        statusHolder.style.display = DisplayStyle.Flex;

        if (resposta) {
            statusLabel.text = "Correto!";
            statusDescricao.text = "Parabéns, você acertou!";
        } else {
            statusLabel.text = "Incorreto!";
            statusDescricao.text = "Que pena, você errou!";
        }

        if (dadosAtuais.explicacao != null && dadosAtuais.explicacao != "") {
            statusDescricao.text += dadosAtuais.explicacao;
        }
    }

    public void GoToNext() {
        statusHolder.style.display = DisplayStyle.None;
        GameManager.instance.ProximaPergunta();
    }

    public bool CheckIfWin() {
        return true;
    }
}
