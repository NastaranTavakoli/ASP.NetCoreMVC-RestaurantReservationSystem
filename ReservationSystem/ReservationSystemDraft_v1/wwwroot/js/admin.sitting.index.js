window.onload = () => {
    document.getElementById('Date').addEventListener('change', (event) => {
        document.getElementById('viewByDateLink').href = `/Admin/Sitting/ViewByDate?date=${event.target.value}`;
    })
};