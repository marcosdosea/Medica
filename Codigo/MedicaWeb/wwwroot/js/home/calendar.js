document.addEventListener('DOMContentLoaded', function () {
    const calendarEl = document.getElementById('calendar');
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
        events: [
            { title: '3 medicamentos', start: '2026-05-01', display: 'block', extendedProps: { status: 'confirmado' } },
            { title: '2 medicamentos', start: '2026-05-06', display: 'block', extendedProps: { status: 'atraso' } },
            { title: '3 medicamentos', start: '2026-05-25', display: 'block', extendedProps: { status: 'sem-confirmacao' } },
            { title: '1 medicamentos', start: '2026-05-18', display: 'block', extendedProps: { status: 'confirmado' } }
        ],
        eventDidMount: function (info) {
            const cell = info.el.closest('.fc-daygrid-day');
            if (cell) {
                const cores = {
                    'confirmado': '#D8FFE3',
                    'atraso': '#FFF3C7',
                    'sem-confirmacao': '#FFDEE3'
                };
                const status = info.event.extendedProps.status;
                if (cores[status]) {
                    cell.style.backgroundColor = cores[status];
                }
            }
        }
    });
    calendar.render();
});