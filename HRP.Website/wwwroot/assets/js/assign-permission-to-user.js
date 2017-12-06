'use strict';

var assignPermissionToUser = (function ($) {
    var $adminPermissionTable,
        $userPermissionTable,
        $checkAllFunc,
        $checkAllColFuncRead,
        $checkAllColFuncHost,
        $checkAllRowFunc,
        $btnSave,
        $btCancel;

    function registerEventCheckAllFunc() {
        $checkAllFunc.on('click', function () {
            var checked = $(this).prop('checked');

            $userPermissionTable.find('input:checkbox').prop('checked', checked);
        });
    }

    function registerEventCheckAllRowFunc() {
        $checkAllRowFunc.on('click', function () {
            var checked = $(this).prop('checked');

            $(this).closest('tr').find('input:checkbox').prop('checked', checked);
        });
    }

    function registerEventCheckAllColFuncRead() {
        $checkAllColFuncRead.on('click', function () {
            var checked = $(this).prop('checked'),
                companyId = $(this).data('cid');

            $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-cid=' + companyId + ']:not(:disabled)').prop('checked', checked);
        });
    }

    function registerEventCheckAllColFuncHost() {
        $checkAllColFuncHost.on('click', function () {
            var checked = $(this).prop('checked'),
                companyId = $(this).data('cid');

            $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-cid=' + companyId + ']:not(:disabled)').prop('checked', checked);
        });
    }

    function registerEventCheckFuncReadOrHost() {
        $adminPermissionTable.on('click', 'input[type=checkbox]:not(:disabled)', function () {
            var el = $(this),
                parentId = el.data('pid'),
                functionId = el.data('fid'),
                checked = el.prop('checked'),
                type = el.prop('name') === 'criteria.funcRead' ? 'criteria\\.funcRead' : 'criteria\\.funcHost';

            if (parentId) {
                var parent = $adminPermissionTable.find('input[type=checkbox][name=' + type + '][data-fid=' + parentId + ']:not(:disabled)');

                if (checked) {
                    if (!parent.prop('checked')) {
                        parent.prop('checked', checked);
                    }
                } else {
                    var childs = $adminPermissionTable.find('input[type=checkbox][name=' + type + '][data-pid=' + parentId + ']:checked');

                    if (childs.length === 0) {
                        parent.prop('checked', checked);
                    }
                }
            } else {
                $adminPermissionTable.find('input[type=checkbox][name=' + type + '][data-pid=' + functionId + ']:not(:disabled)').prop('checked', checked);
            }
        });

        $userPermissionTable.on('click', 'input[type=checkbox]:not(:disabled)', function () {
            var el = $(this),
                parentId = el.data('pid'),
                functionId = el.data('fid'),
                companyId = el.data('cid'),
                checked = el.prop('checked'),
                type = el.prop('name') === 'criteria.funcRead' ? 'criteria\\.funcRead' : 'criteria\\.funcHost';

            if (parentId) {
                var parent = $userPermissionTable.find('input[type=checkbox][name=' + type + '][data-fid=' + parentId + '][data-cid=' + companyId + ']:not(:disabled)');

                if (checked) {
                    if (!parent.prop('checked')) {
                        parent.prop('checked', checked);
                    }
                } else {
                    var childs = $userPermissionTable.find('input[type=checkbox][name=' + type + '][data-pid=' + parentId + '][data-cid=' + companyId + ']:checked');

                    if (childs.length === 0) {
                        parent.prop('checked', checked);
                    }
                }
            } else {
                $userPermissionTable.find('input[type=checkbox][name=' + type + '][data-pid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                    .prop('checked', checked);
            }
        });
    }

    function registerEventSave() {
        $btnSave.on('click', function () {
            var model = [],
                els = [],
                totalItems;

            els = $adminPermissionTable.find('input[type=checkbox]:not(:disabled)');
            totalItems = els.length;

            for (var i = 0; i <= totalItems; i++) {
                var el = $(els[i]);
            }

            els = $userPermissionTable.find('input[type=checkbox]:not(:disabled)');
            totalItems = els.length;

            for (var i = 0; i <= totalItems; i++) {
            }
        });
    }

    function registerEvents() {
        registerEventCheckAllFunc();
        registerEventCheckAllColFuncRead();
        registerEventCheckAllColFuncHost();
        registerEventCheckAllRowFunc();
        registerEventCheckFuncReadOrHost();

        registerEventSave();
    }

    function initModule() {
        $adminPermissionTable = $('#adminPermissionTable');
        $userPermissionTable = $('#userPermissionTable');
        $checkAllFunc = $('#checkAllFunc:not(:disabled)');
        $checkAllColFuncRead = $('.checkAllColFuncRead:not(:disabled)');
        $checkAllColFuncHost = $('.checkAllColFuncHost:not(:disabled)');
        $checkAllRowFunc = $('.checkAllRowFunc:not(:disabled)');
        $btnSave = $('#btnSave');
        $btCancel = $('#btCancel');

        registerEvents();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    assignPermissionToUser.initModule();
});