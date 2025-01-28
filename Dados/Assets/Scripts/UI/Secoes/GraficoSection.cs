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


    Label minValue, maxValue, halfValue;


    public string textoId, graficosHolderId, confirmarId;
    public string minValueId, maxValueId, halfValueId;
    public VisualTreeAsset campoPrefab;


    // REF Values
    bool porcentagem;
    float v_min, v_max;
    
    public void Inicializar(GameUI game) {
        this.game = game;

        var root = game.root;

        secao = root.Q<VisualElement>(secaoId);
        texto = root.Q<Label>(textoId);
        graficosHolder = root.Q<VisualElement>(graficosHolderId);
        confirmar = root.Q<Button>(confirmarId);

        minValue = root.Q<Label>(minValueId);
        maxValue = root.Q<Label>(maxValueId);
        halfValue = root.Q<Label>(halfValueId);
        
        secao.style.display = DisplayStyle.None;

        confirmar.clicked += HandleConfirmar;
    }

    public void HandleConfirmar() {
        UIController.game.OnAttemptButtonClicked();
    }

    public void Comecar(Dados dados) {
        secao.style.display = DisplayStyle.Flex;
        texto.text = dados.texto;

        Dados_GraficoInfo grafico = dados.grafico;
        graficosHolder.Clear();

        string min = "" + grafico.min;
        string half = "" + (grafico.min + grafico.max) / 2;
        string max = "" + grafico.max;

        if (grafico.porcentagem == true) {
            min = "0%";
            half = "50%";
            max = "100%";
        }

        minValue.text = min;
        maxValue.text = max;
        halfValue.text = half;

        porcentagem = grafico.porcentagem;
        v_min = grafico.min;
        v_max = grafico.max;

        foreach (Dados_Grafico_Campo campo in grafico.campos) {
            var campoEl = campoPrefab.Instantiate();
            Label texto = campoEl.Q<Label>("TextoGrafico");
            texto.text = campo.nome;

            VisualElement slider = campoEl.Q<VisualElement>("SliderGrafico");
            AddFillBar(slider);

            graficosHolder.Add(campoEl);

            slider.RegisterCallback<ChangeEvent<float>>(evt => OnSliderChange(slider, evt.newValue));
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

        Label valueView = new Label();
        valueView.name = "InformativoValor";
        valueView.AddToClassList("informativo_valor");
        valueView.text = "eba";
        slider.Add(valueView);

        // OnSliderChange(newDragger);
    }

    void OnSliderChange(VisualElement target) {
        OnSliderChange(target, (target as Slider).value);
    }

    void OnSliderChange(VisualElement target, float value) {
        VisualElement dragger = target.Q<VisualElement>("unity-dragger");
        VisualElement newDragger = target.Q<VisualElement>("NewDragger");
        Label valueView = target.Q<Label>("InformativoValor");

        Vector2 offset = new Vector2((newDragger.layout.width - dragger.layout.width) / 2, 0);

        Vector2 pos = target.LocalToWorld(dragger.transform.position);
        newDragger.transform.position = target.WorldToLocal(pos - offset);
        valueView.transform.position = target.WorldToLocal(pos);

        if (porcentagem) {
            valueView.text = "" + value + "%";
        } else {
            float min = v_min;
            float max = v_max;
            float valor = min + (max - min) * (value/100.0f);
            valueView.text = "" + valor;
        }
    }

    public void Finalizar() {
        secao.style.display = DisplayStyle.None;
    }

    public bool GetResposta() {
        return true;
    }
}
