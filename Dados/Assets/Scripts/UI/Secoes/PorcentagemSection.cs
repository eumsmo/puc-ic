using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PorcentagemSection : MonoBehaviour, SecaoDoJogo {
    GameUI game;
    Dados dados;

    public Text texto, informativoValor;
    public Slider slider;
    public Image sliderImage;

    [Header("Informações do Slider correto")]
    public Slider sliderMostraCorreto;
    public Text textoMostraCorreto;
    public GameObject corretoMenorHolder, corretoMaiorHolder;

    [Header("Cores")]
    public Color corValorErrado;
    public Color corValorCerto;
    public Color corValorNaArea;
    public Color corEscolheValor;

    
    public void Inicializar(GameUI game) {
        this.game = game;
        gameObject.SetActive(false);

        slider.onValueChanged.AddListener(AtulizarInformativoValor);
    }

    public void AtulizarInformativoValor(float valor) {
        informativoValor.text = GetValorInText(valor);
    }

    public string GetValorInText(float valor) {
        return (valor * 100).ToString("0.0") + "%";
    }

    public void HandleConfirmar() {
        slider.interactable = false;

        UIController.game.OnAttemptButtonClicked();

        if (slider.value == dados.respostaFloat) {
            sliderMostraCorreto.gameObject.SetActive(false);
            sliderImage.color = corValorCerto;
            return;
        }

        sliderMostraCorreto.value = dados.respostaFloat;
        textoMostraCorreto.text = GetValorInText(dados.respostaFloat);
        sliderMostraCorreto.gameObject.SetActive(true);

        if (slider.value < dados.respostaFloat) {
            sliderMostraCorreto.transform.SetParent(corretoMaiorHolder.transform);
        } else {
            sliderMostraCorreto.transform.SetParent(corretoMenorHolder.transform);
        }

        if (GetResposta()) // Se tá no range
            sliderImage.color = corValorNaArea;
        else
            sliderImage.color = corValorErrado;
    }

    public void Comecar(Dados dados) {
        gameObject.SetActive(true);

        sliderMostraCorreto.gameObject.SetActive(false);

        slider.interactable = true;
        slider.value = 0.5f;
        sliderImage.color = corEscolheValor;
        AtulizarInformativoValor(slider.value);

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
