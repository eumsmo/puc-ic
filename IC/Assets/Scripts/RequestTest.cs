using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class RequestTest : MonoBehaviour {
    public Text paramsText, respostaText;
    public Button getParamsButton, makeRequestButton;
    public InputField urlInput;


    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    [DllImport("__Internal")]
    private static extern string GetURLParams();

    public void HandleMostrarParams() {
        HelloString("Teste!");
/*
        string parametrosStr = GetURLParams();
        string[] parametros = parametrosStr.Split('&');
        string texto = "";
        foreach (string parametro in parametros) {
            texto += parametro + "\n";
        }*/
        paramsText.text = GetURLParams();;
    }

    public void HandleFazerRequisicao() {
        StartCoroutine(FazerRequisicao());
    }

    IEnumerator FazerRequisicao() {
        string url = urlInput.text;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
                respostaText.text = "Erro de rede: " + webRequest.error;
            } else {
                respostaText.text = webRequest.downloadHandler.text;
            }
        }
    }
}
