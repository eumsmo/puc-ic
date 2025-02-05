using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    public Text acertosLabel, perguntasQuantLabel;

    public GameObject[] secoes;
    SecaoDoJogo[] _secoes;
    public SecaoDoJogo secaoAtual;

    public GameObject statusHolder;
    public Text statusLabel, statusDescricao;

    Dados dadosAtuais;

    public int acertos = 0, erros = 0;


    void Awake() {
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

    // Vou manter essa função aqui caso voltemos atrás na decisão
    public void UpdateTempo(int tempoSeconds) {
        int min = tempoSeconds / 60;
        int sec = tempoSeconds % 60;
        // tempoLabel.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    public void OnAttemptButtonClicked() {
        if (state == CurrentGameState.ShowingStatus) return;
        state = CurrentGameState.ShowingStatus;

        bool resposta = secaoAtual.GetResposta();
        statusHolder.SetActive(true);

        if (resposta) {
            acertos++;
            statusLabel.text = "Correto!";
            statusDescricao.text = "Parabéns, você acertou!";
        } else {
            erros++;
            statusLabel.text = "Incorreto!";
            statusDescricao.text = "Que pena, você errou!";
        }

        if (dadosAtuais.explicacao != null && dadosAtuais.explicacao != "") {
            statusDescricao.text += dadosAtuais.explicacao;
        }

        acertosLabel.text = "" + acertos;
    }

    public void GoToNext() {
        statusHolder.SetActive(false);
        GameManager.instance.ProximaPergunta();
    }

    public bool CheckIfWin() {
        return true;
    }
}
