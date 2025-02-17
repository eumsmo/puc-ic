class OpcaoDados {
    id = -1;
    form = null;
    tipo = "";
    tipo_porcentagem = "";

    perguntaField = null;
    trueRadio = null;
    falseRadio = null;

    slider = null;
    margemDeErro = null;
    valorMinimo = null;
    valorMaximo = null;

    constructor(id) {
        this.id = id;
        this.#criarForm();
    }

    #criarForm() {
        let form = document.createElement("form");
        form.classList.add("dadosForm");
        this.form = form;

        let sectionBase = document.createElement("section");

        this.perguntaField = document.createElement("md-outlined-text-field");
        this.perguntaField.label = "Pergunta";
        this.perguntaField.classList.add("pergunta", "text-input");
        sectionBase.appendChild(this.perguntaField);

        let tipoPerguntaField = document.createElement("md-outlined-select");
        tipoPerguntaField.label = "Tipo de pergunta";
        const tipos = [["booleano", "Afirmativa"], ["porcentagem", "Valor bruto"], ["grafico", "Gráfico"]];
        tipos.forEach((tipo, i) => {
            let option = document.createElement("md-select-option");
            option.value = tipo[0];
            if (i == 0) option.selected = true;

            let div = document.createElement("div");
            div.slot = "headline";
            div.innerText = tipo[1];

            option.appendChild(div);
            tipoPerguntaField.appendChild(option);
        });
        tipoPerguntaField.addEventListener("change", () => this.setTipoPergunta(tipoPerguntaField.value));
        this.setTipoPergunta(tipos[0][0]);
        sectionBase.appendChild(tipoPerguntaField);

        form.appendChild(sectionBase);


        let boolSection = this.#criarSecaoBooleano();
        form.appendChild(boolSection);

        let porcentagemSection = this.#criarSecaoPorcentagem();
        form.appendChild(porcentagemSection);


        let footer = document.createElement("footer");
        footer.classList.add("dadosFooter");
        
        let removerBtn = document.createElement("md-icon-button");
        removerBtn.addEventListener("click", this.removerOpcao);
        let icon = document.createElement("md-icon");
        icon.innerText = "delete";
        removerBtn.appendChild(icon);
        footer.appendChild(removerBtn);
        form.appendChild(footer);

        return form;
    }

    // Retorna section booleana
    #criarSecaoBooleano() {
        let section = document.createElement("section");
        section.classList.add("dadoBooleano");

        const opcoes = [["true", "Verdadeiro"], ["false", "Falso"]];
        
        opcoes.forEach((opcao, i) => {
            let radio = document.createElement("md-radio");
            radio.name = `booleano${this.id}`;
            radio.value = opcao[0];
            if (i == 0) {
                radio.checked = true;
                this.trueRadio = radio;
            } else {
                this.falseRadio = radio;
            }

            let span = document.createElement("span");
            span.appendChild(radio);

            let text = document.createTextNode(opcao[1]);
            span.appendChild(text);

            section.appendChild(span);
        });

        return section;
    }

    // Retorna section porcentagem
    #criarSecaoPorcentagem() {
        let section = document.createElement("section");
        section.classList.add("dadoPorcentagem");

        let tipoValorField = document.createElement("md-outlined-select");
        tipoValorField.label = "Tipo de resposta";

        const opcoes = [["porcentagem", "Porcentagem"], ["valor", "Valor bruto"]];
        opcoes.forEach((tipo, i) => {
            let option = document.createElement("md-select-option");
            option.value = tipo[0];
            if (i == 0) option.selected = true;

            let div = document.createElement("div");
            div.slot = "headline";
            div.innerText = tipo[1];

            option.appendChild(div);
            tipoValorField.appendChild(option);
        });
        tipoValorField.addEventListener("change", () => this.setTipoPorcentagem(tipoValorField.value));
        this.setTipoPorcentagem(opcoes[0][0]);
        section.appendChild(tipoValorField);


        // <md-outlined-text-field class="input-d3" type="number" label="Margem de erro"></md-outlined-text-field>
        this.margemDeErro = document.createElement("md-outlined-text-field");
        this.margemDeErro.label = "Margem de erro";
        this.margemDeErro.type = "number";
        this.margemDeErro.classList.add("input-d3");
        section.appendChild(this.margemDeErro);

        // <md-outlined-text-field class="input-d3" type="number" label="Valor minimo"></md-outlined-text-field>
        this.valorMinimo = document.createElement("md-outlined-text-field");
        this.valorMinimo.label = "Valor mínimo";
        this.valorMinimo.type = "number";
        this.valorMinimo.classList.add("input-d3", "minmax");
        section.appendChild(this.valorMinimo);

        // <md-outlined-text-field class="input-d3" type="number" label="Valor máximo"></md-outlined-text-field>
        this.valorMaximo = document.createElement("md-outlined-text-field");
        this.valorMaximo.label = "Valor máximo";
        this.valorMaximo.type = "number";
        this.valorMaximo.classList.add("input-d3", "minmax");
        section.appendChild(this.valorMaximo);

        // <md-slider class="sliderPorcentagem" labeled></md-slider>
        this.slider = document.createElement("md-slider");
        this.slider.classList.add("sliderPorcentagem");
        this.slider.labeled = true;
        section.appendChild(this.slider);

        return section;
    }


    // Chamado ao escolher um tipo de pergunta
    // tipo: "booleano" ou "porcentagem" (grafico ainda não implementado)
    setTipoPergunta(tipo) {
        const classes = [
            {classe: "dadoBooleano", tipo: "booleano", formSelector: "opcaoBooleano"},
            {classe: "dadoPorcentagem", tipo: "porcentagem", formSelector: "opcaoPorcentagem"}
        ];

        classes.forEach(c => {
            if (tipo == c.tipo) this.form.classList.add(c.formSelector);
            else this.form.classList.remove(c.formSelector);
        });

        this.tipo = tipo;
    }

    setTipoPorcentagem(tipo) {
        const classes = [
            {classe: "porcentagem", tipo: "porcentagem", formSelector: "porcentagemPorcentagem"},
            {classe: "valor", tipo: "valor", formSelector: "porcentagemValor"}
        ];

        classes.forEach(c => {
            if (tipo == c.tipo) this.form.classList.add(c.formSelector);
            else this.form.classList.remove(c.formSelector);
        });

        this.tipo_porcentagem = tipo;
    }

    // Destroi o form
    removerOpcao() {
        this.form.remove();
    }

    // To JSON
    toJSON() {
        const objeto = {
            tipo: this.tipo,
            texto: this.perguntaField.value
        };

        if (this.tipo == "booleano") {
            objeto.respostaBool = this.trueRadio.checked;
        } else if (this.tipo == "porcentagem") {
            objeto.respostaFloat = this.slider.value;
            objeto.range = { };
            
            objeto.range.range = parseFloat(this.margemDeErro.value);
            objeto.range.min = this.valorMinimo.value;
            objeto.range.max = this.valorMaximo.value;
            objeto.range.porcentagem = this.tipo_porcentagem == "porcentagem";

            if (objeto.range.porcentagem) {
                delete objeto.range.min;
                delete objeto.range.max;
                objeto.respostaFloat =  objeto.respostaFloat / 100.0;
            } else {
                objeto.range.min = parseFloat(objeto.range.min);
                objeto.range.max = parseFloat(objeto.range.max);
                delete objeto.range.porcentagem;
            }
        }

        return objeto;
    }
}

// Singleton para gerenciar os dados
class JogoDados {
    tituloInput = null;
    urlInput = null;
    dadosHolder = null;
    novaPerguntaBtn = null;

    #id = -1;
    perguntas = [];

    constructor(controlador) {
        this.tituloInput = document.querySelector("#dadosTitulo");
        this.urlInput = document.querySelector("#dadosUrl");
        this.dadosHolder = document.querySelector("#dadosHolder");
        this.novaPerguntaBtn = document.querySelector("#novaPerguntaBtn");
        this.novaPerguntaBtn.addEventListener("click", () => this.gerarNovaPergunta());

        this.limparPerguntas();
        this.gerarNovaPergunta();
    }

    limparPerguntas() {
        this.dadosHolder.innerHTML = "";
        this.perguntas = [];
    }

    gerarNovaPergunta() {
        this.#id++;

        let opcao = new OpcaoDados(this.#id);
        this.dadosHolder.appendChild(opcao.form);

        this.perguntas.push(opcao);
    }

    gerarDocumento() {
        const objeto = {
            titulo: this.tituloInput.value,
            url: this.urlInput.value
        };

        objeto.dados = this.perguntas.map(pergunta => pergunta.toJSON());
        
        return new File([JSON.stringify(objeto, null, "\t")], 'JogoArtigoDados.json', {type: "application/json"});
    }
}