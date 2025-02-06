const mainHolder = document.querySelector("#holder");
const classeEscolheJogo = "escolha";
const escolheArtigo = document.querySelector("#escolheArtigo");
const classeArtigo = "valores";
const escolheDados = document.querySelector("#escolheDados");
const classeDados = "dados";


escolheArtigo.addEventListener("click", () => {
    mainHolder.classList.remove(classeEscolheJogo);
    mainHolder.classList.add(classeArtigo);
});

escolheDados.addEventListener("click", () => {
    mainHolder.classList.remove(classeEscolheJogo);
    mainHolder.classList.add(classeDados);
});