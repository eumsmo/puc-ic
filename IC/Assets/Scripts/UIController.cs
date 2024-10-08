using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {
    public static UIController instance;

    public string textHolderName;
    public string attemptButtonName;
    public string inputFieldName;
    public string testButtonName;

    public string textoClassName;
    public string ocultoClassName;

    VisualElement textHolder;
    Button attemptButton;
    TextField inputField;

    // Palavras que por padrão não serão ocultas
    string[] palavrasNaoOcultas = new string[] {
        "a", "o", "e", "é", "de", "da", "do", "em", "no", "na", 
        "um", "uma", "uns", "umas", "para", "com", "por", "como", 
        "mas", "ou", "se", "não", "mais", "muito", "também", 
        "quando", "onde", "quem", "qual", "quais", "porque",
        "pois", "então", "assim", "logo"
    };

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        var root = GetComponent<UIDocument>().rootVisualElement;
        textHolder = root.Q<VisualElement>(textHolderName);

        inputField = root.Q<TextField>(inputFieldName);

        attemptButton = root.Q<Button>(attemptButtonName);
        attemptButton.clicked += OnAttemptButtonClicked;

        Button testButton = root.Q<Button>(testButtonName);
        testButton.clicked += OnTestButtonClicked;
    }

    void OnTestButtonClicked() {
        SceneManager.LoadScene("RequestTest");
    }

    void OnAttemptButtonClicked() {
        string tentativa = inputField.value;
        inputField.value = "";

        foreach (VisualElement child in textHolder.Children()) {
            Label label = (Label)child;
            string palavra = (string)label.userData;

            if (palavra == tentativa) {
                label.text = palavra;
                label.RemoveFromClassList(ocultoClassName);
            }
        }
    }

    public void GerarPalavras(List<string> palavras) {
        textHolder.Clear();

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
