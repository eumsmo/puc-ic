using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoleanoSection : MonoBehaviour, SecaoDoJogo {
    GameUI game;
    Dados dados;
    bool resposta;

    public Text texto;

    public void Inicializar(GameUI game) {
        this.game = game;

        gameObject.SetActive(false);

        // sim.clicked += () => HandleConfirmar(true);
        // nao.clicked += () => HandleConfirmar(false);
    }

    public void HandleConfirmar(bool resposta) {
        this.resposta = resposta;
        UIController.game.OnAttemptButtonClicked();
    }

    public void Comecar(Dados dados) {
        gameObject.SetActive(true);
        texto.text = dados.texto;
        this.dados = dados;
    }

    public void Finalizar() {
        gameObject.SetActive(false);
    }

    public bool GetResposta() {
        return resposta == dados.respostaBool;
    }
    
    public string GetErroDetails() {
        return "";
    }
}
