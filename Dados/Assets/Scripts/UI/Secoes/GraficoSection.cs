using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraficoSection : MonoBehaviour, SecaoDoJogo {
    GameUI game;

    public Text texto;
    public GameObject graficosHolder;


    public Text minValue, maxValue, halfValue;

    public GameObject campoPrefab;


    // REF Values
    bool porcentagem;
    float v_min, v_max;
    
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

        Dados_GraficoInfo grafico = dados.grafico;

        foreach (Transform child in graficosHolder.transform) {
            Destroy(child.gameObject);
        }

        string min = "" + grafico.min;
        string half = "" + (grafico.min + grafico.max) / 2;
        string max = "" + grafico.max;

        if (grafico.porcentagem == true) {
            min = "0%";
            half = "50%";
            max = "100%";
        }

        minValue.text = min;
        maxValue.text = max;
        halfValue.text = half;

        porcentagem = grafico.porcentagem;
        v_min = grafico.min;
        v_max = grafico.max;

        foreach (Dados_Grafico_Campo campo in grafico.campos) {
            GerarCampo(campo);
        }
    }

    public Slider GerarCampo(Dados_Grafico_Campo campo) {
        GameObject campoObj = Instantiate(campoPrefab);

        Text label = campoObj.transform.Find("Label").GetComponent<Text>();
        label.text = campo.nome;

        campoObj.transform.SetParent(graficosHolder.transform);
        campoObj.transform.localScale = Vector3.one;

        Slider slider = campoObj.GetComponent<Slider>();
        slider.onValueChanged.AddListener((float value) => OnSliderChange(slider, value));

        return slider;
    }

    void OnSliderChange(Slider slider, float val) {
        float value = slider.value;

        value = Mathf.Round(value*100.0f)/100.0f;
        
        Text valorInfo = slider.transform.Find("Handle Slide Area/Handle/ValorLabel").GetComponent<Text>();

        if (porcentagem) {
            valorInfo.text = "" + (value*100) + "%";
        } else {
            float min = v_min;
            float max = v_max;
            float valor = min + (max - min) * (value/100.0f);
            valorInfo.text = "" + valor;
        }
    }

    public void Finalizar() {
        gameObject.SetActive(false);
    }

    public bool GetResposta() {
        return true;
    }
}
