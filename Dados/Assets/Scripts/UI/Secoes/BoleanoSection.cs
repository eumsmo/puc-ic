using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoleanoSection : MonoBehaviour, SecaoDoJogo {
    VisualElement secao;
    public string secaoId;
    GameUI game;

    Label texto;
    Button sim, nao;

    public string textoId, graficosHolderId, simId, naoId;
    
    public void Inicializar(GameUI game) {
        this.game = game;

        var root = game.root;

        secao = root.Q<VisualElement>(secaoId);
        texto = root.Q<Label>(textoId);
        sim = root.Q<Button>(simId);
        nao = root.Q<Button>(naoId);

        secao.style.display = DisplayStyle.None;

        sim.clicked += HandleConfirmar;
        nao.clicked += HandleConfirmar;
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
