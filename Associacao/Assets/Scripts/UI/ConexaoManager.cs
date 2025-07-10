using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConexaoManager : MonoBehaviour {
    public static ConexaoManager instance;
    public Canvas canvas;
    public GameObject conexaoPrefab;


    public Transform conexaoHolder;
    public AssociacaoUI primeiraAssociacao;
    Conexao conexaoAtual;


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

        GameObject conexaoObj = Instantiate(conexaoPrefab, conexaoHolder);
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

        conectando = false;
        conexaoAtual = null;
        primeiraAssociacao = null;
    }

    public void DestruirConexao(Conexao conexao) {
        if (conexao == null) return;
        Destroy(conexao.gameObject);
    }
}
