let dtPlanejamentos = null;
let todosPlanejamentos = [];
let listaPlanejamentos = [];

const mapaPosicionalDias = [
    { sigla: 'DOM', letra: 'D' },
    { sigla: 'SEG', letra: 'S' },
    { sigla: 'TER', letra: 'T' },
    { sigla: 'QUA', letra: 'Q' },
    { sigla: 'QUI', letra: 'Q' },
    { sigla: 'SEX', letra: 'S' },
    { sigla: 'SAB', letra: 'S' }
];

const FormPlanejamento = {
    init: function (planejamentosJson) {
        todosPlanejamentos = planejamentosJson || [];

        $('#inputMedicamento').select2({
            placeholder: "Medicamento",
            allowClear: true,
            width: '100%'
        }).on('change select2:select select2:clear', function () {
            FormPlanejamento.validarCamposAdicionar();
        });

        dtPlanejamentos = $('#tabelaPlanejamentos').DataTable({
            "language": {
                "url": "https://cdn.datatables.net/plug-ins/1.13.6/i18n/pt-BR.json"
            },
            "lengthMenu": [5, 10, 15],
            "pageLength": 5,
            "searching": false,
            "info": false,
            "dom": 'rt<"bottom d-flex justify-content-between align-items-center p-3 mt-2"lp><"clear">',
            "columnDefs": [
                { "width": "20%", "targets": 0, "className": "font-bold" },
                { "width": "15%", "targets": 1 },
                { "width": "15%", "targets": 2 },
                { "width": "10%", "targets": 3 },
                { "width": "25%", "targets": 4 },
                { "width": "15%", "targets": 5 }
            ]
        });

        $('#inputDataInicio, #inputDataFim, #inputHora, #inputIntervalo, #inputDosagem, #inputUnidade').on('input change', function () {
            FormPlanejamento.validarCamposAdicionar();
        });

        $(document).off('change', '.chk-dia').on('change', '.chk-dia', function () {
            FormPlanejamento.validarCamposAdicionar();
        });

        $('#switchContinuo').on('change', function () {
            FormPlanejamento.toggleContinuo(this.checked);
        });

        $('#chkFiltrarAdicionados').on('change', function () {
            FormPlanejamento.filtrarNovosAdicionados(this.checked);
        });

        const btnAdd = document.getElementById('btnAdicionarPreview');
        if (btnAdd) {
            btnAdd.addEventListener('click', FormPlanejamento.executarAdicionarPreview);
        }

        $('#formPlanejamento').on('submit', function (e) {
            if (listaPlanejamentos.length === 0) {
                e.preventDefault();
                return false;
            }

            const container = document.getElementById('hiddenPlanejamentosContainer');

            if (container) {
                container.innerHTML = '';
                listaPlanejamentos.forEach((item, index) => {
                    const horaFmt = item.hora && item.hora.length === 5 ? `${item.hora}:00` : item.hora;
                    const intervaloFmt = item.intervalo && item.intervalo.length === 5 ? `${item.intervalo}:00` : (item.intervalo || '24:00:00');

                    container.innerHTML += `
                        <input type="hidden" name="Itens[${index}].IdPaciente" value="${item.idPaciente}" />
                        <input type="hidden" name="Itens[${index}].IdMedicamento" value="${item.idMedicamento}" />
                        <input type="hidden" name="Itens[${index}].DataInicio" value="${item.dataInicio}" />
                        <input type="hidden" name="Itens[${index}].DataFim" value="${item.dataFim}" />
                        <input type="hidden" name="Itens[${index}].Continuo" value="${item.continuo}" />
                        <input type="hidden" name="Itens[${index}].Hora" value="${horaFmt}" />
                        <input type="hidden" name="Itens[${index}].IntervaloExecucao" value="${intervaloFmt}" />
                        <input type="hidden" name="Itens[${index}].Dosagem" value="${item.dosagem}" />
                        <input type="hidden" name="Itens[${index}].Unidade" value="${item.unidade}" />
                        <input type="hidden" name="Itens[${index}].DiaSemana" value="${item.diaSemana}" />
                    `;
                });
            }
        });

        const idInicial = $('#IdPaciente').val();
        if (idInicial && idInicial !== "0") {
            const box = document.getElementById(`box-paciente-${idInicial}`);
            const nome = box ? box.querySelector('.paciente-nome').innerText : '';
            FormPlanejamento.carregarPlanejamentosMemoria(idInicial);
            if (nome) {
                $('#tituloSecaoTabela').text(`Planejamentos atuais de ${nome}`);
            }
        }

        FormPlanejamento.validarCamposAdicionar();
    },

    alternarPaciente: function (id, nome) {
        if (listaPlanejamentos.length > 0) return;

        const inputId = document.getElementById('IdPaciente');
        const box = document.getElementById(`box-paciente-${id}`);
        const btn = document.getElementById(`btn-paciente-${id}`);

        if (!inputId || !box || !btn) return;

        if (inputId.value === id.toString()) {
            inputId.value = '0';
            box.classList.remove('active');
            btn.classList.remove('selected');
            btn.innerText = 'Selecionar';
            document.getElementById('tituloSecaoTabela').innerText = 'Planejamentos atuais';
            if (dtPlanejamentos) {
                dtPlanejamentos.clear().draw();
            }
            FormPlanejamento.validarCamposAdicionar();
            return;
        }

        document.querySelectorAll('.paciente-card').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.btn-card-toggle').forEach(b => {
            b.classList.remove('selected');
            b.innerText = 'Selecionar';
        });

        inputId.value = id;
        box.classList.add('active');
        btn.classList.add('selected');
        btn.innerText = 'Desmarcar';
        document.getElementById('tituloSecaoTabela').innerText = `Planejamentos atuais de ${nome}`;

        FormPlanejamento.carregarPlanejamentosMemoria(id);
        FormPlanejamento.validarCamposAdicionar();
    },

    carregarPlanejamentosMemoria: function (idPaciente) {
        if (!dtPlanejamentos) return;
        dtPlanejamentos.clear();

        const filtrados = todosPlanejamentos.filter(p => p.IdPaciente == idPaciente);
        filtrados.forEach(item => {
            dtPlanejamentos.row.add([
                item.MedicamentoNome,
                item.DataInicioFormatada,
                item.DataFimFormatada,
                item.Hora,
                FormPlanejamento.formatarDias(item.DiaSemana),
                item.Dosagem
            ]);
        });

        dtPlanejamentos.columns.adjust().draw();
    },

    obterMascaraDias: function () {
        const mascara = mapaPosicionalDias.map(dia => {
            const checkbox = document.querySelector(`.chk-dia[value='${dia.sigla}']`);
            return (checkbox && checkbox.checked) ? dia.letra : 'X';
        }).join('');

        const temAoMenosUmDia = mascara.split('').some(c => c !== 'X');
        return temAoMenosUmDia ? mascara : '';
    },

    validarCamposAdicionar: function () {
        const paciente = $('#IdPaciente').val();
        const temPaciente = Boolean(paciente && paciente !== "0");
        const med = $('#inputMedicamento').val();
        const temMed = Boolean(med && med.trim() !== "");
        const dataIni = $('#inputDataInicio').val();
        const temDataIni = Boolean(dataIni && dataIni.trim() !== "");
        const isContinuo = $('#switchContinuo').is(':checked');
        const dataFim = $('#inputDataFim').val();
        const dataFimValida = isContinuo ? true : Boolean(dataFim && dataFim.trim() !== '');
        const hora = $('#inputHora').val();
        const temHora = Boolean(hora && hora.trim() !== "");
        const intervalo = $('#inputIntervalo').val();
        const temIntervalo = Boolean(intervalo && intervalo.trim() !== "");
        const dosagem = $('#inputDosagem').val();
        const temDosagem = Boolean(dosagem && parseInt(dosagem, 10) >= 1);
        const unidade = $('#inputUnidade').val();
        const temUnidade = Boolean(unidade && unidade.trim() !== "");
        const temDias = $('.chk-dia:checked').length > 0;

        const todosPreenchidos = Boolean(
            temPaciente &&
            temMed &&
            temDataIni &&
            dataFimValida &&
            temHora &&
            temIntervalo &&
            temDosagem &&
            temUnidade &&
            temDias
        );

        const btn = document.getElementById('btnAdicionarPreview');
        if (btn) {
            btn.disabled = !todosPreenchidos;
        }
    },

    toggleContinuo: function (isContinuo) {
        const dataFimInput = document.getElementById('inputDataFim');
        const txtContinuo = document.getElementById('txtContinuo');

        if (isContinuo) {
            txtContinuo.innerText = "Sim";
            dataFimInput.value = '9999-12-31';
            dataFimInput.disabled = true;
            dataFimInput.style.backgroundColor = '#e9ecef';
        } else {
            txtContinuo.innerText = "Não";
            if (dataFimInput.value === '9999-12-31') {
                dataFimInput.value = '';
            }
            dataFimInput.disabled = false;
            dataFimInput.style.backgroundColor = '';
        }

        FormPlanejamento.validarCamposAdicionar();
    },

    formatarDias: function (dias) {
        if (!dias) return '-';
        if (dias.length === 7) {
            const diasAtivos = [];
            for (let i = 0; i < 7; i++) {
                if (dias[i] !== 'X') diasAtivos.push(dias[i]);
            }
            return diasAtivos.length > 0 ? diasAtivos.join(', ') : '-';
        }
        return dias;
    },

    executarAdicionarPreview: function () {
        const idPaciente = $('#IdPaciente').val();
        if (!idPaciente || idPaciente === "0") return;

        const medSelect = $('#inputMedicamento');
        const medId = medSelect.val();
        const medNome = medSelect.find('option:selected').text();
        const dataInicio = $('#inputDataInicio').val();
        const isContinuo = $('#switchContinuo').is(':checked');
        const dataFim = isContinuo ? '9999-12-31' : $('#inputDataFim').val();
        const hora = $('#inputHora').val();
        const intervalo = $('#inputIntervalo').val() || '08:00';
        const dosagem = $('#inputDosagem').val();
        const unidade = $('#inputUnidade').val();
        const unidadeTexto = $('#inputUnidade option:selected').text();
        const dias = FormPlanejamento.obterMascaraDias();

        listaPlanejamentos.push({
            idPaciente: idPaciente,
            idMedicamento: medId,
            dataInicio: dataInicio,
            dataFim: dataFim,
            continuo: isContinuo,
            hora: hora,
            intervalo: intervalo,
            dosagem: dosagem,
            unidade: unidade,
            diaSemana: dias
        });

        const dataInicioFmt = dataInicio.split('-').reverse().join('/');
        const dataFimFmt = isContinuo ? 'Contínuo' : dataFim.split('-').reverse().join('/');

        const novaLinhaNode = dtPlanejamentos.row.add([
            medNome,
            dataInicioFmt,
            dataFimFmt,
            hora,
            FormPlanejamento.formatarDias(dias),
            `${dosagem} ${unidadeTexto}`
        ]).draw().node();

        $(novaLinhaNode).addClass('linha-preview');

        const btnSalvar = document.getElementById('btnSalvarForm');
        if (btnSalvar) btnSalvar.disabled = false;

        document.querySelectorAll('.btn-card-toggle').forEach(btn => {
            btn.disabled = true;
            btn.style.cursor = 'not-allowed';
        });

        document.querySelectorAll('.paciente-card').forEach(card => {
            if (!card.classList.contains('active')) {
                card.classList.add('pacientes-bloqueados');
            }
        });

        medSelect.val('').trigger('change');
        $('#inputDosagem').val('');
        $('#inputHora').val('');
        $('#inputIntervalo').val('08:00');
        $('#inputUnidade').val('');
        $('.chk-dia').prop('checked', false);

        if (isContinuo) {
            $('#switchContinuo').prop('checked', false);
            FormPlanejamento.toggleContinuo(false);
        } else {
            $('#inputDataFim').val('');
        }

        FormPlanejamento.validarCamposAdicionar();
    },

    filtrarNovosAdicionados: function (filtrar) {
        if (filtrar) {
            $('#tabelaPlanejamentos tbody tr').each(function () {
                if (!$(this).hasClass('linha-preview')) {
                    $(this).hide();
                }
            });
        } else {
            $('#tabelaPlanejamentos tbody tr').show();
        }
    }
};