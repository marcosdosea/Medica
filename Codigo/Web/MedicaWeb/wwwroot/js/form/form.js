function previewImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            const img = document.getElementById('img-preview');
            const icon = document.getElementById('placeholder-icon');
            img.src = e.target.result;
            img.style.display = 'block';
            icon.style.display = 'none';
            document.getElementById('file-name').innerText = input.files[0].name;
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function toggleText(checkbox) {
    const label = document.querySelector(`label[for="${checkbox.id}"]`);
    if (label) {
        if (checkbox.checked) {
            label.innerText = "Desmarcar";
        } else {
            label.innerText = "Selecionar";
        }
    }
}

function buscarCep(cep) {
    const cepLimpo = cep.replace(/\D/g, '');
    const spanErro = document.getElementById('cep-error');

    if (spanErro) spanErro.innerText = '';

    if (cepLimpo.length !== 8) return;

    fetch(`https://viacep.com.br/ws/${cepLimpo}/json/`)
        .then(response => {
            if (!response.ok) throw new Error('Falha na requisição');
            return response.json();
        })
        .then(dados => {
            if (dados.erro) {
                if (spanErro) spanErro.innerText = 'CEP não encontrado.';
                return;
            }

            if (spanErro) spanErro.innerText = '';

            $('#Rua').val(dados.logradouro).valid();
            $('#Bairro').val(dados.bairro).valid();
            $('#Cidade').val(dados.localidade).valid();
            $('#Estado').val(dados.uf).trigger('change');
            $('#Estado').valid();
            $('#Identificador').focus();
        })
        .catch(err => {
            console.error(err);
            if (spanErro) spanErro.innerText = 'Erro ao consultar o CEP.';
        });
}

class DialogConfirmacao {

    static exibir({
        url,
        titulo = "Confirmar Exclusão",
        mensagem = "Deseja realmente excluir este registro?",
        textoBotao = "Excluir",
        classeBotao = "btn-danger"
    }) {
        console.log('aaaaaaaaaaaaaaaaaaaaaaaa');
        const modalElement = document.getElementById('modalConfirmacao');
        if (!modalElement) return;

        document.getElementById('modalConfirmacaoLabel').innerHTML = `<i class="bi bi-exclamation-triangle-fill text-danger me-2"></i> ${titulo}`;
        document.getElementById('modalConfirmacaoMensagem').innerText = mensagem;

        const form = document.getElementById('modalConfirmacaoForm');
        form.action = url;

        const btnAcao = document.getElementById('modalConfirmacaoBtnAcao');
        btnAcao.innerText = textoBotao;
        btnAcao.className = `btn ${classeBotao}`;

        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();
    }
}