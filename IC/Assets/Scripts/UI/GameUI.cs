using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour {
    public string textHolderName = "TextHolder";
    public string attemptButtonName = "ButtonTentativa";
    public string inputFieldName = "InputTentativa";

    public string textoClassName = "texto";
    public string ocultoClassName = "oculto";

    public VisualTreeAsset tentativaTemplate;

    VisualElement textHolder, tituloHolder;
    ScrollView tentativasList;
    Button attemptButton, dicaButton;
    TextField inputField;
    Label tempoLabel;

    VisualElement btnDicaProgress;
    bool dicaDisponivel = true;
    public float timeToDica = 5f;

    // Palavras que por padrão não serão ocultas
    string[] palavrasNaoOcultas = new string[] {
        "a", "o", "e", "é", "de", "da", "do", "em", "no", "na", 
        "um", "uma", "uns", "umas", "para", "com", "por", "como", 
        "mas", "ou", "se", "não", "mais", "muito", "também", 
        "quando", "onde", "quem", "qual", "quais", "porque",
        "pois", "então", "assim", "logo"
    };

    string pontuacoes = ".!?,;:()[]{}<>\"'";

    List<string> tentativas = new List<string>();

    void Awake() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        textHolder = root.Q<VisualElement>(textHolderName);
        tituloHolder = root.Q<VisualElement>("TituloHolder");

        tempoLabel = root.Q<Label>("Tempo");
        tentativasList = root.Q<ScrollView>("TentativasList");

        tentativasList.Clear();
        tentativas.Clear();

        inputField = root.Q<TextField>(inputFieldName);

        attemptButton = root.Q<Button>(attemptButtonName);
        attemptButton.clicked += OnAttemptButtonClicked;

        dicaButton = root.Q<Button>("ButtonDica");
        dicaButton.clicked += OnDicaButtonClicked;

        btnDicaProgress = root.Q<VisualElement>("ButtonDicaProgress");
    }

    void Start() {
        GameManager.instance.controls.Game.Submit.performed += ctx => OnAttemptButtonClicked();

        btnDicaProgress.style.width = Length.Percent(100);
    }

    public void GerarStopWords(string[] words) {
        List<string> stopWords = new List<string>();
        stopWords.AddRange(words);
        stopWords.AddRange(pontuacoes.ToCharArray().Select(c => c.ToString()).ToArray());

        palavrasNaoOcultas = stopWords.ToArray();
    }

    public void UpdateTempo(int tempoSeconds) {
        int min = tempoSeconds / 60;
        int sec = tempoSeconds % 60;
        tempoLabel.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    public bool Comparar(string a, string b) {/*
        foreach (char pontuacao in pontuacoes) {
            string pontuacaoStr = pontuacao + "";
            a = a.Replace(pontuacaoStr + "", "");
            b = b.Replace(pontuacaoStr + "", "");
        }*/
        
        return a.ToUpper() == b.ToUpper();
    }

    public void Tentar(string tentativa) {
        if (tentativas.Contains(tentativa) || tentativa.Trim() == "") return;

        IEnumerable<VisualElement> children = textHolder.Children();
        children = children.Concat(tituloHolder.Children());

        int encontrados = 0;
        foreach (VisualElement child in children) {
            Label label = (Label)child;
            string palavra = (string)label.userData;

            // Palavras já reveladas não precisam ser verificadas
            if (palavra == null) continue;

            if (Comparar(palavra, tentativa)) {
                label.text = palavra;
                label.userData = null;
                label.RemoveFromClassList(ocultoClassName);
                encontrados++;
            }
        }

        ArmazenarTentativa(tentativa, encontrados);

        if (CheckIfWin()) {
            GameManager.instance.EndGame();
        }
    }

    void OnAttemptButtonClicked() {
        string tentativa = inputField.value;
        inputField.value = "";

        Tentar(tentativa);
    }

    public bool CheckIfWin() {
        foreach (VisualElement child in tituloHolder.Children()) {
            Label label = (Label)child;
            if (label.ClassListContains(ocultoClassName)) {
                return false;
            }
        }

        return true;
    }

    public void ArmazenarTentativa(string tentativa, int resultados) {
        var tentativaEl = tentativaTemplate.Instantiate();
        tentativaEl.Q<Label>("palavra").text = tentativa;
        tentativaEl.Q<Label>("numero").text = "" + resultados;

        tentativasList.Add(tentativaEl);
        tentativas.Add(tentativa);
    }

    public void GerarPalavras(List<string> titulo, List<string> palavras) {
        tituloHolder.Clear();
        textHolder.Clear();

        foreach (string palavra in palavras) {
            Label label = GerarPalavra(palavra, textHolder);

            if (pontuacoes.Contains(palavra)) {
                label.AddToClassList("pontuacao");
            } else if (OcultarPalavra(palavra))
                SetPalavraOculta(label);
        }

        foreach (string palavra in titulo) {
            Label label = GerarPalavra(palavra, tituloHolder);

            if (pontuacoes.Contains(palavra)) {
                label.AddToClassList("pontuacao");
            } else if (OcultarPalavra(palavra))
                SetPalavraOculta(label);
        }
    }

    public Label GerarPalavra(string texto, VisualElement holder) {
        Label label = new Label(texto);
        label.AddToClassList(textoClassName);
        holder.Add(label);
        return label;
    }

    public bool OcultarPalavra(string palavra) {
        foreach (string naoOculta in palavrasNaoOcultas) {
            if (Comparar(palavra, naoOculta)) {
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
            if (pontuacoes.Contains(palavra[i])) {
                palavraOculta += palavra[i];
            } else {
                palavraOculta += "_";
            }
        }

        label.text = palavraOculta;
        label.AddToClassList(ocultoClassName);
    }

    public void OnDicaButtonClicked() {
        if (!dicaDisponivel) return;

        string dica = GetPalavraChave();
        if (dica == null || dica == "") return;

        Tentar(dica);

        dicaDisponivel = false;
        StartCoroutine(LoadDicaProgress());
    }

    IEnumerator LoadDicaProgress() {
        float time = 0;
        btnDicaProgress.style.width = Length.Percent(0);
        while (time < timeToDica) {
            time += Time.deltaTime;
            btnDicaProgress.style.width = Length.Percent((time / timeToDica) * 100);
            yield return null;
        }

        dicaDisponivel = true;
    }

    public string GetPalavraChave() {
        // if has palavra chave on artigo (não tem ainda)
        // else:
        Dictionary<string, int> palavras = new Dictionary<string, int>();

        foreach (VisualElement child in textHolder.Children()) {
            Label label = (Label)child;
            if (label.ClassListContains(ocultoClassName)) {
                string palavra = (string)label.userData;
                if (palavras.ContainsKey(palavra)) {
                    palavras[palavra]++;
                } else {
                    palavras[palavra] = 1;
                }
            }
        }

        foreach (VisualElement child in tituloHolder.Children()) {
            Label label = (Label)child;
            if (label.ClassListContains(ocultoClassName)) {
                string palavra = (string)label.userData;
                if (palavras.ContainsKey(palavra)) {
                    palavras[palavra]++;
                } else {
                    palavras[palavra] = 1;
                }
            }
        }

        List<string> menosOcorrencias = new List<string>();
        int menorOcorrencia = int.MaxValue;
        foreach (KeyValuePair<string, int> entry in palavras) {
            if (entry.Value < menorOcorrencia) {
                menorOcorrencia = entry.Value;
                menosOcorrencias.Clear();
                menosOcorrencias.Add(entry.Key);
            } else if (entry.Value == menorOcorrencia) {
                menosOcorrencias.Add(entry.Key);
            }
        }

        int indexOcorrencia = (textHolder.childCount - palavras.Count) % menosOcorrencias.Count;
        return menosOcorrencias[indexOcorrencia];
    }
}
