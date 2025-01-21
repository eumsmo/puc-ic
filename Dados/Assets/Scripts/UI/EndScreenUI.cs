using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EndScreenUI : MonoBehaviour {
    Label statusLabel, tempoLabel;
    Label tituloArtigo, conteudoArtigo;
    Button irAoArtigoButton;

    void Awake() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        statusLabel = root.Q<Label>("VitoriaTitulo");
        tempoLabel = root.Q<Label>("VitoriaIndicadorTempo");

        tituloArtigo = root.Q<Label>("InfoTitulo");
        conteudoArtigo = root.Q<Label>("InfoConteudo");

        irAoArtigoButton = root.Q<Button>("IrAoArtigo");
        irAoArtigoButton.clicked += OnIrAoArtigoButtonClicked;
    }

    public void SetarValores(bool vitoria) {
        DadosInfo info = GameManager.instance.GetInfo();
        float tempo = GameManager.instance.TempoPartida;

        if (vitoria) {
            statusLabel.text = "Parabéns!";
            tempoLabel.text = "Você acertou 50% dos problemas em" + string.Format(" {0:00}:{1:00}", tempo / 60, tempo % 60);
        } else {
            statusLabel.text = "Que pena!";
            tempoLabel.text = "Tempo esgotado!";
        }

        tituloArtigo.text = info.titulo;
        // conteudoArtigo.text = info.resumo;
    }

    public void OnIrAoArtigoButtonClicked() {
        GameManager.instance.IrAoArtigo();
    }
}
