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
    public List<ValorGrafico> campos = new List<ValorGrafico>();

    // REF Values
    bool porcentagem;
    float v_min, v_max, range;
    
    public void Inicializar(GameUI game) {
        this.game = game;

        gameObject.SetActive(false);
    }

    public void HandleConfirmar() {
        UIController.game.OnAttemptButtonClicked();

        foreach (ValorGrafico campo in campos) {
            campo.Tentar();
        }
    }

    public void Comecar(Dados dados) {
        gameObject.SetActive(true);
        texto.text = dados.texto;

        Dados_GraficoInfo grafico = dados.grafico;

        foreach (Transform child in graficosHolder.transform) {
            Destroy(child.gameObject);
        }
        campos.Clear();

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
        range = dados.range;

        foreach (Dados_Grafico_Campo campo in grafico.campos) {
            ValorGrafico vg = GerarCampo(campo);
            campos.Add(vg);
        }
    }

    public ValorGrafico GerarCampo(Dados_Grafico_Campo campo) {
        GameObject campoObj = Instantiate(campoPrefab);
        ValorGrafico valorGrafico = campoObj.GetComponent<ValorGrafico>();
        valorGrafico.Inicializar(campo.nome, campo.valor, v_min, v_max, range, porcentagem);

        campoObj.transform.SetParent(graficosHolder.transform);
        campoObj.transform.localScale = Vector3.one;

        return valorGrafico;
    }

    public void Finalizar() {
        gameObject.SetActive(false);
    }

    public bool GetResposta() {
        int corretas = 0, incorretas = 0;
        foreach (ValorGrafico campo in campos) {
            if (campo.GetResposta()) corretas++;
            else incorretas++;
        }

        return corretas >= incorretas;
    }
}
