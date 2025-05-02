using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ValorGrafico : MonoBehaviour {
    public Slider slider;
    public Image sliderFill, pedacoExtra;
    public Text informativo, label;

    [Header("Slider correto")]
    public Slider sliderCorreto;
    public Text informativoCorreto;
    public GameObject holderMenor, holderMaior;

    [Header("Cores")]
    public Color corValorErrado;
    public Color corValorCerto;
    public Color corValorNaArea;
    public Color corEscolheValor;

    [Header("Valores"), Range(0, 0.3f)]
    public float overlapRange = 0.05f; // 5%
    [Range(0, 1)]
    public float opacityOnOverlap = 0.5f;

    [Header("Feedback")]
    public Vector3 feedbackTextoOffset = new Vector3(0, 0.5f, 0);

    float resposta;
    float min, max, range;
    bool porcentagem;

    public void Inicializar(string label, float correto, float min, float max, float range, bool porcentagem) {
        this.label.text = label;

        this.resposta = correto;
        this.min = min;
        this.max = max;
        this.range = range;
        this.porcentagem = porcentagem;

        slider.onValueChanged.AddListener(AtualizarInformativo);

        sliderCorreto.gameObject.SetActive(false);
        //informativo.gameObject.SetActive(true);

        sliderFill.color = corEscolheValor;
        pedacoExtra.color = corEscolheValor;

        SetValor((min + max) / 2);
    }

    public float GetValorNormalizado(float valor) {
        return (valor - min) / (max - min);
    }

    public float GetValorConvertido(float valor) {
        valor = Mathf.Round(valor*100.0f)/100.0f;
        return min + (max - min) * valor;
    }

    public string GetValorString(float valor, bool esta_convertido = true) {
        if (!esta_convertido) {
            valor = GetValorConvertido(valor);
        }

        if (porcentagem) {
            return (GetValorNormalizado(valor)*100) + "%";
        }

        return valor + "";
    }

    public void AtualizarInformativo(float valor) {
        informativo.text = GetValorString(valor, false);
    }

    public void SetValor(float valor) {
        float normal = GetValorNormalizado(valor);
        slider.value = normal;
        AtualizarInformativo(normal);
    }

    public void Tentar(float tempoAnimacao = 1f) {


        slider.interactable = false;

        float valorConvertido = GetValorConvertido(slider.value);
        float valorCorretoNormalizado = GetValorNormalizado(resposta);


        bool corretissima = GetRespostaCorretissima();
        bool correta = GetResposta();

        Color corSlider = Color.white;

        if (corretissima) {
            corSlider = corValorCerto;
        } else if (correta) { // Se tá no range
            corSlider = corValorNaArea;
        } else {
            corSlider = corValorErrado;
        }

        sliderFill.color = corSlider;
        pedacoExtra.color = corSlider;

        sliderCorreto.value = 0;
        // sliderCorreto.DOValue(valorCorretoNormalizado, 0.5f);

        var atualizaSlider = DOTween.To(() => sliderCorreto.value, x => sliderCorreto.value = x, valorCorretoNormalizado, tempoAnimacao).OnUpdate(() => {
            if (valorConvertido < sliderCorreto.value) {
                sliderCorreto.transform.SetParent(holderMaior.transform);
                pedacoExtra.color = corSlider;
            } else {
                sliderCorreto.transform.SetParent(holderMenor.transform);
                pedacoExtra.color = corValorCerto;
            }

            Color corInformativo = informativo.color;
            
            if (Mathf.Abs(sliderCorreto.value - slider.value) <= overlapRange) {
                corInformativo.a = opacityOnOverlap;
            } else {
                corInformativo.a = 1;
            }

            informativo.color = corInformativo;
        });
        
        float informativoNum = 0;
        var atualizaInformativo = DOTween.To(() => informativoNum, x => informativoNum = x, resposta, tempoAnimacao).OnUpdate(() => {
            float rounded = Mathf.Round(informativoNum * 100.0f) / 100.0f;
            informativoCorreto.text = GetValorString(rounded);
        });

        sliderCorreto.gameObject.SetActive(true);


        Sequence tweener = DOTween.Sequence();
        tweener.Append(atualizaSlider);
        tweener.Join(atualizaInformativo);

        tweener.OnComplete(() => {
            RespostaStatus status = RespostaStatus.Correto;

            if (corretissima) {
                transform.DOPunchScale(new Vector3(0.3f, 0f, 0f), 0.3f, 1, 1);
                status = RespostaStatus.Correto;
            } else if (correta) {
                transform.DOPunchScale(new Vector3(0.3f, 0f, 0f), 0.3f, 1, 1);
                status = RespostaStatus.Quase;
            } else {
                transform.DOShakePosition(0.3f, new Vector3(10, 0f, 0f), 20, 0.5f, false, true);
                status = RespostaStatus.Incorreto;
            }


            Vector3 flavorPos = Vector3.zero;
            
            if (valorConvertido < resposta) flavorPos = informativoCorreto.transform.position;
            else flavorPos = informativo.transform.position;

            UIController.game.SpawnStatusAt(flavorPos + feedbackTextoOffset, status);
        });

       
        

    }

    public bool GetResposta() {
        float valor = GetValorConvertido(slider.value);

        return Mathf.Abs(valor - resposta) <= range;
    }

    public bool GetRespostaCorretissima()  {
        float valor = GetValorConvertido(slider.value);

        return Mathf.Abs(valor - resposta) <= 0.01f;
    }

}
