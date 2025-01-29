using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    public GameObject tentativaPrefab, termoPrefab, termoPrefabTitulo;

    public GameObject textHolder, tituloHolder;
    public GameObject tentativasList;
    public InputField inputField;
    public Text tempoLabel;

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
        foreach (Transform child in tentativasList.transform) {
            Destroy(child.gameObject);
        }

        tentativas.Clear();

        // dicaButton.clicked += OnDicaButtonClicked;
    }

    void Start() {
        GameManager.instance.controls.Game.Submit.performed += ctx => OnAttemptButtonClicked();

        // btnDicaProgress.style.width = Length.Percent(100);
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
        
        List<Transform> children = new List<Transform>();

        foreach (Transform child in textHolder.transform) {
            children.Add(child);
        }

        foreach (Transform child in tituloHolder.transform) {
            children.Add(child);
        }

        int encontrados = 0;
        foreach (Transform child in children) {
            Termo termo = child.GetComponent<Termo>();

            if (termo == null || !termo.oculto) continue;

            if (Comparar(termo.termo, tentativa)) {
                termo.oculto = false;
                encontrados++;
            }
        }

        ArmazenarTentativa(tentativa, encontrados);

        if (CheckIfWin()) {
            GameManager.instance.EndGame();
        }
    }

    public void OnAttemptButtonClicked() {
        string tentativa = inputField.text;
        inputField.text = "";

        Tentar(tentativa);
    }

    public bool CheckIfWin() {
        foreach (Transform child in tituloHolder.transform) {
            Termo termo = child.GetComponent<Termo>();

            if (termo != null && termo.oculto) {
                return false;
            }
        }

        return true;
    }

    public void ArmazenarTentativa(string tentativa, int resultados) {
        GameObject tentativaEl = Instantiate(tentativaPrefab);

        tentativaEl.transform.localScale = Vector3.one;

        Text palavra = tentativaEl.transform.Find("Palavra").GetComponent<Text>();
        Text numero = tentativaEl.transform.Find("Numero").GetComponent<Text>();

        palavra.text = tentativa;
        numero.text = "" + resultados;
        
        tentativaEl.transform.SetParent(tentativasList.transform);
        tentativas.Add(tentativa);

        tentativaEl.transform.localScale = Vector3.one;
    }

    public void GerarPalavras(List<string> titulo, List<string> palavras) {
        GameObject[] holders = new GameObject[] { textHolder, tituloHolder };
        List<string>[] palavrasList = new List<string>[] { palavras, titulo };

        for (int i = 0; i < holders.Length; i++) {
            GameObject holder = holders[i];
            List<string> termos = palavrasList[i];

            foreach (Transform child in holder.transform) {
                Destroy(child.gameObject);
            }

            foreach (string palavra in termos) {
                Termo termo = GerarPalavra(palavra, holder, i == 1);

                if (pontuacoes.Contains(palavra))
                    termo.SetPontuacao();
                else if (OcultarPalavra(palavra))
                    termo.SetOculto();
            }

            holder.GetComponent<HorizontalLayoutGroup>().enabled = false;
            holder.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }

        Canvas.ForceUpdateCanvases();
    }

    public Termo GerarPalavra(string texto, GameObject holder, bool titulo) {
        GameObject termoObj = Instantiate(titulo ? termoPrefabTitulo : termoPrefab);

        Termo termo = termoObj.GetComponent<Termo>();
        termo.SetTermo(texto);

        termoObj.transform.SetParent(holder.transform);
        termoObj.transform.localScale = Vector3.one;

        return termo;
    }

    public bool OcultarPalavra(string palavra) {
        foreach (string naoOculta in palavrasNaoOcultas) {
            if (Comparar(palavra, naoOculta)) {
                return false;
            }
        }

        return true;
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
        // btnDicaProgress.style.width = Length.Percent(0);
        while (time < timeToDica) {
            time += Time.deltaTime;
            // btnDicaProgress.style.width = Length.Percent((time / timeToDica) * 100);
            yield return null;
        }

        dicaDisponivel = true;
    }

    public string GetPalavraChave() {
        // if has palavra chave on artigo (não tem ainda)
        // else:
        Dictionary<string, int> palavras = new Dictionary<string, int>();

        GameObject[] holders = new GameObject[] { textHolder, tituloHolder };

        foreach (GameObject holder in holders) {
            foreach (Transform child in holder.transform) {
                Termo termo = child.GetComponent<Termo>();
                if (termo != null && termo.oculto) {
                    if (palavras.ContainsKey(termo.termo))  palavras[termo.termo]++;
                    else palavras[termo.termo] = 1;
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

        int indexOcorrencia = (textHolder.transform.childCount - palavras.Count) % menosOcorrencias.Count;
        return menosOcorrencias[indexOcorrencia];
    }
}
