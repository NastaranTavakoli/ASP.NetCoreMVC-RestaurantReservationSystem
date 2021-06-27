var availableDays = [];
function getAvailableDays(year, month) {
    let restaurantId = $('#RestaurantId').val();
    console.log(restaurantId);
    $.ajax({
        type: 'GET',
        url: '/api/sittings/available/' + restaurantId + '/' + year + '/' + month,
        dataType: "json",
        /*without disabling async, beforeShowDay is triggered at the same time of ajax call.*/
        async: false,
        success: function (data) {
            availableDays = data;
        }
    });
}

let guest = 0;

$(() => {
    $('#datepicker').datepicker({
        dateFormat: "yy-mm-dd",
        minDate: new Date(),
        showAnim: "clip",

        //triggered just before the datepicker is displayed
        beforeShow: function (input, inst) {
            var defaultDate = $(this).datepicker('getDate');
            getAvailableDays(defaultDate.getFullYear(), defaultDate.getMonth() + 1);
            $("#datepicker").datepicker("refresh");
        },
        //triggered when the datepicker moves to a new month and/or year
        onChangeMonthYear: function (year, month) {
            let restaurantId = $('#RestaurantId').val();
            //ajax pass in year and month
            $.ajax({
                type: 'GET',
                url: '/api/sittings/available/' + restaurantId + '/' + year + '/' + month,
                dataType: "json",
                async: false,
                success: function (data) {
                    availableDays = data;
                }
            });
            //on callback update availabilities & refresh
            $("#datepicker").datepicker("refresh");
        },
        //triggered for each day in the datepicker before it is displayed
        beforeShowDay: function (date) {
            var formatedDate = `${date.getDate()}/${date.getMonth()+1}/${date.getFullYear()}`;
            if (jQuery.inArray(formatedDate, availableDays) == -1) {
                return [false, "", "Sorry, we're closed!"];
            }
            return [true, "", "We are open!"];
        }
    });
});

$("#datepicker").change(function () {
    $("#selectdatetime-form").submit();
});
let timeout;
$("#guest").on('keyup', function () {
    clearTimeout(timeout);
    if ($("#guest").val()) {
        timeout = setTimeout(() => $("#selectdatetime-form").submit(), 1000);    
    }
});