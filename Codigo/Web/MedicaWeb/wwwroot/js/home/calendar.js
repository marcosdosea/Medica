document.addEventListener('DOMContentLoaded', function () {
    const elementoCalendario = document.getElementById('calendar');
    const elementoTituloEsquerdo = document.getElementById('titulo-cronograma');
    let mapaEventos = window.PacientesEventos || {};
    let requisicaoEmAndamento = false;

    function alternarBotoesCalendario(bloquear) {
        const botaoPrev = document.querySelector('.fc-prev-button');
        const botaoNext = document.querySelector('.fc-next-button');
        if (botaoPrev && botaoNext) {
            botaoPrev.disabled = bloquear;
            botaoNext.disabled = bloquear;
            botaoPrev.style.opacity = bloquear ? '0.5' : '1';
            botaoNext.style.opacity = bloquear ? '0.5' : '1';
        }
    }

    const calendario = new FullCalendar.Calendar(elementoCalendario, {
        initialView: 'dayGridMonth',
        locale: 'pt-br',
        height: 'auto',
        aspectRatio: 1.2,
        expandRows: true,
        dayMaxEvents: false,
        datesSet: function (informacoesExibicao) {
            const dataFoco = calendario.getDate();
            const mes = dataFoco.getMonth() + 1;
            const ano = dataFoco.getFullYear();

            if (requisicaoEmAndamento) {
                return;
            }
            requisicaoEmAndamento = true;
            alternarBotoesCalendario(true);

            fetch(`/Home/DetailsExecucoes?ano=${ano}&mes=${mes}`)
                .then(resp => resp.json())
                .then(listaPacientes => {
                    const eventos = {};
                    listaPacientes.forEach(paciente => {
                        eventos[paciente.id] = paciente.execucoesFalhas.map(falha => ({
                            title: falha.nomeMedicamentoHora,
                            start: falha.data,
                            display: 'block',
                            extendedProps: { status: falha.status }
                        }));
                    });
                    mapaEventos = eventos;
                    window.PacientesEventos = eventos;
                    document.querySelector('.paciente-clicavel.active')?.click();
                })
                .catch(erro => console.error("Erro ao atualizar o DTO:", erro))
                .finally(() => {
                    requisicaoEmAndamento = false;
                    alternarBotoesCalendario(false);
                });
        },
        headerToolbar: {
            left: '',
            center: 'title',
            right: 'prev,next'
        },
        events: [],
        eventContent: function (argumentoEvento) {
            const status = argumentoEvento.event.extendedProps.status;
            const classeStatus = status === 'FALHA' ? 'status-falha' : 'status-atraso';
            return {
                html: `<div class="badge-medicamento ${classeStatus}">${argumentoEvento.event.title}</div>`
            };
        }
    });
    calendario.render();

    const cardsPacientes = document.querySelectorAll('.paciente-clicavel');

    cardsPacientes.forEach(card => {
        card.addEventListener('click', function () {
            const idPaciente = this.getAttribute('data-id');
            const nomePacientePartes = this.querySelector('.paciente-nome').textContent.trim().split(/\s+/);
            cardsPacientes.forEach(c => c.classList.remove('active'));
            this.classList.add('active');
            if (elementoTituloEsquerdo && nomePacientePartes.length > 0) {
                const primeiroNome = nomePacientePartes[0];
                const ultimoNome = nomePacientePartes[nomePacientePartes.length - 1];

                const nomeExibicao = nomePacientePartes.length > 1
                    ? `${primeiroNome} ${ultimoNome}`
                    : primeiroNome;

                elementoTituloEsquerdo.textContent = `Cronograma de ${nomeExibicao}`;
            }
            calendario.removeAllEventSources();
            calendario.addEventSource(mapaEventos[idPaciente] || []);
        });
    });

    if (cardsPacientes?.length > 0) {
        cardsPacientes[0].click();
    }
});