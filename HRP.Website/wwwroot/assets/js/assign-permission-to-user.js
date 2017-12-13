'use strict';

var assignPermissionToUser = (function ($) {
    var $adminPermissionTable,
        $userPermissionTable,
        $checkAllFunc,
        $checkAllColFuncRead,
        $checkAllColFuncHost,
        $checkAllRowFunc,
        $btnSave,
        attrRead = 'criteria\\.funcRead',
        attrHost = 'criteria\\.funcHost';

    function registerEventCheckAdminFunc() {
        $adminPermissionTable.find('input[type=checkbox]:not(:disabled)').on('click', function () {
            var el = $(this),
                parentId = el.data('pid'),
                functionId = el.data('fid'),
                checked = el.prop('checked'),
                attrname = el.attr('name');

            if (!checked && attrname === 'criteria.funcRead') {
                $adminPermissionTable.find('input[type=checkbox][name=' + attrHost + '][data-fid=' + functionId + ']:not(:disabled)')
                    .prop('checked', true).trigger('click');
            } else if (checked && attrname === 'criteria.funcHost') {
                $adminPermissionTable.find('input[type=checkbox][name=' + attrRead + '][data-fid=' + functionId + ']:not(:disabled)')
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

    function updateParent(type, companyId) {
        var $this = $(this),
            pid = $this.data('pid');

        if (pid) {
            var checked = $this.prop('checked'),
                parent = $('#func' + type + pid + companyId);

            if (checked && !parent.prop('checked')) {
                parent.prop('checked', true);
            }

            if (!checked) {
                var siblings = $('[name=criteria\\.func' + type + '][data-pid=' + parent.data('fid') + '][data-cid=' + companyId + ']:checked');

                if (siblings.length === 0) {
                    parent.prop('checked', false);

                    if (type === 'Read') {
                        parent.closest('tr').find('[name=criteria\\.funcHost][data-cid=' + companyId + ']').prop('checked', false);
                    }
                }
            }

            updateParent.call(parent.get(0), type, companyId);
        }
    }

    function checkAllForCheckBoxFuncs(type, companyId, isChecked) {
        $userPermissionTable.find('[type=checkbox][name=criteria\\.' + type + '][data-cid=' + companyId + ']:not(:disabled)')
            .prop('checked', isChecked).change();
    }

    function checkForCheckBoxAllFuncRead(companyId) {
        var $el = $('.checkAllColFuncRead[data-cid=' + companyId + ']'),
            chkFuncs = $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead][data-cid=' + companyId + ']:not(:checked)');

        if (chkFuncs.length === 0) {
            $el.prop('checked', true);
        }
        else {
            $el.prop('checked', false);
        }
    }

    function checkForCheckBoxAllFuncHost(companyId) {
        var $el = $('.checkAllColFuncHost[data-cid=' + companyId + ']'),
            chkFuncs = $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost][data-cid=' + companyId + ']:not(:checked)');

        if (chkFuncs.length === 0) {
            $el.prop('checked', true);
        }
        else {
            $el.prop('checked', false);
        }
    }

    function checkForCheckBoxAllRow(functionId) {
        var $el = $('.checkAllRowFunc[data-fid=' + functionId + ']'),
            parentId = $el.data('pid'),
            row = $el.closest('tr'),
            chkFuncsRead = row.find('input[type=checkbox][name=criteria\\.funcRead][data-fid=' + functionId + ']:not(:checked)'),
            chkFuncsHost = row.find('input[type=checkbox][name=criteria\\.funcHost][data-fid=' + functionId + ']:not(:checked)');

        if (chkFuncsRead.length === 0 && chkFuncsHost.length === 0) {
            $el.prop('checked', true);
        } else {
            $el.prop('checked', false);
        }

        if (parentId) {
            checkForCheckBoxAllRow(parentId);
        }
    }

    function checkForCheckBoxAllFuncs() {
        if ($userPermissionTable.find('input[type=checkbox][name=criteria\\.funcRead]:not(:checked)').length === 0
            && $userPermissionTable.find('input[type=checkbox][name=criteria\\.funcHost]:not(:checked)').length === 0) {
            $checkAllFunc.prop('checked', true);
        } else {
            $checkAllFunc.prop('checked', false);
        }
    }

    function registerEventCheckAllFuncs() {
        $checkAllFunc.on('click', function () {
            var isChecked = $(this).prop('checked');

            $userPermissionTable.find('input[type=checkbox]:not(:disabled)').prop('checked', isChecked);
        });
    }

    function registerEventCheckAllRowFunc() {
        $checkAllRowFunc.on('click', function () {
            var el = $(this),
                tr = el.closest('tr'),
                isChecked = $(this).prop('checked');

            if (isChecked) {
                tr.find('[type=checkbox][name=criteria\\.funcHost]').prop('checked', true).change();
            } else {
                tr.find('[type=checkbox][name=criteria\\.funcRead]').prop('checked', false).change();
            }
        });
    }

    function registerEventCheckAllColFuncRead() {
        $checkAllColFuncRead.on('click', function () {
            var isChecked = $(this).prop('checked'),
                companyId = $(this).data('cid');

            if (isChecked) {
                checkAllForCheckBoxFuncs('funcRead', companyId, true);
            }
            else {
                checkAllForCheckBoxFuncs('funcRead', companyId, false);

                $('.checkAllColFuncHost[data-cid=' + companyId + ']').prop('checked', false);
                checkAllForCheckBoxFuncs('funcHost', companyId, false);
            }
        });
    }

    function registerEventCheckAllColFuncHost() {
        $checkAllColFuncHost.on('click', function () {
            var isChecked = $(this).prop('checked'),
                companyId = $(this).data('cid');

            if (isChecked) {
                checkAllForCheckBoxFuncs('funcHost', companyId, true);

                $('.checkAllColFuncRead[data-cid=' + companyId + ']').prop('checked', true);
                checkAllForCheckBoxFuncs('funcRead', companyId, true);
            }
            else {
                checkAllForCheckBoxFuncs('funcHost', companyId, false);
            }
        });
    }

    function registerEventCheckUserFunc() {
        registerEventCheckAllFuncs();
        registerEventCheckAllRowFunc();
        registerEventCheckAllColFuncRead();
        registerEventCheckAllColFuncHost();

        $userPermissionTable.find('input[type=checkbox]:not(:disabled)').on('change', function () {
            var $this = $(this),
                functionId = $this.data('fid'),
                isChecked = $this.prop('checked'),
                companyId = $this.data('cid'),
                row = $this.closest('tr');

            switch ($this.attr('name')) {
                case 'criteria.funcHost':
                    // host checked -> read checked
                    if (isChecked) {
                        updateParent.call(
                            row.find('[type=checkbox][name=criteria\\.funcRead][data-cid=' + companyId + ']').prop('checked', true).get(0),
                            'Read',
                            companyId);
                        $userPermissionTable.find('[type=checkbox][name=criteria\\.funcRead][data-pid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                            .attr('checked', true).change();
                    }

                    // parent changed -> children changed
                    $userPermissionTable.find('[type=checkbox][name=criteria\\.funcHost][data-pid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                        .prop('checked', isChecked).change();

                    // child checked -> parent checked
                    // last child unchecked -> parent unchecked
                    updateParent.call($this.get(0), 'Host', companyId);

                    checkForCheckBoxAllFuncHost(companyId);
                    checkForCheckBoxAllFuncRead(companyId);
                    checkForCheckBoxAllRow(functionId);
                    checkForCheckBoxAllFuncs();
                    break;

                case 'criteria.funcRead':
                    // read unchecked -> host unchecked
                    if (!isChecked) {
                        updateParent.call(
                            row.find('[type=checkbox][name=criteria\\.funcHost][data-cid=' + companyId + ']:not(:disabled)').prop('checked', false).get(0),
                            'Host',
                            companyId);
                    }

                    // parent changed -> children changed
                    $userPermissionTable.find('[type=checkbox][name=criteria\\.funcRead][data-pid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                        .prop('checked', isChecked).change();

                    // child checked -> parent checked
                    // last child unchecked -> parent unchecked
                    updateParent.call($this.get(0), 'Read', companyId);

                    checkForCheckBoxAllFuncHost(companyId);
                    checkForCheckBoxAllFuncRead(companyId);
                    checkForCheckBoxAllRow(functionId);
                    checkForCheckBoxAllFuncs();
                    break;
            }
        });
    }

    function registerEventSave() {
        $btnSave.on('click', function () {
            var userId = $('#UserId').val(),
                model = [],
                els = [],
                totalItems;

            els = $adminPermissionTable.find('input[type=checkbox][name=' + attrRead + ']:not(:disabled)');
            totalItems = els.length;

            for (var i = 0; i < totalItems; i++) {
                var el = $(els[i]),
                    functionId = el.data('fid'),
                    companyId = el.data('cid'),
                    isRead = el.prop('checked'),
                    isHost = $adminPermissionTable
                        .find('input[type=checkbox][name=' + attrHost + '][data-fid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
                        .prop('checked');

                model.push({ UserId: userId, FunctionId: functionId, CompanyId: companyId, IsRead: isRead, IsHost: isHost });
            }

            els = $userPermissionTable.find('input[type=checkbox][name=' + attrRead + ']:not(:disabled)');
            totalItems = els.length;

            for (var i = 0; i < totalItems; i++) {
                var el = $(els[i]),
                    functionId = el.data('fid'),
                    companyId = el.data('cid'),
                    isRead = el.prop('checked'),
                    isHost = $userPermissionTable
                        .find('input[type=checkbox][name=' + attrHost + '][data-fid=' + functionId + '][data-cid=' + companyId + ']:not(:disabled)')
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
        if ($userPermissionTable.find('input[type=checkbox][name=' + attrRead + ']:not(:checked)').length === 0
            && $userPermissionTable.find('input[type=checkbox][name=' + attrHost + ']:not(:checked)').length === 0) {
            $checkAllFunc.trigger('click');
        } else {
            $('.checkAllRowFunc').each(function () {
                var tr = $(this);

                if (tr.find('input[type=checkbox][name=' + attrRead + ']:not(:checked)').length === 0
                    && tr.find('input[type=checkbox][name=' + attrHost + ']:not(:checked)').length === 0) {
                    tr.find('.checkAllRowFunc').prop('checked', true);
                }
            });

            $('.checkAllColFuncRead').each(function () {
                var td = $(this),
                    companyId = td.data('cid');

                if ($userPermissionTable.find('input[type=checkbox][name=' + attrRead + '][data-cid=' + companyId + ']:not(:checked)').length === 0) {
                    td.prop('checked', true);
                }
            });

            $('.checkAllColFuncHost').each(function () {
                var td = $(this),
                    companyId = td.data('cid');

                if ($userPermissionTable.find('input[type=checkbox][name=' + attrHost + '][data-cid=' + companyId + ']:not(:checked)').length === 0) {
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