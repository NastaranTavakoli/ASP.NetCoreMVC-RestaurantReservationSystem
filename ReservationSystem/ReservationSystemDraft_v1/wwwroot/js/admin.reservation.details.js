function getCustomerInfo() {
    let firstName = document.getElementById('Customer_FirstName').value;
    let lastName = document.getElementById('Customer_LastName').value;
    let email = document.getElementById('Customer_Email').value;
    let phone = document.getElementById('Customer_Phone').value;
    let notes = document.getElementById('Notes').value;
    return {
        firstName,
        lastName,
        email,
        phone,
        notes
    }
}

function checkForChange() {
    let button = document.getElementById('saveCustomerChangesButton');
    let newInfo = getCustomerInfo();    
    if (JSON.stringify(newInfo) !== JSON.stringify(window.initialState)) {
        button.disabled = false;
        button.classList.remove('disabled');
    } else {
        button.disabled = true;
        button.classList.add('disabled');
    }
}

window.onload = () => {
    window.initialState = getCustomerInfo();
    document.getElementById('Customer_FirstName').oninput = () => checkForChange();
    document.getElementById('Customer_LastName').oninput = () => checkForChange();
    document.getElementById('Customer_Email').oninput = () => checkForChange();
    document.getElementById('Customer_Phone').oninput = () => checkForChange();
    document.getElementById('Notes').oninput = () => checkForChange();
}