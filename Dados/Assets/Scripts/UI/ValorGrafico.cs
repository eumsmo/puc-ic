using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void Tentar() {
        slider.interactable = false;

        float valorConvertido = GetValorConvertido(slider.value);

        if (valorConvertido == resposta) {
            sliderCorreto.gameObject.SetActive(false);
            sliderFill.color = corValorCerto;
            pedacoExtra.color = corValorCerto;
            return;
        }

        sliderCorreto.value = GetValorNormalizado(resposta);

        //informativo.gameObject.SetActive(false);

        informativoCorreto.text = GetValorString(resposta);
        sliderCorreto.gameObject.SetActive(true);


        if (GetResposta()) { // Se tá no range
            sliderFill.color = corValorNaArea;
            pedacoExtra.color = corValorNaArea;
        } else {
            sliderFill.color = corValorErrado;
            pedacoExtra.color = corValorErrado;
        }

        if (valorConvertido < resposta) {
            sliderCorreto.transform.SetParent(holderMaior.transform);
        } else {
            sliderCorreto.transform.SetParent(holderMenor.transform);
            pedacoExtra.color = corValorCerto;
        }
    }

    public bool GetResposta() {
        float valor = GetValorConvertido(slider.value);

        return Mathf.Abs(valor - resposta) <= range;
    }
}
