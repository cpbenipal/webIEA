var treeJsonData = [];
var availableLanguages = [];
var availableRoles = [];
var isNewNode = true;
var targetKey = "";
var targetNode = null;
var parentNode = null;
var newIndexes = 1;

function SiteMapManagerAddNew(targetTag) {
    var isNewNode = true;
    targetKey = jQuery(targetTag).closest("h4.panel-title").find(".sitemap-item").attr("data-resourceKey");
    targetNode = null;
    parentNode = null;

    FindSiteMapNodeByKey(treeJsonData, targetKey);
}

function SiteMapManagerSave() {
    var parentKey = jQuery("#page-sitemapsManager #cmbParent").val();
    var jsonText = '{';
    if (isNewNode == true) {
        jsonText += '"title": "' + jQuery("#page-sitemapsManager #sitemap-title").val() + '"';
        jsonText += ',"description":"' + jQuery("#page-sitemapsManager #sitemap-description").val() + '"';
    }
    if (isNewNode == false) {
        if (jQuery("#page-sitemapsManager #en-title").length > 0) {
            jsonText += '"title": "' + jQuery("#page-sitemapsManager #en-title").val() + '"';
        }
        if (jQuery("#page-sitemapsManager #en-description").length > 0) {
            jsonText += ',"description":"' + jQuery("#page-sitemapsManager #en-description").val() + '"';
        }
    }
   
    if (jQuery("#page-sitemapsManager #radioPageURL").prop("checked") == true) {
        jQuery("#page-sitemapsManager #ReferencePageURlManual").val();
        jsonText += ',"referencepageurl":"' + jQuery("#page-sitemapsManager #ReferencePageURlManual").val() + '"';
    }
    else if (jQuery("#page-sitemapsManager #radioPageSelector").prop("checked") == true) {
        jsonText += ',"referencepageurl":"' + jQuery('#ReferencePageURlSelector').val('') + '"';
    }
    else {
        jsonText += ',"referencepageurl":""';
    }

    var titles = '"titles":{"en":"' + jQuery("#page-sitemapsManager #sitemap-title").val() + '",';
    var descriptions = '"descriptions":{"en":"' + jQuery("#page-sitemapsManager #sitemap-description").val() + '",';
    if (isNewNode == false) {
        if (jQuery("#page-sitemapsManager #en-title").length > 0) {
            titles = '"titles":{"en":"' + jQuery("#page-sitemapsManager #en-title").val() + '",';
        }
        if (jQuery("#page-sitemapsManager #en-description").length > 0) {
            descriptions = '"descriptions":{"en":"' + jQuery("#page-sitemapsManager #en-description").val() + '",';
        }
    }

    jQuery.each(availableLanguages, function (index, lang) {
        if (lang != "en") {
            if (jQuery("#page-sitemapsManager #" + lang + "-title").length > 0) {
                titles += '"' + lang + '":"' + jQuery("#page-sitemapsManager #" + lang + "-title").val() + '",';
            }
            if (jQuery("#page-sitemapsManager #" + lang + "-description").length > 0) {
                descriptions += '"' + lang + '":"' + jQuery("#page-sitemapsManager #" + lang + "-description").val() + '",';
            }
        }
    });

    titles = trimLastCommaAndTrailingWhiteSpace(titles) + '}';
    descriptions = trimLastCommaAndTrailingWhiteSpace(descriptions) + '}';

    if (isNewNode == true) {
        jsonText += ',' + titles + ',' + descriptions + ',' + '"key":"' + newIndexes + '"';
        newIndexes = newIndexes + 1;
    } else {
        jsonText += ',' + titles + ',' + descriptions + ',' + '"key":"' + targetKey + '"';
    }

    var visibleForLanguages = '"languages":"';
    jQuery.each(availableLanguages, function (index, lang) {
        if (jQuery("#page-sitemapsManager #chkVisibleForLang" + lang).length > 0) {
            if (jQuery("#page-sitemapsManager #chkVisibleForLang" + lang).prop('checked') == true) {
                visibleForLanguages += lang + ';';
            }
        }
    });
    visibleForLanguages = trimLastCollonAndTrailingWhiteSpace(visibleForLanguages) + '"';

    jsonText += ',' + visibleForLanguages + ',"url":"' + jQuery("#page-sitemapsManager #txtPageUrl").val() + '", "target":"' + jQuery("#page-sitemapsManager #txtPageTarget").val() + '"';

    var visibleRoles = '"visibleRoles":"';
    jQuery.each(availableRoles, function (index, role) {
        if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).length > 0) {
            if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('disabled') == false) {
                if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('checked') == true) {
                    visibleRoles += role + ';';
                }
            }
        }
    });
    visibleRoles = trimLastCollonAndTrailingWhiteSpace(visibleRoles) + '"';
    jsonText += ',' + visibleRoles;

    var enabledRoles = '"roles":"';
    jQuery.each(availableRoles, function (index, role) {
        if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
            if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled') == false) {
                if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('checked') == true) {
                    enabledRoles += role + ';';
                }
            }
        }
    });
    enabledRoles = trimLastCollonAndTrailingWhiteSpace(enabledRoles) + '"';
    jsonText += ',' + enabledRoles;

    if (jQuery("#page-sitemapsManager #chkVisibleReadOnly").prop('disabled') == false) {
        jsonText += ', "readonly":"' + jQuery("#page-sitemapsManager #chkVisibleReadOnly").prop('checked') + '"';
    }
    else {
        jsonText += ', "readonly":"false"';
    }

    if (jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").prop('disabled') == false) {
        jsonText += ', "IsAnonymousOnly":"' + jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").prop('checked') + '"';
    }
    else {
        jsonText += ', "IsAnonymousOnly":"false"';
    }

    if (jQuery("#page-sitemapsManager #chkVisible").prop('disabled') == false) {
        jsonText += ', "visible":"' + jQuery("#page-sitemapsManager #chkVisible").prop('checked') + '"';
    }
    else {
        jsonText += ', "visible":"false"';
    }
    jsonText += '}';
    var jsonObject = JSON.parse(jsonText);

    //
    //if (parentKey != 'root') {
    //    FindParentNodeByKey(treeJsonData, parentKey);
    //}


    //if (isNewNode == true) {
    //    if (parentKey != 'root') {
    //        parentNode["nodes"].push(jsonObject);
    //    }
    //    else {
    //        treeJsonData.push(jsonObject);
    //    }
    //}
    //else {
    //    var currentIndex = 0;
    //    targetNode = null;
    //    if (parentKey != 'root') {
    //        jQuery.each(parentNode["nodes"], function (index, nodeObject) {
    //            if (targetNode != null) {
    //                return false;
    //            }
    //            else if (targetKey === nodeObject["key"]) {
    //                targetNode = nodeObject;
    //                currentIndex = index;
    //            }
    //        });
    //        parentNode["nodes"][currentIndex] = jsonObject;
    //    }
    //    else {
    //        jQuery.each(treeJsonData, function (index, nodeObject) {
    //            if (targetNode != null) {
    //                return false;
    //            }
    //            else if (targetKey === nodeObject["key"]) {
    //                targetNode = nodeObject;
    //                currentIndex = index;
    //            }
    //        });
    //        treeJsonData[currentIndex] = jsonObject;
    //    }
    //}
    if (isNewNode == true) {
        jQuery("#page-sitemapsManager #UpdateType").val("0");

        if (parentKey != 'root') {
            FindParentNodeByKey(treeJsonData, parentKey);
            jQuery("#page-sitemapsManager #UpdateNodeParentKey").val(parentKey);
        }
        else {
            jQuery("#page-sitemapsManager #UpdateNodeParentKey").val('root');
        }
        jQuery("#page-sitemapsManager #UpdateRelocateToParentKey").val('');
    }
    else {
        jQuery("#page-sitemapsManager #UpdateType").val("1");
    }
    
    jQuery("#page-sitemapsManager #UpdateNodeJson").val(jsonText);
    jQuery("#page-sitemapsManager #UpdateRelocateNewIndex").val('-1');
}

function SiteMapManagerReset() {
    isNewNode = true;
    targetKey = "";
    targetNode = null;
    parentNode = null;
    jQuery("#page-sitemapsManager #sitemap-title").val('');

    jQuery("#page-sitemapsManager #cmbParent").val('root');

    jQuery("#page-sitemapsManager #ReferencePageURlSelector").val('');
    jQuery("#page-sitemapsManager #radioPageSelector").prop("checked", true);
    jQuery("#page-sitemapsManager #radioPageURL").prop("checked", false);
    jQuery("#page-sitemapsManager #ReferencePageURlManual").val('');

    jQuery.each(availableLanguages, function (index, lang) {
        if (lang === "en") {
            if (jQuery("#page-sitemapsManager #" + lang + "-title").length > 0) {
                jQuery("#page-sitemapsManager #" + lang + "-title").val('');
            }
            if (jQuery("#page-sitemapsManager #" + lang + "-description").length > 0) {
                jQuery("#page-sitemapsManager #" + lang + "-description").val('');
            }
        }
        else {
            if (jQuery("#page-sitemapsManager #" + lang + "-title").length > 0) {
                jQuery("#page-sitemapsManager #" + lang + "-title").val('');
            }
            if (jQuery("#page-sitemapsManager #" + lang + "-description").length > 0) {
                jQuery("#page-sitemapsManager #" + lang + "-description").val('');
            }
        }
    });

    jQuery("#page-sitemapsManager #chkVisibleForAllLanguages").prop('checked', false);
    jQuery.each(availableLanguages, function (index, lang) {
        if (jQuery("#page-sitemapsManager #chkVisibleForLang" + lang).length > 0) {
            jQuery("#page-sitemapsManager #chkVisibleForLang" + lang).prop('checked', false);
        }
    });

    jQuery("#page-sitemapsManager #txtPageUrl").text('');
    jQuery("#page-sitemapsManager #txtPageTarget").val('');

    jQuery.each(availableRoles, function (index, role) {
        if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).length > 0) {
            jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('checked', false);
            jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('disabled', false);
        }
    });

    jQuery.each(availableRoles, function (index, role) {
        if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
            jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('checked', false);
            jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled', false);
        }
    });

    jQuery("#page-sitemapsManager #chkVisibleReadOnly").prop('checked', false);
    jQuery("#page-sitemapsManager #chkVisibleReadOnly").prop('disabled', false);

    jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").prop('checked', false);
    jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").prop('disabled', false);

    jQuery("#page-sitemapsManager #chkVisible").prop('checked', false);
}

function SiteMapManagerRemove(targetTag) {
    isNewNode = false;
    targetKey = jQuery(targetTag).closest("h4.panel-title").find(".sitemap-item").attr("data-resourceKey");
    targetNode = null;
    parentNode = null;

    FindSiteMapNodeByKey(treeJsonData, targetKey);

    SiteMapManagerReset();

    if (parentNode == null) {
        var index = treeJsonData.findIndex(function (item, i) {
            return item.key === targetKey
        });
        treeJsonData.splice(index, 1);
    }
    else {
        var index = parentNode.findIndex(function (item, i) {
            return item.key === targetKey
        });
        parentNode.splice(index, 1);
    }
    jQuery(targetTag).closest("div.panel-group").remove();

    jQuery("#page-sitemapsManager #UpdateNodeParentKey").val(targetKey);
    jQuery("#page-sitemapsManager #UpdateRelocateToParentKey").val('');
    jQuery("#page-sitemapsManager #UpdateType").val("3");
    jQuery("#page-sitemapsManager #UpdateRelocateNewIndex").val('-1');

    var targetKey = "";
    var targetNode = null;
    var parentNode = null;
}

function SiteMapManagerEdit(targetTag) {
    isNewNode = false;
    targetKey = jQuery(targetTag).closest("h4.panel-title").find(".sitemap-item").attr("data-resourceKey");
    targetNode = null;
    parentNode = null;

    FindSiteMapNodeByKey(treeJsonData, targetKey);

    if (targetNode != null) {
        jQuery("#page-sitemapsManager #right-sitemap").hide();
        jQuery("#page-sitemapsManager #right-homepage").show();
        jQuery("#page-sitemapsManager #sitemap-title").val(targetNode["title"]);

        if (parentNode != null) {
            jQuery("#page-sitemapsManager #cmbParent").val(parentNode["key"]);
            jQuery("#page-sitemapsManager #UpdateNodeParentKey").val(parentNode["key"]);
        }
        else {
            jQuery("#page-sitemapsManager #UpdateNodeParentKey").val('root');
        }

        jQuery("#page-sitemapsManager #ReferencePageURlSelector").val(targetNode["referencepageurl"]);

        jQuery("#page-sitemapsManager #ReferencePageURlManual").val('');
        if (targetNode["referencepageurl"] == jQuery("#page-sitemapsManager #ReferencePageURlSelector").val()) {
            jQuery("#page-sitemapsManager #ReferencePageURlManual").val('');
            jQuery("#page-sitemapsManager #radioPageSelector").prop("checked", true);
            jQuery("#page-sitemapsManager #radioPageURL").prop("checked", false);
        }
        else {
            jQuery("#page-sitemapsManager #ReferencePageURlSelector").val('');
            jQuery("#page-sitemapsManager #radioPageSelector").prop("checked", false);
            jQuery("#page-sitemapsManager #radioPageURL").prop("checked", true);
            jQuery("#page-sitemapsManager #ReferencePageURlManual").val(targetNode["referencepageurl"]);
        }

        jQuery.each(availableLanguages, function (index, lang) {
            if (lang === "en") {
                if (jQuery("#page-sitemapsManager #" + lang + "-title").length > 0) {
                    jQuery("#page-sitemapsManager #" + lang + "-title").val(targetNode["title"]);
                }
                if (jQuery("#page-sitemapsManager #" + lang + "-description").length > 0) {
                    jQuery("#page-sitemapsManager #" + lang + "-description").val(targetNode["description"]);
                }
            }
            else {
                if (jQuery("#page-sitemapsManager #" + lang + "-title").length > 0) {
                    if (targetNode["titles"]["" + lang + ""]) {
                        jQuery("#" + lang + "-title").val(targetNode["titles"]["" + lang + ""]);
                    }
                }
                if (jQuery("#page-sitemapsManager #" + lang + "-description").length > 0) {
                    if (targetNode["descriptions"]["" + lang + ""]) {
                        jQuery("#" + lang + "-description").val(targetNode["descriptions"]["" + lang + ""]);
                    }
                }
            }
        });

        jQuery("#page-sitemapsManager #chkVisibleForAllLanguages").prop('checked', false);
        if (targetNode["languages"] != null) {
            var languages = targetNode["languages"].split(';');
            jQuery.each(languages, function (index, lang) {
                if (jQuery("#page-sitemapsManager #chkVisibleForLang" + lang).length > 0) {
                    jQuery("#page-sitemapsManager #chkVisibleForLang" + lang).prop('checked', true);
                }
            });
        }

        jQuery("#page-sitemapsManager #txtPageUrl").val(targetNode["url"]);
        jQuery("#page-sitemapsManager #txtPageTarget").val(targetNode["target"]);

        var visibleRoles = [];
        if (targetNode["visibleRoles"] != null) {
            visibleRoles = targetNode["visibleRoles"].split(';');
        } else {
            visibleRoles = availableRoles;
        }

        jQuery.each(visibleRoles, function (index, role) {
            if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).length > 0) {
                jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('checked', true);
            }
        });

        var enabledRoles = [];
        if (targetNode["roles"] != null) {
            enabledRoles = targetNode["roles"].split(';');
        }
        else {
            enabledRoles = availableRoles;
        }
        jQuery.each(enabledRoles, function (index, role) {
            if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
                jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('checked', true);
            }
        });

        if (targetNode["readonly"] == null) {
            jQuery("#page-sitemapsManager #chkVisibleReadOnly").prop('checked', false);
        }
        else {
            jQuery("#page-sitemapsManager #chkVisibleReadOnly").prop('checked', targetNode["readonly"]);
        }

        if (targetNode["IsAnonymousOnly"] == null) {
            jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").prop('checked', true);
        }
        else {
            jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").prop('checked', targetNode["IsAnonymousOnly"]);
        }

        if (targetNode["visible"] == null) {
            jQuery("#page-sitemapsManager #chkVisible").prop('checked', true);
        }
        else {
            jQuery("#page-sitemapsManager #chkVisible").prop('checked', targetNode["visible"]);
        }
    }
    else {
        InitNew();
    }
}

function FindSiteMapNodeByKey(treeJsonData, targetKey) {
    jQuery.each(treeJsonData, function (index, nodeObject) {
        if (targetNode != null) {
            return false;
        }
        else if (targetKey === nodeObject["key"]) {
            targetNode = nodeObject;
        }
        else if (nodeObject["nodes"].length > 0) {
            parentNode = nodeObject;
            FindSiteMapNodeByKey(nodeObject["nodes"], targetKey);
        }
    });
}

function FindParentNodeByKey(treeJsonData, targetKey) {
    jQuery.each(treeJsonData, function (index, nodeObject) {
        if (parentNode != null) {
            return false;
        }
        if (targetKey === nodeObject["key"]) {
            parentNode = nodeObject;
        }
        else if (nodeObject["nodes"].length > 0) {
            FindParentNodeByKey(nodeObject["nodes"], targetKey);
        }
    });
}

function SiteMapManagerChangeParent(cmbParent) {
    if (jQuery(cmbParent).val() == "root") {
        parentNode = null;
    }
    else {
        parentNode = null;
        FindParentNodeByKey(treeJsonData, jQuery(cmbParent).val());
    } 
    if (isNewNode == false) {
        if (parentNode != null) {
            jQuery("#page-sitemapsManager #cmbParent").val(parentNode["key"]);
            jQuery("#page-sitemapsManager #UpdateRelocateToParentKey").val(parentNode["key"]);
        }
        else {
            jQuery("#page-sitemapsManager #UpdateRelocateToParentKey").val('root');
        }
    }
}

function InitNew() {
    SiteMapManagerReset();
    jQuery('#page-sitemapsManager #right-homepage').hide();
    jQuery('#page-sitemapsManager #right-sitemap').show();
}

function SiteMapManagerAttachEvents() {
    jQuery.each(availableRoles, function (index, role) {
        if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
            jQuery("#page-sitemapsManager #chkEnabledForRole" + role).on("change", function (event) {
                var currentRole = jQuery(this).prop('id').replace('chkEnabledForRole', '');

                if (jQuery(this).prop('checked') == true) {
                    if (jQuery("#page-sitemapsManager #chkEnabledForRole" + currentRole).length > 0) {
                        if (jQuery("#page-sitemapsManager #chkEnabledForRole" + currentRole).prop('checked') == false) {
                            jQuery("#page-sitemapsManager #chkEnabledForRole" + currentRole).prop('checked', true);
                        }
                    }
                }
            });

            jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).on("change", function (event) {
                var currentRole = jQuery(this).prop('id').replace('chkVisibleForEnabledRole', '');
                if (jQuery(this).prop('checked') == false) {
                    if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + currentRole).length > 0) {
                        if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + currentRole).prop('checked') == true) {
                            jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + currentRole).prop('checked', false);
                        }
                    }
                }
            });
        }

        jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").on("change", function (event) {
            if (jQuery(this).prop('checked') == true) {
                jQuery("#page-sitemapsManager #chkVisible").prop('checked', true);

                jQuery.each(availableRoles, function (index, role) {
                    if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('checked', false);
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled', true);
                    }

                    if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('checked', false);
                        jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('disabled', true);
                    }
                });
            }
            else {
                jQuery.each(availableRoles, function (index, role) {
                    if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled', false);
                    }

                    if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('disabled', false);
                    }
                });
            }
        });

        jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").on("change", function (event) {
            if (jQuery(this).prop('checked') == true) {
                jQuery.each(availableRoles, function (index, role) {
                    if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('checked', false);
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled', true);
                    }

                    if (jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('checked', false);
                        jQuery("#page-sitemapsManager #chkVisibleForEnabledRole" + role).prop('disabled', true);
                    }
                });
            }
        });

        jQuery("#page-sitemapsManager #chkVisible").on("change", function (event) {
            if (jQuery("#page-sitemapsManager #chkVisible").prop('checked') == true) {
                jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").prop('disabled', false);

                jQuery.each(availableRoles, function (index, role) {
                    if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled', false);
                    }

                    if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled', false);
                    }
                });
            }
            else {
                jQuery("#page-sitemapsManager #chkVisibleForAnonymousOnly").prop('disabled', true);

                jQuery.each(availableRoles, function (index, role) {
                    if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled', true);
                    }

                    if (jQuery("#page-sitemapsManager #chkEnabledForRole" + role).length > 0) {
                        jQuery("#page-sitemapsManager #chkEnabledForRole" + role).prop('disabled', true);
                    }
                });
            }
        });
    });

    jQuery('#page-sitemapsManager #chkVisibleForAllLanguages').on("change", function (event) {
        var checkedStatus = jQuery("#page-sitemapsManager #chkVisibleForAllLanguages").prop('checked');
        jQuery.each(availableLanguages, function (index, lang) {
            if (jQuery("#page-sitemapsManager #chkVisibleForLang" + lang).length > 0) {
                jQuery("#page-sitemapsManager #chkVisibleForLang" + lang).prop('checked', checkedStatus);
            }
        });
    });

    jQuery('#page-sitemapsManager #SiteMapManagerAddNewItem').on('click', function (event) {
        SiteMapManagerSave();
        jQuery('#page-sitemapsForm').submit();
    })
    jQuery('#page-sitemapsManager #SiteMapManagerEditItem').on('click', function (event) {
        SiteMapManagerSave();
        jQuery('#page-sitemapsForm').submit();
    })
}
