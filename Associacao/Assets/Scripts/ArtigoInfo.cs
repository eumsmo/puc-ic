[System.Serializable]
public class AssociacaoInfo {
    public string titulo;
    public string url;
    public Associacao[] associacoes;
}

[System.Serializable]
public class Associacao {
    public string palavra1;
    public string palavra2;
}