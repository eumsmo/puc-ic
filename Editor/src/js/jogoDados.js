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
    valorMinimo_porcentagem = null;
    valorMaximo_porcentagem = null;

    valorMinimo_grafico = null;
    valorMaximo_grafico = null;

    controlador = null;
    pai = null; // pai = JogoDados

    get valorMinimo() {
        return this.tipo == "porcentagem" ? this.valorMinimo_porcentagem : this.valorMinimo_grafico;
    }
    get valorMaximo() {
        return this.tipo == "porcentagem" ? this.valorMaximo_porcentagem : this.valorMaximo_grafico;
    }

    get min() {
        return this.tipo_porcentagem == "porcentagem" ? 0 : parseFloat(this.valorMinimo.value);
    }

    get max() {
        return this.tipo_porcentagem == "porcentagem" ? 100 : parseFloat(this.valorMaximo.value);
    }

    graficoHolder = null;
    graficoIdBase = 0;

    constructor(pai, id) {
        this.id = id;
        this.pai = pai;
        this.controlador = pai.controlador;

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

        let graficoSection = this.#criarSecaoGrafico();
        form.appendChild(graficoSection);


        let footer = document.createElement("footer");
        footer.classList.add("dadosFooter");
        
        let infoButton = document.createElement("md-icon-button");
        infoButton.addEventListener("click", evt => {
            this.mostrarInformativo.call(this);
            evt.preventDefault();
        });

        let iconI = document.createElement("md-icon");
        iconI.innerText = "info";
        infoButton.appendChild(iconI);
        footer.appendChild(infoButton);

        let removerBtn = document.createElement("md-icon-button");
        removerBtn.addEventListener("click", evt => {
            this.removerOpcao.call(this);
            evt.preventDefault();
        });
        
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
        /*
        this.margemDeErro = document.createElement("md-outlined-text-field");
        this.margemDeErro.label = "Margem de erro";
        this.margemDeErro.type = "number";
        this.margemDeErro.classList.add("input-d3");
        section.appendChild(this.margemDeErro);
        */

        // <md-outlined-text-field class="input-d3" type="number" label="Valor minimo"></md-outlined-text-field>
        this.valorMinimo_porcentagem = document.createElement("md-outlined-text-field");
        this.valorMinimo_porcentagem.label = "Valor mínimo";
        this.valorMinimo_porcentagem.type = "number";
        this.valorMinimo_porcentagem.value = 0;
        this.valorMinimo_porcentagem.classList.add("input-d3", "minmax");
        this.valorMinimo_porcentagem.addEventListener("change", this.onMinMaxChange.bind(this));
        section.appendChild(this.valorMinimo_porcentagem);

        // <md-outlined-text-field class="input-d3" type="number" label="Valor máximo"></md-outlined-text-field>
        this.valorMaximo_porcentagem = document.createElement("md-outlined-text-field");
        this.valorMaximo_porcentagem.label = "Valor máximo";
        this.valorMaximo_porcentagem.type = "number";
        this.valorMaximo_porcentagem.value = 100;
        this.valorMaximo_porcentagem.classList.add("input-d3", "minmax");
        this.valorMaximo_porcentagem.addEventListener("change", this.onMinMaxChange.bind(this));
        section.appendChild(this.valorMaximo_porcentagem);

        let div = document.createElement("div");
        div.classList.add("porcentagemSliderHolder");

        let valor = document.createElement("md-outlined-text-field");
        valor.label = "Valor";
        valor.type = "number";
        valor.dataset.id = 0;
        valor.classList.add("input-d3");
        div.appendChild(valor);

        // <md-slider class="sliderPorcentagem" labeled></md-slider>
        this.slider = document.createElement("md-slider");
        this.slider.classList.add("sliderPorcentagem");
        this.slider.labeled = true;
        this.slider.dataset.id = 0;
        div.appendChild(this.slider);

        valor.addEventListener("change", () => this.#atualizaSliderPeloInput(valor, this.slider));

        this.slider.addEventListener("change", () => this.#atualizaInputPeloSlider(valor, this.slider));

        section.appendChild(div);

        return section;
    }

    // Retorna section grafico
    #criarSecaoGrafico() {
        let section = document.createElement("section");
        section.classList.add("dadoGrafico");

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

        // <md-outlined-text-field class="input-d3" type="number" label="Valor minimo"></md-outlined-text-field>
        this.valorMinimo_grafico = document.createElement("md-outlined-text-field");
        this.valorMinimo_grafico.label = "Valor mínimo";
        this.valorMinimo_grafico.type = "number";
        this.valorMinimo_grafico.value = 0;
        this.valorMinimo_grafico.classList.add("input-d3", "minmax");
        this.valorMinimo_grafico.addEventListener("change", this.onMinMaxChange.bind(this));
        section.appendChild(this.valorMinimo_grafico);

        // <md-outlined-text-field class="input-d3" type="number" label="Valor máximo"></md-outlined-text-field>
        this.valorMaximo_grafico = document.createElement("md-outlined-text-field");
        this.valorMaximo_grafico.label = "Valor máximo";
        this.valorMaximo_grafico.type = "number";
        this.valorMaximo_grafico.value = 100;
        this.valorMaximo_grafico.classList.add("input-d3", "minmax");
        this.valorMaximo_grafico.addEventListener("change", this.onMinMaxChange.bind(this));
        section.appendChild(this.valorMaximo_grafico);

        // <md-filled-button> Adicionar campo </md-filled-button>
        let novoCampoBtn = document.createElement("md-filled-button");
        novoCampoBtn.innerText = "Adicionar campo";
        novoCampoBtn.href = '#';
        novoCampoBtn.addEventListener("click", (evt) => {
            let grafico = this.#criarCampoGrafico();
            let slider = grafico.querySelector("md-slider");
            this.graficoHolder.appendChild(grafico);
            this.updateMinMaxOfSlider(slider);
            evt.preventDefault();
        });
        section.appendChild(novoCampoBtn);


        this.graficoHolder = document.createElement("div");
        this.graficoHolder.classList.add("graficoHolder");

        let grafico = this.#criarCampoGrafico();
        this.graficoHolder.appendChild(grafico);

        section.appendChild(this.graficoHolder);
        

        return section;
    }

    #criarCampoGrafico() {
        const id = this.graficoIdBase++;

        let div = document.createElement("div");
        div.classList.add("grafico");
        div.dataset.id = id;

        // <img src="https://via.placeholder.com/150" alt="Grafico">
        let removerBtn = document.createElement("md-icon-button");
        removerBtn.addEventListener("click", evt => {
            this.removeGrafico.call(this, id);
            evt.preventDefault();
        });

        let icon = document.createElement("md-icon");
        icon.innerText = "close";
        removerBtn.appendChild(icon);
        div.appendChild(removerBtn);


        // <md-outlined-text-field class="input-d3" label="Nome"></md-outlined-text-field>
        let nome = document.createElement("md-outlined-text-field");
        nome.label = "Nome";
        nome.classList.add("input-d3");
        div.appendChild(nome);

        // <md-outlined-text-field class="input-d3" type="number" label="Valor"></md-outlined-text-field>
        let valor = document.createElement("md-outlined-text-field");
        valor.label = "Valor";
        valor.type = "number";
        valor.dataset.id = id;
        valor.classList.add("input-d3");
        div.appendChild(valor);

        let slider = document.createElement("md-slider");
        slider.labeled = true;
        slider.dataset.id = id;
        div.appendChild(slider);


        valor.addEventListener("change", () => this.#atualizaSliderPeloInput(valor, slider));
        slider.addEventListener("change", () => this.#atualizaInputPeloSlider(valor, slider));

        return div;
    }

    #atualizaSliderPeloInput(valor, slider) {
        let valorFloat = parseFloat(valor.value);
        if (isNaN(valorFloat)) valorFloat = 0;
        if (valorFloat < this.min) valorFloat = this.min;
        if (valorFloat > this.max) valorFloat = this.max;

        valor.value = valorFloat;
        if (slider.value != valorFloat) slider.value = valorFloat;
    }

    #atualizaInputPeloSlider(valor, slider) {
        let valorFloat = parseFloat(slider.value);
        if (isNaN(valorFloat)) valorFloat = 0;
        if (valorFloat < this.min) valorFloat = this.min;
        if (valorFloat > this.max) valorFloat = this.max;
        
        slider.value = valorFloat;
        if (valor.value != slider.value) valor.value = slider.value;
    }

    removeGrafico(id) {
        let grafico = this.graficoHolder.querySelector(`div[data-id="${id}"]`);
        if (grafico) grafico.remove();
    }

    onMinMaxChange() {
        let min = 0;
        let max = 100;

        if (this.tipo_porcentagem != "porcentagem") {
            min = parseFloat(this.valorMinimo.value);
            max = parseFloat(this.valorMaximo.value);

            if (min > max) {
                this.valorMaximo.value = min;
            }
        }

        console.log("Min: " + min + " Max: " + max);

        
        let sliders = this.form.querySelectorAll("md-slider");
        sliders.forEach(slider => {
            let valor = parseFloat(slider.value);
            if (valor < min) slider.value = min;
            if (valor > max) slider.value = max;

            slider.min = min;
            slider.max = max;

            let id = slider.dataset.id;
            let input = this.form.querySelector(`md-outlined-text-field[data-id="${id}"]`);
            this.#atualizaInputPeloSlider(input, slider);
        });
    }

    updateMinMaxOfSlider(slider) {
        let min = 0;
        let max = 100;

        if (this.tipo_porcentagem != "porcentagem") {
            min = parseFloat(this.valorMinimo.value);
            max = parseFloat(this.valorMaximo.value);

            if (min > max) {
                this.valorMaximo.value = min;
            }
        }

        let valor = parseFloat(slider.value);
        if (valor < min) slider.value = min;
        if (valor > max) slider.value = max;

        slider.min = min;
        slider.max = max;
    }


    // Chamado ao escolher um tipo de pergunta
    // tipo: "booleano" ou "porcentagem" (grafico ainda não implementado)
    setTipoPergunta(tipo) {
        const classes = [
            {classe: "dadoBooleano", tipo: "booleano", formSelector: "opcaoBooleano"},
            {classe: "dadoPorcentagem", tipo: "porcentagem", formSelector: "opcaoPorcentagem"},
            {classe: "dadoGrafico", tipo: "grafico", formSelector: "opcaoGrafico"}
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

        this.onMinMaxChange();
    }

    // Destroi o form
    removerOpcao() {
        this.form.remove();
        this.pai.removePergunta(this);
    }

    mostrarInformativo() {
        if (this.tipo == "booleano") this.controlador.modal.abrir("dados_booleano");
        else if (this.tipo == "porcentagem") this.controlador.modal.abrir("dados_porcentagem");
        else if (this.tipo == "grafico") this.controlador.modal.abrir("dados_grafico");
        else console.error("Tipo de pergunta desconhecido: " + this.tipo);
    }

    pegarTipoCorreto() {
        if (this.tipo == "booleano") return "Boleano";
        if (this.tipo == "porcentagem") return "Porcentagem";
        if (this.tipo == "grafico") return "Grafico";
        return "Desconhecido";
    }

    // To JSON
    toJSON() {
        const objeto = {
            tipo: this.pegarTipoCorreto(),
            texto: this.perguntaField.value
        };

        if (this.tipo == "booleano") {
            objeto.respostaBool = this.trueRadio.checked;
        } else if (this.tipo == "porcentagem" || this.tipo == "grafico") {
            objeto.range = { };
            
            // objeto.range.range = parseFloat(this.margemDeErro.value);
            objeto.range.min = this.valorMinimo.value;
            objeto.range.max = this.valorMaximo.value;
            objeto.range.porcentagem = this.tipo_porcentagem == "porcentagem";

            if (objeto.range.porcentagem) {
                delete objeto.range.min;
                delete objeto.range.max;
                objeto.range.range = 0.05; // 5%
            } else {
                objeto.range.min = parseFloat(objeto.range.min);
                objeto.range.max = parseFloat(objeto.range.max);
                objeto.range.range = (objeto.range.max - objeto.range.min) * 0.05;
                delete objeto.range.porcentagem;
            }
        } 
        
        if (this.tipo == "porcentagem") {
            objeto.respostaFloat = this.slider.value;

            if (objeto.range.porcentagem) objeto.respostaFloat =  objeto.respostaFloat / 100.0;

        } else if (this.tipo == "grafico") {
            objeto.grafico = [];
            let campos = this.graficoHolder.querySelectorAll("div.grafico");
            campos.forEach(campo => {
                let obj = {
                    nome: campo.querySelector("md-outlined-text-field").value,
                    valor: parseFloat(campo.querySelector("md-outlined-text-field[type=number]").value)
                };

                if (obj.valor == NaN || obj.valor == null) obj.valor = parseFloat(campo.querySelector("md-slider").value);

                if (objeto.range.porcentagem) obj.valor = obj.valor / 100.0;

                objeto.grafico.push(obj);
            });
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

    controlador = null;

    constructor(controlador) {
        this.controlador = controlador;

        this.tituloInput = document.querySelector("#dadosTitulo");
        this.urlInput = document.querySelector("#dadosUrl");
        this.dadosHolder = document.querySelector("#dadosHolder");
        this.novaPerguntaBtn = document.querySelector("#novaPerguntaBtn");
        this.novaPerguntaBtn.addEventListener("click", evt=> {
            this.gerarNovaPergunta();
            evt.preventDefault();
        });

        this.limparPerguntas();
        this.gerarNovaPergunta();
    }

    removePergunta(pergunta) {
        let index = this.perguntas.indexOf(pergunta);
        if (index >= 0) {
            this.perguntas.splice(index, 1);
        }
    }

    limparPerguntas() {
        console.log("Limpando perguntas");
        this.dadosHolder.innerHTML = "";
        this.perguntas = [];
    }

    gerarNovaPergunta() {
        this.#id++;

        let opcao = new OpcaoDados(this, this.#id);
        this.dadosHolder.appendChild(opcao.form);

        this.perguntas.push(opcao);
    }

    gerarDocumento() {
        const objeto = {
            titulo: this.tituloInput.value,
            url: this.urlInput.value
        };

        objeto.dados = this.perguntas.map(pergunta => pergunta.toJSON());
        
        return new PseudoDocumento(objeto, 'JogoArtigoDados.json', {type: "application/json"});
    }
}