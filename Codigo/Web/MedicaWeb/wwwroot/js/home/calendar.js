document.addEventListener('DOMContentLoaded', function () {
    const calendarEl = document.getElementById('calendar');
    const eventosDoBanco = window.EventosDoCalendario || [];
    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        locale: 'pt-br',
        height: 'auto',
        aspectRatio: 1.2,
        expandRows: true,
        headerToolbar: {
            left: 'title',
            center: '',
            right: 'prev,next'
        },
        titleFormat: function (date) {
            const month = date.date.marker.toLocaleString('pt-BR', { month: 'long' });
            const year = date.date.marker.getFullYear();
            const monthCap = month.charAt(0).toUpperCase() + month.slice(1);
            return `${monthCap} - ${year}`;
        },
        events: eventosDoBanco,
        eventDidMount: function (info) {
            const cell = info.el.closest('.fc-daygrid-day');
            if (cell) {
                const cores = {
                    'ATRASO': '#FFF3C7',
                    'FALHA': '#FFDEE3'
                };
                const status = info.event.extendedProps.status;
                const statusAtualDaCelula = cell.getAttribute('data-status');
                if (status === 'FALHA') {
                    cell.style.setProperty('background-color', cores['FALHA'], 'important');
                    cell.setAttribute('data-status', 'FALHA');
                } else if (status === 'ATRASO') {
                    if (statusAtualDaCelula !== 'FALHA') {
                        cell.style.setProperty('background-color', cores['ATRASO'], 'important');
                        cell.setAttribute('data-status', 'ATRASO');
                    }
                }
            }
        }
    });
    calendar.render();
});