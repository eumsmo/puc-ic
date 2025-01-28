using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PorcentagemSection : MonoBehaviour, SecaoDoJogo {
    VisualElement secao;
    public string secaoId;
    GameUI game;
    Dados dados;

    Label texto;
    VisualElement slider;
    Button confirmar;

    public string textoId, sliderId, confirmarId;
    
    public void Inicializar(GameUI game) {
        this.game = game;

        var root = game.root;

        secao = root.Q<VisualElement>(secaoId);
        texto = root.Q<Label>(textoId);
        slider = root.Q<VisualElement>(sliderId);
        confirmar = root.Q<Button>(confirmarId);
        
        secao.style.display = DisplayStyle.None;
        
        confirmar.clicked += HandleConfirmar;
    }

    public void HandleConfirmar() {
        UIController.game.OnAttemptButtonClicked();
    }

    public void Comecar(Dados dados) {
        secao.style.display = DisplayStyle.Flex;
        texto.text = dados.texto;
        this.dados = dados;
    }

    public void Finalizar() {
        secao.style.display = DisplayStyle.None;
    }

    public bool GetResposta() {
        Slider sliderAsSlider = slider as Slider;
        float resposta = sliderAsSlider.value;
        float respostaCorreta = dados.respostaFloat;
        float range = dados.range;

        return Mathf.Abs(resposta - respostaCorreta) <= range;
    }
}
