using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Termo : MonoBehaviour {
    public static string pontuacoes = ".!?,;:()[]{}<>\"'”©®‐–—•…";

    public enum Estado { Oculto, Revelado, Pontuacao }

    public Estado estado;

    public Text text;
    public Image fundo;
    public HorizontalLayoutGroup layout;

    [Header("Valores"), Range(0, 10)]
    public int espacamentoEntreLetras = 2;
    public int pontuacaoFontSize = 10;

    [Header("Cores")]
    public Color escondidoFundo;
    public Color escondidoTexto;
    public Color pontuacaoFundo, pontuacaoTexto, pontuacaoTextoTitulo;
    public Color reveladoFundo, reveladoTexto;

    public string termo {
        get { return _termo; }
        set { SetTermo(value); }
    }
    string _termo;

    public bool oculto {
        get { return estado == Estado.Oculto; }
        set { if (value) SetOculto(); else SetRevelado(); }
    }

    public void SetTermo(string termo) {
        _termo = termo;
        text.text = termo;
        Canvas.ForceUpdateCanvases();
    }

    public void SetPontuacao(bool isTitutlo = false) {
        estado = Estado.Pontuacao;

        text.color = isTitutlo ? pontuacaoTextoTitulo : pontuacaoTexto;
        fundo.color = pontuacaoFundo;

        text.fontSize = pontuacaoFontSize;
        text.resizeTextForBestFit = true;
        fundo.enabled = false;
        layout.padding = new RectOffset(0, 0, 0, 0);
    }

    public void SetRevelado() {
        estado = Estado.Revelado;
        text.text = termo;
        
        text.color = reveladoTexto;
        fundo.color = reveladoFundo;
    }

    public void SetOculto() {
        estado = Estado.Oculto;

        string palavraOculta = "";
        int length = termo.Length;

        for (int i = 0; i < length; i++) {
            if (pontuacoes.Contains(termo[i])) {
                palavraOculta += termo[i];
            } else {
                palavraOculta += "_";
                if (i < length - 1 && !pontuacoes.Contains(termo[i + 1]))
                    palavraOculta += "<size=" + espacamentoEntreLetras + "><color=#00000000>.</color></size>";
            }
        }

        text.text = palavraOculta;

        text.color = escondidoTexto;
        fundo.color = escondidoFundo;
    }
}
