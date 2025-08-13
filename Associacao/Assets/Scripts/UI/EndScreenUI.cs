using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndScreenUI : MonoBehaviour {
    public Text statusLabel, tempoLabel;
    public Text tituloArtigo, conteudoArtigo;

    public Transform performancePanel;
    public Text acertosLabel, errosLabel, naoMarcadosLabel, totalLabel;
    public GameObject statusPanel, respostaPanel;


    [Header("Associacoes")]
    public Transform conexoesHolder;
    public Transform coluna1, coluna2;
    AssociacaoInfo info;

    OrderedDictionary tituloBaseadoNaPorcentagem = new OrderedDictionary();

    void Awake() {
        
        // irAoArtigoButton.clicked += OnIrAoArtigoButtonClicked;
    }

    void InicializarDicionario() {
        if (tituloBaseadoNaPorcentagem.Count > 0) {
            return; // Já inicializado
        }

        tituloBaseadoNaPorcentagem.Add(100f, "Incrível!");
        tituloBaseadoNaPorcentagem.Add(80f, "Parabéns!");
        tituloBaseadoNaPorcentagem.Add(60f, "Bom trabalho!");
        tituloBaseadoNaPorcentagem.Add(40f, "Quase lá!");
        tituloBaseadoNaPorcentagem.Add(20f, "Eita!");
        tituloBaseadoNaPorcentagem.Add(0.001f, "Poxa...");
        tituloBaseadoNaPorcentagem.Add(0f, "Nenhuma?");
    }

    public void SetarValores(bool vitoria) {
        AssociacaoInfo info = GameManager.instance.GetInfo();
        float tempo = GameManager.instance.TempoPartida;

        int acertos = UIController.game.acertos;
        int naoFeitos = UIController.game.naoFeitos;
        float porcentagem = (acertos + naoFeitos) == 0 ? 0 : ( 1.0f * acertos / (acertos + naoFeitos)) * 100;
        string porcentagemStr = Mathf.Round(porcentagem) + "%";

        string status = "";

        InicializarDicionario(); // Inicializa dicionario, apenas se não houver

        foreach (DictionaryEntry item in tituloBaseadoNaPorcentagem) {
            float min = (float)item.Key;
            string label = (string)item.Value;

            if (porcentagem >= min) {
                status = label;
                break;
            }
        }

        statusLabel.text = status;

        if (porcentagem >= 60) {
            tempoLabel.text = "Você acertou " + porcentagemStr + " das conexões!";
        } else if (porcentagem > 0) {
            tempoLabel.text = "Você acertou apenas " + porcentagemStr + " das conexões!";
        } else {
            tempoLabel.text = "Você não acertou nenhuma conexão.";
        }

        performancePanel.DOPunchScale(Vector3.one * 0.1f, 0.5f, 1, 0.5f);
        totalLabel.text = "<b>Total:</b> " + (UIController.game.acertos + UIController.game.naoFeitos);
        acertosLabel.text = "<b>Acertos:</b> " + UIController.game.acertos;
        errosLabel.text = "<b>Erros:</b> " + UIController.game.erros;
        naoMarcadosLabel.text = "<b>Não Marcados:</b> " + UIController.game.naoFeitos;

        MostrarStatus();

        GerarAssociacoesCorretas(info);
    }

    public void OnIrAoArtigoButtonClicked() {
        GameManager.instance.IrAoArtigo();
    }


    public void GerarAssociacoesCorretas(AssociacaoInfo info) {
        Transform coluna1Jogo = UIController.game.coluna1;
        Transform coluna2Jogo = UIController.game.coluna2;

        this.info = info;

        foreach (Transform child in coluna1) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in coluna2) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in conexoesHolder) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in coluna1Jogo) {
            GameObject associacao = Instantiate(child.gameObject, coluna1);
            AssociacaoUI associacaoUI = associacao.GetComponent<AssociacaoUI>();
            associacaoUI.ClearAssociacoes();

            associacaoUI.onAssociacaoClicada += MostrarConexoes;
        }

        foreach (Transform child in coluna2Jogo) {
            GameObject associacao = Instantiate(child.gameObject, coluna2);
            AssociacaoUI associacaoUI = associacao.GetComponent<AssociacaoUI>();
            associacaoUI.ClearAssociacoes();

            associacaoUI.onAssociacaoClicada += MostrarConexoes;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(coluna1.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(coluna2.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(coluna1.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(coluna2.GetComponent<RectTransform>());

        Invoke("GerarConexoes", 0.1f);
    }

    public void GerarConexoes() {
        Dictionary<string, AssociacaoUI> associacoesColuna1 = new Dictionary<string, AssociacaoUI>();

        foreach (Transform child in conexoesHolder) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in coluna1) {
            AssociacaoUI associacaoUI = child.GetComponent<AssociacaoUI>();
            associacoesColuna1[associacaoUI.palavra] = associacaoUI;

            associacaoUI.ClearAssociacoes();
            associacaoUI.SetarModoFeedback();
        }

        foreach (Transform child in coluna2) {
            AssociacaoUI associacaoUI = child.GetComponent<AssociacaoUI>();
            associacaoUI.ClearAssociacoes();

            string[] palavras = info.GetPalavras(associacaoUI.palavra);
            Debug.Log("AssociacaoUI: " + associacaoUI.palavra + " - Palavras: " + string.Join(", ", palavras));
            foreach (string conexao in palavras) {
                if (!associacoesColuna1.ContainsKey(conexao)) {
                    Debug.LogWarning("Conexão ["+ conexao + "] não encontrada para palavra [" + associacaoUI.palavra + "]");
                    continue;
                }

                AssociacaoUI associacaoColuna1 = associacoesColuna1[conexao];
                CriarConexao(associacaoColuna1, associacaoUI);
            }

            associacaoUI.SetarModoFeedback();
        }
    }

    public GameObject CriarConexao(AssociacaoUI associacao1, AssociacaoUI associacao2) {
        GameObject conexaoPrefab = ConexaoManager.instance.conexaoPrefab;

        GameObject conexaoObj = Instantiate(conexaoPrefab, conexoesHolder);
        Conexao conexao = conexaoObj.GetComponent<Conexao>();

        conexao.SetarAssociacao(associacao1, associacao2);

        return conexaoObj;
    }

    AssociacaoUI mostrandoConexoes;

    public void MostrarConexoes(AssociacaoUI associacao) {
        if (mostrandoConexoes != null) {
            mostrandoConexoes.SetarModoFeedback();
        }

        mostrandoConexoes = associacao;
        associacao.MostrarConexoes();
    }


    public void MostrarStatus() {
        statusPanel.SetActive(true);
        respostaPanel.SetActive(false);
    }

    public void MostrarResposta() {
        StartCoroutine(MostrarRespostaCoroutine());
    }

    IEnumerator MostrarRespostaCoroutine() {
        statusPanel.SetActive(false);
        respostaPanel.SetActive(true);
        yield return null;
        GerarConexoes();
    }
}
