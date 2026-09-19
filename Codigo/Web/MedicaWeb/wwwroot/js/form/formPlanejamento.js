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
            "searching": true,
            "info": false,
            "dom": 'rt<"bottom d-flex justify-content-between align-items-center p-3 mt-2"lp><"clear">',
            "drawCallback": function () {
                FormPlanejamento.atualizarEstadoBotoesAcaoTabela();
            },
            "createdRow": function (row, data, dataIndex) {
                if (data[7] && typeof data[7] === 'string' && data[7].indexOf('Remover da lista') !== -1) {
                    $(row).addClass('linha-preview');
                }
            },
            "columnDefs": [
                { "width": "18%", "targets": 0, "className": "font-bold" },
                { "width": "12%", "targets": 1 },
                { "width": "12%", "targets": 2 },
                { "width": "8%", "targets": 3 },
                { "width": "8%", "targets": 4 },
                { "width": "20%", "targets": 5 },
                { "width": "12%", "targets": 6 },
                { "width": "10%", "targets": 7, "orderable": false, "className": "text-center" }
            ]
        });

        if (!FormPlanejamento._filtroRegistrado) {
            $.fn.dataTable.ext.search.push(function (settings, searchData, dataIndex, rowData) {
                if (settings.sTableId !== 'tabelaPlanejamentos' && (!settings.nTable || settings.nTable.id !== 'tabelaPlanejamentos')) {
                    return true;
                }
                const filtrarNovos = $('#chkFiltrarAdicionados').is(':checked');
                if (!filtrarNovos) {
                    return true;
                }
                const htmlAcoes = (rowData && rowData[7]) ? rowData[7] : (searchData && searchData[7] ? searchData[7] : '');
                return typeof htmlAcoes === 'string' && htmlAcoes.indexOf('Remover da lista') !== -1;
            });
            FormPlanejamento._filtroRegistrado = true;
        }

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

        const btnCancelarEdicao = document.getElementById('btnCancelarEdicao');
        if (btnCancelarEdicao) {
            btnCancelarEdicao.addEventListener('click', FormPlanejamento.cancelarEdicao);
        }

        const btnCancelarFooter = document.getElementById('btnCancelarFooter');
        if (btnCancelarFooter) {
            btnCancelarFooter.style.display = 'none';
            btnCancelarFooter.addEventListener('click', function (e) {
                e.preventDefault();
                FormPlanejamento.cancelarNovosAdicionados();
            });
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
        if (listaPlanejamentos.length > 0) {
            FormPlanejamento.exibirNotificacaoAviso('Você tem planejamentos adicionados não salvos. Primeiro finalize essa ação.');
            return;
        }
        if (FormPlanejamento.estaEditando()) {
            FormPlanejamento.exibirNotificacaoAviso('Você está editando um planejamento. Primeiro finalize ou cancele essa edição.');
            return;
        }

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

        const filtrados = todosPlanejamentos.filter(p => (p.IdPaciente ?? p.idPaciente) == idPaciente);
        filtrados.forEach(item => {
            const id = item.Id ?? item.id;
            const medNome = item.MedicamentoNome ?? item.medicamentoNome ?? '';
            const dtIni = item.DataInicioFormatada ?? item.dataInicioFormatada ?? '';
            const dtFim = item.DataFimFormatada ?? item.dataFimFormatada ?? '';
            const hora = item.Hora ?? item.hora ?? '';
            const intervalo = item.IntervaloFormatado ?? item.intervaloFormatado ?? '-';
            const dias = item.DiaSemana ?? item.diaSemana ?? '';
            const dosagem = item.Dosagem ?? item.dosagem ?? '';

            const acoesHtml = `
                <div class="acoes-tabela-container">
                    <button type="button" class="btn-acao-tabela btn-acao-editar" title="Editar planejamento" onclick="FormPlanejamento.editarPlanejamento(${id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button type="button" class="btn-acao-tabela btn-acao-excluir" title="Excluir planejamento" onclick="FormPlanejamento.excluirPlanejamento(${id}, '${medNome}')">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>`;

            dtPlanejamentos.row.add([
                medNome,
                dtIni,
                dtFim,
                hora,
                intervalo,
                FormPlanejamento.formatarDias(dias),
                dosagem,
                acoesHtml
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

        if (FormPlanejamento.estaEditando()) {
            const idEdit = $('#idPlanejamentoEdit').val();
            const formEditar = document.getElementById('formEditarPlanejamento');
            if (!formEditar) return;

            formEditar.action = `/Planejamento/Edit/${idEdit}`;
            $('#editFormId').val(idEdit);
            $('#editFormIdPaciente').val(idPaciente);
            $('#editFormIdMedicamento').val(medId);
            $('#editFormDataInicio').val(dataInicio);
            $('#editFormDataFim').val(dataFim);
            $('#editFormContinuo').val(isContinuo ? "true" : "false");
            $('#editFormHora').val(hora && hora.length === 5 ? `${hora}:00` : hora);
            $('#editFormIntervalo').val(intervalo && intervalo.length === 5 ? `${intervalo}:00` : intervalo);
            $('#editFormDosagem').val(dosagem);
            $('#editFormUnidade').val(unidade);
            $('#editFormDiaSemana').val(dias);

            FormPlanejamento.cancelarEdicao();

            formEditar.submit();
            return;
        }

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

        const indexItem = listaPlanejamentos.length - 1;
        const acoesPreviewHtml = `
            <div class="acoes-tabela-container">
                <button type="button" class="btn-acao-tabela btn-acao-excluir" title="Remover da lista" onclick="FormPlanejamento.removerItemLista(${indexItem}, this)">
                    <i class="bi bi-trash"></i>
                </button>
            </div>`;

        const novaLinhaNode = dtPlanejamentos.row.add([
            medNome,
            dataInicioFmt,
            dataFimFmt,
            hora,
            intervalo || '08:00',
            FormPlanejamento.formatarDias(dias),
            `${dosagem} ${unidadeTexto}`,
            acoesPreviewHtml
        ]).draw().node();

        $(novaLinhaNode).addClass('linha-preview');
        FormPlanejamento.atualizarEstadoBotoesAcaoTabela();

        const btnSalvar = document.getElementById('btnSalvarForm');
        if (btnSalvar) btnSalvar.disabled = false;

        const btnCancelarFooter = document.getElementById('btnCancelarFooter');
        if (btnCancelarFooter) btnCancelarFooter.style.display = 'inline-block';

        document.querySelectorAll('.btn-card-toggle').forEach(btn => {
            btn.disabled = true;
            btn.style.cursor = 'not-allowed';
        });

        document.querySelectorAll('.paciente-card').forEach(card => {
            if (!card.classList.contains('active')) {
                card.classList.add('pacientes-bloqueados');
            }
        });

        FormPlanejamento.limparFormulario();
        FormPlanejamento.validarCamposAdicionar();
    },

    estaEditando: function () {
        const idEdit = $('#idPlanejamentoEdit').val();
        return Boolean(idEdit && idEdit !== "0");
    },

    editarPlanejamento: function (id) {
        if (listaPlanejamentos.length > 0) {
            FormPlanejamento.exibirNotificacaoAviso('Você tem planejamentos adicionados não salvos. Primeiro finalize essa ação.');
            return;
        }
        if (FormPlanejamento.estaEditando()) {
            FormPlanejamento.exibirNotificacaoAviso('Você já está editando um planejamento. Finalize ou cancele a edição atual primeiro.');
            return;
        }

        const item = todosPlanejamentos.find(p => (p.id ?? p.Id) == id);
        if (!item) return;

        const pId = item.id ?? item.Id;
        const pIdMedicamento = item.idMedicamento ?? item.IdMedicamento;
        const pDataInicioIso = (item.dataInicioIso ?? item.DataInicioIso ?? '').trim();
        const pDataFimIso = (item.dataFimIso ?? item.DataFimIso ?? '').trim();
        const pDataFimFormatada = (item.dataFimFormatada ?? item.DataFimFormatada ?? '').trim();
        const pContinuo = Boolean(item.continuo ?? item.Continuo) ||
            pDataFimFormatada.toLowerCase().includes('cont') ||
            pDataFimFormatada.includes('9999') ||
            pDataFimIso.includes('9999');
        const pHora = (item.hora ?? item.Hora ?? '').trim();
        const pIntervalo = (item.intervaloFormatado ?? item.IntervaloFormatado ?? '').trim();
        
        let pDosagemValor = item.dosagemValor ?? item.DosagemValor;
        let pUnidadeDosagem = (item.unidadeDosagem ?? item.UnidadeDosagem ?? '').trim();
        const dosagemTexto = (item.dosagem ?? item.Dosagem ?? '').trim();

        if (!pDosagemValor || pDosagemValor === 0) {
            const matchDos = dosagemTexto.match(/^(\d+)/);
            if (matchDos) {
                pDosagemValor = parseInt(matchDos[1], 10);
            }
        }

        if (!pUnidadeDosagem && dosagemTexto) {
            const matchUnid = dosagemTexto.match(/([a-zA-Z]+)/);
            if (matchUnid) {
                pUnidadeDosagem = matchUnid[1];
            }
        }
        if (!pUnidadeDosagem) {
            pUnidadeDosagem = 'ML';
        }

        const pDiaSemana = (item.diaSemana ?? item.DiaSemana ?? '').trim();
        $('#idPlanejamentoEdit').val(pId);

        if (pIdMedicamento) {
            $('#inputMedicamento').val(String(pIdMedicamento)).trigger('change');
        }
        if (!$('#inputMedicamento').val() && (item.medicamentoNome || item.MedicamentoNome)) {
            const nomeMed = (item.medicamentoNome || item.MedicamentoNome).trim().toLowerCase();
            $("#inputMedicamento option").each(function () {
                if ($(this).text().trim().toLowerCase() === nomeMed) {
                    $('#inputMedicamento').val($(this).val()).trigger('change');
                    return false;
                }
            });
        }

        if (pDataInicioIso) {
            $('#inputDataInicio').val(pDataInicioIso);
        } else if (item.dataInicioFormatada || item.DataInicioFormatada) {
            const partes = (item.dataInicioFormatada || item.DataInicioFormatada).split('/');
            if (partes.length === 3) {
                $('#inputDataInicio').val(`${partes[2]}-${partes[1]}-${partes[0]}`);
            }
        }

        if (pContinuo) {
            $('#switchContinuo').prop('checked', true);
            FormPlanejamento.toggleContinuo(true);
        } else {
            $('#switchContinuo').prop('checked', false);
            FormPlanejamento.toggleContinuo(false);
            if (pDataFimIso && !pDataFimIso.includes('9999')) {
                $('#inputDataFim').val(pDataFimIso);
            } else if (pDataFimFormatada && !pDataFimFormatada.includes('9999') && !pDataFimFormatada.toLowerCase().includes('cont')) {
                const partes = pDataFimFormatada.split('/');
                if (partes.length === 3) {
                    $('#inputDataFim').val(`${partes[2]}-${partes[1]}-${partes[0]}`);
                }
            }
        }

        if (pHora) {
            $('#inputHora').val(pHora.length >= 5 ? pHora.substring(0, 5) : pHora);
        }

        if (pIntervalo) {
            $('#inputIntervalo').val(pIntervalo.length >= 5 ? pIntervalo.substring(0, 5) : pIntervalo);
        } else {
            $('#inputIntervalo').val('08:00');
        }

        if (pDosagemValor !== undefined && pDosagemValor !== null && pDosagemValor !== '') {
            $('#inputDosagem').val(pDosagemValor);
        }

        const unidUpper = pUnidadeDosagem.toUpperCase().trim();
        let encontrou = false;
        $('#inputUnidade option').each(function () {
            const optVal = $(this).val().toUpperCase();
            const optTxt = $(this).text().toUpperCase();
            if (optVal && (optVal === unidUpper || optTxt.includes(unidUpper) || unidUpper.includes(optVal))) {
                $('#inputUnidade').val($(this).val()).trigger('change');
                encontrou = true;
                return false;
            }
        });
        if (!encontrou) {
            const primeiraOpcao = $('#inputUnidade option:not([value=""])').first().val();
            if (primeiraOpcao) {
                $('#inputUnidade').val(primeiraOpcao).trigger('change');
            }
        }

        $('.chk-dia').prop('checked', false);
        if (pDiaSemana && pDiaSemana.length === 7) {
            mapaPosicionalDias.forEach((d, i) => {
                if (pDiaSemana[i] !== 'X') {
                    $(`.chk-dia[value='${d.sigla}']`).prop('checked', true);
                }
            });
        } else if (pDiaSemana) {
            const diasLimpos = pDiaSemana.split(',').map(s => s.trim().toUpperCase());
            mapaPosicionalDias.forEach(d => {
                if (diasLimpos.includes(d.sigla) || diasLimpos.includes(d.letra)) {
                    $(`.chk-dia[value='${d.sigla}']`).prop('checked', true);
                }
            });
        }

        $('#tituloSecaoForm').text('Editar planejamento');
        $('#containerBotoesForm').addClass('em-edicao');

        const btnAdd = $('#btnAdicionarPreview');
        btnAdd.text('Salvar');
        btnAdd.addClass('btn-salvar-edicao');
        $('#btnCancelarEdicao').show();

        document.getElementById('tituloSecaoForm').scrollIntoView({ behavior: 'smooth', block: 'center' });

        FormPlanejamento.validarCamposAdicionar();
        FormPlanejamento.atualizarEstadoBotoesAcaoTabela();
    },

    limparFormulario: function () {
        $('#inputMedicamento').val('').trigger('change');
        $('#inputDataInicio').val(new Date().toISOString().split('T')[0]);
        $('#switchContinuo').prop('checked', false);
        FormPlanejamento.toggleContinuo(false);
        $('#inputDataFim').val('');
        $('#inputHora').val('');
        $('#inputIntervalo').val('');
        $('#inputDosagem').val('');
        $('#inputUnidade').val('').trigger('change');
        $('.chk-dia').prop('checked', false);
    },

    cancelarEdicao: function () {
        $('#idPlanejamentoEdit').val('0');
        $('#tituloSecaoForm').text('Novo planejamento');
        $('#containerBotoesForm').removeClass('em-edicao');

        const btnAdd = $('#btnAdicionarPreview');
        btnAdd.text('Adicionar');
        btnAdd.removeClass('btn-salvar-edicao');
        $('#btnCancelarEdicao').hide();

        FormPlanejamento.limparFormulario();
        FormPlanejamento.validarCamposAdicionar();
        FormPlanejamento.atualizarEstadoBotoesAcaoTabela();
    },

    cancelarNovosAdicionados: function () {
        listaPlanejamentos = [];
        $('#chkFiltrarAdicionados').prop('checked', false);
        const idPaciente = $('#IdPaciente').val();
        if (idPaciente && idPaciente !== "0") {
            FormPlanejamento.carregarPlanejamentosMemoria(idPaciente);
        } else if (dtPlanejamentos) {
            dtPlanejamentos.clear().draw();
        }

        const btnSalvar = document.getElementById('btnSalvarForm');
        if (btnSalvar) btnSalvar.disabled = true;

        const btnCancelarFooter = document.getElementById('btnCancelarFooter');
        if (btnCancelarFooter) btnCancelarFooter.style.display = 'none';

        document.querySelectorAll('.btn-card-toggle').forEach(btn => {
            btn.disabled = false;
            btn.style.cursor = 'pointer';
        });

        document.querySelectorAll('.paciente-card').forEach(card => {
            card.classList.remove('pacientes-bloqueados');
        });

        FormPlanejamento.cancelarEdicao();
        FormPlanejamento.atualizarEstadoBotoesAcaoTabela();
    },

    removerItemLista: function (index, btnElement) {
        if (index >= 0 && index < listaPlanejamentos.length) {
            listaPlanejamentos.splice(index, 1);
        }

        const tr = $(btnElement).closest('tr');
        dtPlanejamentos.row(tr).remove().draw();

        if (listaPlanejamentos.length === 0) {
            $('#chkFiltrarAdicionados').prop('checked', false);
            dtPlanejamentos.draw();

            const btnSalvar = document.getElementById('btnSalvarForm');
            if (btnSalvar) btnSalvar.disabled = true;

            const btnCancelarFooter = document.getElementById('btnCancelarFooter');
            if (btnCancelarFooter) btnCancelarFooter.style.display = 'none';

            document.querySelectorAll('.btn-card-toggle').forEach(btn => {
                btn.disabled = false;
                btn.style.cursor = 'pointer';
            });

            document.querySelectorAll('.paciente-card').forEach(card => {
                card.classList.remove('pacientes-bloqueados');
            });
        }

        FormPlanejamento.atualizarEstadoBotoesAcaoTabela();
    },

    excluirPlanejamento: function (id, nomeMedicamento) {
        if (listaPlanejamentos.length > 0) {
            FormPlanejamento.exibirNotificacaoAviso('Você tem planejamentos adicionados não salvos. Primeiro finalize essa ação.');
            return;
        }
        if (FormPlanejamento.estaEditando()) {
            FormPlanejamento.exibirNotificacaoAviso('Você já está editando um planejamento. Finalize ou cancele a edição atual primeiro.');
            return;
        }

        const formExcluir = document.getElementById('formExcluirPlanejamento');
        if (!formExcluir) return;

        formExcluir.action = `/Planejamento/Delete/${id}`;

        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Confirmar Exclusão',
                text: `Deseja realmente excluir o planejamento de ${nomeMedicamento}?`,
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
            if (confirm(`Deseja realmente excluir o planejamento de ${nomeMedicamento}?`)) {
                formExcluir.submit();
            }
        }
    },

    filtrarNovosAdicionados: function (filtrar) {
        if (dtPlanejamentos) {
            dtPlanejamentos.draw();
        }
    },

    exibirNotificacaoAviso: function (mensagem) {
        if (typeof Swal !== 'undefined') {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer);
                    toast.addEventListener('mouseleave', Swal.resumeTimer);
                }
            });
            Toast.fire({
                icon: 'warning',
                title: 'Atenção',
                html: mensagem
            });
        } else {
            alert(mensagem);
        }
    },

    atualizarEstadoBotoesAcaoTabela: function () {
        const bloqueado = (listaPlanejamentos.length > 0) || FormPlanejamento.estaEditando();
        $('#tabelaPlanejamentos tbody tr:not(.linha-preview) .btn-acao-tabela').toggleClass('desabilitado', bloqueado);
    }
};