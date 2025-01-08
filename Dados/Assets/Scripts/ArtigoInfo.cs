[System.Serializable]
public class Dados {
    public string tipo; // Boleano, Porcentagem, Grafico
    public string texto;
    public float respostaFloat;
    public bool respostaBool;
    public string explicacao;
}

public class ArtigoInfo {
    public string url;
    public string titulo;
    public string[] autores;
    public string resumo;
    public Dados[] dados;
}
