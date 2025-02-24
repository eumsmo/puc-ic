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
}