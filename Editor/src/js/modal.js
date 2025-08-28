const modalInfo = {
    dados_grafico: {
        titulo: 'Gráfico',
        texto: 'Na pergunta do tipo "Gráfico", o jogador deve selecionar o valor (ou porcentagem) mais próximo do valor real do gráfico. Há uma margem de erro de 5%. Esse tipo só conta como acerto se o jogador acertar o valor de pelo menos metade dos campos do gráfico!',
        img: 'src/imgs/exemplos/grafico.png'
    },
    dados_booleano: {
        titulo: 'Afirmativa',
        texto: 'Na pergunta do tipo "Afirmativa", o jogador deve responder se a afirmação é verdadeira ou falsa.',
        img: 'src/imgs/exemplos/afirmativa.png'
    },
    dados_porcentagem: {
        titulo: 'Porcentagem',
        texto: 'Na pergunta do tipo "Porcentagem", o jogador deve selecionar a porcentagem correta de um valor. Há uma margem de erro de 5%.',
        img: 'src/imgs/exemplos/porcentagem.png'
    },
    dados: {
        titulo: 'Responder Perguntas',
        texto: 'O jogo é composto por diversas perguntas que podem ser de três tipos diferentes: Gráfico, Afirmativa e Porcentagem. Cada tipo de pergunta tem uma forma diferente de responder. O objetivo é testar o conhecimento geral do jogador com base em perguntas relacionadas ao artigo.',
        img: 'src/imgs/dados.png'
    },
    resumo: {
        titulo: 'Descubrir o Titulo',
        texto: 'Nesse jogo, o jogador deve descobrir o título do artigo com base em um resumo. O resumo é uma descrição curta do conteúdo do artigo. Tanto o titulo quanto o resumo terão suas palavras ocultadas que só serão reveladas se o jogador digitar a palavra corretamente.',
        img: 'src/imgs/artigo.png'
    },
    conexoes: {
        titulo: 'Associações',
        texto: 'Nesse jogo, o jogador deve conectar palavras entre duas colunas. Ao clicar em uma palavra de uma das colunas, o jogador pode conecta-la a outra palavra de uma outra coluna. Cada palavra pode ter mais de uma conexão, porém o limite a ser inserido são no máximo 3 conexões por palavra.',
        img: 'src/imgs/conexao.png'
    },
};

const abrirDadosEl = document.querySelector('#modalDados');
const abrirArtigoEl = document.querySelector('#modalArtigo');
const abrirConexoesEl = document.querySelector('#modalAssociacoes');

class Modal {
    modal = document.querySelector('#modal');
    modalClose = document.querySelector('#fecharModal');
    modalImg = document.querySelector('#modalImg');
    modalTitulo = document.querySelector('#modalTitle');
    modalTexto = document.querySelector('#modalText');

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

        abrirDadosEl.addEventListener('click', () => this.abrir('dados'));
        abrirArtigoEl.addEventListener('click', () => this.abrir('resumo'));
        abrirConexoesEl.addEventListener('click', () => this.abrir('conexoes'));
    }

    abrir(tipo) {
        this.atualizarValores(tipo);
        this.modal.classList.add('aberto');
    }

    fechar() {
        this.modal.classList.remove('aberto');
    }

    atualizarValores(tipo) {
        this.modalTitulo.innerText = modalInfo[tipo].titulo;
        this.modalTexto.innerText = modalInfo[tipo].texto;
        this.modalImg.src = modalInfo[tipo].img;
    }
}
