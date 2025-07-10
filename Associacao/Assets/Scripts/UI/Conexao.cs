using Radishmouse;
using UnityEngine;
using UnityEngine.EventSystems;

public class Conexao : MonoBehaviour, IPointerClickHandler {
    public AssociacaoUI associacao1, associacao2;
    public UILineRenderer linhaConexao;
    Vector2[] pontos = new Vector2[2];

    public void OnPointerClick(PointerEventData eventData) {
        // Destruir a conexão ao clicar nela
        Debug.Log("Conexão clicada, destruindo...");
        ConexaoManager.instance.DestruirConexao(this);
    }

    public void SetarPosicaoInicial(Vector2 posicaoInicial) {
        pontos[0] = posicaoInicial;
        if (pontos[1] == Vector2.zero) pontos[1] = posicaoInicial;
        linhaConexao.points = pontos;
        linhaConexao.SetVerticesDirty();
    }

    public void SetarPosicaoFinal(Vector2 posicaoFinal) {
        pontos[1] = posicaoFinal;
        linhaConexao.points = pontos;
        linhaConexao.SetVerticesDirty();
    }

    public void SetarCor1(Color cor) {
        if (linhaConexao == null) return;
        linhaConexao.color = cor;
    }

    public void SetarCor2(Color cor) {
        if (linhaConexao == null) return;
        linhaConexao.color2 = cor;
    }

    public void SetarAssociacao(AssociacaoUI associacao1, AssociacaoUI associacao2) {
        if (associacao1 == null || associacao2 == null) return;
        this.associacao1 = associacao1;
        this.associacao2 = associacao2;

        // Atualizar cores
        linhaConexao.color = associacao1.cor;
        linhaConexao.color2 = associacao2.cor;
    }
}
