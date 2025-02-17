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
    
        return new File([JSON.stringify(info, null, "\t")], 'JogoArtigoResumo.json', {type: 'application/json'});
    }
}