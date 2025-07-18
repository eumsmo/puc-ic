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


    public void OnPointerDown(PointerEventData eventData) {
        ComecarAssociacao();
    }

    public void OnPointerUp(PointerEventData eventData) {
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


    public void AdicionarAssociacao(AssociacaoUI associacao) {
        if (!associacoesConectadas.Contains(associacao)) {
            associacoesConectadas.Add(associacao);
        }
    }

    public void RemoverAssociacao(AssociacaoUI associacao) {
        if (associacoesConectadas.Contains(associacao)) {
            associacoesConectadas.Remove(associacao);
        }
    }

    public bool EstaConectadaCom(AssociacaoUI associacao) {
        return associacoesConectadas.Contains(associacao);
    }

}
