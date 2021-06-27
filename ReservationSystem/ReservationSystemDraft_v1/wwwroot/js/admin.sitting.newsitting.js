function toggleOneOffSitting(event) {
    if (event.target.checked) {
        document.getElementById("startDateLabel").innerText = "Schedule on";
        let days = document.getElementsByClassName("day-of-week");
        for (let day of days) {
            day.disabled = true;
        }
        document.getElementById("EndDate").disabled = true;
        document.getElementById("nameLabel").innerText = "Sitting Name";
    } else {
        let days = document.getElementsByClassName("day-of-week");
        for (let day of days) {
            day.disabled = false;
        }
        document.getElementById("startDateLabel").innerText = "Schedule from";
        document.getElementById("EndDate").disabled = false;
        document.getElementById("nameLabel").innerText = "Schedule Name";
    }
}

function toggleNewSittingType(event) {
    if (event.target.value == "0") {
        document.getElementById("selectSittingType").hidden = true;
        document.getElementById("newSittingType").hidden = false;
        event.target.innerText = "Choose Existing Type"
        event.target.value = "1";
    } else {
        document.getElementById("selectSittingType").hidden = false;
        document.getElementById("newSittingType").hidden = true;
        event.target.innerText = "New Sitting Type"
        event.target.value = "0";
    }
}