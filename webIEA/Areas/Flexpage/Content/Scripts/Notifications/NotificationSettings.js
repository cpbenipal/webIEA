if (typeof ShowNotificationSettings !== 'function') {
    function ShowNotificationSettings() {
        fp_popupControlOpen({ command: 'notification', blocklistID: '0', blockType: 'NotificationSettings', blockAlias: '', action: 'GetUserNotificationSettings', controller: 'Notifications', title: 'Notification Settings' }, function (save) { });
    }
}
function NotificationsCustomButtonClick(s, e) {
    switch (e.buttonID) {
        case "customEditBtn":
            s.GetRowValues(e.visibleIndex,
                'Path',
                function (nameFolder) {
                    fp_popupControlOpen({
                            command: 'notification',
                            blocklistID: '0',
                            blockType: 'FolderNotification',
                            blockAlias: '',
                            action: 'GetFolderNotifications',
                            controller: 'Notifications',
                            blockID: s.GetRowKey(e.visibleIndex),
                            title: 'Folder Notifications ' + nameFolder
                        },
                        function (save) {
                            s.PerformCallback();
                        });
                });
            break;
        case "customDeleteBtn":
            fp_ConfirmDialog("Confirm",
                "Do you really want to disable notifications for the changes in this folder?",
                function() {
                    $.ajax({
                        url: "/Notifications/DeleteFolderNotificationSettings",
                        data: { 'folderID': s.GetRowKey(e.visibleIndex)},
                        type: "post",
                        cache: false,
                        success: function() {
                            s.PerformCallback();
                        },
                        error: function() {
                            s.PerformCallback();
                        }
                    });
                },
                function() { s.PerformCallback() });
            break;
    }
}
