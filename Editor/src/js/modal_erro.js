class Erro {
    modal = document.querySelector('#modal_erro');
    modalClose = document.querySelector('#fecharModal_erro');
    modalTitulo = document.querySelector('#modalErro_Title');
    modalTexto = document.querySelector('#modalErro_Text');

    constructor() {
        this.modalClose.addEventListener('click', () => this.fechar());
        this.modal.addEventListener('click', (event) => {
            if (event.target === this.modal) {
                this.fechar();
            }
        });

        document.addEventListener('keydown', (event) => {
            if (event.key === 'Escape') {
                this.fechar();
            }
        });
    }

    abrir(erro_info) {
        this.atualizarValores(erro_info);
        this.modal.classList.add('aberto');
    }

    fechar() {
        this.modal.classList.remove('aberto');
    }

    atualizarValores(erro_info) {
        this.modalTitulo.innerText = "Valores invalidos";
        this.modalTexto.innerText = erro_info;
    }
}
