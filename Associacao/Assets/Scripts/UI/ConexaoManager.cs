using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConexaoManager : MonoBehaviour {
    public static ConexaoManager instance;
    public Canvas canvas;
    public GameObject conexaoPrefab;


    public Transform conexaoHolder, duranteConexaoHolder;
    public AssociacaoUI primeiraAssociacao;
    Conexao conexaoAtual;

    int quantidadeConexoes = 0;
    bool conectando = false;


    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void Update() {
        if (UIController.game.state != GameUI.CurrentGameState.Playing) return;
        if (!conectando || primeiraAssociacao == null) {
            return;
        }


        // Pegar posição do mouse na UI
        Vector2 posicaoMouse = Input.mousePosition;
        AcharElementoEmBaixo(posicaoMouse);

        Vector2 posicaoLocal;
    
        if (associacaoEmBaixo != null && associacaoEmBaixo.ladoConexao != primeiraAssociacao.ladoConexao) {
            posicaoLocal = associacaoEmBaixo.posicaoLadoSelecionado;
            conexaoAtual.SetarPosicaoFinal(posicaoLocal);
            conexaoAtual.SetarCor2(associacaoEmBaixo.cor);
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle (
            conexaoHolder.transform as RectTransform,
            posicaoMouse,
            canvas.worldCamera,
            out posicaoLocal
        );


        // Atualizar posição do cursor
        conexaoAtual.SetarPosicaoFinal(posicaoLocal);
        conexaoAtual.SetarCor2(primeiraAssociacao.cor);
    }

    public void Reset() {
        foreach (Transform child in conexaoHolder) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in duranteConexaoHolder) {
            Destroy(child.gameObject);
        }

        conectando = false;
        primeiraAssociacao = null;
        associacaoEmBaixo = null;
        conexaoAtual = null;

        quantidadeConexoes = 0;
    }

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    AssociacaoUI associacaoEmBaixo;

    public void AcharElementoEmBaixo(Vector2 posicaoMouse) {
        var pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = posicaoMouse;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results) {
            if (result.gameObject.TryGetComponent<AssociacaoUI>(out AssociacaoUI associacao)) {
                associacaoEmBaixo = associacao;
                return;
            }
        }

        associacaoEmBaixo = null;
    }

    public void ComecarAssociacao(AssociacaoUI associacao) {
        if (primeiraAssociacao != null) {
            return;
        }

        GameObject conexaoObj = Instantiate(conexaoPrefab, duranteConexaoHolder);
        conexaoAtual = conexaoObj.GetComponent<Conexao>();

        Vector2 posicaoInicial = associacao.posicaoLadoSelecionado;
        conexaoAtual.SetarPosicaoInicial(posicaoInicial);
        conexaoAtual.SetarCor1(associacao.cor);

        primeiraAssociacao = associacao;
        conectando = true;
    }

    public void TerminarAssociacao(AssociacaoUI associacao) {
        if (!conectando || associacao == null) {
            return;
        }

        if (associacaoEmBaixo == null || associacaoEmBaixo.ladoConexao == primeiraAssociacao.ladoConexao) {
            Destroy(conexaoAtual.gameObject);
        } else {
            conexaoAtual.SetarAssociacao(primeiraAssociacao, associacaoEmBaixo);
        }

        conexaoAtual.transform.SetParent(conexaoHolder, true);

        conectando = false;
        conexaoAtual = null;
        primeiraAssociacao = null;

        quantidadeConexoes++;
        AtualizarQuantidadeConexoes();
    }

    public void RegistrarConexaoDestruida() {
        quantidadeConexoes--;
        AtualizarQuantidadeConexoes();
    }

    public Conexao[] GetConexoes() {
        List<Conexao> conexoes = new List<Conexao>();
        foreach (Transform child in conexaoHolder) {
            if (child.TryGetComponent(out Conexao conexao)) {
                conexoes.Add(conexao);
            }
        }

        quantidadeConexoes = conexoes.Count;
        AtualizarQuantidadeConexoes();
        return conexoes.ToArray();
    }

    public void AtualizarQuantidadeConexoes() {
        UIController.game.AtualizarQuantidadeConexoes(quantidadeConexoes);
    }
}
