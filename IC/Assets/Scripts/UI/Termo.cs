using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Termo : MonoBehaviour {
    public static string pontuacoes = ".!?,;:()[]{}<>\"'";

    public enum Estado { Oculto, Revelado, Pontuacao }

    public Estado estado;

    public Text text;
    public Image fundo;

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

    public void SetPontuacao() {
        estado = Estado.Pontuacao;
    }

    public void SetRevelado() {
        estado = Estado.Revelado;
        text.text = termo;
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
            }
        }

        text.text = palavraOculta;
    }
}
