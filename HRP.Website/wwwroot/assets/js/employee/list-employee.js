'use strict';

var listEmployee = {

    init: function () {
        listEmployee.query();
        listEmployee.searchEmployeeSuggestion();
    },

    query: function () {
        $.ajax({
            url: commonFunctions.rootURL + 'EmployeeManagement/EmployeeList',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (response) {
                $('#employee-list').html(response);
                listEmployee.initDataTable();
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    },

    initDataTable: function () {
        $('#employee-table').dataTable({
            'ordering': false,
            'lengthMenu': [[20, 50, 100], [20, 50, 100]],
            'dom': 't<".clearfix"<".pull-left" l><".pull-right" ip>>',
            'info': false,
            'language': {
                'lengthMenu': 'Page size _MENU_',
                'search': '',
                'paginate': {
                    'first': 'First',
                    'last': 'Last',
                    'next': '<i class="icon-next"></i>',
                    'previous': '<i class="icon-back"></i>'
                }
            }
        });
    },

    searchEmployeeSuggestion: function () {
        $("#SearchEmployee").autocomplete({
            source: function (request, response) {
                var term = request.term;

                $.ajax({
                    url: commonFunctions.rootURL + 'EmployeeManagement/GetEmployeeSuggestionByName',
                    dataType: 'json',
                    data: {
                        name: term
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.nickName + ' (' + item.fullName + ')',
                                    value: item.employeeId
                                };
                            }));
                        }
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $("#SearchEmployee").val(ui.item.label);
                $("#EmployeeId").val(ui.item.value);
                return false;
            }
        });
    }
};

$(document).ready(function () {
    listEmployee.init();
});

$('#quick-search').off('keyup').on('keyup', function () {
    $('#employee-table').DataTable().search($(this).val()).draw();
});

$('form.form-filter').submit(function (e) {
    e.preventDefault();
    listEmployee.query();
});

var myGroupData =
    [{
        id: "Food",
        value: "Food",
        text: "Food",
        checked: true,
        enable: true,
        cssClass: "demo-class",
        items: [
            {
                id: "Meet",
                value: "Meet",
                text: "Meet",
                checked: true,
                enable: true,
                items: [{
                    id: "Beef",
                    value: "Beef",
                    text: "Beef",
                    checked: true,
                    enable: true,
                }, {
                    id: "Pork",
                    value: "Pork",
                    text: "Pork",
                    checked: true,
                    enable: true,
                }, {
                    id: "Chicken",
                    value: "Chicken",
                    text: "Chicken",
                    checked: true,
                    enable: true,
                }]
            },
            {
                id: "Fruit",
                value: "Fruit",
                text: "Fruit",
                checked: true,
                enable: true,
                items: [{
                    id: "Red",
                    value: "Red",
                    text: "Red",
                    checked: true,
                    enable: true,
                    items: [{
                        id: "Cherry",
                        value: "Cherry",
                        text: "Cherry",
                        checked: true,
                        enable: false,
                    }, {
                        id: "Strawberry",
                        value: "Strawberry",
                        text: "Strawberry",
                        checked: true,
                        enable: true,
                    }, {
                        id: "Pineapple",
                        value: "Pineapple",
                        text: "Pineapple",
                        checked: true,
                        enable: true,
                    }]
                }, {
                    id: "Yellow",
                    value: "Yellow",
                    text: "Yellow",
                    checked: true,
                    enable: true,
                    show: false,
                    items: [{
                        id: "Banana",
                        value: "Banana",
                        text: "Banana",
                        checked: true,
                        enable: true,
                    }, {
                        id: "Pear",
                        value: "Pear",
                        text: "Pear",
                        checked: true,
                        enable: true,
                    }]
                }]
            },
            {
                id: "Vegetable",
                value: "Vegetable",
                text: "Vegetable",
                checked: true,
                enable: true,
                items: [{
                    id: "Cucumber",
                    value: "Cucumber",
                    text: "Cucumber",
                    checked: true,
                    enable: true,
                }, {
                    id: "Spinach",
                    value: "Spinach",
                    text: "Spinach",
                    checked: true,
                    enable: true,
                }, {
                    id: "Onion",
                    value: "Onion",
                    text: "Onion",
                    checked: true,
                    enable: true,
                }]
            }
        ]
    }, {
        id: "Beverage",
        value: "Beverage",
        text: "Beverage",
        checked: true,
        enable: true,
        items: [{
            id: "Beer",
            value: "Beer",
            text: "Beer",
            checked: true,
            enable: true
        }, {
            id: "Wine",
            value: "Wine",
            text: "Wine",
            checked: true,
            enable: true
        }]
    }];

$(document).ready(function () {
    $("#companyMultiSelect").fanexMultiSelect({
        listData: myGroupData
    });
});