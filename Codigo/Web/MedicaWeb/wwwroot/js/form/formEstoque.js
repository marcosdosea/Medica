let dtEstoques = null;
let todosEstoques = [];
let selectedPaciente = null;
let selectedMedicamento = null;

const FormEstoque = {
    init: function (estoquesJson, initialPaciente, initialMedicamento) {
        todosEstoques = estoquesJson || [];

        if (initialPaciente && initialPaciente !== "0" && initialPaciente !== 0) {
            selectedPaciente = String(initialPaciente);
        }

        if (initialMedicamento && initialMedicamento !== "0" && initialMedicamento !== 0) {
            selectedMedicamento = String(initialMedicamento);
        }
        dtEstoques = $('#tabelaEstoques').DataTable({
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/pt-BR.json"
            },
            pageLength: 5,
            lengthMenu: [5, 10, 15],
            searching: true,
            info: false,
            dom: 'rt<"bottom d-flex justify-content-between align-items-center p-3 mt-2"lp><"clear">',
            columnDefs: [
                { width: "20%", targets: 0, className: "font-bold" },
                { width: "22%", targets: 1 },
                { width: "16%", targets: 2 },
                { width: "10%", targets: 3, className: "text-center" },
                { width: "10%", targets: 4, className: "text-center" },
                { width: "10%", targets: 5, className: "text-center" },
                { width: "6%", targets: 6, className: "text-center" },
                { width: "6%", targets: 7, orderable: false, className: "text-center" }
            ]
        });
        $('#inputQuantidade, #inputQuantidadeMinima, #inputDataValidade').on('input change', function () {
            FormEstoque.validarFormulario();
        });
        FormEstoque.atualizarTabela();
        FormEstoque.validarFormulario();
    },

    alternarPaciente: function (id, nome) {
        const idStr = String(id);
        const box = document.getElementById(`box-paciente-${idStr}`);
        const btn = document.getElementById(`btn-paciente-${idStr}`);

        if (selectedPaciente === idStr) {
            selectedPaciente = null;
            if (box) box.classList.remove('active');
            if (btn) {
                btn.classList.remove('selected');
                btn.innerText = 'Selecionar';
            }
            $('#IdPaciente').val(0);
        } else {
            if (selectedPaciente) {
                const oldBox = document.getElementById(`box-paciente-${selectedPaciente}`);
                const oldBtn = document.getElementById(`btn-paciente-${selectedPaciente}`);
                if (oldBox) oldBox.classList.remove('active');
                if (oldBtn) {
                    oldBtn.classList.remove('selected');
                    oldBtn.innerText = 'Selecionar';
                }
            }

            selectedPaciente = idStr;
            if (box) box.classList.add('active');
            if (btn) {
                btn.classList.add('selected');
                btn.innerText = 'Desmarcar';
            }
            $('#IdPaciente').val(idStr);
        }

        FormEstoque.validarFormulario();
    },

    alternarMedicamento: function (id, nome) {
        const idStr = String(id);
        const box = document.getElementById(`box-medicamento-${idStr}`);
        const btn = document.getElementById(`btn-medicamento-${idStr}`);

        if (selectedMedicamento === idStr) {
            selectedMedicamento = null;
            if (box) box.classList.remove('active');
            if (btn) {
                btn.classList.remove('selected');
                btn.innerText = 'Selecionar';
            }
            $('#IdMedicamento').val(0);
        } else {
            if (selectedMedicamento) {
                const oldBox = document.getElementById(`box-medicamento-${selectedMedicamento}`);
                const oldBtn = document.getElementById(`btn-medicamento-${selectedMedicamento}`);
                if (oldBox) oldBox.classList.remove('active');
                if (oldBtn) {
                    oldBtn.classList.remove('selected');
                    oldBtn.innerText = 'Selecionar';
                }
            }

            selectedMedicamento = idStr;
            if (box) box.classList.add('active');
            if (btn) {
                btn.classList.add('selected');
                btn.innerText = 'Desmarcar';
            }
            $('#IdMedicamento').val(idStr);
        }

        FormEstoque.validarFormulario();
    },

    atualizarTabela: function () {
        if (!dtEstoques) return;
        dtEstoques.clear();

        todosEstoques.forEach(item => {
            const id = item.id ?? item.Id;
            const nomeMed = item.nomeMedicamento ?? item.NomeMedicamento ?? '';
            const formaFarm = item.formaFarmaceutica ?? item.FormaFarmaceutica ?? '';
            const nomePaciente = item.nomePaciente ?? item.NomePaciente ?? item.pacientesNomes ?? item.PacientesNomes ?? '-';

            const qtd = item.quantidade ?? item.Quantidade ?? 0;
            const qtdMin = item.quantidadeMinima ?? item.QuantidadeMinima ?? 0;
            const validade = item.dataValidadeFormatada ?? item.DataValidadeFormatada ?? '';
            const status = (item.status ?? item.Status ?? 'REGULAR').toUpperCase();

            let statusBadge = '<span class="badge bg-success">Regular</span>';
            if (status === 'BAIXO') {
                statusBadge = '<span class="badge bg-warning text-dark">Baixo</span>';
            } else if (status === 'INSUFICIENTE') {
                statusBadge = '<span class="badge bg-danger">Insuficiente</span>';
            }

            const acoesHtml = `
                <div class="acoes-tabela-container">
                    <button type="button" class="btn-acao-tabela btn-acao-editar" title="Editar estoque" onclick="FormEstoque.editarEstoque(${id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button type="button" class="btn-acao-tabela btn-acao-excluir" title="Excluir estoque" onclick="FormEstoque.excluirEstoque(${id}, '${nomeMed.replace(/'/g, "\\'")}')">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>`;

            dtEstoques.row.add([
                nomeMed,
                nomePaciente,
                formaFarm || '-',
                qtd,
                qtdMin,
                validade,
                statusBadge,
                acoesHtml
            ]);
        });

        dtEstoques.columns.adjust().draw();
    },

    validarFormulario: function () {
        const temPaciente = Boolean(selectedPaciente && selectedPaciente !== "0");
        const temMed = Boolean(selectedMedicamento && selectedMedicamento !== "0");
        const qtdStr = $('#inputQuantidade').val();
        const qtdVal = parseInt(qtdStr, 10);
        const temQtd = Boolean(qtdStr && qtdStr.trim() !== '' && !isNaN(qtdVal) && qtdVal >= 1);
        const qtdMinStr = $('#inputQuantidadeMinima').val();
        const qtdMinVal = parseInt(qtdMinStr, 10);
        const temQtdMin = Boolean(qtdMinStr && qtdMinStr.trim() !== '' && !isNaN(qtdMinVal) && qtdMinVal >= 0);
        const validade = $('#inputDataValidade').val();
        const temValidade = Boolean(validade && validade.trim() !== '');
        const tudoValido = temPaciente && temMed && temQtd && temQtdMin && temValidade;
        const btnSalvar = document.getElementById('btnSalvarEstoque');
        if (btnSalvar) {
            btnSalvar.disabled = !tudoValido;
        }
    },

    editarEstoque: function (id) {
        const item = todosEstoques.find(e => (e.id ?? e.Id) == id);
        if (!item) return;

        $('#idEstoqueEdit').val(id);
        $('#formEstoque').attr('action', `/Estoque/Edit/${id}`);
        const titulo = document.getElementById('tituloSecaoForm');
        if (titulo) titulo.innerText = 'Editar estoque';

        const btnCancelar = document.getElementById('btnCancelarEdicao');
        if (btnCancelar) btnCancelar.style.display = 'inline-block';

        selectedPaciente = null;
        document.querySelectorAll('[id^="box-paciente-"]').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('[id^="btn-paciente-"]').forEach(b => {
            b.classList.remove('selected');
            b.innerText = 'Selecionar';
        });

        const pId = item.idPaciente ?? item.IdPaciente ?? (item.idsPacientes && item.idsPacientes.length > 0 ? item.idsPacientes[0] : null);
        if (pId && pId !== 0) {
            const pIdStr = String(pId);
            selectedPaciente = pIdStr;
            $('#IdPaciente').val(pIdStr);
            const box = document.getElementById(`box-paciente-${pIdStr}`);
            const btn = document.getElementById(`btn-paciente-${pIdStr}`);
            if (box) box.classList.add('active');
            if (btn) {
                btn.classList.add('selected');
                btn.innerText = 'Desmarcar';
            }
        }

        const medId = item.idMedicamento ?? item.IdMedicamento;
        if (medId) {
            const medIdStr = String(medId);
            selectedMedicamento = medIdStr;
            $('#IdMedicamento').val(medIdStr);
            const boxMed = document.getElementById(`box-medicamento-${medIdStr}`);
            const btnMed = document.getElementById(`btn-medicamento-${medIdStr}`);
            if (boxMed) boxMed.classList.add('active');
            if (btnMed) {
                btnMed.classList.add('selected');
                btnMed.innerText = 'Desmarcar';
            }
        }
        $('#inputQuantidade').val(item.quantidade ?? item.Quantidade ?? '');
        $('#inputQuantidadeMinima').val(item.quantidadeMinima ?? item.QuantidadeMinima ?? '');
        const dataValIso = item.dataValidadeIso ?? item.DataValidadeIso ?? '';
        if (dataValIso) {
            $('#inputDataValidade').val(dataValIso);
        } else if (item.dataValidade || item.DataValidade) {
            const dtRaw = item.dataValidade || item.DataValidade;
            $('#inputDataValidade').val(String(dtRaw).split('T')[0]);
        }
        const el = document.getElementById('tituloSecaoForm');
        if (el) el.scrollIntoView({ behavior: 'smooth' });

        FormEstoque.validarFormulario();
    },

    cancelarEdicao: function () {
        $('#idEstoqueEdit').val(0);
        $('#formEstoque').attr('action', '/Estoque/Create');
        const titulo = document.getElementById('tituloSecaoForm');
        if (titulo) titulo.innerText = 'Novo estoque';
        const btnCancelar = document.getElementById('btnCancelarEdicao');
        if (btnCancelar) btnCancelar.style.display = 'none';
        selectedPaciente = null;
        selectedMedicamento = null;
        document.querySelectorAll('.paciente-card').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.btn-card-toggle').forEach(b => {
            b.classList.remove('selected');
            b.innerText = 'Selecionar';
        });
        $('#IdPaciente').val(0);
        $('#IdMedicamento').val(0);
        $('#inputQuantidade').val('');
        $('#inputQuantidadeMinima').val('');
        $('#inputDataValidade').val('');

        FormEstoque.validarFormulario();
    },

    excluirEstoque: function (id, nomeMedicamento) {
        const formExcluir = document.getElementById('formExcluirEstoque');
        if (!formExcluir) return;
        formExcluir.action = `/Estoque/Delete/${id}`;
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Confirmar Exclusão',
                text: `Deseja realmente excluir o estoque de ${nomeMedicamento}?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Sim, excluir',
                cancelButtonText: 'Cancelar',
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    formExcluir.submit();
                }
            });
        } else {
            if (confirm(`Deseja realmente excluir o estoque de ${nomeMedicamento}?`)) {
                formExcluir.submit();
            }
        }
    }
};
