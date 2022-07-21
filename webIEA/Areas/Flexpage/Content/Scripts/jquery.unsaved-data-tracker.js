/*!
 * jQuery Plugin: Are-You-Sure (Dirty Form Detection)
 * https://github.com/codedance/jquery.AreYouSure/
 *
 * Copyright (c) 2012-2014, Chris Dance and PaperCut Software http://www.papercut.com/
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * Author:  chris.dance@papercut.com
 * Version: 1.9.0
 * Date:    13th August 2014
 */

/*!
 * This structure of this library is closely inspired by the here-above quoted project.
 * The main changes made by Pluritech.be are as follows:
 *   Plugin name has changed.
 *   identifier used in .data() has changed.
 *   Only fields having css class .tracked are tracked.
 *   Except rescan, events can be trigger on forms or fields.
 *   Adding the suppport of some DevExpress asp controls.
 *   The dirty field is considered dirty, not the parent form.
 *   Changing default message in settings.
 *   Removing reset event.
 *   Only chekcs visible and enabled fields when user is leaving.
 */
(function ($) {
    $.fn.track = function (options) {

        var settings = $.extend(
            {
                'message': 'If you leave, unsaved data will be lost. Do you really want to pursue?',
                'dirtyClass': 'tracker-dirty',
                'change': null,
                'silent': false,
                'addRemoveFieldsMarksDirty': false,
                'fieldEvents': 'change keyup propertychange input',
                'fieldSelector': ":input:not(input[type=submit]):not(input[type=button])",
                'trackedCssClass' : 'tracked'
            }, options);

        // returns the jquery object where the element data is stored
        var getSavingTag = function (element) {
            if (typeof (ASPxClientControl) !== "undefined" && element instanceof ASPxClientControl) {
                return $(element.GetMainElement());
            }
            else if (element instanceof $) {
                return element;
            }
            else {
                return $(element);
            }
        }

        var getValue = function (field) {
            if (typeof (ASPxClientTextBox) !== "undefined" && field instanceof ASPxClientTextBox) {
                return field.GetText();
            }
            else if (typeof (ASPxClientComboBox) !== "undefined" && field instanceof ASPxClientComboBox) {
                return field.GetValue();
            }
            else if (typeof (ASPxClientCheckBox) !== "undefined" && field instanceof ASPxClientCheckBox) {
                return field.GetValue();
            }
            else if (typeof (ASPxClientMemo) !== "undefined" && field instanceof ASPxClientMemo) {
                return field.GetValue();
            }
            else if (typeof (ASPxClientDateEdit) !== "undefined" && field instanceof ASPxClientDateEdit) {
                return field.GetValue();
            }
            else if (typeof (ASPxClientGridView) !== "undefined" && field instanceof ASPxClientGridView) {
                return field.editItemVisibleIndex + "$" + field.editState;
            }
            else if (field instanceof $) {

                if (!field.hasClass(settings.trackedCssClass)) {
                    return null;
                }

                if (field.is(':disabled') || !field.is(':visible')) {
                    return 'data-disabled';
                }

                var type = field.attr('type');
                if (field.is('select')) {
                    type = 'select';
                }

                var val;
                switch (type) {
                    case 'checkbox':
                    case 'radio':
                        val = field.is(':checked');
                        break;
                    case 'select':
                        val = '';
                        field.find('option').each(function (o) {
                            var $option = $(this);
                            if ($option.is(':selected')) {
                                val += $option.val();
                            }
                        });
                        break;
                    default:
                        val = field.val();
                }

                return val;
            }
            throw "field is not a supported type";
        };

        var storeOrigValue = function (field, value) {
            var $target = getSavingTag(field);
            if (typeof(value) === 'undefined')
                value = getValue(field);
            $target.data('data-orig', value);
        };

        var removeOrigValue = function (field) {
            var $target = getSavingTag(field);
            $target.removeData('data-orig');
        }

        var check = function (evt, arg) {
            var isFieldDirty = function (field) {
                var origValue = getSavingTag(field).data('data-orig');
                if (undefined === origValue) {
                    return false;
                }
                return (getValue(field) != origValue);
            };

            var field;
            if (typeof (ASPxClientEdit) === "undefined" || typeof (arg) === 'undefined' || !(arg instanceof ASPxClientEdit)) {
                if (typeof (evt.target) !== "undefined")
                    field = $(evt.target);
                else
                    field = evt;
            }
            else {
                field = arg;
            }
            setDirtyStatus(getSavingTag(field), isFieldDirty(field));
            return false; //don't propagate event to parent
        };

        var shouldEditDxControlsBeTracked = function (control) {
            return control instanceof ASPxClientEdit && control.GetMainElement().className.indexOf(settings.trackedCssClass) >= 0 && getSavingTag(control).data('data-orig') === undefined;
        };

        var shoudDxControlBeTracked = function (control) {
            return (control instanceof ASPxClientEdit || control instanceof ASPxClientGridView) && control.GetMainElement().className.indexOf(settings.trackedCssClass) >= 0 && getSavingTag(control).data('data-orig') === undefined
        }

        var initForm = function ($form) {
            // reset data
            if (typeof (ASPxClientControl) !== "undefined") {
                var dxControls = ASPxClientControl.GetControlCollection();
                dxControls.ForEachControl(function (c) {
                    if ((c instanceof ASPxClientEdit || c instanceof ASPxClientGridView) && c.GetMainElement().className.indexOf(settings.trackedCssClass) >= 0 && getSavingTag(c).data('data-orig') !== undefined)
                    {
                        removeOrigValue(c);
                    }
                });
            }
            var regularFields = $form.find("[class*=" + settings.trackedCssClass + "]" + settings.fieldSelector);
            $(regularFields).each(function () {
                if ((typeof (ASPxClientEdit) === "undefined" || !(this instanceof ASPxClientEdit)) && (typeof (ASPxClientGridView) === "undefined" || !(this instanceof ASPxClientGridView))) {
                    if ($(this).data('data-orig') !== undefined)
                        removeOrigValue($(this));
                }
            });
            resetAllDirtyFields();

            //Scan untracked fields
            $form.trigger('rescan.tracker');
        };

        var setDirtyStatus = function ($field, isDirty) {
            var changed = isDirty != $field.hasClass(settings.dirtyClass);
            $field.toggleClass(settings.dirtyClass, isDirty);

            // Fire change event if required
            if (changed) {
                if (settings.change) settings.change.call($field, $field);

                if (isDirty) $field.trigger('dirty.tracker');
                if (!isDirty) $field.trigger('clean.tracker');
                $field.trigger('change.tracker');
            }
        };

        var resetAllDirtyFields = function () {
            $dirtyForms = $("form").find('[class*="' + settings.dirtyClass + '"]');
            $dirtyForms.removeClass(settings.dirtyClass);
        }

        var _onGridViewENCallback = function (s, e) {
            check(s);
        };

        // Rescan a from and add new fields to be tracked
        var rescan = function () {
            var $form = $(this);
            var regularFields = $form.find("[class*=" + settings.trackedCssClass + "]" + settings.fieldSelector);
            var addedRegularFields = 0;
            var dxFieldsNb = 0;

            //TODO for thow event aren't not detach. Might have to ask the support what's going on.
            ASPxClientEdit.DetachEditorModificationListener(check, shouldEditDxControlsBeTracked);
            ASPxClientEdit.AttachEditorModificationListener(check, shouldEditDxControlsBeTracked);

            if (typeof (ASPxClientControl) !== "undefined") {
                var dxControls = ASPxClientControl.GetControlCollection();
                dxControls.ForEachControl(function (c) {
                    if (shoudDxControlBeTracked(c)) {
                        if (c instanceof ASPxClientGridView) {
                            storeOrigValue(c, c.editItemVisibleIndex + "$0"); //Set a value as if field.editState == 0
                            c.EndCallback.RemoveHandler(_onGridViewENCallback);
                            c.EndCallback.AddHandler(_onGridViewENCallback);
                        }
                        else {
                            storeOrigValue(c);
                        }
                        getSavingTag(c).unbind('reinitialize.tracker', reinitialize);
                        getSavingTag(c).unbind('recheck.tracker', check);
                        getSavingTag(c).bind('reinitialize.tracker', reinitialize);
                        getSavingTag(c).bind('recheck.tracker', check);
                        dxFieldsNb++;
                    }
                });
            }
            $(regularFields).each(function () {
                if ((typeof (ASPxClientEdit) === "undefined" || !(this instanceof ASPxClientEdit)) && (typeof (ASPxClientGridView) === "undefined" || !(this instanceof ASPxClientGridView))) {
                    if ($(this).data('data-orig') !== undefined)
                        return;
                    storeOrigValue($(this));
                    $(this).unbind(settings.fieldEvents, check);
                    $(this).unbind('reinitialize.tracker', reinitialize);
                    $(this).unbind('recheck.tracker', check);
                    $(this).bind(settings.fieldEvents, check);
                    $(this).bind('reinitialize.tracker', reinitialize);
                    $(this).bind('recheck.tracker', check);
                    addedRegularFields++;
                }
            });

            var previousCount = typeof ($form.data("data-orig-field-count")) !== 'undefined' ? $form.data("data-orig-field-count") : 0;
            $form.data("data-orig-field-count", previousCount + addedRegularFields + dxFieldsNb);

            return false; //don't propagate event to parent
        };

        var reinitialize = function (evt, arg) {
            var field;

            if (typeof (arg) !== 'undefined' && (typeof (ASPxClientEdit) !== "undefined" && arg instanceof ASPxClientEdit) ||
                (typeof (ASPxClientGridView) !== "undefined" && arg instanceof ASPxClientGridView))  {
                field = arg;
            }
            else {
                if (typeof (evt.target) !== "undefined")
                    field = $(evt.target);
                else
                    field = evt;
            }

            var $field = getSavingTag(field)

            if ($field.is('form')) {
                initForm($field);
                resetAllDirtyFields();
            } else {
                if(field instanceof ASPxClientGridView)
                    storeOrigValue(field, field.editItemVisibleIndex + "$0"); //Set a value as if field.editState == 0)
                else
                    storeOrigValue(field)
                $field.removeClass(settings.dirtyClass);
            }
            return false; //don't propagate event to parent
        }

        if (!settings.silent && !window.aysUnloadSet) {
            window.aysUnloadSet = true;
            $(window).bind('beforeunload', function () {
                $dirtyForms = $("form").find('.' + settings.trackedCssClass + '.' + settings.dirtyClass + ':visible:not([readonly]):not([disabled])');
                if ($dirtyForms.length == 0) {
                    return;
                }
                // Prevent multiple prompts - seen on Chrome and IE
                if (navigator.userAgent.toLowerCase().match(/msie|chrome/)) {
                    if (window.aysHasPrompted) {
                        return;
                    }
                    window.aysHasPrompted = true;
                    window.setTimeout(function () { window.aysHasPrompted = false; }, 900);
                }
                return settings.message;
            });
        }

        // Adding functions to ASPxClientEdit in order to raise event.
        if (typeof (ASPxClientEdit) !== "undefined") {
            ASPxClientEdit.__proto__.ChangeTracker_Reinitialize = function (control) {
                $(control.GetMainElement()).trigger('reinitialize.tracker', control);
            }

            ASPxClientEdit.__proto__.ChangeTracker_Recheck = function (control) {
                $(control.GetMainElement()).trigger('recheck.tracker', control);
            }
        }

        if (typeof (ASPxClientGridView) !== "undefined") {
            ASPxClientGridView.__proto__.ChangeTracker_Reinitialize = function (control) {
                $(control.GetMainElement()).trigger('reinitialize.tracker', control);
            }

            ASPxClientGridView.__proto__.ChangeTracker_Recheck = function (control) {
                $(control.GetMainElement()).trigger('recheck.tracker', control);
            }
        }

        return this.each(function (elem) {
            if (!$(this).is('form')) {
                return;
            }
            var $form = $(this);
            $form.submit(resetAllDirtyFields);

            // Add a custom events
            $form.bind('rescan.tracker', rescan);
            $form.bind('reinitialize.tracker', reinitialize);
            $form.bind('recheck.tracker', check);
            initForm($form);
        });
    };
})(jQuery);

