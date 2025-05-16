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
    public Image sureVoltarMenuPanel;
    public Transform sureVoltarMenuModal;

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
        stopWords.AddRange(Termo.pontuacoes.ToCharArray().Select(c => c.ToString()).ToArray());

        palavrasNaoOcultas = stopWords.ToArray();
    }

    public void UpdateTempo(int tempoSeconds) {
        int min = tempoSeconds / 60;
        int sec = tempoSeconds % 60;
        tempoLabel.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    public bool Comparar(string a, string b) {/*
        foreach (char pontuacao in Termo.pontuacoes) {
            string pontuacaoStr = pontuacao + "";
            a = a.Replace(pontuacaoStr + "", "");
            b = b.Replace(pontuacaoStr + "", "");
        }*/
        
        return RemoveAccents(a.ToUpper()) == RemoveAccents(b.ToUpper());
    }

    public void Tentar(string tentativa) {
        tentativa = tentativa.Trim();
        string versaoSalva = RemoveAccents(tentativa).ToLower();

        if (tentativa == "" || palavrasNaoOcultas.Contains(tentativa)) return;
        if (tentativas.Contains(versaoSalva))  {
            MostrarTentativaJaFeita(versaoSalva);
            return;
        }

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

    void MostrarTentativaJaFeita(string tentativa) {
        Tentativa[] tentativas = tentativasList.GetComponentsInChildren<Tentativa>();
        foreach (Tentativa tent in tentativas) {
            if (tent.tentativa == tentativa) {
                tent.transform.SetAsLastSibling();
                tent.transform.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0.25f);
                return;
            }
        }
    }

    public void OnAttemptButtonClicked()
    {
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
                return false;
            }
        }

        return true;
    }

    public void ArmazenarTentativa(string tentativa, int resultados) {
        string tentativaDeFato = RemoveAccents(tentativa).ToLower();

        GameObject tentativaEl = Instantiate(tentativaPrefab);

        Tentativa tentativaScript = tentativaEl.GetComponent<Tentativa>();
        tentativaScript.SetTentativa(tentativa, resultados, tentativaDeFato);
        tentativaEl.transform.SetParent(tentativasList.transform);

        tentativaEl.transform.localScale = Vector3.one;
        tentativaEl.transform.DOPunchScale(Vector3.one * 0.05f, 0.15f, 1, 0.5f);

        tentativas.Add(tentativaDeFato);
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
                string palavraDeFato = palavra.Trim();
                Termo termo = GerarPalavra(palavraDeFato, holder, i == 1);

                if (Termo.pontuacoes.Contains(palavraDeFato))
                    termo.SetPontuacao(holder == tituloHolder);
                else if (OcultarPalavra(palavraDeFato))
                    termo.SetOculto();
                else
                    termo.SetRevelado();

                caracteres += palavraDeFato.Length + 1;
                
                if (linha == null || caracteres > limiteCaracteres) {
                    if (linha != null) {
                        linhas.Add(linha);
                    }

                    linha = Instantiate(linhaTermosPrefab);
                    linha.transform.SetParent(holder.transform);
                    linha.transform.localScale = Vector3.one;

                    caracteres = palavraDeFato.Length;
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
        List<Termo> termos = GetTermos(true, true, true);

        foreach (Termo termo in termos) {
            if (termo != null && termo.oculto) {
                if (palavras.ContainsKey(termo.termo))  palavras[termo.termo]++;
                else palavras[termo.termo] = 1;
            }
        }


        List<string> maioresOcorrencias = new List<string>();
        int maiorOcorrencia = 0;

        foreach (KeyValuePair<string, int> entry in palavras) {
            if (entry.Value > maiorOcorrencia) {
                maiorOcorrencia = entry.Value;
                maioresOcorrencias.Clear();
                maioresOcorrencias.Add(entry.Key);
            } else if (entry.Value == maiorOcorrencia) {
                maioresOcorrencias.Add(entry.Key);
            }
        }

        Debug.Log("Maiores ocorrencias [" + maiorOcorrencia + "]: " + string.Join(", ", maioresOcorrencias));

        int indexOcorrencia = Random.Range(0, maioresOcorrencias.Count);
        return maioresOcorrencias[indexOcorrencia];
    }

    public List<Termo> GetTermos(bool titulo = true, bool palavra = true, bool ignorarJaRevelados = false) {
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
                        if (ignorarJaRevelados && !termo.oculto) continue;

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

    public void MostrarVoltarMenu() {
        sureVoltarMenuPanel.gameObject.SetActive(true);

        sureVoltarMenuModal.localScale = Vector3.one;
        sureVoltarMenuModal.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0.25f);
    }

    public void FecharVoltarMenu() {
        sureVoltarMenuPanel.gameObject.SetActive(false);
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
