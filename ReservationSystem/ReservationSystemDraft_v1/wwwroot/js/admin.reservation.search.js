//document.querySelector("#orderSelector").addEventListener("change",
//    () => {
//        //document.querySelector("#indexForm").submit();
//        generateLink();
//    }
//);

document.getElementById('StartDate').onchange = () => generateLink();
document.getElementById('EndDate').onchange = () => generateLink();
document.getElementById('OrderOptionId').onchange = () => generateLink();
document.getElementById('SearchString').oninput = () => generateLink();

//document.querySelectorAll(".pageNumberLink").forEach(item => item.addEventListener("click",
//        e => {
//            document.querySelector("#pageNumberValue").value = e.target.innerHTML;
//        })
//);


//document.querySelector("#previousButton").addEventListener("click",
//    () => {
//        document.querySelector("#pageNumberValue").innerHTML = previousPage;
//    }
//);


//document.querySelector("#nextButton").addEventListener("click",
//    () => {
//        document.querySelector("#pageNumberValue").innerHTML = nextPage;
//    }
//);

function generateLink() {
    let startDate = document.getElementById('StartDate').value;
    let endDate = document.getElementById('EndDate').value;
    if (new Date(startDate) > new Date(endDate)) {
        endDate = startDate;
        document.getElementById('EndDate').value = startDate;
    }
    let orderOptionId = document.getElementById('OrderOptionId').value;
    let searchString = document.getElementById('SearchString').value;
    document.getElementById('searchLink').href = `/Admin/Reservation/Search?startDate=${startDate}&endDate=${endDate}&orderOptionId=${orderOptionId}` + (searchString ? `&searchString=${searchString}` : '');
    document.getElementById('searchLink').classList.remove("disabled");
}
