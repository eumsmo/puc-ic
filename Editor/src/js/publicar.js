// URL API
const drive_token = 'https://oauth2.googleapis.com/token';
const drive_criar = 'https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart';
const drive_permission =  (id) => 'https://www.googleapis.com/drive/v3/files/' + id + '/permissions';

// Valores fixos
const redirect_uri = 'https://eumsmo.github.io';
const link_jogoResumo = file_id => 'https://eumsmo.github.io/puc-ic/Build/?drive=' + file_id;
const link_jogoDados = file_id => 'https://eumsmo.github.io/puc-ic/Dados/Build/?drive=' + file_id;

// Elements
const holder = document.querySelector('#holder');
const indicador = document.querySelector('#indicadorProvisorio');
const escolheDados = document.querySelector("#escolheDados");
const escolheArtigo = document.querySelector("#escolheArtigo");

const voltarBtns = document.querySelectorAll('.voltar');
const irInicioBtns = document.querySelectorAll('.ir-inicio');
const irManualBtns = document.querySelectorAll('.irManual');
const irGoogleBtns = document.querySelectorAll('.irGoogle');
const baixarManualBtn = document.querySelector('#baixarArquivoBtn');

const compartilhamentoLinkInput = document.querySelector('#compartilhamentoLink');
const gerarLinkBtn = document.querySelector('#gerarLinkBtn');

const googleProgress = document.querySelector('#googleProgress');
const googleProgressText = document.querySelector('#googleProgressText');

const linkHolder = document.querySelector('#linkEl');


class Publicador {
    #auth_token = '';
    #client_id = '';
    #client_secret = '';

    #setup_promise = null;
    controlador = null;

    constructor(controlador) {
        this.controlador = controlador;
        this.setup();
    }

    setup() {
        if (this.#setup_promise != null) return this.#setup_promise;

        let promises = [];
        promises.push(fetch('./src/id').then(res => res.text()).then(id => this.#client_id = id));
        promises.push(fetch('./src/secret').then(res => res.text()).then(secret => this.#client_secret = secret));
        this.#setup_promise = Promise.all(promises);
        return this.#setup_promise;
    }

    async autenticar() {
        console.log('Autenticando...');

        googleProgress.value = 0;
        googleProgressText.innerHTML = 'Aguardando autenticação do usuário em outra janela...';

        let res = null;

        try {
            res = await new Promise(async (resolve, reject) => {
                const client = google.accounts.oauth2.initCodeClient({
                    client_id: this.#client_id,
                    scope: 'https://www.googleapis.com/auth/drive.file',
                    ux_mode: 'popup',
                    callback: res => {
                        this.#auth_token = res.code;
                        resolve(res);
                    },
                    error_callback: reject
                });
                client.requestCode();
            });
        } catch (error) {
            googleProgressText.innerHTML = 'Falha na autenticação';
            this.voltarAntesDeGerar();
            console.error(error);
        }

        if (res == null) return;

        googleProgress.value = 0.15;
        googleProgressText.innerHTML = 'Gerando código de acesso...';

        let code = res.code;

        let token_res = await fetch(drive_token, {
            method: 'POST',
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            body: `client_id=${this.#client_id}&code=${code}&client_secret=${this.#client_secret}&grant_type=authorization_code&redirect_uri=${redirect_uri}`
        });

        let json = await token_res.json();
        this.#auth_token = json.access_token;
        return json;
    }


    async uplodarDocumentoDrive(documento, access_token) {
        const metadados = {
            name: documento.name,
            mimeType: 'application/json'
        };

        let formdata = new FormData();
        formdata.append('metadata', new Blob([JSON.stringify(metadados)], {type: 'application/json'}));
        formdata.append('file', documento);

        const response = await fetch(drive_criar, {
            method: 'POST',
            headers: {
                "Authorization": `Bearer ${access_token}`
            },
            body: formdata
        });

        return response.json();
    }

    async setarPermissoesDrive(file_id, access_token) {
        const response = await fetch(drive_permission(file_id), {
            method: 'POST',
            headers: {
                "Authorization": `Bearer ${access_token}`,
                "Content-Type": "application/json",
                "Accept": "application/json"
            },
            body: JSON.stringify({
                role: 'reader',
                type: 'anyone'
            })
        });

        return response.json();
    }

    async salvarDocumentoDrive(documento, access_token = this.#auth_token) {
        googleProgress.value = 0.3; // 30%
        googleProgressText.innerHTML = 'Dando upload do arquivo no drive...';

        let file_id = null;
        let deu_erro = false;

        try {
            // Criar o documento
            const uploadRes = await this.uplodarDocumentoDrive(documento, access_token);

            googleProgress.value = 0.85; // 85%
            googleProgressText.innerHTML = 'Setando permissões...';

            file_id = uploadRes.id;
            const permRes = await this.setarPermissoesDrive(file_id, access_token);

            googleProgress.value = 0.95; // 95%
            googleProgressText.innerHTML = 'Gerando link...';
            
        } catch (error) {
            googleProgressText.innerHTML = 'Falha ao salvar documento';
            this.controlador.setarErro();
            console.error(error);
            deu_erro = true;
        }

        if (deu_erro) return;
        
        let link = this.controlador.pegarLinkFunc()(file_id);
        console.log(link);

        googleProgress.value = 1;
        googleProgressText.innerHTML = 'Concluído';

        this.setarLink(link);
    }

    setarLink(link) {
        linkHolder.innerHTML = link;
        linkHolder.href = link;
    
        this.controlador.setarEstado('link');
    }

    voltarAntesDeGerar() {
        let jogo = this.controlador.pegarTipoJogo();
        if (jogo == 'resumo') this.controlador.setarEstado('valores');
        else if (jogo == 'dados') this.controlador.setarEstado('dados');
        else this.controlador.setarEstado('escolha');
    }

    async gerarGoogle() {
        await this.setup();
        this.controlador.setarEstado('google');

        let file = this.controlador.pegarJogo().gerarDocumento();

        let res = null;

        try {
            res = await this.autenticar();
        } catch (error) {
            googleProgressText.innerHTML = 'Falha na autenticação';
            this.controlador.setarErro();
            console.error(error);
        }

        if (res == null) return;

        let access_token = res.access_token;

        this.salvarDocumentoDrive(file, access_token);
    }

    salvarDocumentoPC(file) {
        const a = document.createElement('a');
        a.href = URL.createObjectURL(file);
        a.download = file.name;
        a.click();
    }

    gerarManual() {
        const file = this.controlador.pegarJogo().gerarDocumento();
        this.salvarDocumentoPC(file);
    }

    linkCompartilhamentoToGame(link) {
        let parte = link.split('file/d/');
        let id = parte[1].split('/')[0];
        return this.controlador.pegarLinkFunc()(id);
    }
}

class Controlador {
    static instancia = null;
    estados = ['escolha', 'valores', 'dados', 'manual', 'google', 'link', 'erro'];
    jogo_selecionado = null; // 'resumo' ou 'dados'

    publicador = null;
    jogo_resumo = null;
    jogo_dados = null;

    constructor() {
        Controlador.instancia = this;

        this.publicador = new Publicador(this);
        this.jogo_resumo = new JogoResumo(this);
        this.jogo_dados = new JogoDados(this);
    }

    setarEstado(estado) {
        this.estados.forEach(e => holder.classList.remove(e));
        holder.classList.add(estado);

        if (estado == 'valores' && this.jogo_selecionado != "resumo") this.setarJogo("resumo");
        else if (estado == 'dados' && this.jogo_selecionado != "dados") this.setarJogo("dados");
    }

    pegarEstado() {
        for (let i = 0; i < this.estados.length; i++) {
            if (holder.classList.contains(this.estados[i])) return this.estados[i];
        }

        return escolha;
    }

    setarErro() {
        holder.classList.add('erro');
    }

    setarJogo(jogo) {
        this.jogo_selecionado = jogo;

        if (jogo == 'resumo') this.setarEstado('valores');
        else if (jogo == 'dados') this.setarEstado('dados');
    }

    pegarJogo() {
        if (this.jogo_selecionado == 'resumo') return this.jogo_resumo;
        if (this.jogo_selecionado == 'dados') return this.jogo_dados;
        return null;
    }

    pegarTipoJogo() {
        return this.jogo_selecionado;
    }

    pegarLinkFunc() {
        if (this.jogo_selecionado == 'resumo') return link_jogoResumo;
        if (this.jogo_selecionado == 'dados') return link_jogoDados;
        return null;
    }
}

const controlador = new Controlador();

function receberAutenticacao(conteudo) {
    controlador.publicador.receberAutenticacao(conteudo);
}

gerarLinkBtn.addEventListener('click', () => {
    let link = controlador.publicador.linkCompartilhamentoToGame(compartilhamentoLinkInput.value);
    controlador.publicador.setarLink(link);
});

voltarBtns.forEach(btn => btn.addEventListener('click', () => controlador.publicador.voltarAntesDeGerar()));
irInicioBtns.forEach(btn => btn.addEventListener('click', () => controlador.setarEstado('escolha')));


escolheArtigo.addEventListener("click", () => controlador.setarEstado('valores'));
escolheDados.addEventListener("click", () => controlador.setarEstado('dados'));

baixarManualBtn.addEventListener('click', () => controlador.publicador.gerarManual());
irManualBtns.forEach(btn => btn.addEventListener('click', () => controlador.setarEstado('manual')));
irGoogleBtns.forEach(btn => btn.addEventListener('click', () => controlador.publicador.gerarGoogle()));