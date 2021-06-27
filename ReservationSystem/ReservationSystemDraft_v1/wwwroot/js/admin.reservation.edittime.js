function toggleShowAvailableTimes(event) {
    if (event.target.value == "0") {
        document.getElementById('avaliableTimesSection').hidden = false;
        event.target.innerText = "Hide";
        event.target.value = "1";
    } else {
        document.getElementById('avaliableTimesSection').hidden = true;
        event.target.innerText = "Show Available Times";
        event.target.value = "0";
    }
}

function selectTime(event) {
    document.getElementById('StartTIme').value = event.target.value;
}
