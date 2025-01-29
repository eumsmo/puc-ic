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
        ArtigoInfo info = GameManager.instance.GetArtigoInfo();
        float tempo = GameManager.instance.TempoPartida;

        if (vitoria) {
            statusLabel.text = "Parabéns!";
            tempoLabel.text = "Resolvido em" + string.Format(" {0:00}:{1:00}", tempo / 60, tempo % 60);
        } else {
            statusLabel.text = "Que pena!";
            tempoLabel.text = "Tempo esgotado!";
        }

        tituloArtigo.text = info.titulo;
        conteudoArtigo.text = info.resumo;
    }

    public void OnIrAoArtigoButtonClicked() {
        GameManager.instance.IrAoArtigo();
    }
}
