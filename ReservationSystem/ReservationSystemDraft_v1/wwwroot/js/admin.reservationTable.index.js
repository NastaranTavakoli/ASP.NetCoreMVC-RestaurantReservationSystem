window.onload = () => {
    let removeButtons = document.getElementsByName('remove-table-button');
    removeButtons.forEach(b => b.onclick = (e) => removeTable(e));
    let removeAllButtons = document.getElementsByName('remove-all-tables-button');
    removeAllButtons.forEach(b => b.onclick = (e) => removeAllTables(e));
    let addTableButtons = document.getElementsByName('add-table-button');
    addTableButtons.forEach(b => b.onclick = (e) => assignTable(e));
}

async function assignTable(event) {
    document.getElementById('errorAlert').hidden = true;
    let button = event.currentTarget;
    let reservationId = button.value;
    let select = button.previousElementSibling;
    let tableId = select.value;
    let response = await fetch('/api/ReservationTables', {
        method: 'post',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            reservationId: Number(reservationId),
            tableId: Number(tableId)
        })
    });
    if (!response.ok) {
        document.getElementById('errorMessage').innerHTML = "An error occurred when trying to assign that table.";
        document.getElementById('errorAlert').hidden = false;
        return;
    }
    window.location.replace(window.location.href);
    //let reservationTable = await response.json();
    //let tablesCell = document.getElementById('tablesFor' + reservationId);
    //let deleteButton = document.createElement('button');
    //button.className = 'btn btn-secondary p-1';
    //button.value = reservationTable.id;
    //button.onclick = "removeTable(event)";
    //button.innerHTML = `${reservationTable.table.name}
    //    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
    //        <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"></path>
    //        <path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"></path>
    //    </svg>`
    //tablesCell.appendChild(button);
}

async function removeTable(event) {
    let button = event.currentTarget;
    document.getElementById('errorAlert').hidden = true;
    let response = await fetch('/api/ReservationTables/' + button.value, {
        method: 'delete'
    });
    if (!response.ok) {
        document.getElementById('errorMessage').innerHTML = "An error occurred when trying to remove that table.";
        document.getElementById('errorAlert').hidden = false;
        return;
    }
    if (button.previousElementSibling == null && button.nextElementSibling == null) {
        window.location.replace(window.location.href);
    } else {
        button.remove();
    }
}

async function removeAllTables(event) {
    let button = event.currentTarget;
    document.getElementById('errorAlert').hidden = true;
    let response = await fetch('/api/ReservationTables/reservation/' + button.value, {
        method: 'delete'
    });
    if (!response.ok) {
        document.getElementById('errorMessage').innerHTML = "An error occurred when trying to remove the tables.";
        document.getElementById('errorAlert').hidden = false;
        return;
    }
    window.location.replace(window.location.href);
}