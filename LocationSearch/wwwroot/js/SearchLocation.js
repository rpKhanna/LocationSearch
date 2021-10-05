$(document).ready(function () {
    $("#locationGrid").hide();
    $("#locationGrid_wrapper").hide();
    $("#Address").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $("#spinner").show();
            $.ajax({
                url: "/Search/GetLocations",
                type: "POST",
                dataType: "json",
                data: { prefix: request.term },
                success: function (data) {
                    response($.map(data.slice(0, 50), function (item) {
                        return { label: item.address, value: item.address };
                    }))
                    $("#spinner").hide();
                },
                complete: function () {
                    $("#spinner").hide();
                },
                error: function () {
                    $("#spinner").hide();
                }
            })
        },
        messages: {
            noResults: "", results: function (resultsCount) { }
        }
    });
});

$(document).on('submit', '#search_form', function (e) {
    e.preventDefault();
    postAjaxForm(e.target);
});

$(document).on('click', '#reset', function (e) {
    $(this).closest('form').find("input, textarea").val("");
    if ($.fn.DataTable.isDataTable('#locationGrid')) {
        $('#locationGrid').DataTable().destroy();
    }

    $('#locationGrid tbody').empty();
    $("#locationGrid").hide();
    $("#locationGrid_wrapper").hide();
});

var postAjaxForm = function (form) {
    $(form).validate();
    if ($(form).valid()) {
        $("#spinner").show();

        if ($.fn.DataTable.isDataTable('#locationGrid')) {
            $('#locationGrid').DataTable().destroy();
        }

        $('#locationGrid tbody').empty();

        $('#locationGrid').DataTable({
            "proccessing": true,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                url: "/Search/Search",
                type: 'POST',
                "datatype": "json",
                data: function (d) {
                    return $.extend({}, d, {
                        "location": $("#Address").val().toLowerCase(),
                        "maxdistance": $("#MaxDistance").val(),
                        "maxresult": $("#MaxResult").val()
                    });
                },
                dataFilter: function (data) {
                    $("#spinner").hide();
                    $("#locationGrid").show();
                    $("#locationGrid_wrapper").show();
                    return data;
                },
                error: function (xhr, error, code) {
                    $("#spinner").hide();
                    $("#locationGrid").hide();
                    $("#locationGrid_wrapper").hide();
                    console.log(xhr.responseJSON.error);
                    alert("Some error occurred. Please try again with valid request");
                }
            },
            "columns": [
                { "data": "address", "name": "Address", "autoWidth": true },
                { "data": "latitude", "name": "Latitude", "autoWidth": true },
                { "data": "longitude", "name": "Longitude", "autoWidth": true },
                { "data": "distance", "name": "Distance", "autoWidth": true },
            ]
        });
    }
    else {
        console.log("Invaild");
    }
    return true;
}