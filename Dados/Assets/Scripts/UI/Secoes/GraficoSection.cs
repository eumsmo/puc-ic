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


    Label minValue, maxValue;


    public string textoId, graficosHolderId, confirmarId;
    public string minValueId, maxValueId;
    public VisualTreeAsset campoPrefab;
    
    public void Inicializar(GameUI game) {
        this.game = game;

        var root = game.root;

        secao = root.Q<VisualElement>(secaoId);
        texto = root.Q<Label>(textoId);
        graficosHolder = root.Q<VisualElement>(graficosHolderId);
        confirmar = root.Q<Button>(confirmarId);

        minValue = root.Q<Label>(minValueId);
        maxValue = root.Q<Label>(maxValueId);
        
        secao.style.display = DisplayStyle.None;

        confirmar.clicked += HandleConfirmar;
    }

    public void HandleConfirmar() {
        GameManager.instance.ProximaPergunta();
    }

    public void Comecar(Dados dados) {
        secao.style.display = DisplayStyle.Flex;
        texto.text = dados.texto;

        Dados_GraficoInfo grafico = dados.grafico;
        graficosHolder.Clear();

        string min = "" + grafico.min;
        string max = "" + grafico.max;

        if (grafico.min == 0 && grafico.max == 1) {
            min = "0%";
            max = "100%";
        }

        minValue.text = min;
        maxValue.text = max;

        foreach (Dados_Grafico_Campo campo in grafico.campos) {
            var campoEl = campoPrefab.Instantiate();
            Label texto = campoEl.Q<Label>("TextoGrafico");
            texto.text = campo.nome;

            VisualElement slider = campoEl.Q<VisualElement>("SliderGrafico");
            AddFillBar(slider);

            graficosHolder.Add(campoEl);

            slider.RegisterCallback<ChangeEvent<float>>(evt => OnSliderChange(slider));
            slider.RegisterCallback<GeometryChangedEvent>(evt => OnSliderChange(slider));
        }
    }

    void AddFillBar(VisualElement slider) {
        VisualElement dragger = slider.Q<VisualElement>("unity-dragger");
        VisualElement fill = new VisualElement();
        fill.name = "SliderFill";
        fill.AddToClassList("slider_bar");
        dragger.Add(fill);

        VisualElement newDragger = new VisualElement();
        slider.Add(newDragger);
        newDragger.name = "NewDragger";
        newDragger.AddToClassList("new_dragger");
        newDragger.pickingMode = PickingMode.Ignore;

        // OnSliderChange(newDragger);
    }

    void OnSliderChange(VisualElement target) {
        VisualElement dragger = target.Q<VisualElement>("unity-dragger");
        VisualElement newDragger = target.Q<VisualElement>("NewDragger");

        Vector2 offset = new Vector2((newDragger.layout.width - dragger.layout.width) / 2, 0);
        Vector2 pos = target.LocalToWorld(dragger.transform.position);
        newDragger.transform.position = target.WorldToLocal(pos - offset);
    }

    public void Finalizar() {
        secao.style.display = DisplayStyle.None;
    }
}
