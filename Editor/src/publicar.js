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
const tituloInput = document.querySelector('#titulo');
const urlInput = document.querySelector('#url');
const resumoInput = document.querySelector('#resumo');
const indicador = document.querySelector('#indicadorProvisorio');

const gerarBtn = document.querySelector('#gerarBtn');
const gerarBtnGoogle = document.querySelector('#gerarBtnGoogle');

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
            console.error(error);
        }

        if (res == null) return;

        console.log(res);
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
        // Criar o documento
        const uploadRes = await this.uplodarDocumentoDrive(documento, access_token);

        const file_id = uploadRes.id;
        const permRes = await this.setarPermissoesDrive(file_id, access_token);
        
        let link = criar_link_jogo(file_id);
        console.log(link);

        indicador.innerHTML = `Link do jogo: <a href="${link}" target="_blank">${link}</a>`;
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

        const file = this.gerarDocumento();
        let res = null;

        try {
            res = await this.autenticar();
        } catch (error) {
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
}

function receberAutenticacao(conteudo) {
    publicador.receberAutenticacao(conteudo);
}

const publicador = new Publicador();

gerarBtnGoogle.addEventListener('click', () => publicador.gerarGoogle());
gerarBtn.addEventListener('click', () => publicador.gerarManual());