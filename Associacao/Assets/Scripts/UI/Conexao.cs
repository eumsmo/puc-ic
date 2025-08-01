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

    [Header("Feedback")]
    public Color corretoColor = Color.green;
    public Color erradoColor = Color.red;
    public float alphaOculto = 0.5f;

    public string palavra1 => associacao1?.palavra;
    public string palavra2 => associacao2?.palavra;



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
        if (UIController.game.state != GameUI.CurrentGameState.Playing) return;
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
        if (associacao1 == null || associacao2 == null) {
            Debug.LogWarning("Tentativa de conectar associações nulas.");
            Destruir();
            return;
        }
        
        if (associacao1.EstaConectadaCom(associacao2) || associacao2.EstaConectadaCom(associacao1)) {
            Debug.LogWarning("As associações já estão conectadas.");
            Destruir();
            return;
        }

        // Atualizar cores
        linhaConexao.color = associacao1.cor;
        linhaConexao.color2 = associacao2.cor;

        pontos[0] = associacao1.posicaoLadoSelecionado;
        pontos[1] = associacao2.posicaoLadoSelecionado;
        linhaConexao.points = pontos;
        linhaConexao.SetVerticesDirty();

        RefreshColisor();

        associacao1.AdicionarAssociacao(associacao2, this);
        associacao2.AdicionarAssociacao(associacao1, this);

        this.associacao1 = associacao1;
        this.associacao2 = associacao2;
    }
    
    bool destruindo = false;
    public void Destruir() {
        if (destruindo) return;
        destruindo = true;
        
        if (associacao1 != null) associacao1.RemoverAssociacao(associacao2);
        if (associacao2 != null) associacao2.RemoverAssociacao(associacao1);
        Destroy(gameObject);
    }



    #region Feedback

    public void OcultarParaFeedback() {
        if (linhaConexao == null) return;
        
        linhaConexao.color = new Color(linhaConexao.color.r, linhaConexao.color.g, linhaConexao.color.b, alphaOculto);
        linhaConexao.color2 = new Color(linhaConexao.color2.r, linhaConexao.color2.g, linhaConexao.color2.b, alphaOculto);
    }

    public void DesocultarParaFeedback() {
        if (linhaConexao == null) return;

        linhaConexao.color = new Color(linhaConexao.color.r, linhaConexao.color.g, linhaConexao.color.b, 1f);
        linhaConexao.color2 = new Color(linhaConexao.color2.r, linhaConexao.color2.g, linhaConexao.color2.b, 1f);
    }

    public void MostrarFeedback(bool acerto) {
        if (linhaConexao == null) return;

        Color cor = acerto ? corretoColor : erradoColor;
        linhaConexao.color = cor;
        linhaConexao.color2 = cor;
    }

    #endregion
}
