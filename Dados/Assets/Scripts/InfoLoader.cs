using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public enum LoadMode {
    LOCAL,
    WEB_URL,
    WEB_DRIVE
}

public class InfoLoader : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern string GetURLParams();
    public DadosInfo info;
    public LoadMode loadMode = LoadMode.LOCAL;

    private const string localPath = "Resources/teste.json";


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

        if (string.IsNullOrEmpty(url)){
            loadMode = LoadMode.LOCAL;
            yield return StartCoroutine(LoadTextoLocal());
        } else {
            yield return StartCoroutine(LoadTextoWeb(url));
        }
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
                    loadMode = LoadMode.WEB_URL;
                    break;
                } else if (parametro.StartsWith("drive=")) {
                    string fileID = parametro.Substring(6);
                    texto = string.Format("https://www.googleapis.com/drive/v3/files/{0}?alt=media&key={1}", fileID, API_KEY);
                    loadMode = LoadMode.WEB_DRIVE;
                    break;
                } else {
                    loadMode = LoadMode.LOCAL;
                }
            }

            return texto;

        #else

            return "";

        #endif
    }

    public DadosInfo ConvertTextIntoInfo(string text) {
        // Remove conteudo antes do objeto (para comentários legiveis a humanos)
        int start = text.IndexOf("{");
        if (start == -1) {
            throw new System.Exception("Erro ao encontrar objeto JSON");
        }

        string json = text.Substring(start);

        return JsonUtility.FromJson<DadosInfo>(json);
    }

    public IEnumerator LoadTextoWeb(string url) {
        bool deuCerto = false;
        string errorMessage = "Erro não especificado";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success) {
                try {
                    string json = webRequest.downloadHandler.text;
                    info = ConvertTextIntoInfo(json);
                    deuCerto = info != null;

                    if (deuCerto) SetarArtigo(info);
                } catch (System.Exception e) {
                    errorMessage = "Erro de resposta: " + e.Message;

                    switch (loadMode) {
                        case LoadMode.WEB_URL:
                            GameManager.instance.SetError("Não foi possível carregar o conteúdo do link fornecido");
                            break;
                        case LoadMode.WEB_DRIVE:
                            GameManager.instance.SetError("Não foi possível carregar o conteúdo apartir do ID fornecido");
                            break;
                        default:
                            GameManager.instance.SetError("Um erro não especificado ocorreu ao carregar o conteúdo do jogo");
                            break;
                    }
                }
            } else if(webRequest.responseCode == 404) {
                switch (loadMode) {
                    case LoadMode.WEB_URL:
                        GameManager.instance.SetError("Não foi possível encontrar o conteúdo do link fornecido");
                        break;
                    case LoadMode.WEB_DRIVE:
                        GameManager.instance.SetError("Não foi possível encontrar o conteúdo apartir do ID fornecido");
                        break;
                    default:
                        GameManager.instance.SetError("Um erro não especificado ocorreu ao carregar o conteúdo do jogo");
                        break;
                }
            } else {
                errorMessage = "Erro de rede: " + webRequest.error;
                GameManager.instance.SetError("Problema de conexão com a internet");
            }
        }

        if (!deuCerto) {
            Debug.LogError(errorMessage);
            yield return StartCoroutine(LoadTextoLocal());
        }
    }

    public IEnumerator LoadTextoLocal() {
        string filePath = Application.dataPath + "/" + localPath;

        if (!System.IO.File.Exists(filePath)) {
            Debug.LogError("Arquivo não encontrado: " + filePath);

            if (loadMode == LoadMode.LOCAL) {
                GameManager.instance.SetError("Nenhum artigo selecionado, tenha certeza que digitou o link corretamente");
            }
        } else {
            string json = System.IO.File.ReadAllText(filePath);
            info = ConvertTextIntoInfo(json);
            SetarArtigo(info);
        }

        yield return null;
    }

    public void SetarArtigo(DadosInfo info) {
        GameManager.instance.OnLoadedInfo(info);
    }
}
