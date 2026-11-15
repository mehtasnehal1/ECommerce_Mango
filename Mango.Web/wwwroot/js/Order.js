var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else {

        if (url.includes("readyforpickup")) {
            loadDataTable("readyforpickup");

        }
        else {
            if (url.includes("cancelled")) {
                loadDataTable("cancelled");
            }
            else {
                loadDataTable("all");
            }

        }

    }

});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        order: [[0, 'desc']],
        ajax: {
            url: "/Order/GetAll",
            type: "GET",
            data: { status: status }
        },
        dataSrc: "data",
        columns: [
            { data: 'orderHeaderId', width: '5%', defaultContent: '' },
            { data: 'email', width: '25%', defaultContent: '' },
            { data: 'name', width: '20%', defaultContent: '' },
            { data: 'phone', width: '10%', defaultContent: '' },
            { data: 'status', width: '10%', defaultContent: '' },
            { data: 'orderTotal', width: '10%', defaultContent: '' },
            {
                data: 'orderHeaderId',
                width: '10%',
                orderable: false,
                searchable: false,
                "render": function (data, type, row) {
                    if (!data) return '';
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/order/orderDetail?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`;

                },
                width: "10%",
                defaultContent: ''
            }
        ],
        destroy: true
    });
}