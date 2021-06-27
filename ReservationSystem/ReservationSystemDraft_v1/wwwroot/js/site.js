// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.getElementById('darkModeSwitch').onchange = (e) => {
    if (event.target.checked) {
        document.getElementById('modeStyle').href = '/css/admin.css';
        window.localStorage.setItem('viewMode', 'dark');
    } else {
        document.getElementById('modeStyle').href = '/css/standard.css';
        window.localStorage.setItem('viewMode', 'light');
    }
}