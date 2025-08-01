using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class AssociacaoInfo {
    public string titulo;
    public string url;
    public Associacao[] associacoes;

    public bool Existe(string palavra, string conexao) {
        foreach (Associacao associacao in associacoes) {
            if (associacao.palavra == palavra) {
                foreach (string c in associacao.conexoes) {
                    if (c == conexao) {
                        return true;
                    }
                }
                break;
            }
        }
        return false;
    }

    public string[] GetPalavras() {
        List<string> palavras = new List<string>();
        foreach (Associacao associacao in associacoes) {
            palavras.Add(associacao.palavra);
        }
        return palavras.ToArray();
    }

    public string[] GetPalavras(string conexao) {
        List<string> palavras = new List<string>();
        foreach (Associacao associacao in associacoes) {
            if (associacao.conexoes.Contains(conexao)) {
                palavras.Add(associacao.palavra);
            }
        }
        return palavras.ToArray();
    }

    public string[] GetConexoes() {
        HashSet<string> conexoes = new HashSet<string>();
        foreach (Associacao associacao in associacoes) {
            foreach (string conexao in associacao.conexoes) {
                conexoes.Add(conexao);
            }
        }
        return new List<string>(conexoes).ToArray();
    }

    public string[] GetConexoes(string palavra) {
        foreach (Associacao associacao in associacoes) {
            if (associacao.palavra == palavra) {
                return associacao.conexoes;
            }
        }
        return new string[0];
    }
}

[System.Serializable]
public class Associacao {
    public string palavra;
    public string[] conexoes;
}