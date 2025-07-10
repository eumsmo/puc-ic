using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndScreenUI : MonoBehaviour {
    public Text statusLabel, tempoLabel;
    public Text tituloArtigo, conteudoArtigo;

    public Transform performancePanel;
    public Text acertosLabel, errosLabel, totalLabel;

    void Awake() {
        // irAoArtigoButton.clicked += OnIrAoArtigoButtonClicked;
    }

    public void SetarValores(bool vitoria) {
        AssociacaoInfo info = GameManager.instance.GetInfo();
        float tempo = GameManager.instance.TempoPartida;

        int acertos = UIController.game.acertos;
        int erros = UIController.game.erros;
        float porcentagem = (acertos + erros) == 0 ? 0 : ( 1.0f * acertos / (acertos + erros)) * 100;
        string porcentagemStr = Mathf.Round(porcentagem) + "%";

        if (vitoria) {
            statusLabel.text = "Parabéns!";
            tempoLabel.text = "Você acertou " + porcentagemStr + " das questões!";
        } else {
            statusLabel.text = "Que pena!";
            tempoLabel.text = "Você acertou apenas " + porcentagemStr + " das questões!";
        }

        performancePanel.DOPunchScale(Vector3.one * 0.1f, 0.5f, 1, 0.5f);
        totalLabel.text = "<b>Total:</b> " + (UIController.game.acertos + UIController.game.erros);
        acertosLabel.text = "<b>Acertos:</b> " + UIController.game.acertos;
        errosLabel.text = "<b>Erros:</b> " + UIController.game.erros;
    }

    public void OnIrAoArtigoButtonClicked() {
        GameManager.instance.IrAoArtigo();
    }
}
