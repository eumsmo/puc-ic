using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUI : MonoBehaviour {
    public GameObject tentativaPrefab, termoPrefab, termoPrefabTitulo;

    public Transform rodape, dicaButton;
    public GameObject holdersHolder, textHolder, tituloHolder;
    public GameObject tentativasList;
    public InputField inputField;
    public Slider dicaProgress;
    public Text tempoLabel;
    public Image instrucoesPanel;
    public Transform instrucoesModal;
    public Image surePanel;
    public Transform sureModal;

    bool dicaDisponivel = true;
    public float timeToDica = 5f;

    [Header("Separar termos em linhas")]
    public GameObject linhaTermosPrefab;
    public int caracteresPorLinhaTitulo = 20;
    public int caracteresPorLinha = 20;


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

    }

    // Chamado quando o jogo é iniciado
    public void OnGameStarted() {
        foreach (Transform child in tentativasList.transform) {
            Destroy(child.gameObject);
        }

        tentativas.Clear();

        dicaProgress.value = 1;
        ForceUpdate();
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
        
        return RemoveAccents(a.ToUpper()) == RemoveAccents(b.ToUpper());
    }

    public void Tentar(string tentativa) {
        tentativa = tentativa.Trim();
        string versaoSalva = RemoveAccents(tentativa).ToLower();

        if (tentativas.Contains(versaoSalva) || tentativa == "") return;
        if (palavrasNaoOcultas.Contains(tentativa)) return;

        List<Termo> termos = GetTermos();

        string escritaCorreta = tentativa.ToLower();

        int encontrados = 0;
        foreach (Termo termo in termos) {
            if (termo == null || !termo.oculto) continue;

            if (Comparar(termo.termo, tentativa)) {
                escritaCorreta = termo.termo.ToLower();
                termo.oculto = false;
                encontrados++;
            }
        }

        ArmazenarTentativa(escritaCorreta, encontrados);

        if (CheckIfWin()) {
            GameManager.instance.EndGame();
        }

        ForceUpdate();
    }

    public void OnAttemptButtonClicked() {
        string tentativa = inputField.text;
        inputField.text = "";

        Tentar(tentativa);

        inputField.ActivateInputField();
        inputField.Select();
    }

    public bool CheckIfWin() {
        List<Termo> termosTitulo = GetTermos(true, false);

        foreach (Termo termo in termosTitulo) {
            if (termo != null && termo.oculto) {
                // Debug.Log("Ainda tem palavra oculta: " + termo.termo);
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
        tentativas.Add(RemoveAccents(tentativa).ToLower());

        tentativaEl.transform.localScale = Vector3.one;
        tentativaEl.transform.DOPunchScale(Vector3.one * 0.05f, 0.15f, 1, 0.5f);

        if (resultados == 0) {
            rodape.transform.DOShakePosition(0.3f, 5, 20, 90, false, false);
        }

    }

    public IEnumerator ForceUpdateAsync(int calls = 0) {
        Canvas.ForceUpdateCanvases();

        List<VerticalLayoutGroup> layouts = new List<VerticalLayoutGroup>();
        layouts.AddRange(holdersHolder.GetComponentsInChildren<VerticalLayoutGroup>());
        layouts.Add(holdersHolder.GetComponent<VerticalLayoutGroup>());

        foreach (VerticalLayoutGroup layout in layouts) {
            if (layout == null) continue;
            layout.enabled = false;
            // yield return null;

            layout.enabled = true;
            yield return null;
        }
    }

    public void ForceUpdate(int calls = 0) {
        StartCoroutine(ForceUpdateAsync(calls));
    }

    public void GerarPalavras(List<string> titulo, List<string> palavras) {
        GameObject[] holders = new GameObject[] { textHolder, tituloHolder };
        List<string>[] palavrasList = new List<string>[] { palavras, titulo };

        foreach (GameObject holder in holders) {
            foreach (Transform child in holder.transform) {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < holders.Length; i++) {
            GameObject holder = holders[i];
            List<string> termos = palavrasList[i];

            List<GameObject> linhas = new List<GameObject>();

            foreach (Transform child in holder.transform) {
                Destroy(child.gameObject);
            }

            GameObject linha = null;
            int caracteres = 0;

            int limiteCaracteres = i == 1 ? caracteresPorLinhaTitulo : caracteresPorLinha;

            foreach (string palavra in termos) {
                Termo termo = GerarPalavra(palavra, holder, i == 1);

                if (pontuacoes.Contains(palavra))
                    termo.SetPontuacao();
                else if (OcultarPalavra(palavra))
                    termo.SetOculto();
                else
                    termo.SetRevelado();

                caracteres += palavra.Length + 1;
                
                if (linha == null || caracteres > limiteCaracteres) {
                    if (linha != null) {
                        linhas.Add(linha);
                    }

                    linha = Instantiate(linhaTermosPrefab);
                    linha.transform.SetParent(holder.transform);
                    linha.transform.localScale = Vector3.one;

                    caracteres = palavra.Length;
                }

                termo.transform.SetParent(linha.transform);
                termo.transform.localScale = Vector3.one;
            }

            if (linha != null) {
                linhas.Add(linha);
            }
        }
    }

    public Termo GerarPalavra(string texto, GameObject holder, bool titulo) {
        GameObject termoObj = Instantiate(titulo ? termoPrefabTitulo : termoPrefab);

        Termo termo = termoObj.GetComponent<Termo>();
        termo.SetTermo(texto);

        //termoObj.transform.SetParent(holder.transform);
        //termoObj.transform.localScale = Vector3.one;

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
        dicaProgress.value = 0;

        while (time < timeToDica) {
            time += Time.deltaTime;
            dicaProgress.value = time / timeToDica;
            yield return null;
        }

        dicaDisponivel = true;

        dicaButton.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0.25f);
    }

    public string GetPalavraChave() {
        // if has palavra chave on artigo (não tem ainda)
        // else:
        Dictionary<string, int> palavras = new Dictionary<string, int>();

        List<Termo> termosTexto = GetTermos(false, true);
        List<Termo> termosTitulo = GetTermos(true, false);
        List<Termo> termos = new List<Termo>();

        termos.AddRange(termosTexto);
        termos.AddRange(termosTitulo);

        foreach (Termo termo in termos) {
            if (termo != null && termo.oculto) {
                if (palavras.ContainsKey(termo.termo))  palavras[termo.termo]++;
                else palavras[termo.termo] = 1;
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

        int indexOcorrencia = (termosTexto.Count - palavras.Count) % menosOcorrencias.Count;
        return menosOcorrencias[indexOcorrencia];
    }

    public List<Termo> GetTermos(bool titulo = true, bool palavra = true) {
        List<Termo> termos = new List<Termo>();

        GameObject[] holders;

        if (titulo && palavra) holders = new GameObject[] { textHolder, tituloHolder };
        else if (titulo) holders = new GameObject[] { tituloHolder };
        else holders = new GameObject[] { textHolder };

        foreach (GameObject holder in holders) {
            foreach (Transform linha in holder.transform) {
                foreach (Transform child in linha.transform) {
                    Termo termo = child.GetComponent<Termo>();
                    if (termo != null) {
                        termos.Add(termo);
                    }
                }
            }
        }

        return termos;
    }

    public void MostrarDesistir() {
        surePanel.gameObject.SetActive(true);

        sureModal.localScale = Vector3.one;
        sureModal.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0.25f);
    }

    public void FecharDesistir() {
        surePanel.gameObject.SetActive(false);
    }

    public void MostrarInstrucoes() {
        instrucoesPanel.gameObject.SetActive(true);

        instrucoesModal.localScale = Vector3.one;
        instrucoesModal.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0.25f);
    }

    public void FecharInstrucoes() {
        instrucoesPanel.gameObject.SetActive(false);
    }


    public string RemoveAccents(string text){   
        StringBuilder sbReturn = new StringBuilder();   
        var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
        foreach (char letter in arrayText){   
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(letter) != System.Globalization.UnicodeCategory.NonSpacingMark)
                sbReturn.Append(letter);   
        }   
        return sbReturn.ToString();   
    } 
}
