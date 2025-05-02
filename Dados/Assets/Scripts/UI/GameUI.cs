using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public interface SecaoDoJogo {
    void Inicializar(GameUI game);
    void Comecar(Dados dados);
    void Finalizar();
    bool GetResposta();
}

[System.Serializable]
public class Flavor {
    public GameObject corretoPrefab, quasePrefab, incorretoPrefab;
}

public enum RespostaStatus { Correto, Quase, Incorreto }


public class GameUI : MonoBehaviour {
    public enum CurrentGameState {
        WaitingStart,
        Playing,
        ShowingStatus,
        Ended
    }

    public CurrentGameState state = CurrentGameState.WaitingStart;

    public Text acertosLabel, perguntasQuantLabel;
    public Image acertosImage;

    public GameObject[] secoes;
    SecaoDoJogo[] _secoes;
    public SecaoDoJogo secaoAtual;

    public GameObject statusHolder;
    public Animator statusHolderController;
    public Text statusLabel, statusDescricao;

    public GameObject instrucoes;

    public Flavor flavor;

    Dados dadosAtuais;

    public int acertos = 0, erros = 0;


    void Awake() {
        _secoes = new SecaoDoJogo[secoes.Length];
        int i = 0;
        foreach (GameObject secaoObj in secoes) {
            SecaoDoJogo secao = secaoObj.GetComponent<SecaoDoJogo>();
            secao.Inicializar(this);
            _secoes[i] = secao;
            i++;
        }
    }

    void Start() {
        GameManager.instance.controls.Game.Submit.performed += ctx => OnAttemptButtonClicked();
    }

    public void UpdateQual(int qual, int quantas) {
        perguntasQuantLabel.text = (qual+1) + "/" + quantas;
    }

    public void LoadSecao(int secaoId, Dados dados) {
        if (secaoAtual != null) secaoAtual.Finalizar();
        secaoAtual = _secoes[secaoId];
        dadosAtuais = dados;
        state = CurrentGameState.Playing;
        secaoAtual.Comecar(dados);
    }

    // Vou manter essa função aqui caso voltemos atrás na decisão
    public void UpdateTempo(int tempoSeconds) {
        int min = tempoSeconds / 60;
        int sec = tempoSeconds % 60;
        // tempoLabel.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    bool ultimoFoiAcerto = false;
    public void OnAttemptButtonClicked() {
        if (state == CurrentGameState.ShowingStatus) return;
        state = CurrentGameState.ShowingStatus;

        bool resposta = secaoAtual.GetResposta();
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
            statusDescricao.text = "Que pena, você errou!";
        }

        if (dadosAtuais.explicacao != null && dadosAtuais.explicacao != "") {
            statusDescricao.text += dadosAtuais.explicacao;
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
        perguntasQuantLabel.text = "0/0";
        statusHolder.SetActive(false);
        state = CurrentGameState.WaitingStart;
    }


    public void MostrarInstrucoes() {
        instrucoes.SetActive(true);
        Time.timeScale = 0;
    }

    public void FecharInstrucoes() {
        instrucoes.SetActive(false);
        Time.timeScale = 1;
    }

    public void SpawnStatusAt(Vector2 posInCanvas, RespostaStatus status) {
        GameObject prefab = null;
        switch (status) {
            case RespostaStatus.Correto:
                prefab = flavor.corretoPrefab;
                break;
            case RespostaStatus.Quase:
                prefab = flavor.quasePrefab;
                break;
            case RespostaStatus.Incorreto:
                prefab = flavor.incorretoPrefab;
                break;
        }

        GameObject obj = Instantiate(prefab, posInCanvas, Quaternion.identity, transform);
        Destroy(obj, 2f);
    }

    public RespostaStatus CorretissimaCorretaToStatus(bool corretissima, bool correta) {
        if (corretissima) return RespostaStatus.Correto;
        if (correta) return RespostaStatus.Quase;
        return RespostaStatus.Incorreto;
    }
}
