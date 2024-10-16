using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class InfoLoader : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern string GetURLParams();
    public ArtigoInfo info;

    public string API_KEY { 
        get { 
            Secrets asset = Resources.Load<Secrets>("SECRETS");
            if (asset == null) {
                Debug.LogError("SECRETS não encontrado, defina um scriptable 'SECRETS' na pasta Resources");
                return "";
            }
            return asset.API_KEY;
        } 
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
                } else if (parametro.StartsWith("drive=")) {
                    string fileID = parametro.Substring(6);
                    texto = string.Format("https://www.googleapis.com/drive/v3/files/{0}?alt=media&key={1}", fileID, API_KEY);
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

            if (webRequest.result == UnityWebRequest.Result.Success) {
                try {
                    string json = webRequest.downloadHandler.text;
                    info = JsonUtility.FromJson<ArtigoInfo>(json);

                    List<string> palavras = SepararPalavras(info.resumo);
                    deuCerto = true;

                    UIController.instance.GerarPalavras(palavras);
                } catch (System.Exception e) {
                    errorMessage = "Erro de resposta: " + e.Message;
                }
            } else {
                errorMessage = "Erro de rede: " + webRequest.error;
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
            info = JsonUtility.FromJson<ArtigoInfo>(json);
            
            List<string> palavras = SepararPalavras(info.resumo);
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
