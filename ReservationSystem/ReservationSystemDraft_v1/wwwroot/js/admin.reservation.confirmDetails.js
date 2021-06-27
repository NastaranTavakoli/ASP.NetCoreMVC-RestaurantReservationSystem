document.querySelector("#email").addEventListener("input",
    () => {
        var value = document.querySelector("#email").value;
        var restaurantId = document.querySelector("#RestaurantId").value;
        fetch(`/api/customers?search=${value}&restaurantId=${restaurantId}`)
            .then(response => response.json())
            .then(data => {
                var datalist = document.querySelector("#datalistOptions");
                datalist.innerHTML = "";

                data.forEach(item => {
                    var opt = document.createElement('option');
                    opt.appendChild(document.createTextNode(item.email));
                    opt.setAttribute("data-value", item.id);
                    datalist.appendChild(opt);
                });


            });
    }
);


document.querySelector("#email").addEventListener("blur",
    () => {
        var id = document.querySelector("#datalistOptions").options[0].getAttribute("data-value");
        fetch(`/api/customers/${id}`)
                .then(response => response.json())
                .then(data => {
                    document.querySelector("#firstName").value = data.firstName;
                    document.querySelector("#lastName").value = data.lastName;
                    document.querySelector("#phone").value = data.phone;
                });
        console.log(id);


        }
);
