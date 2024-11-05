// URL API
const drive_token = 'https://oauth2.googleapis.com/token';
const drive_criar = 'https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart';
const drive_permission =  (id) => 'https://www.googleapis.com/drive/v3/files/' + id + '/permissions';

// Valores fixos
const redirect_uri = 'https://eumsmo.github.io';
const criar_link_jogo = file_id => 'https://eumsmo.github.io/puc-ic/Build/?drive=' + file_id;

// Outros valores
const file_name = 'jogo_artigo_info.json';

// Elements
const holder = document.querySelector('#holder');
const tituloInput = document.querySelector('#titulo');
const urlInput = document.querySelector('#url');
const resumoInput = document.querySelector('#resumo');
const indicador = document.querySelector('#indicadorProvisorio');

const voltarBtns = document.querySelectorAll('.voltar');

const irParaManualBtn = document.querySelector('#irManual');
const baixarBtn = document.querySelector('#baixarArquivoBtn');
const compartilhamentoLinkInput = document.querySelector('#compartilhamentoLink');
const gerarLinkBtn = document.querySelector('#gerarLinkBtn');

const irParaGoogleBtn = document.querySelector('#irGoogle');
const googleProgress = document.querySelector('#googleProgress');
const googleProgressText = document.querySelector('#googleProgressText');

const linkHolder = document.querySelector('#linkEl');


class Publicador {
    #auth_token = '';
    #client_id = '';
    #client_secret = '';

    #setup_promise = null;

    constructor() {
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
            setarEstado('valores');
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
            name: file_name,
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

            googleProgress.value = 0.7; // 70%
            googleProgressText.innerHTML = 'Setando permissões...';

            file_id = uploadRes.id;
            const permRes = await this.setarPermissoesDrive(file_id, access_token);

            googleProgress.value = 0.9; // 90%
            googleProgressText.innerHTML = 'Gerando link...';
            
        } catch (error) {
            googleProgressText.innerHTML = 'Falha ao salvar documento';
            setarErro();
            console.error(error);
            deu_erro = true;
        }

        if (deu_erro) return;
        
        let link = criar_link_jogo(file_id);
        console.log(link);

        googleProgress.value = 1; // 90%
        googleProgressText.innerHTML = 'Concluído';

        setarLink(link);
    }
    
    gerarDocumento() {
        let info = {
            titulo: tituloInput.value,
            url: urlInput.value,
            resumo: resumoInput.value
        };
    
        return new File([JSON.stringify(info)], file_name, {type: 'application/json'});
    }

    async gerarGoogle() {
        await this.setup();
        setarEstado('google');

        const file = this.gerarDocumento();
        let res = null;

        try {
            res = await this.autenticar();
        } catch (error) {
            googleProgressText.innerHTML = 'Falha na autenticação';
            setarErro();
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
        const file = this.gerarDocumento();
        this.salvarDocumentoPC(file);
    }

    linkCompartilhamentoToGame(link) {
        let parte = link.split('file/d/');
        let id = parte[1].split('/')[0];
        return criar_link_jogo(id);
    }
}

estados = ['valores', 'manual', 'google', 'link', 'erro'];
function setarEstado(estado) {
    estados.forEach(e => holder.classList.remove(e));
    holder.classList.add(estado);
}

function setarErro() {
    holder.classList.add('erro');
}

function setarLink(link) {
    linkHolder.innerHTML = link;
    linkHolder.href = link;

    setarEstado('link');
}

function receberAutenticacao(conteudo) {
    publicador.receberAutenticacao(conteudo);
}

const publicador = new Publicador();



baixarBtn.addEventListener('click', () => publicador.gerarManual());
gerarLinkBtn.addEventListener('click', () => {
    let link = publicador.linkCompartilhamentoToGame(compartilhamentoLinkInput.value);
    setarLink(link);
});

irParaManualBtn.addEventListener('click', () => setarEstado('manual'));
irParaGoogleBtn.addEventListener('click', () => publicador.gerarGoogle());

voltarBtns.forEach(btn => btn.addEventListener('click', () => setarEstado('valores')));