using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenUI : MonoBehaviour {
    public Text statusLabel, tempoLabel;
    public Text tituloArtigo, conteudoArtigo;

    void Awake() {
        // irAoArtigoButton.clicked += OnIrAoArtigoButtonClicked;
    }

    public void SetarValores(bool vitoria) {
        DadosInfo info = GameManager.instance.GetInfo();
        float tempo = GameManager.instance.TempoPartida;

        int acertos = UIController.game.acertos;
        int erros = UIController.game.erros;
        float porcentagem = (acertos + erros) == 0 ? 0 : ( 1.0f * acertos / (acertos + erros)) * 100;
        string porcentagemStr = Mathf.Round(porcentagem) + "%";

        if (vitoria) {
            statusLabel.text = "Parabéns!";
            tempoLabel.text = "Você acertou " + porcentagemStr + " dos problemas!";
        } else {
            statusLabel.text = "Que pena!";
            tempoLabel.text = "Você acertou apenas " + porcentagemStr + " dos problemas!";
        }

        tituloArtigo.text = info.titulo;
        // conteudoArtigo.text = info.resumo;
    }

    public void OnIrAoArtigoButtonClicked() {
        GameManager.instance.IrAoArtigo();
    }
}
