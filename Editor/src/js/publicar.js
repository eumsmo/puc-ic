// URL API
const drive_token = 'https://oauth2.googleapis.com/token';
const drive_criar = 'https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart';
const drive_permission =  (id) => 'https://www.googleapis.com/drive/v3/files/' + id + '/permissions';

const achar_pasta_query = (nome) => `q=name='${nome}' and mimeType='application/vnd.google-apps.folder'`;
const drive_achar_pasta = 'https://www.googleapis.com/drive/v3/files?pageSize=1&';
const drive_criar_pasta = 'https://www.googleapis.com/drive/v3/files';
const drive_atualizar = (id) => 'https://www.googleapis.com/drive/v3/files/' + id + '?uploadType=multipart';

// Valores fixos
const redirect_uri = 'https://eumsmo.github.io';
const nome_pasta = 'Jogos-Artigos';
const link_editor = 'https://eumsmo.github.io/puc-ic/Editor/';
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

class PseudoDocumento {
    name = '';
    conteudo = {};
    metadados = {};
    titulo = '';
    prefixo = '';

    constructor(conteudo, name, metadados) {
        this.name = name;
        this.conteudo = conteudo;
        this.metadados = metadados;
    }

    asFile() {
        return new File([this.prefixo + JSON.stringify(this.conteudo, null, "\t")], this.name, this.metadados);
    }
}


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

        this.setarProgresso(0, 'Aguardando autenticação do usuário em outra janela...');

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
        
        this.setarProgresso(0.15, 'Gerando código de acesso...');

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

    async encontrarIdPasta(access_token) {
        const link_achar_pasta = drive_achar_pasta + achar_pasta_query(nome_pasta);

        const response = await fetch(link_achar_pasta, {
            method: 'GET',
            headers: {
                "Authorization": `Bearer ${access_token}`
            }
        });

        const json = await response.json();

        if (json.files == null || json.files.length == 0) return null;
        return json.files[0].id;
    }

    async criarPastaDrive(access_token) {
        const metadados = {
            name: nome_pasta,
            mimeType: 'application/vnd.google-apps.folder'
        };

        const response = await fetch(drive_criar_pasta, {
            method: 'POST',
            headers: {
                "Authorization": `Bearer ${access_token}`
            },
            body: JSON.stringify(metadados)
        });

        const json = await response.json();
        return json.id;
    }

    async uplodarDocumentoDrive(documento, folder_id, access_token) {
        const metadados = {
            name: documento.name,
            mimeType: 'application/json',
            parents: [folder_id]
        };

        let formdata = new FormData();
        formdata.append('metadata', new Blob([JSON.stringify(metadados)], {type: 'application/json'}));
        formdata.append('file', documento.asFile());

        const response = await fetch(drive_criar, {
            method: 'POST',
            headers: {
                "Authorization": `Bearer ${access_token}`
            },
            body: formdata
        });

        return await response.json();
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

    async atualizarDocumentoDrive(file_id, documento, access_token) {
        const metadados = {
            name: documento.name,
            mimeType: 'application/json'
        };

        documento.prefixo = `
/* 
Este é um arquivo essencial para o funcionamento do jogo "${this.controlador.pegarNomeJogo()}" do artigo "${documento.conteudo.titulo}".
O jogo é acessivel pelo link "${this.controlador.pegarLinkFunc()(file_id)}".

Alterar o conteúdo deste arquivo pode causar problemas no jogo.
Arquivo criado automaticamente através do site "${link_editor}".
*/
`;

        let formdata = new FormData();
        formdata.append('metadata', new Blob([JSON.stringify(metadados)], {type: 'application/json'}));
        formdata.append('file', documento.asFile());

        const response = await fetch(drive_atualizar(file_id), {
            method: 'PATCH',
            headers: {
                "Authorization": `Bearer ${access_token}`
            },
            body: formdata
        });

        return await response.json();
    }

    setarProgresso(valor, texto) {
        googleProgress.value = valor;
        googleProgressText.innerHTML = texto;
    }

    async salvarDocumentoDrive(documento, access_token = this.#auth_token) {
        let file_id = null;
        let deu_erro = false;

        try {
            // Acha pasta
            this.setarProgresso(0.25, 'Procurando pasta base no drive...');
            let folder_id = await this.encontrarIdPasta(access_token);

            if (folder_id == null) {
                this.setarProgresso(0.35, 'Não encontrada, criando pasta...');

                // Cria pasta
                folder_id = await this.criarPastaDrive(access_token);
            }
            
            this.setarProgresso(0.5, 'Dando upload do arquivo no drive...');

            // Criar o documento
            const uploadRes = await this.uplodarDocumentoDrive(documento, folder_id, access_token);

            this.setarProgresso(0.8, 'Setando permissões...');

            file_id = uploadRes.id;
            const permRes = await this.setarPermissoesDrive(file_id, access_token);
            
            this.setarProgresso(0.85, 'Skipando o atualizar arquivo...');
            //await this.atualizarDocumentoDrive(file_id, documento, access_token);

            this.setarProgresso(0.95, 'Gerando link...');
            
        } catch (error) {
            googleProgressText.innerHTML = 'Falha ao salvar documento';
            this.controlador.setarErro();
            console.error(error);
            deu_erro = true;
        }

        if (deu_erro) return;
        
        let link = this.controlador.pegarLinkFunc()(file_id);
        console.log(link);

        this.setarProgresso(1, 'Concluído');

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

        file.prefixo = `
/* 
Este é um arquivo essencial para o funcionamento do jogo "${this.controlador.pegarNomeJogo()}" do artigo "${file.conteudo.titulo}".
O jogo é acessivel pelo link "${this.controlador.pegarLinkFunc()("SEU_ID_AQUI")}".
Para conseguir o ID, você deve publicar o arquivo no Google Drive e pegar o ID do arquivo no link compartilhado (deve estar público).
Exemplo do link do drive: https://drive.google.com/file/d/[ID AQUI]/view?usp=sharing

Alterar o conteúdo deste arquivo pode causar problemas no jogo.
Arquivo criado automaticamente através do site "${link_editor}".
*/
`;

        a.href = URL.createObjectURL(file.asFile());
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

    modal = null;
    erro = null;

    constructor() {
        Controlador.instancia = this;

        this.modal = new Modal();
        this.erro = new Erro();
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

    pegarNomeJogo() {
        if (this.jogo_selecionado == 'resumo') return 'Descubrir o Titulo';
        if (this.jogo_selecionado == 'dados') return 'Responder Perguntas';
        return null;
    }

    validar() {
        let jogo = this.pegarJogo();
        if (jogo == null) return false;

        let validou = jogo.validar();
        if (!validou.status) {
            this.erro.abrir(validou.msg);
            return false;
        }

        return true;
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
irManualBtns.forEach(btn => btn.addEventListener('click', () => { if (controlador.validar()) controlador.setarEstado('manual'); }));
irGoogleBtns.forEach(btn => btn.addEventListener('click', () => { if (controlador.validar()) controlador.publicador.gerarGoogle() }));