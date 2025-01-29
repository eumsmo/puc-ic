using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PorcentagemSection : MonoBehaviour, SecaoDoJogo {
    GameUI game;
    Dados dados;

    public Text texto;
    public Slider slider;

    
    public void Inicializar(GameUI game) {
        this.game = game;
        
        gameObject.SetActive(false);
        
        // confirmar.clicked += HandleConfirmar;
    }

    public void HandleConfirmar() {
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
        float resposta = slider.value;
        float respostaCorreta = dados.respostaFloat;
        float range = dados.range;

        return Mathf.Abs(resposta - respostaCorreta) <= range;
    }
}
