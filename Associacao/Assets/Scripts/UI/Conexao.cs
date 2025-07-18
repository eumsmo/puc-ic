using Radishmouse;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Conexao : MonoBehaviour, IPointerDownHandler {
    public AssociacaoUI associacao1, associacao2;
    public UILineRenderer linhaConexao;
    Vector2[] pontos = new Vector2[2];

    [Header("Colisor de Raycast")]
    public Image colisorRaycast;
    public float paddingThickness;

    [ContextMenu("Atualizar Colisor")]
    public void RefreshColisor() {
        if (colisorRaycast == null) return;

        Vector2[] pontosLinha = linhaConexao.points;
        colisorRaycast.rectTransform.anchoredPosition = pontosLinha[0];

        // Atualizar a posição e o tamanho do colisor
        float altura = linhaConexao.thickness + paddingThickness * 2;
        float largura = Vector2.Distance(pontosLinha[0], pontosLinha[1]);
        float angulo = Vector2.SignedAngle(Vector2.right, pontosLinha[1] - pontosLinha[0]);

        colisorRaycast.rectTransform.sizeDelta = new Vector2(largura, altura);
        colisorRaycast.rectTransform.localEulerAngles = new Vector3(0, 0, angulo);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Conexão clicada, destruindo...");
        Destruir();
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
        
        if (associacao1.EstaConectadaCom(associacao2) || associacao2.EstaConectadaCom(associacao1)) {
            Debug.LogWarning("As associações já estão conectadas.");
            Destruir();
            return;
        }

        // Atualizar cores
        linhaConexao.color = associacao1.cor;
        linhaConexao.color2 = associacao2.cor;

        RefreshColisor();

        associacao1.AdicionarAssociacao(associacao2);
        associacao2.AdicionarAssociacao(associacao1);

        this.associacao1 = associacao1;
        this.associacao2 = associacao2;
    }

    public void Destruir() {
        if (associacao1 != null) associacao1.RemoverAssociacao(associacao2);
        if (associacao2 != null) associacao2.RemoverAssociacao(associacao1);
        ConexaoManager.instance.DestruirConexao(this);
    }
}
