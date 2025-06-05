
    // tela de envio de foto / checkbox
   document.getElementById("fotoForm").addEventListener("submit", function (event) {
        const fotoInput = document.getElementById("foto");
    const mensagemErro = document.getElementById("mensagemErro");
    const produtoId = document.getElementById("Id_Produto").value;

    if (!fotoInput.files.length) {
        event.preventDefault();
    mensagemErro.style.display = "block";
    mensagemErro.innerText = "Selecione uma imagem antes de enviar.";
    return;
        }

    if (produtoId === "") {
        event.preventDefault();
    mensagemErro.style.display = "block";
    mensagemErro.innerText = "Selecione um produto.";
        }
    });
    // tela de envio de foto / checkbox