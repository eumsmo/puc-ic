using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public interface SecaoDoJogo {
    void Inicializar(GameUI game);
    void Comecar(Dados dados);
    void Finalizar();
}
public class GameUI : MonoBehaviour {
    public VisualElement root;
    Label tempoLabel, perguntasQuantLabel;

    public GameObject[] secoes;
    SecaoDoJogo[] _secoes;
    public SecaoDoJogo secaoAtual;


    void Awake() {
        root = GetComponent<UIDocument>().rootVisualElement;
        tempoLabel = root.Q<Label>("Tempo");
        perguntasQuantLabel = root.Q<Label>("Rodadas");

        _secoes = new SecaoDoJogo[secoes.Length];
        int i = 0;
        foreach (GameObject secaoObj in secoes) {
            SecaoDoJogo secao = secaoObj.GetComponent<SecaoDoJogo>();
            secao.Inicializar(this);
            _secoes[i] = secao;
            i++;
        }
    }

    void Start() {
        GameManager.instance.controls.Game.Submit.performed += ctx => OnAttemptButtonClicked();
    }

    public void UpdateQual(int qual, int quantas) {
        perguntasQuantLabel.text = (qual+1) + "/" + quantas;
    }

    public void LoadSecao(int secaoId, Dados dados) {
        Debug.Log(secaoId);
        if (secaoAtual != null) secaoAtual.Finalizar();
        secaoAtual = _secoes[secaoId];
        Debug.Log(secaoAtual);
        secaoAtual.Comecar(dados);
    }

    public void UpdateTempo(int tempoSeconds) {
        int min = tempoSeconds / 60;
        int sec = tempoSeconds % 60;
        tempoLabel.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    void OnAttemptButtonClicked() {
    }

    public bool CheckIfWin() {
        return true;
    }
}
