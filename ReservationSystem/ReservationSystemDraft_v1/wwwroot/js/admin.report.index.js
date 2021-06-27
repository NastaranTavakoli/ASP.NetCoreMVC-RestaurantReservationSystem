let weeklyChartBySittingEl = document.getElementById("weeklyChartBySitting").getContext('2d');
let weeklyChartBySitting = new Chart(weeklyChartBySittingEl, {
    data: {
        labels: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        datasets: [
            {
                type: 'line',
                label: 'Canceled Seats',
                //canceledBookings
                data: yValues_WeeklySeatsBookedList[2],
                borderColor: '#a13d63',
                backgroundColor: '#a13d63'
            },
            {
                type: 'line',
                label: 'Booked Seats',
                //totalBookings
                data: yValues_WeeklySeatsBookedList[0],
                borderColor: '#6dd3ce',
                backgroundColor: '#6dd3ce'
            },
            {
                type: 'bar',
                label: 'Completed Seats',
                //completedBookings
                data: yValues_WeeklySeatsBookedList[1],
                backgroundColor: '#f7a278'
            },
        ]
    },
    options: {
        responsive: true,
        aspectRatio: 1,
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
                text: 'SEATS BY SITTING TYPE'
            },
        },
    }
});

let weeklyChartBySourceEl = document.getElementById("weeklyChartBySource").getContext('2d');
let weeklyChartBySource = new Chart(weeklyChartBySourceEl, {
    type: 'polarArea',
    data: {
        labels: ['Online', 'Email', 'Phone', 'In Person'],
        datasets: [
            {
                label: 'Reservations',
                data: yValues_WeeklyReservationsBySource,
                backgroundColor: changeColorOnMax(yValues_WeeklyReservationsBySource),
            },
        ]
    },
    options: {
        animation: {
            duration: 0,
        },
        responsive: true,
        aspectRatio: 1,
        scales: {
            y: {
                display: false,
                title: {
                    display: true,
                    text: 'Reservations'
                }
            },
        },
        plugins: {
            title: {
                display: true,
                text: 'RESERVATION TYPE BREAKDOWN'
            },
        },
    }
});

function changeColorOnMax(arr) {
    var max = Math.max(...arr);
    var colorArr = [];
    for (i = 0; i < arr.length; i++) {
        if (arr[i] == max) {
            colorArr.push('#226ce0');
        } else {
            colorArr.push('#d3d3d3');
        }
    }
    return colorArr;
}

//let weeklyPieEl = document.getElementById("weeklyPie").getContext('2d');
//let bookedPercentage = 100 - canceledSeatsPercentage;
//let weeklyPie = new Chart(weeklyPieEl, {
//    type: 'doughnut',
//    data: {
//        labels: ['NoShow', 'Showed'],
//        datasets: [
//            {
//                label: "Test",
//                data: [canceledSeatsPercentage, bookedPercentage],
//                backgroundColor: ['blue', 'gray']
//            },
//        ]
//    },
//    options: {
//        legend: {
//            display: false,
//        },
//        title: {
//            display: true,
//            text: `${canceledSeatsPercentage} %`
//        }
//    }
//});


$("#sittingTypeSelectList").change(function () {
    $("#adminReportsForm").submit();
});


//document.addEventListener("DOMContentLoaded", function (event) {
//    var scrollpos = localStorage.getItem('scrollpos');
//    if (scrollpos) window.scrollTo(0, scrollpos);
//});

//window.onbeforeunload = function (e) {
//    localStorage.setItem('scrollpos', window.scrollY);
//};

//$('#nextBtn').on('click', () => {
//    var scrollpos = localStorage.getItem('scrollpos');
//    if (scrollpos) window.scrollTo(0, scrollpos);
//});
