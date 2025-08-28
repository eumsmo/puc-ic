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

    string erroDetails = "";

    public void Inicializar(GameUI game) {
        this.game = game;

        gameObject.SetActive(false);
    }

    bool confirmou = false;

    public void HandleConfirmar() {
        if (confirmou) return;
        confirmou = true;

        game.StartCoroutine(HandleConfirmarAnimation());
    }

    IEnumerator HandleConfirmarAnimation() {
        foreach (ValorGrafico campo in campos) {
            campo.Tentar(0.75f);
            yield return new WaitForSeconds(0.8f);
        }

        UIController.game.OnAttemptButtonClicked();
    }

    public void Comecar(Dados dados) {
        gameObject.SetActive(true);
        confirmou = false;

        texto.text = dados.texto;

        Dados_Range rangeInfo = dados.range;

        foreach (Transform child in graficosHolder.transform) {
            Destroy(child.gameObject);
        }
        campos.Clear();

        string min = "" + rangeInfo.min;
        string half = "" + (rangeInfo.min + rangeInfo.max) / 2;
        string max = "" + rangeInfo.max;

        if (rangeInfo.porcentagem == true) {
            min = "0%";
            half = "50%";
            max = "100%";
        }

        minValue.text = min;
        maxValue.text = max;
        halfValue.text = half;

        porcentagem = rangeInfo.porcentagem;
        v_min = porcentagem ? 0 : rangeInfo.min;
        v_max = porcentagem ? 1 : rangeInfo.max;
        range = rangeInfo.range;

        foreach (Dados_Grafico_Campo campo in dados.grafico) {
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
        
        Vector3 pos = campoObj.transform.localPosition;
        pos.z = 0;
        campoObj.transform.localPosition = pos;

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

        if (corretas >= incorretas) return true;

        erroDetails = "Você acertou menos da metade dos campos!";
        return false;
    }
    
    public string GetErroDetails() {
        return erroDetails;
    }
}
