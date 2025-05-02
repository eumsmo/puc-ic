using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PorcentagemSection : MonoBehaviour, SecaoDoJogo {
    GameUI game;
    Dados dados;

    public Vector3 feedbackTextoOffset = new Vector3(0, -20f, 0);

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


    bool confirmou = false;
    public void HandleConfirmar() {
        if (confirmou) return;
        confirmou = true;

        slider.interactable = false;

        bool corretissima = GetRespostaCorretissima();
        bool correta = GetResposta();
        RespostaStatus status = UIController.game.CorretissimaCorretaToStatus(corretissima, correta);

        float resposta = slider.value;
        float respostaCorreta = dados.respostaFloat;
        float tempoAnimacao = 0.5f;

        sliderMostraCorreto.value = 0;
        sliderMostraCorreto.DOValue(respostaCorreta, tempoAnimacao).OnUpdate(() => {
            if (slider.value < sliderMostraCorreto.value) {
                sliderMostraCorreto.transform.SetParent(corretoMaiorHolder.transform);
            } else {
                sliderMostraCorreto.transform.SetParent(corretoMenorHolder.transform);
            }
        });

        float informativoNum = 0;
        DOTween.To(() => informativoNum, x => informativoNum = x, respostaCorreta, tempoAnimacao).OnUpdate(() => {
            float rounded = Mathf.Round(informativoNum * 100.0f) / 100.0f;
            textoMostraCorreto.text = GetValorInText(rounded);
        }).OnComplete(() => {
            if (corretissima || correta) transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 1, 1);
            else transform.DOShakePosition(0.3f, 5f, 20, 0.5f, false, true);

            UIController.game.SpawnStatusAt(slider.transform.position + feedbackTextoOffset, status);
            UIController.game.OnAttemptButtonClicked();
        });

        sliderMostraCorreto.gameObject.SetActive(true);

        
        
        
        if (corretissima) // Se tá certo
            sliderImage.color = corValorCerto;
        else if (correta) // Se tá no range
            sliderImage.color = corValorNaArea;
        else
            sliderImage.color = corValorErrado;
    }

    public void Comecar(Dados dados) {
        gameObject.SetActive(true);
        confirmou = false;

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
        float range = dados.range.range;

        return Mathf.Abs(resposta - respostaCorreta) <= range;
    }

    public bool GetRespostaCorretissima() {
        float resposta = slider.value;
        float respostaCorreta = dados.respostaFloat;

        return Mathf.Abs(resposta - respostaCorreta) <= 0.01f;
    }
}
