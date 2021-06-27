function generateSearchLink() {
    let searchString = document.getElementById('SearchString').value;
    let sittingId = document.getElementById('SittingId').value;
    if (searchString) {
        document.getElementById('searchLink').href = `/Admin/Reservation?sittingId=${sittingId}&searchString=${searchString}`;
        document.getElementById('searchLink').classList.remove('disabled');
    } else {
        document.getElementById('searchLink').href = '#';
        document.getElementById('searchLink').classList.add('disabled');
    }
}

document.getElementById('SearchString').oninput = () => generateSearchLink();