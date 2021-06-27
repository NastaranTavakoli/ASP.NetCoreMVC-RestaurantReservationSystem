
//document.querySelector("#datePicker").addEventListener("change",
//    () => {
//        var selectedDate = document.querySelector("#datePicker").value;
//        fetch(`/api/Sittings/${id}/${selectedDate}`)
//            .then(response => response.json())
//            .then(data => {
//                var picker = document.querySelector("#sittingPicker");

//                var opt = document.createElement('option');
//                opt.appendChild(document.createTextNode("Select from available sittings ⮟"));
//                opt.setAttribute("disabled", true);
//                opt.setAttribute("selected", true);
//                picker.innerHTML = "";
//                picker.appendChild(opt);

//                data.forEach(item => {
//                    var opt = document.createElement('option');
//                    opt.appendChild(document.createTextNode(item.description));
//                    opt.value = item.id;
//                    picker.appendChild(opt);
//                });
//            });
//    }
//);


document.querySelector("#datePicker").addEventListener("change",
    () => {
        document.querySelector("#selectTimeForm").submit();
    }
);
document.querySelector("select").addEventListener("change",
    () => {
        document.querySelector("#selectTimeForm").submit();
    }
);
let timeout;
document.querySelector("#guestsInput").addEventListener("keyup",
    () => {
        clearTimeout(timeout);
        if (document.querySelector("#guestsInput").value) {
            timeout = setTimeout(() => document.querySelector("#selectTimeForm").submit(), 1000);
        }
    }
);
document.querySelector("#displayAllCheck").addEventListener("click",
    () => {
        document.querySelector("#selectTimeForm").submit();
    }
);

function toggleShowAvailableTimes(event) {
    if (event.target.value == "0") {
        document.getElementById('availableTimesSection').hidden = false;
        event.target.innerText = "Hide";
        event.target.classList.remove("btn-primary");
        event.target.classList.add("btn-secondary");
        event.target.value = "1";
    } else {
        document.getElementById('availableTimesSection').hidden = true;
        event.target.innerText = "Show Available Times";
        event.target.classList.remove("btn-secondary");
        event.target.classList.add("btn-primary");
        event.target.value = "0";
    }
}

function selectTime(event) {
    let newTime = event.target.value;
    let sittingId = event.target.dataset.sittingid;
    document.getElementById('StartTime').value = newTime;
    document.getElementById('SittingId').value = sittingId;
}

