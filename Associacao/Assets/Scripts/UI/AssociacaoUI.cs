using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Radishmouse;
using System.Collections.Generic;

public enum LadoConexao {
    Esquerda,
    Direita
}

public class AssociacaoUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Color cor;
    public Image imagemFundo;
    public Text texto;
    public Transform ladoEsquerdo, ladoDireito;

    public Transform ladoSelecionado => ladoConexao == LadoConexao.Esquerda ? ladoEsquerdo : ladoDireito;
    public Vector2 posicaoLadoSelecionado => (Vector2)(ladoSelecionado.localPosition + transform.localPosition + transform.parent.localPosition);

    public LadoConexao ladoConexao;
    public UILineRenderer linhaConexao;

    [SerializeField]
    protected List<AssociacaoUI> associacoesConectadas = new List<AssociacaoUI>();
    public List<Conexao> conexoes = new List<Conexao>();

    public System.Action<AssociacaoUI> onAssociacaoClicada;

    public string palavra => texto.text;


    public void OnPointerDown(PointerEventData eventData) {
        onAssociacaoClicada?.Invoke(this);
        if (UIController.game.state != GameUI.CurrentGameState.Playing) return;
        ComecarAssociacao();
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (UIController.game.state != GameUI.CurrentGameState.Playing) return;
        ConexaoManager.instance.TerminarAssociacao(this);
    }


    public void SetarTexto(string palavra) {
        texto.text = palavra;
    }

    public void ComecarAssociacao() {
        ConexaoManager.instance.ComecarAssociacao(this);
    }

    public void SetarCor(Color cor) {
        this.cor = cor;
        imagemFundo.color = cor;
    }


    public void AdicionarAssociacao(AssociacaoUI associacao, Conexao conexao = null) {
        if (!associacoesConectadas.Contains(associacao)) {
            associacoesConectadas.Add(associacao);
        }

        if (conexao != null && !conexoes.Contains(conexao)) {
            conexoes.Add(conexao);
        }
    }

    public void RemoverAssociacao(AssociacaoUI associacao) {
        if (associacoesConectadas.Contains(associacao)) {
            associacoesConectadas.Remove(associacao);
        }

        foreach (Conexao conexao in conexoes) {
            if (conexao.associacao1 == associacao || conexao.associacao2 == associacao) {
                conexao.Destruir();
            }
        }
    }

    public bool EstaConectadaCom(AssociacaoUI associacao) {
        return associacoesConectadas.Contains(associacao);
    }

    public void ClearAssociacoes() {
        associacoesConectadas.Clear();
        conexoes.Clear();
    }


    #region Feedback

    bool modoFeedback = false;
    public void SetarModoFeedback() {
        modoFeedback = true;
        
        foreach (Conexao conexao in conexoes) {
            conexao.OcultarParaFeedback();
        }
    }

    public void MostrarConexoes() {
        if (!modoFeedback) return;

        foreach (Conexao conexao in conexoes) {
            conexao.transform.SetAsLastSibling();
            conexao.DesocultarParaFeedback();
        }
    }

    #endregion

}
