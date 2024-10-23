using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour {
    public string textHolderName = "TextHolder";
    public string attemptButtonName = "ButtonTentativa";
    public string inputFieldName = "InputTentativa";

    public string textoClassName = "texto";
    public string ocultoClassName = "oculto";

    public VisualTreeAsset tentativaTemplate;

    VisualElement textHolder;
    ScrollView tentativasList;
    Button attemptButton;
    TextField inputField;
    Label tempoLabel;

    // Palavras que por padrão não serão ocultas
    string[] palavrasNaoOcultas = new string[] {
        "a", "o", "e", "é", "de", "da", "do", "em", "no", "na", 
        "um", "uma", "uns", "umas", "para", "com", "por", "como", 
        "mas", "ou", "se", "não", "mais", "muito", "também", 
        "quando", "onde", "quem", "qual", "quais", "porque",
        "pois", "então", "assim", "logo"
    };

    List<string> tentativas = new List<string>();

    void Awake() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        textHolder = root.Q<VisualElement>(textHolderName);
        tempoLabel = root.Q<Label>("Tempo");
        tentativasList = root.Q<ScrollView>("TentativasList");

        tentativasList.Clear();
        tentativas.Clear();

        inputField = root.Q<TextField>(inputFieldName);

        attemptButton = root.Q<Button>(attemptButtonName);
        attemptButton.clicked += OnAttemptButtonClicked;
    }

    public void GerarStopWords(string[] words) {
        palavrasNaoOcultas = words;
    }

    public void UpdateTempo(int tempoSeconds) {
        int min = tempoSeconds / 60;
        int sec = tempoSeconds % 60;
        tempoLabel.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    void OnAttemptButtonClicked() {
        string tentativa = inputField.value;
        inputField.value = "";

        if (tentativas.Contains(tentativa)) return;

        int encontrados = 0;
        foreach (VisualElement child in textHolder.Children()) {
            Label label = (Label)child;
            string palavra = (string)label.userData;

            if (palavra == tentativa) {
                label.text = palavra;
                label.RemoveFromClassList(ocultoClassName);
                encontrados++;
            }
        }

        ArmazenarTentativa(tentativa, encontrados);
    }

    public void ArmazenarTentativa(string tentativa, int resultados) {
        var tentativaEl = tentativaTemplate.Instantiate();
        tentativaEl.Q<Label>("palavra").text = tentativa;
        tentativaEl.Q<Label>("numero").text = "" + resultados;

        tentativasList.Add(tentativaEl);
        tentativas.Add(tentativa);
    }

    public void GerarPalavras(List<string> palavras) {
        textHolder.Clear();

        Debug.Log(string.Join("\n", palavrasNaoOcultas));

        foreach (string palavra in palavras) {
            Label label = GerarPalavra(palavra);

            if (OcultarPalavra(palavra))
                SetPalavraOculta(label);
        }
    }

    public Label GerarPalavra(string texto) {
        Label label = new Label(texto);
        label.AddToClassList(textoClassName);
        textHolder.Add(label);
        return label;
    }

    public bool OcultarPalavra(string palavra) {
        foreach (string naoOculta in palavrasNaoOcultas) {
            if (palavra == naoOculta) {
                return false;
            }
        }

        return true;
    }

    public void SetPalavraOculta(Label label) {
        string palavra = label.text;
        label.userData = palavra;
            
        int length = palavra.Length;
        string palavraOculta = "";
        for (int i = 0; i < length; i++) {
            palavraOculta += "_";
        }

        label.text = palavraOculta;
        label.AddToClassList(ocultoClassName);
    }
}
