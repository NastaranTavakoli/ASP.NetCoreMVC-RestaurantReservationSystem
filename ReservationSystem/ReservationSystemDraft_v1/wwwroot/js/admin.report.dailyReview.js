let dailyChartEl = document.getElementById("dailyChart").getContext('2d');

let formatTimeList = xAxes_TimeSlots.map(t => new Date(t).toLocaleTimeString());

let dailyChart = new Chart(dailyChartEl, {
    type: 'bar',
    data: {
        labels: formatTimeList,
        datasets: [
            {
                label: 'Booked',
                data: yValues_TotalSeats,
                backgroundColor: '#6dd3ce',
            },
            {
                label: 'Completed',
                data: yValues_SeatsCompleted,
                backgroundColor: '#f7a278',
            },
            {
                label: 'Canceled',
                data: yValues_SeatsCanceled,
                backgroundColor: '#a13d63',
            },
            {
                type: 'line',
                label: 'Capacity',
                data: yValues_Capacities,
            },
        ]
    },
    options: {
        responsive: true,
        aspectRatio: () => screen.width < 750 ? 1 : 2,
        scales: {
            y: {
                display: true,
                //min: 0,
                //max: 70,
                title: {
                    display: true,
                    text: 'Seats'
                }
            },
        },
        plugins: {
            title: {
                display: true,
                text: 'Daily Seat Chart'
            },
        },
    }
});
