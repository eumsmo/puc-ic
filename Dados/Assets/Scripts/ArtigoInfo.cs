public class DadosInfo {
    public string titulo;
    public string url;
    public Dados[] dados;
}

[System.Serializable]
public class Dados {
    public string tipo; // Boleano, Porcentagem, Grafico
    public string texto;
    public bool respostaBool;
    public float respostaFloat;
    public float range;
    public Dados_GraficoInfo grafico;
    public string explicacao;
}

[System.Serializable]
public class Dados_GraficoInfo {
    public float min, max;
    public Dados_Grafico_Campo[] campos;
}

[System.Serializable]
public class Dados_Grafico_Campo {
    public string nome;
    public float valor;
}

public class ArtigoInfo {
    public string url;
    public string titulo;
    public string[] autores;
    public string resumo;
    public Dados[] dados;
}