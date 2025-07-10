using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct IdDIR {
    public string id;
    public bool horizontal;
}

public class CustomSlider : MonoBehaviour {
    public string sliderId = "CustomSlider";
    public string barClass = "slider_bar";
    public VisualElement slider, dragger;
    public VisualElement fill;

    public IdDIR[] ids;

    public void Start() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        foreach (IdDIR id in ids) {
            slider = root.Q<VisualElement>(id.id);
            dragger = slider.Q<VisualElement>("unity-dragger");
            AddFillBar(id.horizontal);
        }

    }

    void AddFillBar(bool horizontal = true) {
        fill = new VisualElement();
        fill.AddToClassList(barClass);
        dragger.Add(fill);
    }
}
