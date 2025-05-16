using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tentativa : MonoBehaviour {
    public Text textoTentativa, numeroTentativa;
    public string tentativa;


    public void SetTentativa(string texto, int numero, string tentativaDeFato = "") {
        textoTentativa.text = texto;
        numeroTentativa.text = numero.ToString();
        
        tentativa = tentativaDeFato != "" ? tentativaDeFato : texto;
        
        if (numero == 0)
        {
            transform.DOShakePosition(0.3f, 5, 20, 90, false, false);
        }
    }
}
