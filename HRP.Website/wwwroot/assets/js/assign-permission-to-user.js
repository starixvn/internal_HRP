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

    function registerEventCheckAdminFunc() {
        $adminPermissionTable.find('input[type=checkbox]:not(:disabled)').on('click', function () {
            var el = $(this),
                parentId = el.data('pid'),
                functionId = el.data('fid'),
                checked = el.prop('checked'),
                attrname = el.attr('name'),
                attrRead = 'criteria\\.funcRead',
                attrHost = 'criteria\\.funcHost';

            if (!checked && attrname === 'criteria.funcRead') {
                $adminPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-fid=' + functionId + ']:not(:disabled)')
                    .prop('checked', true).trigger('click');
            } else if (checked && attrname === 'criteria.funcHost') {
                $adminPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-fid=' + functionId + ']:not(:disabled)')
                    .prop('checked', false).trigger('click');
            }

            if (parentId) {
                var parent = $adminPermissionTable.find('input[type=checkbox][name='
                    + (attrname === 'criteria.funcRead' ? attrRead : attrHost) + '][data-fid=' + parentId + ']:not(:disabled)');

                if (checked) {
                    if (!parent.prop('checked')) {
                        parent.prop('checked', checked);
                    }
                } else {
                    var childs = $adminPermissionTable.find('input[type=checkbox][name='
                        + (attrname === 'criteria.funcRead' ? attrRead : attrHost) + '][data-pid=' + parentId + ']:checked');

                    if (childs.length === 0) {
                        parent.prop('checked', checked);
                    }
                }
            } else {
                $adminPermissionTable.find('input[type=checkbox][name='
                    + (attrname === 'criteria.funcRead' ? attrRead : attrHost) + '][data-pid=' + functionId + ']:not(:disabled)')
                    .prop('checked', checked);
            }
        });
    }

    function registerEventCheckAllFunc() {
        $checkAllFunc.on('click', function () {
            var checked = $(this).prop('checked');

            $userPermissionTable.find('input[type=checkbox]:not(:disabled)').prop('checked', checked);
        });
    }

    function registerEventCheckAllRowFunc() {
        $checkAllRowFunc.on('click', function () {
            var el = $(this),
                tr = el.closest('tr'),
                parentId = el.data('pid'),
                functionId = el.data('fid'),
                checked = $(this).prop('checked'),
                companyId;

            tr.find('input[type=checkbox]:not(:disabled)').prop('checked', checked);

            if (parentId) {
                if (checked) {
                    $userPermissionTable.find('input.checkAllRowFunc[data-fid=' + parentId + ']:not(:disabled)')
                        .closest('tr').find('input[type=checkbox]:not(:disabled)').prop('checked', checked);
                } else if ($userPermissionTable.find('input.checkAllRowFunc[data-pid=' + parentId + ']:not(:disabled)')
                    .closest('tr').find('input[type=checkbox]:checked').length === 0) {
                    $userPermissionTable.find('input.checkAllRowFunc[data-fid=' + parentId + ']:not(:disabled)')
                        .closest('tr').find('input[type=checkbox]:not(:disabled)').prop('checked', checked);
                }
            } else {
                $userPermissionTable.find('input.checkAllRowFunc[data-pid=' + functionId + ']:not(:disabled)').trigger('click');
            }

            if (!checked) {
                tr.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').each(function () {
                    companyId = $(this).data('cid');
                    if ($('.checkAllColFuncRead[data-cid=' + companyId + ']:not(:disabled)').prop('checked')) {
                        $('.checkAllColFuncRead[data-cid=' + companyId + ']:not(:disabled)').prop('checked', false);
                    }
                });

                tr.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').each(function () {
                    companyId = $(this).data('cid');
                    if ($('.checkAllColFuncHost[data-cid=' + companyId + ']:not(:disabled)').prop('checked')) {
                        $('.checkAllColFuncHost[data-cid=' + companyId + ']:not(:disabled)').prop('checked', false);
                    }
                });
            }

            if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
                && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
                if (!$checkAllFunc.prop('checked')) {
                    $checkAllFunc.trigger('click');
                }
            } else {
                if ($checkAllFunc.prop('checked')) {
                    $checkAllFunc.prop('checked', false);
                }
            }
        });
    }

    function registerEventCheckAllColFuncRead() {
        $checkAllColFuncRead.on('click', function () {
            var checked = $(this).prop('checked'),
                companyId = $(this).data('cid');

            $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-cid=' + companyId + ']:not(:disabled)')
                .prop('checked', checked);

            if (!checked) {
                $('.checkAllColFuncHost[data-cid=' + companyId + ']:not(:disabled)').prop('checked', checked);
                $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-cid=' + companyId + ']:not(:disabled)')
                    .prop('checked', checked);
            }

            $userPermissionTable.find('tbody tr').each(function () {
                if ($(this).find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
                    && $(this).find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
                    $(this).find('.checkAllRowFunc:not(:disabled)').prop('checked', true);
                } else {
                    $(this).find('.checkAllRowFunc:not(:disabled)').prop('checked', false);
                }
            });

            if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
                && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
                if (!$checkAllFunc.prop('checked')) {
                    $checkAllFunc.trigger('click');
                }
            } else {
                if ($checkAllFunc.prop('checked')) {
                    $checkAllFunc.prop('checked', false);
                }
            }
        });
    }

    function registerEventCheckAllColFuncHost() {
        $checkAllColFuncHost.on('click', function () {
            var checked = $(this).prop('checked'),
                companyId = $(this).data('cid');

            $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-cid=' + companyId + ']:not(:disabled)')
                .prop('checked', checked);

            if (checked) {
                $('.checkAllColFuncRead[data-cid=' + companyId + ']:not(:disabled)').prop('checked', checked);
                $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-cid=' + companyId + ']:not(:disabled)')
                    .prop('checked', checked);
            }

            $userPermissionTable.find('tbody tr').each(function () {
                if ($(this).find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
                    && $(this).find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
                    $(this).find('.checkAllRowFunc:not(:disabled)').prop('checked', true);
                } else {
                    $(this).find('.checkAllRowFunc:not(:disabled)').prop('checked', false);
                }
            });

            if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
                && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
                if (!$checkAllFunc.prop('checked')) {
                    $checkAllFunc.trigger('click');
                }
            } else {
                if ($checkAllFunc.prop('checked')) {
                    $checkAllFunc.prop('checked', false);
                }
            }
        });
    }

    function registerEventCheckUserFunc() {
        registerEventCheckAllFunc();
        registerEventCheckAllRowFunc();
        registerEventCheckAllColFuncRead();
        registerEventCheckAllColFuncHost();

        $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:disabled)').on('click', function () {
            var el = $(this),
                parentId = el.data('pid'),
                functionId = el.data('fid'),
                companyId = el.data('cid'),
                tr = el.closest('tr'),
                elCheckAllRow = tr.find('.checkAllRowFunc:not(:disabled)'),
                elCheckAllCol = $userPermissionTable.find('.checkAllColFuncRead[data-cid=' + companyId + ']:not(:disabled)'),
                checked = el.prop('checked');

            if (parentId) {
                var parent = $userPermissionTable
                    .find('input[type=checkbox][name=criteria\\.funcRead][data-fid=' + parentId + '][data-cid=' + companyId + ']:not(:disabled)');

                if (checked) {
                    if (!parent.prop('checked')) {
                        parent.prop('checked', checked);
                    }
                } else {
                    var childs = $userPermissionTable
                        .find('input[type=checkbox][name=criteria\\.funcRead][data-pid=' + parentId + '][data-cid=' + companyId + ']:checked');

                    if (childs.length === 0) {
                        parent.prop('checked', checked);
                    }
                }
            } else {
                $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-pid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                    .prop('checked', checked);
            }

            if (!checked) {
                $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-fid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                    .prop('checked', true).trigger('click');

                if (elCheckAllRow.prop('checked')) {
                    elCheckAllRow.prop('checked', false);
                }

                if (elCheckAllCol.prop('checked')) {
                    elCheckAllCol.prop('checked', false);
                }
            } else {
                if (!elCheckAllRow.prop('checked') && tr.find('input[type=checkbox][data-fid=' + functionId + ']:not(:checked)').length === 0) {
                    elCheckAllRow.prop('checked', true);
                }

                if (!elCheckAllCol.prop('checked')
                    && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-cid=' + companyId + ']:not(:checked)').length === 0) {
                    elCheckAllCol.prop('checked', true);
                }
            }

            parentId = elCheckAllRow.data('pid');
            checked = elCheckAllRow.prop('checked');
            if (parentId) {
                if (checked) {
                    $userPermissionTable.find('.checkAllRowFunc[data-fid=' + parentId + ']:not(:disabled)').prop('checked', checked);
                } else if ($userPermissionTable.find('.checkAllRowFunc[data-pid=' + parentId + ']:checked').length === 0) {
                    $userPermissionTable.find('.checkAllRowFunc[data-fid=' + parentId + ']:not(:disabled)').prop('checked', checked);
                }
            } else {
                $userPermissionTable.find('.checkAllRowFunc[data-pid=' + functionId + ']:not(:disabled)').prop('checked', checked);
            }

            if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
                && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
                if (!$checkAllFunc.prop('checked')) {
                    $checkAllFunc.trigger('click');
                }
            } else {
                if ($checkAllFunc.prop('checked')) {
                    $checkAllFunc.prop('checked', false);
                }
            }
        });

        $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost]:not(:disabled)').on('click', function () {
            var el = $(this),
                parentId = el.data('pid'),
                functionId = el.data('fid'),
                companyId = el.data('cid'),
                tr = el.closest('tr'),
                elCheckAllRow = tr.find('.checkAllRowFunc:not(:disabled)'),
                elCheckAllCol = $userPermissionTable.find('.checkAllColFuncHost[data-cid=' + companyId + ']:not(:disabled)'),
                checked = el.prop('checked');

            if (parentId) {
                var parent = $userPermissionTable
                    .find('input[type=checkbox][name=criteria\\.funcHost][data-fid=' + parentId + '][data-cid=' + companyId + ']:not(:disabled)');

                if (checked) {
                    if (!parent.prop('checked')) {
                        parent.prop('checked', checked);
                    }
                } else {
                    var childs = $userPermissionTable
                        .find('input[type=checkbox][name=criteria\\.funcHost][data-pid=' + parentId + '][data-cid=' + companyId + ']:checked');

                    if (childs.length === 0) {
                        parent.prop('checked', checked);
                    }
                }
            } else {
                $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-pid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                    .prop('checked', checked);
            }

            if (checked) {
                $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-fid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                    .prop('checked', false).trigger('click');

                if (!elCheckAllRow.prop('checked') && tr.find('input[type=checkbox][data-fid=' + functionId + ']:not(:checked)').length === 0) {
                    elCheckAllRow.prop('checked', true);
                }

                if (!elCheckAllCol.prop('checked')
                    && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-cid=' + companyId + ']:not(:checked)').length === 0) {
                    elCheckAllCol.prop('checked', true);
                }
            } else {
                if (elCheckAllRow.prop('checked')) {
                    elCheckAllRow.prop('checked', false);
                }

                if (elCheckAllCol.prop('checked')) {
                    elCheckAllCol.prop('checked', false);
                }
            }

            parentId = elCheckAllRow.data('pid');
            checked = elCheckAllRow.prop('checked');
            if (parentId) {
                if (checked) {
                    $userPermissionTable.find('.checkAllRowFunc[data-fid=' + parentId + ']:not(:disabled)').prop('checked', checked);
                } else if ($userPermissionTable.find('.checkAllRowFunc[data-pid=' + parentId + ']:checked').length === 0) {
                    $userPermissionTable.find('.checkAllRowFunc[data-fid=' + parentId + ']:not(:disabled)').prop('checked', checked);
                }
            } else {
                $userPermissionTable.find('.checkAllRowFunc[data-pid=' + functionId + ']:not(:disabled)').prop('checked', checked);
            }

            if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
                && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
                if (!$checkAllFunc.prop('checked')) {
                    $checkAllFunc.trigger('click');
                }
            } else {
                if ($checkAllFunc.prop('checked')) {
                    $checkAllFunc.prop('checked', false);
                }
            }
        });
    }

    function registerEventSave() {
        $btnSave.on('click', function () {
            var userId = $('#UserId').val(),
                model = [],
                els = [],
                totalItems;

            els = $adminPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:disabled)');
            totalItems = els.length;

            for (var i = 0; i < totalItems; i++) {
                var el = $(els[i]),
                    functionId = el.data('fid'),
                    companyId = el.data('cid'),
                    isRead = el.prop('checked'),
                    isHost = $adminPermissionTable
                        .find('input[type=checkbox][name=criteria\\.funcHost][data-fid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                        .prop('checked');

                model.push({ UserId: userId, FunctionId: functionId, CompanyId: companyId, IsRead: isRead, IsHost: isHost });
            }

            els = $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:disabled)');
            totalItems = els.length;

            for (var i = 0; i < totalItems; i++) {
                var el = $(els[i]),
                    functionId = el.data('fid'),
                    companyId = el.data('cid'),
                    isRead = el.prop('checked'),
                    isHost = $userPermissionTable
                        .find('input[type=checkbox][name=criteria\\.funcHost][data-fid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                        .prop('checked');

                model.push({ UserId: userId, FunctionId: functionId, CompanyId: companyId, IsRead: isRead, IsHost: isHost });
            }

            if (model.length > 0) {
                var permissions = JSON.stringify(model);
                $.ajax({
                    type: 'POST',
                    url: '/UserManagement/AssignPermission',
                    data: {
                        'permissions': permissions
                    },
                    success: function (response) {
                        if (response.IsSuccessful) {
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                    }
                });
            }
        });
    }

    function initCheckedInUserTable() {
        if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
            && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
            $checkAllFunc.trigger('click');
        } else {
            $('.checkAllRowFunc').each(function () {
                var tr = $(this);

                if (tr.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
                    && tr.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
                    tr.find('.checkAllRowFunc').prop('checked', true);
                }
            });

            $('.checkAllColFuncRead').each(function () {
                var td = $(this),
                    companyId = td.data('cid');

                if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-cid=' + companyId + ']:not(:checked)').length === 0) {
                    td.prop('checked', true);
                }
            });

            $('.checkAllColFuncHost').each(function () {
                var td = $(this),
                    companyId = td.data('cid');

                if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-cid=' + companyId + ']:not(:checked)').length === 0) {
                    td.prop('checked', true);
                }
            });
        }
    }

    function registerEvents() {
        registerEventCheckAdminFunc();
        registerEventCheckUserFunc();
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
        initCheckedInUserTable();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    assignPermissionToUser.initModule();
});