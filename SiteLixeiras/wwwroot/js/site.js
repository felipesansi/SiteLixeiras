// Tela de login e registro
    document.addEventListener("DOMContentLoaded", function () {
        const loginContainer = document.getElementById("loginContainer");
    const registerContainer = document.getElementById("registerContainer");

    const showRegisterBtn = document.getElementById("showRegister");
    const showLoginBtn = document.getElementById("showLogin");

    showRegisterBtn.addEventListener("click", function () {
        loginContainer.classList.add("slide-out-left");
    registerContainer.classList.remove("d-none");
    registerContainer.classList.add("slide-in-right");

            // Esconde login depois da animação
            setTimeout(() => {
        loginContainer.classList.add("d-none");
    loginContainer.classList.remove("slide-out-left");
    registerContainer.classList.remove("slide-in-right");
            }, 600);
        });

    showLoginBtn.addEventListener("click", function () {
        registerContainer.classList.add("slide-out-right");
    loginContainer.classList.remove("d-none");
    loginContainer.classList.add("slide-in-left");

            setTimeout(() => {
        registerContainer.classList.add("d-none");
    registerContainer.classList.remove("slide-out-right");
    loginContainer.classList.remove("slide-in-left");
            }, 600);
        });
    });
// Tela de login e registro

    // trla de envio de foto / checkbox
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
    // trla de envio de foto / checkbox