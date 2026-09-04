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

async function buscarCep(valor) {
    const cep = valor.replace(/\D/g, '');
    const spanErro = document.getElementById('cep-error');

    if (spanErro) spanErro.innerText = '';

    if (cep.length !== 8) return;

    try {
        const resposta = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
        const dados = await resposta.json();

        if (dados.erro) {
            if (spanErro) spanErro.innerText = 'CEP não encontrado.';
            return;
        }

        const ruaInput = document.getElementById('Rua');
        const bairroInput = document.getElementById('Bairro');
        const cidadeInput = document.getElementById('Cidade');
        const estadoSelect = document.getElementById('Estado');

        if (ruaInput) ruaInput.value = dados.logradouro || '';
        if (bairroInput) bairroInput.value = dados.bairro || '';
        if (cidadeInput) cidadeInput.value = dados.localidade || '';

        if (dados.uf && estadoSelect) {
            const ufViaCep = dados.uf.toUpperCase();
            for (let i = 0; i < estadoSelect.options.length; i++) {
                const opt = estadoSelect.options[i];
                if (opt.text.toUpperCase() === ufViaCep || opt.value.toUpperCase() === ufViaCep) {
                    estadoSelect.selectedIndex = i;
                    break;
                }
            }
        }

        const identificadorInput = document.getElementById('Identificador');
        if (identificadorInput) {
            identificadorInput.focus();
        }
    } catch {
        if (spanErro) spanErro.innerText = 'CEP não encontrado.';
    }
}