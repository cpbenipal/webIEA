function GeneralPermissionService() {
    this.url = "/Admin/GetWhoPermissions";
    this.idWaitDiv = "#fp_popupPleaseWaitDiv";
    this.whoGridContainer = ".who-grid";
    this.ModelId = $("#ModelId").val();
    this.init();
    this.newData = [];
    this.selectPermissionId = $("#selectPermissionId");
    this.selectWhoPermissionId = $("#selectWhoPermissionId");
    this.parameters = "";
    this.selectWhoPermissionIndex = -1;
}

GeneralPermissionService.prototype.showWait = function() {
    $(this.idWaitDiv ).css("display", "block");
}

GeneralPermissionService.prototype.hideWait = function() {
    $(this.idWaitDiv ).css("display", "none");
}

GeneralPermissionService.prototype.clickRowListPermission = function(s, e) {
    s.GetRowValues(e.visibleIndex, "Id", this.clickRowPermissionCallback);
}

GeneralPermissionService.prototype.sendAjax = function(data) {
    var that = this;
    
    that.showWait();
    $.ajax({
        method: 'POST',
        url: that.url,
        data:data,
        success: function(data) {
            $(that.whoGridContainer).html(data);
            that.hideWait();
        }
    });
}

GeneralPermissionService.prototype.clickRowPermissionCallback = function(values) {
    generalPermissionService.sendAjax({ 'id': values, 'newData': JSON.stringify(generalPermissionService.newData) });
    generalPermissionService.selectPermissionId.val(values);
}

GeneralPermissionService.prototype.popupMenuItemWhoClick = function(s, e) {
    generalPermissionService.parameters = e.item.name;
    if (e.item.name == "AddPerson" || e.item.name == "AddGroup") {
        fp_popupControlOpen({ command: 'edit', blockType: 'PermitRule', blockAlias: 'PermitRule', blockID: '0', OneButtonText:'OK', parameters: e.item.name }, function() {
            generalPermissionService.addNewData(window.parameters);
            window.parameters = "";
        });
    }
    else if (e.item.name == "AddASPNETUser") {
        var isFind;
        for (var i = 0; i < generalPermissionService.newData.length; i++) {
            if (generalPermissionService.newData[i].name == ", ASPNET" && generalPermissionService.newData[i].permissionId == generalPermissionService.selectPermissionId.val() ) {
                isFind = true;
            }
        }
        if (!isFind) {
            generalPermissionService.newData.push({
                 id: -1,
                 parameters: e.item.name, 
                 permissionId: generalPermissionService.selectPermissionId.val(),
                 name: ", ASPNET",
                 type: "Person",
                 isDeny: "False"
            });
            generalPermissionService.sendAjax({
                 'id': generalPermissionService.selectPermissionId.val(), 
                 'newData': JSON.stringify(generalPermissionService.newData)
            });
        }
    }
    else if (e.item.name == "AddEveryone") {
        var isFind;
        for (var i = 0; i < generalPermissionService.newData.length; i++) {
            if (generalPermissionService.newData[i].name == "Everyone" && generalPermissionService.newData[i].permissionId == generalPermissionService.selectPermissionId.val() ) {
                isFind = true;
            }
        }
        if (!isFind) {
            generalPermissionService.newData.push({
                id: 0,
                parameters: e.item.name, 
                permissionId: generalPermissionService.selectPermissionId.val(),
                name: "Everyone",
                type: "Everyone",
                isDeny: "False"
            });
            generalPermissionService.sendAjax({
                'id': generalPermissionService.selectPermissionId.val(), 
                'newData': JSON.stringify(generalPermissionService.newData)
            });
        }
    }
    else if (e.item.name == "Delete") {
        var nameCheck = $.trim(generalPermissionService.selectWhoPermissionId.val());
        if (generalPermissionService.newData.length > 0) {
            for (var i = 0; i < generalPermissionService.newData.length; i++) {
                if ($.trim(generalPermissionService.newData[i].name) == nameCheck 
                    && generalPermissionService.newData[i].permissionId == generalPermissionService.selectPermissionId.val()) {
                    generalPermissionService.newData[i].parameters = e.item.name;
                    generalPermissionService.sendAjax({
                        'id': generalPermissionService.selectPermissionId.val(),
                        'newData': JSON.stringify(generalPermissionService.newData)
                    });

                    generalPermissionService.newData.splice(i, 1);
                    return;
                }
            }
        }

        generalPermissionService.newData.push({
            id: generalPermissionService.getId(generalPermissionService.selectWhoPermissionIndex),
            parameters: e.item.name,
            permissionId: generalPermissionService.selectPermissionId.val(),
            name: nameCheck,
            isRemoved: 'True',
            type: generalPermissionService.getType(generalPermissionService.selectWhoPermissionIndex)
        });

        generalPermissionService.sendAjax({
            'id': generalPermissionService.selectPermissionId.val(), 
            'newData': JSON.stringify(generalPermissionService.newData)
        });

    }
    else if (e.item.name == "Permit") {
        var nameCheck = $.trim(generalPermissionService.selectWhoPermissionId.val());
        if (generalPermissionService.newData.length > 0) {
            for (var i = 0; i < generalPermissionService.newData.length; i++) {
                if ($.trim(generalPermissionService.newData[i].name) == nameCheck 
                    && generalPermissionService.newData[i].permissionId == generalPermissionService.selectPermissionId.val()) {
                    
                    if (generalPermissionService.newData[i].parameters != "Deny" &&
                        generalPermissionService.newData[i].parameters != "Permit")
                        generalPermissionService.newData[i].LastAction = generalPermissionService.newData[i].parameters;

                    generalPermissionService.newData[i].parameters = e.item.name;
                    generalPermissionService.newData[i].isDeny = "False";
                    generalPermissionService.sendAjax({
                        'id': generalPermissionService.selectPermissionId.val(),
                        'newData': JSON.stringify(generalPermissionService.newData)
                    });

                    return;
                }
            }
        }

        generalPermissionService.newData.push({
            id: generalPermissionService.getId(generalPermissionService.selectWhoPermissionIndex),
            parameters: e.item.name,
            permissionId: generalPermissionService.selectPermissionId.val(),
            name: nameCheck,
            isDeny: 'False',
            type: generalPermissionService.getType(generalPermissionService.selectWhoPermissionIndex)

        });

        generalPermissionService.sendAjax({
            'id': generalPermissionService.selectPermissionId.val(), 
            'newData': JSON.stringify(generalPermissionService.newData)
        });
    }
    else if (e.item.name == "Deny") {
        var nameCheck = $.trim(generalPermissionService.selectWhoPermissionId.val());
        if (generalPermissionService.newData.length > 0) {
            for (var i = 0; i < generalPermissionService.newData.length; i++) {
                if ($.trim(generalPermissionService.newData[i].name) == nameCheck 
                    && generalPermissionService.newData[i].permissionId == generalPermissionService.selectPermissionId.val()) {

                    if (generalPermissionService.newData[i].parameters != "Deny" &&
                        generalPermissionService.newData[i].parameters != "Permit")
                        generalPermissionService.newData[i].LastAction = generalPermissionService.newData[i].parameters;

                    generalPermissionService.newData[i].parameters = e.item.name;
                    generalPermissionService.newData[i].isDeny = "True";
                    generalPermissionService.sendAjax({
                        'id': generalPermissionService.selectPermissionId.val(),
                        'newData': JSON.stringify(generalPermissionService.newData)
                    });

                    return;
                }
            }
        }

        generalPermissionService.newData.push({
            id: generalPermissionService.getId(generalPermissionService.selectWhoPermissionIndex),
            parameters: e.item.name,
            permissionId: generalPermissionService.selectPermissionId.val(),
            name: nameCheck,
            isDeny: 'True',
            type: generalPermissionService.getType(generalPermissionService.selectWhoPermissionIndex)

        });

        generalPermissionService.sendAjax({
            'id': generalPermissionService.selectPermissionId.val(), 
            'newData': JSON.stringify(generalPermissionService.newData)
        });
    }
}

GeneralPermissionService.prototype.init = function() {
    fp_popupControlChangeTitle('GENERAL PERMISSION', '#fp_generalPermission');

    $("#generalPermissionForm").submit(function() {
        $("#JsonDataSave").val(JSON.stringify(generalPermissionService.newData));
    });
}

GeneralPermissionService.prototype.showContextMenu = function(args) {
    if (this.selectPermissionId.val() == "") {
        return;
    }
    if (typeof args.pageY == "undefined") {
        args.pageY = args.event.htmlEvent.pageY;
    }
    if (typeof args.pageX == "undefined") {
        args.pageX = args.event.htmlEvent.pageX;
    }

    args.$contextMenu = $('#PopupMenuWho' + args.ID)
    eval("PopupMenuWho.ShowAtPos(" + args.pageX + ", " + args.pageY + ")");
    return args;
}

GeneralPermissionService.prototype.disableEnablePopupItem = function(itemName, index) {
    var item = PopupMenuWho.GetItemByName(itemName);  

    if (index < 0) {
        item.SetEnabled(false);
    } else {
        item.SetEnabled(true);
    }
}

GeneralPermissionService.prototype.contextWhoMenu = function(s, e) {
    generalPermissionService.disableEnablePopupItem('Delete', fp_WhoPermissionsGrid.lastMultiSelectIndex);
    generalPermissionService.disableEnablePopupItem('Permit', fp_WhoPermissionsGrid.lastMultiSelectIndex);
    generalPermissionService.disableEnablePopupItem('Deny', fp_WhoPermissionsGrid.lastMultiSelectIndex);

    generalPermissionService.showContextMenu({ "pageY": e.pageY, "pageX": e.pageX, "event": e });
    return false;
}

GeneralPermissionService.prototype.addNewData = function(data) {
    if (!data)
        return;
    if (data.length > 0) {
        data[0].permissionId = this.selectPermissionId.val();
        data[0].isDeny = 0;
        data[0].parameters = this.parameters;
    }
    
    if (this.newData.length == 0 && data.length > 0) {
        this.newData.push(data[0]);
    }
    else if (data.length > 0) {
        if (this.parameters == "AddPerson") {
            var isFind = false;
            for (var i = 0; i < this.newData.length; i++) {
                if ((this.newData[i].id == data[0].id || $.trim(this.newData[i].name) == $.trim(data[0].name))
                    && this.newData[i].permissionId == data[0].permissionId) {
                    isFind = true;
                }
            }

            if (!isFind) {
                this.newData.push(data[0]);
            }
        } else {
            var isFind = false;
            for (var i = 0; i < this.newData.length; i++) {
                if (this.newData[i].name == data[0].name && this.newData[i].permissionId == data[0].permissionId) {
                    isFind = true;
                }
            }

            if (!isFind) {
                this.newData.push(data[0]);
            }
        }
    }

    this.sendAjax({ 'id': this.selectPermissionId.val(), 'newData': JSON.stringify(this.newData) });
}

GeneralPermissionService.prototype.clickRowWhoPermission = function(s, e) {
    generalPermissionService.selectWhoPermissionId.val(
        $("#fp_WhoPermissionsGrid_DXDataRow" + e.visibleIndex + ">td:first").text());
    generalPermissionService.selectWhoPermissionIndex = e.visibleIndex;
}

GeneralPermissionService.prototype.getType = function(index) {
    return $("#fp_WhoPermissionsGrid_DXDataRow" + index + ">td:first").data("type");
}
GeneralPermissionService.prototype.getId = function(index) {
    return $("#fp_WhoPermissionsGrid_DXDataRow" + index + ">td:first").data("id");
}

var generalPermissionService = new GeneralPermissionService();
