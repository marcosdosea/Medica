function previewImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            const img = document.getElementById('img-preview');
            const icon = document.getElementById('placeholder-icon');
            const btnRemover = document.getElementById('btn-remover-foto');
            const inputRemover = document.getElementById('inputRemoverFoto');

            if (img) {
                img.src = e.target.result;
                img.style.display = 'block';
            }
            if (icon) icon.style.display = 'none';
            const fileName = document.getElementById('file-name');
            if (fileName) fileName.innerText = input.files[0].name;

            if (btnRemover) btnRemover.style.display = 'flex';
            if (inputRemover) inputRemover.value = 'false';
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function removerFoto() {
    const inputFoto = document.getElementById('FotoInput') || document.getElementById('f-upload');
    const img = document.getElementById('img-preview');
    const icon = document.getElementById('placeholder-icon');
    const fileName = document.getElementById('file-name');
    const btnRemover = document.getElementById('btn-remover-foto');
    const inputRemover = document.getElementById('inputRemoverFoto');

    if (inputFoto) inputFoto.value = '';
    if (img) {
        img.src = '';
        img.style.display = 'none';
    }
    if (icon) icon.style.display = 'block';
    if (fileName) fileName.innerText = 'Upload.png';
    if (btnRemover) btnRemover.style.display = 'none';
    if (inputRemover) inputRemover.value = 'true';
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

document.addEventListener('DOMContentLoaded', function () {
    const botoesCancelar = document.querySelectorAll('.btn-cancelar');
    botoesCancelar.forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const urlDestino = this.getAttribute('href');
            
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Tem certeza?',
                    text: "Os dados não salvos serão perdidos!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Sim, cancelar',
                    cancelButtonText: 'Voltar',
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed && urlDestino) {
                        window.location.href = urlDestino;
                    }
                });
            } else {
                if (confirm("Tem certeza? Os dados não salvos serão perdidos!")) {
                    window.location.href = urlDestino;
                }
            }
        });
    });

    const botoesSalvar = document.querySelectorAll('.btn-salvar');
    botoesSalvar.forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            const form = this.closest('form');
            if (!form) return;

            if (typeof $(form).valid === 'function' && !$(form).valid()) {
                return;
            }

            e.preventDefault();
            
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Deseja salvar?',
                    text: "Confirme se os dados inseridos estão corretos.",
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: '#28a745',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Sim, salvar',
                    cancelButtonText: 'Cancelar',
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        if (form.requestSubmit) {
                            form.requestSubmit();
                        } else {
                            form.submit();
                        }
                    }
                });
            } else {
                if (confirm("Deseja salvar os dados?")) {
                    if (form.requestSubmit) {
                        form.requestSubmit();
                    } else {
                        form.submit();
                    }
                }
            }
        });
    });

    const botoesExcluir = document.querySelectorAll('.btn-excluir');
    botoesExcluir.forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const form = this.closest('form');
            if (!form) return;

            const nomeItem = this.getAttribute('data-nome') || 'este registro';
            const actionUrl = this.getAttribute('formaction');
            
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Confirmar Exclusão',
                    text: `Deseja realmente excluir ${nomeItem}?`,
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Sim, excluir',
                    cancelButtonText: 'Cancelar',
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        if (actionUrl) form.setAttribute('action', actionUrl);
                        form.submit();
                    }
                });
            } else {
                if (confirm(`Deseja realmente excluir ${nomeItem}?`)) {
                    if (actionUrl) form.setAttribute('action', actionUrl);
                    form.submit();
                }
            }
        });
    });
});