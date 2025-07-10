using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Radishmouse;

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
    Vector2[] pontos = new Vector2[2];

    public Vector3 pos, localPos;
    public Vector2 rectPos;

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

}
