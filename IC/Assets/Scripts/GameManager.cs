using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public string texto = "teste.json";

    ArtigoInfo artigoInfo;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        LoadTexto();
    }

    public void LoadTexto() {
        TextAsset file = Resources.Load<TextAsset>(texto);
        if (file == null) {
            Debug.LogError("Arquivo não encontrado");
            return;
        }


        string json = file.text;
        artigoInfo = JsonUtility.FromJson<ArtigoInfo>(json);

        List<string> palavras = SepararPalavras(artigoInfo.resumo);
        UIController.instance.GerarPalavras(palavras);
    }

    List<string> SepararPalavras(string texto) {
        List<string> palavras = new List<string>();
        string[] palavrasSeparadas = texto.Split(' ');
        foreach (string palavra in palavrasSeparadas) {
            if (!string.IsNullOrEmpty(palavra)) {
                palavras.Add(palavra);
            }
        }
        return palavras;
    }
}
