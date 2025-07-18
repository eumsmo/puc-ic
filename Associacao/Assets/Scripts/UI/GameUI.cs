using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GameUI : MonoBehaviour {
    public enum CurrentGameState {
        WaitingStart,
        Playing,
        ShowingStatus,
        Ended
    }

    public CurrentGameState state = CurrentGameState.WaitingStart;

    public Text acertosLabel;
    public Image acertosImage;

    public GameObject statusHolder;
    public Animator statusHolderController;
    public Text statusLabel, statusDescricao;

    public GameObject instrucoes;
    public GameObject instrucoesModal;
    public GameObject sureVoltarMenuPanel;
    public GameObject sureVoltarMenuModal;

    [Header("Associacoes")]
    public Transform coluna1, coluna2;
    public GameObject associacaoPrefab;
    public Color[] cores;


    public int acertos = 0, erros = 0;


    void Start() {
        GameManager.instance.controls.Game.Submit.performed += ctx => OnAttemptButtonClicked();
    }

    // Vou manter essa função aqui caso voltemos atrás na decisão
    public void UpdateTempo(int tempoSeconds) {
        int min = tempoSeconds / 60;
        int sec = tempoSeconds % 60;
        // tempoLabel.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    public void GerarAssociacoes(AssociacaoInfo info) {
        if (state != CurrentGameState.WaitingStart) return;
        state = CurrentGameState.Playing;
        
        HashSet<string> coluna1Set = new HashSet<string>();
        HashSet<string> coluna2Set = new HashSet<string>();
        
        foreach (Associacao associacao in info.associacoes) {
            string palavra1 = associacao.palavra1;
            string palavra2 = associacao.palavra2;

            coluna1Set.Add(palavra1);
            coluna2Set.Add(palavra2);
        }


        // Embaralhar as colunas
        List<string> coluna1List = coluna1Set.OrderBy(x => Random.value).ToList();
        List<string> coluna2List = coluna2Set.OrderBy(x => Random.value).ToList();

        coluna1Set.Clear();
        coluna1Set = null;

        coluna2Set.Clear();
        coluna2Set = null;

        foreach (Transform child in coluna1) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in coluna2) {
            Destroy(child.gameObject);
        }


        int coresIndex = 0;
        Color cor = cores[coresIndex];

        for (int i = 0; i < coluna1List.Count; i++) {
            GameObject associacao = Instantiate(associacaoPrefab, coluna1);
            AssociacaoUI associacaoUI = associacao.GetComponent<AssociacaoUI>();
            associacaoUI.SetarTexto(coluna1List[i]);
            associacaoUI.ladoConexao = LadoConexao.Direita;
            
            associacaoUI.SetarCor(cor);
            coresIndex = (coresIndex + 1) % cores.Length;
            cor = cores[coresIndex];
        }

        for (int i = 0; i < coluna2List.Count; i++) {
            GameObject associacao = Instantiate(associacaoPrefab, coluna2);
            AssociacaoUI associacaoUI = associacao.GetComponent<AssociacaoUI>();
            associacaoUI.SetarTexto(coluna2List[i]);
            associacaoUI.ladoConexao = LadoConexao.Esquerda;

            associacaoUI.SetarCor(cor);
            coresIndex = (coresIndex + 1) % cores.Length;
            cor = cores[coresIndex];
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(coluna1.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(coluna2.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(coluna1.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(coluna2.GetComponent<RectTransform>());

    }


    bool ultimoFoiAcerto = false;
    public void OnAttemptButtonClicked() {
        if (state == CurrentGameState.ShowingStatus) return;
        state = CurrentGameState.ShowingStatus;

        bool resposta = true;
        statusHolder.SetActive(true);
        statusHolderController.SetTrigger("Aparecer");

        ultimoFoiAcerto = resposta;

        if (resposta) {
            acertos++;
            statusLabel.text = "Correto!";
            statusDescricao.text = "Parabéns, você acertou!";
            
            acertosLabel.transform.DOPunchScale(Vector3.one * 0.4f, 0.5f, 1, 0.5f);
            acertosImage.transform.DOPunchScale(Vector3.one * 0.4f, 0.5f, 1, 0.5f);
        } else {
            erros++;
            statusLabel.text = "Incorreto!";
            string detalhes = null;
            if (detalhes != null && detalhes != "") {
                statusDescricao.text = "Que pena, você errou!\n" + detalhes;
            } else {
                statusDescricao.text = "Que pena, você errou!";
            }
        }

        acertosLabel.text = "" + acertos;
        StartCoroutine(FeedbackNoStatusHolder());
    }

    public void GoToNext() {
        if (state != CurrentGameState.ShowingStatus) return;

        // statusHolder.SetActive(false);
        statusHolderController.SetTrigger("Desaparecer");
        GameManager.instance.ProximaPergunta();
    }

    IEnumerator FeedbackNoStatusHolder() {
        yield return new WaitForSeconds(0.25f);
        if (ultimoFoiAcerto) {
            statusHolder.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 1, 0.5f);
        } else {
            statusHolder.transform.DOShakePosition(0.3f, new Vector3(2, 0f, 0f), 20, 0.5f, false, true);
        }
    }

    public bool CheckIfWin() {
        return true;
    }

    public void ResetGame() {
        acertos = 0;
        erros = 0;
        acertosLabel.text = "" + acertos;
        statusHolder.SetActive(false);
        state = CurrentGameState.WaitingStart;
    }

    public void MostrarInstrucoes() {
        instrucoes.SetActive(true);
        instrucoesModal.transform.localScale = Vector3.one;
        instrucoesModal.transform.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0.25f).OnComplete(() => {
            instrucoesModal.transform.localScale = Vector3.one;
            Time.timeScale = 0;
        });
    }

    public void FecharInstrucoes() {
        instrucoes.SetActive(false);
        Time.timeScale = 1;
    }

    public void MostrarConfirmarVoltar() {
        sureVoltarMenuPanel.SetActive(true);

        sureVoltarMenuModal.transform.localScale = Vector3.one;
        sureVoltarMenuModal.transform.DOPunchScale(Vector3.one * 0.05f, 0.5f, 5, 0.25f).OnComplete(() => {
            sureVoltarMenuModal.transform.localScale = Vector3.one;
            Time.timeScale = 0;
        });
    }

    public void FecharConfirmarVoltar() {
        sureVoltarMenuPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
