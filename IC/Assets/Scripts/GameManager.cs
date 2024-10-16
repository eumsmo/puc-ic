using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern string GetURLParams();


    public static GameManager instance;
    public string texto = "info.json";

    ArtigoInfo artigoInfo;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        StartCoroutine(LoadTexto());
    }

    public IEnumerator LoadTexto() {
        string url = GetURLInParams();

        if (string.IsNullOrEmpty(url))
            yield return StartCoroutine(LoadTextoLocal());
        else
            yield return StartCoroutine(LoadTextoWeb(url));
    }

    public string GetURLInParams() {
        #if UNITY_WEBGL && !UNITY_EDITOR

            string parametrosStr = GetURLParams();

            if (string.IsNullOrEmpty(parametrosStr) || !parametrosStr.StartsWith("?")) {
                return "";
            }

            // Remove o ? inicial
            parametrosStr = parametrosStr.Substring(1);

            // Separa cada parâmetro
            string[] parametros = parametrosStr.Split('&');
            string texto = "";

            // Procura por pelo parâmetro 'url'
            foreach (string parametro in parametros) {
                if (parametro.StartsWith("url=")) {
                    texto = parametro.Substring(4);
                    break;
                }
            }

            return texto;

        #else

            return "";

        #endif
    }

    public IEnumerator LoadTextoWeb(string url) {
        bool deuCerto = false;
        string errorMessage = "Erro não especificado";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
                errorMessage = "Erro de rede: " + webRequest.error;
            } else {
                try {
                    string json = webRequest.downloadHandler.text;
                    artigoInfo = JsonUtility.FromJson<ArtigoInfo>(json);

                    List<string> palavras = SepararPalavras(artigoInfo.resumo);
                    deuCerto = true;

                    UIController.instance.GerarPalavras(palavras);
                } catch (System.Exception e) {
                    errorMessage = "Erro de resposta: " + e.Message;
                }
            }
        }

        if (!deuCerto) {
            Debug.LogError(errorMessage);
            yield return StartCoroutine(LoadTextoLocal());
        }
    }

    public IEnumerator LoadTextoLocal() {
        string rootPath = Application.dataPath;
        string filePath = rootPath + "/info.json";

        if (!System.IO.File.Exists(filePath)) {
            Debug.LogError("Arquivo não encontrado: " + filePath);
        } else {
            string json = System.IO.File.ReadAllText(filePath);
            artigoInfo = JsonUtility.FromJson<ArtigoInfo>(json);
            
            List<string> palavras = SepararPalavras(artigoInfo.resumo);
            UIController.instance.GerarPalavras(palavras);
        }

        yield return null;
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
