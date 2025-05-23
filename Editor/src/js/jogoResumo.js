class JogoResumo {
    tituloInput = document.querySelector('#titulo');
    urlInput = document.querySelector('#url');
    resumoInput = document.querySelector('#resumo');

    constructor(controlador) { }

    gerarDocumento() {
        let info = {
            titulo: this.tituloInput.value,
            url: this.urlInput.value,
            resumo: this.resumoInput.value
        };
    
        return new PseudoDocumento(info, 'JogoArtigoResumo.json', {type: 'application/json'});
    }

    // Retorna: {status: true} ou {status: false, msg: "Mensagem de erro"}
    validar() {
        if (this.tituloInput.value.trim() == "") return {status: false, msg: "Título vazio."};
        if (this.urlInput.value.trim() == "") return {status: false, msg: "URL do artigo vazia."};
        if (this.resumoInput.value.trim() == "") return {status: false, msg: "O resumo do artigo está vazio."};
        
        return {status: true};
    }
}