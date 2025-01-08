using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GraficoSection : MonoBehaviour, SecaoDoJogo {
    VisualElement secao;
    public string secaoId;
    GameUI game;

    Label texto;
    VisualElement graficosHolder;
    Button confirmar;

    public string textoId, graficosHolderId, confirmarId;
    
    public void Inicializar(GameUI game) {
        this.game = game;

        var root = game.root;

        secao = root.Q<VisualElement>(secaoId);
        texto = root.Q<Label>(textoId);
        graficosHolder = root.Q<VisualElement>(graficosHolderId);
        confirmar = root.Q<Button>(confirmarId);
        
        secao.style.display = DisplayStyle.None;

        confirmar.clicked += HandleConfirmar;
    }

    public void HandleConfirmar() {
        GameManager.instance.ProximaPergunta();
    }

    public void Comecar(Dados dados) {
        secao.style.display = DisplayStyle.Flex;
    }

    public void Finalizar() {
        secao.style.display = DisplayStyle.None;
    }
}
