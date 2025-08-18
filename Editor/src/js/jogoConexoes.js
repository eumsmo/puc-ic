const MAXIMO_TERMOS = 10;
const MAXIMO_CONEXOES_POR_TERMO = 3;

function setErrorAtInput(input, errorText) {
    input.errorText = errorText;
    input.error = true;

    input.addEventListener("input", unErrorInput);
}

function unErrorInput(evt) {
    const input = evt instanceof Event ? evt.currentTarget : evt; 
    input.errorText = "";
    input.error = false;

    input.removeEventListener("input", unErrorInput);
}

class JogoConexoes {
    tituloInput = null;
    urlInput = null;

    tabsHolder = null;
    tabCategorias = null;
    tabConexoes = null;

    termosCat1Inp = null;
    termosCat1Count = null;
    termosCat1Btn = null;
    termosCat1Chipset = null;
    termosCat1List = [];

    termosCat2Inp = null;
    termosCat2Count = null;
    termosCat2Btn = null;
    termosCat2Chipset = null;
    termosCat2List = [];

    criarConexoesBtn = null;
    conexoesHolder = null;
    cat1Selects = []; // todos os selects de categoria 1
    cat2Selects = []; // todos os selects de categoria 2

    constructor() {
        this.tituloInput = document.querySelector("#associacaoTitulo");
        this.urlInput = document.querySelector("#associacaoUrl");

        this.tabsHolder = document.querySelector("#associacoesTabs");
        this.tabsHolder.addEventListener("change", this.handleTabChange.bind(this));
        this.tabCategorias = document.querySelector("#categorias-panel");
        

        this.termosCat1Inp = document.querySelector("#termosCaterogia1Inp");
        this.termosCat1Count = document.querySelector("#termosCategoria1Count");
        this.termosCat1Count.innerHTML = "(0/" + MAXIMO_TERMOS + ")";
        this.termosCat1Btn = document.querySelector("#termosCaterogia1Btn");
        this.termosCat1Chipset = document.querySelector("#termosCat1Chipset");
        this.termosCat1Btn.addEventListener("click", this.handleTermoBtnClick(1));

        this.termosCat2Inp = document.querySelector("#termosCaterogia2Inp");
        this.termosCat2Count = document.querySelector("#termosCategoria2Count");
        this.termosCat2Count.innerHTML = "(0/" + MAXIMO_TERMOS + ")";
        this.termosCat2Btn = document.querySelector("#termosCaterogia2Btn");
        this.termosCat2Chipset = document.querySelector("#termosCat2Chipset");
        this.termosCat2Btn.addEventListener("click", this.handleTermoBtnClick(2));

        this.conexoesHolder = document.querySelector("#associacoesHolder");
        this.criarConexoesBtn = document.querySelector("#novaAssociacaoBtn");
        this.criarConexoesBtn.addEventListener("click", e => { e.preventDefault(); this.criarNovaConexao(); });

        this.tabCategorias.classList.add("ativo");
        this.conexoesHolder.classList.remove("ativo");
    }

    handleTabChange(evt) {
        const ativo = evt.currentTarget.activeTabIndex;

        if (ativo == 0) {
            this.tabCategorias.classList.add("ativo");
            this.conexoesHolder.classList.remove("ativo");
        } else if (ativo == 1) {
            this.tabCategorias.classList.remove("ativo");
            this.conexoesHolder.classList.add("ativo");
        }
    }

    handleTermoBtnClick(categoria) {
        const input = categoria == 1 ? this.termosCat1Inp : this.termosCat2Inp;

        return (evt) => {
            evt.preventDefault();

            if (input.value.trim() == "") return;

            this.criarTermo(categoria, input.value);
            input.value = "";
        }
    }

    // categoria: 1 ou 2, termo: string
    criarTermo(categoria, termo) {
        const chipset = categoria == 1 ? this.termosCat1Chipset : this.termosCat2Chipset;
        const count = categoria == 1 ? this.termosCat1Count : this.termosCat2Count;
        const list = categoria == 1 ? this.termosCat1List : this.termosCat2List;

        if (list.includes(termo)) {
            const input = categoria == 1 ? this.termosCat1Inp : this.termosCat2Inp;
            setErrorAtInput(input, "Termo já existe!");
            return;
        }

        if (list.length >= MAXIMO_TERMOS) {
            const input = categoria == 1 ? this.termosCat1Inp : this.termosCat2Inp;
            setErrorAtInput(input, "Número máximo de termos alcançado!");
            return;
        }

        const chip = document.createElement("md-input-chip");
        chip.label = termo;
        list.push(termo);

        chip.addEventListener("remove", e => {
            this.onTermoDeleted(categoria, termo);
        });

        chipset.appendChild(chip);

        count.innerHTML = `(${list.length}/${MAXIMO_TERMOS})`;

        this.updateTermoList(categoria);
    }

    updateTermoList(categoria) {
        const selects = categoria == 1? this.cat1Selects : this.cat2Selects;
        selects.forEach(select => this.updateSelectOptions(select, categoria));
    }

    onTermoDeleted(cat, termo) {
        if (cat == 1) {
            this.termosCat1List = this.termosCat1List.filter(x => x != termo);
            this.termosCat1Count.innerHTML = `(${this.termosCat1List.length}/${MAXIMO_TERMOS})`;
        } else {
            this.termosCat2List = this.termosCat2List.filter(x => x != termo);
            this.termosCat2Count.innerHTML = `(${this.termosCat2List.length}/${MAXIMO_TERMOS})`;

        }

        this.updateTermoList(cat);
    }


    criarNovaConexao() {
        const form = document.createElement("form");
        form.classList.add("associacoesForm");

        const section1 = document.createElement("section");
        const select1 = document.createElement("md-outlined-select");
        select1.label = "Categoria 1";
        this.cat1Selects.push(select1);

        this.updateSelectOptions(select1, 1);
        
        section1.appendChild(select1);
        form.appendChild(section1);


        const section2 = document.createElement("section");
        const select2 = document.createElement("md-outlined-select");
        select2.label = "Categoria 2";
        this.cat2Selects.push(select2);

        this.updateSelectOptions(select2, 2);

        section2.appendChild(select2);
        form.appendChild(section2);


        const footer = document.createElement("footer");
        const delButton = document.createElement("md-icon-button");
        delButton.addEventListener("click", this.onConexaoDeleted.bind(this));

        const delIcon = document.createElement("md-icon");
        delIcon.innerHTML = "delete";
        delButton.appendChild(delIcon);

        footer.appendChild(delButton);
        form.appendChild(footer);
        
        this.conexoesHolder.insertBefore(form, this.criarConexoesBtn.parentNode);
    }

    onConexaoDeleted(evt) {
        evt.preventDefault();

        const button = evt.currentTarget;
        const holder = button.parentNode.parentNode; // button -> footer -> form;
        const select1 = holder.querySelectorAll("md-outlined-select[label='Categoria 1']");
        const select2 = holder.querySelectorAll("md-outlined-select[label='Categoria 2']");

        this.cat1Selects = this.cat1Selects.filter(x => x != select1);
        this.cat2Selects = this.cat2Selects.filter(x => x != select2);

        this.conexoesHolder.removeChild(holder);
    }

    updateSelectOptions(select, categoria) {
        const list = categoria == 1 ? this.termosCat1List : this.termosCat2List;
        let value = select.value;
        select.innerHTML = "";

        for (const termo of list) {
            const option = document.createElement("md-select-option");
            option.value = termo;

            const headline = document.createElement("div");
            headline.slot = "headline";
            headline.innerHTML = termo;

            option.appendChild(headline);
            select.appendChild(option);
        }

        if (list.includes(value)) select.select(value);
        else select.reset();
    }

    // Retorna: {status: true} ou {status: false, msg: "Mensagem de erro"}
    validar() {
        if (this.tituloInput.value.trim() == "") return {status: false, msg: "Título vazio."};
        if (this.urlInput.value.trim() == "") return { status: false, msg: "URL do artigo vazia." };
        
        const cat1Count = {}, cat2Count = {};
        let conexoesCount = 0;
        let ok = true;

        for (const conexao of this.conexoesHolder.children) {
            console.log(conexao, this.criarConexoesBtn.parentNode);
            if (conexao == this.criarConexoesBtn.parentNode) continue; // Ignora botão de criar conexão

            const selects = conexao.querySelectorAll("md-outlined-select");
            const s1 = selects[0], s2 = selects[1];
            const cat1 = s1.value, cat2 = s2.value;

            if (cat1.trim() == "") {
                setErrorAtInput(s1, "Valor não pode ser vazio!");
                ok = false;
            }

            if (cat2.trim() == "") {
                setErrorAtInput(s2, "Valor não pode ser vazio!");
                ok = false;
            }

            cat1Count[cat1] = cat1Count[cat1] == null ? 1 : cat1Count[cat1]+1;
            if (cat1Count[cat1] > MAXIMO_CONEXOES_POR_TERMO) return {status: false, msg: "A categoria \"" + cat1 + "\" possui mais de " + MAXIMO_CONEXOES_POR_TERMO + " conexões, o que não é permitido."}

            cat2Count[cat2] = cat2Count[cat2] == null? 1 : cat2Count[cat2]+1;
            if (cat1Count[cat2] > MAXIMO_CONEXOES_POR_TERMO) return {status: false, msg: "A categoria \"" + cat2 + "\" possui mais de " + MAXIMO_CONEXOES_POR_TERMO + " conexões, o que não é permitido."}
            
            conexoesCount++;
        }

        if (!ok) return {status: false, msg: "Nem todos os campos estão preenchidos!"};

        if (conexoesCount == 0) return {status: false, msg: "Nenhuma conexão selecionada!"};

        return {status: true}
    }


    gerarDocumento() {
        const doc = {
            titulo: this.tituloInput.value,
            url: this.urlInput.value,
            associacoes: []
        };

        const conexoes = {};
        for (const conexao of this.conexoesHolder.children) {
            if (conexao == this.criarConexoesBtn.parentNode) continue; // Ignora botão de criar conexão
            const [s1, s2] = conexao.querySelectorAll("md-outlined-select");
            const cat1 = s1.value, cat2 = s2.value;

            if (cat1.trim() == "" || cat2.trim() == "") continue;
            
            if (conexoes[cat1] != null) conexoes[cat1].push(cat2);
            else conexoes[cat1] = [cat2];
        }

        for (const cat1 in conexoes) {
            const cat2 = conexoes[cat1];
            doc.associacoes.push({palavra: cat1, conexoes: cat2});
        }

        console.log(doc);

        return new PseudoDocumento(doc, 'JogoArtigoConexoes.json', {type: "application/json"});
    }
}