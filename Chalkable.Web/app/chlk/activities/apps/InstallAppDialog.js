REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.InstallAppDialogTpl');
REQUIRE('chlk.templates.apps.InstallAppPriceTpl');
NAMESPACE('chlk.activities.apps', function () {
    /** @class chlk.activities.apps.InstallAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.InstallAppDialogTpl)],
        [chlk.activities.lib.FixedTop(80)],
        [chlk.activities.lib.ModelWaitClass('install-app-dialog-model-wait dialog-model-wait')],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.InstallAppDialogTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.InstallAppPriceTpl, 'getAppPrice', '.calculated-price', ria.mvc.PartialUpdateRuleActions.Replace)],

        'InstallAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            Number, 'priceCalcTimeout',

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_){
                if(msg_ == 'getAppPrice'){
                    var installGroups = this.dom.find('input[install-group]:checked:not(:disabled)');
                    var isInstallBtnEnabled = installGroups.count() > 0;
                    if(isInstallBtnEnabled)
                        this.enableInstallBtn_();
                }
                BASE(model, msg_);

            },

            VOID, function prepareAppInstallPostData_(){
                var ids = [{
                    id: chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ,
                    name: 'classes'
                }, {
                    id: chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER,
                    name: 'currentPerson'
                }];

                var allType = chlk.models.apps.AppInstallGroupTypeEnum.ALL.valueOf();
                if(this.dom.find('input[install-group="' + allType + '"]:checked:not(:disabled)').exists()){
                    var checkedBoxes = this.dom.find('input[install-group="' + ids[0].id.valueOf() + '"]:not(:disabled)') || [];
                    this.preparePostDataBySelectedBoxes_(checkedBoxes, ids[0].name)
                }
                else{
                    for(var i = 0; i < ids.length; ++i){
                        var checkedBoxes = this.dom.find('input[install-group="' + ids[i].id.valueOf() + '"]:checked:not(:disabled)') || [];
                        this.preparePostDataBySelectedBoxes_(checkedBoxes, ids[i].name);
                    }
                }
            },

            [[ria.dom.Dom, String]],
            function preparePostDataBySelectedBoxes_(checkedBoxes, groupName){
                var selectedIds = [];
                checkedBoxes.forEach(function(elem){
                    var elemId = elem.getAttr('name').split('chk-').pop();
                    selectedIds.push(elemId);
                });
                this.dom.find('input[name=' + groupName + ']').setValue(selectedIds.join(','));
            },

            OVERRIDE, VOID, function onPause_() {
                BASE();
                this.dom.find('iframe[wmode]').addClass('x-hidden');
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this.dom.find('iframe[wmode]').removeClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', '.chlk-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                this.prepareAppInstallPostData_();
                //this.dom.find('input[name=submitActionType]').setValue('install');
            },

            VOID, function enableInstallBtn_(){
                this.dom.find('.chlk-button').removeAttr('disabled');
                this.dom.find('.chlk-button').removeClass('disabled');
                this.dom.find('button').removeAttr('disabled');
            },

            VOID, function disableInstallBtn_(){
                this.dom.find('.chlk-button').setAttr('disabled', 'disabled');
                this.dom.find('.chlk-button').addClass('disabled');
                this.dom.find('button').setAttr('disabled', 'disabled');
            },

            VOID, function refreshPrice_() {
                var installGroups = this.dom.find('input[install-group]:checked:not(:disabled)');
                var isInstallBtnEnabled = installGroups.count() > 0;
                if(!isInstallBtnEnabled)
                    this.disableInstallBtn_();
                else {
                    _DEBUG && console.info('starting calc price timeout');
                    this.priceCalcTimeout && clearTimeout(this.priceCalcTimeout);
                    this.priceCalcTimeout = setTimeout(function () {
                        _DEBUG && console.info('doing calc price');
                        this.prepareAppInstallPostData_();
                        this.dom.find('input[name=submitActionType]').setValue('getAppPrice');
                        this.dom.find('form').trigger('submit');
                    }.bind(this), _DEBUG ? 5000 : 1000);
                }
            },

            [ria.mvc.DomEventBind('change', 'input[type=checkbox][install-group]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleInstallGroups(node, event){
                var groupType = node.getAttr('install-group');
                event = chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf();

                if(groupType == chlk.models.apps.AppInstallGroupTypeEnum.ALL
                    || groupType == chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER){
                    this.dom.find('input[install-group]:not(:disabled)').filter(function(elem){
                        return elem.getAttr('install-group') != node.getAttr('install-group');
                    }).trigger(event, [false]);
                }else{
                    this.dom.find('input[install-group=' + chlk.models.apps.AppInstallGroupTypeEnum.ALL.valueOf() + ']').trigger(event, [false]);
                    this.dom.find('input[install-group=' + chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER.valueOf() + ']:not(:disabled)').trigger(event, [false]);
                }

                this.refreshPrice_();
            },

            [ria.mvc.DomEventBind('change', '.class input[type=checkbox][school-id][course-type-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSchoolClass(node, event){
                var schoolId = node.getAttr('school-id'),
                    courseTypeId = node.getAttr('course-type-id');

                var anyChecked = this.dom.find('.class input[type=checkbox][school-id=' + schoolId + '][course-type-id=' + courseTypeId + ']:checked:not(:disabled)').exists(),
                    anyUnchecked = this.dom.find('.class input[type=checkbox][school-id=' + schoolId + '][course-type-id=' + courseTypeId + ']:not(:checked):not(:disabled)').exists(),
                    anyPartiallyChecked = this.dom.find('.class input[type=checkbox][school-id=' + schoolId + '][course-type-id=' + courseTypeId + '].partially-checked:not(:disabled)').exists();

                setTimeout(function () {
                    var allSchoolsChk = this.dom.find('.course input[school-id=' + schoolId + '][course-type-id=' + courseTypeId + ']');
                    if (anyChecked && !anyPartiallyChecked && !anyUnchecked) {
                        chlk.controls.CheckboxControl.SET_CHECKED(allSchoolsChk);
                    } else if (!anyChecked && !anyPartiallyChecked) {
                        chlk.controls.CheckboxControl.SET_UNCHECKED(allSchoolsChk);
                    } else {
                        chlk.controls.CheckboxControl.SET_PARTIALLY_CHECKED(allSchoolsChk);
                    }

                    anyChecked = this.dom.find('.course input[type=checkbox][school-id=' + schoolId + ']:checked:not(:disabled)').exists();
                    anyUnchecked = this.dom.find('.course input[type=checkbox][school-id=' + schoolId + ']:not(:checked):not(:disabled)').exists();
                    anyPartiallyChecked = this.dom.find('.course input[type=checkbox][school-id=' + schoolId + '].partially-checked:not(:disabled)').exists();

                    allSchoolsChk = this.dom.find('.school input[school-id=' + schoolId + ']');
                    if (anyChecked && !anyPartiallyChecked && !anyUnchecked) {
                        chlk.controls.CheckboxControl.SET_CHECKED(allSchoolsChk);
                    } else if (!anyChecked && !anyPartiallyChecked) {
                        chlk.controls.CheckboxControl.SET_UNCHECKED(allSchoolsChk);
                    } else {
                        chlk.controls.CheckboxControl.SET_PARTIALLY_CHECKED(allSchoolsChk);
                    }

                    anyChecked = this.dom.find('.school input[type=checkbox]:checked:not([school-id=-1]):not(:disabled)').exists();
                    anyUnchecked = this.dom.find('.school input[type=checkbox]:not(:checked):not([school-id=-1]):not(:disabled)').exists();
                    anyPartiallyChecked = this.dom.find('.school input[type=checkbox].partially-checked:not([school-id=-1]):not(:disabled)').exists();

                    allSchoolsChk = this.dom.find('.school input[school-id=-1]');
                    if (anyChecked && !anyPartiallyChecked && !anyUnchecked) {
                        chlk.controls.CheckboxControl.SET_CHECKED(allSchoolsChk);
                    } else if (!anyChecked && !anyPartiallyChecked) {
                        chlk.controls.CheckboxControl.SET_UNCHECKED(allSchoolsChk);
                    } else {
                        chlk.controls.CheckboxControl.SET_PARTIALLY_CHECKED(allSchoolsChk);
                    }
                }.bind(this), 1);
            },

            [ria.mvc.DomEventBind('change', '.school input[type=checkbox][school-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSchoolCheckbox(node, event) {
                var schoolId = node.getAttr('school-id'),
                    val = node.is(':checked');

                if (schoolId == -1) {
                    this.dom.find('.class input[type=checkbox]:not([disabled])')
                        .forEach(function (_) {
                            chlk.controls.CheckboxControl.SET_CHECKED_VALUE(_, val);
                        });
                    this.dom.find('.course input[type=checkbox]:not([disabled])')
                        .forEach(function (_) {
                            chlk.controls.CheckboxControl.SET_CHECKED_VALUE(_, val);
                        });
                    this.dom.find('.school input[type=checkbox]:not([disabled])')
                        .forEach(function (_) {
                            chlk.controls.CheckboxControl.SET_CHECKED_VALUE(_, val);
                        });
                } else {
                    this.dom.find('.class input[type=checkbox][install-group][school-id=' + schoolId + ']:not(:disabled)')
                        .forEach(function (_) {
                            chlk.controls.CheckboxControl.SET_CHECKED_VALUE(_, val);
                        });

                    this.dom.find('.course input[type=checkbox][school-id=' + schoolId + ']:not(:disabled)')
                        .forEach(function (_) {
                            chlk.controls.CheckboxControl.SET_CHECKED_VALUE(_, val);
                        });

                    setTimeout(function () {
                        var anyChecked = this.dom.find('.school input[type=checkbox]:checked:not([school-id=-1]):not(:disabled)').exists(),
                            anyUnchecked = this.dom.find('.school input[type=checkbox]:not(:checked):not([school-id=-1]):not(:disabled)').exists(),
                            anyPartiallyChecked = this.dom.find('.school input[type=checkbox].partially-checked:not([school-id=-1]):not(:disabled)').exists();

                        var allSchoolsChk = this.dom.find('.school input[school-id=-1]');
                        if (anyChecked && !anyPartiallyChecked && !anyUnchecked) {
                            chlk.controls.CheckboxControl.SET_CHECKED(allSchoolsChk);
                        } else if (!anyChecked && !anyPartiallyChecked) {
                            chlk.controls.CheckboxControl.SET_UNCHECKED(allSchoolsChk);
                        } else {
                            chlk.controls.CheckboxControl.SET_PARTIALLY_CHECKED(allSchoolsChk);
                        }
                    }.bind(this), 1);
                }

                this.refreshPrice_();
            },

            [ria.mvc.DomEventBind('click', '.school[filter-school-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSchoolClassesVisibility(node, event) {
                var schoolId = node.getAttr('filter-school-id');
                this.dom.find('.course').addClass('x-hidden');
                this.dom.find('.class').addClass('x-hidden');

                this.dom.find('.school.active').removeClass('active');
                this.dom.find('.course.active').removeClass('active');
                node.addClass('active');

                if (schoolId != -1) {
                    this.dom.find('.course[school-id=' + schoolId + ']').removeClass('x-hidden');
                }
            },

            [ria.mvc.DomEventBind('change', '.course input[type=checkbox][course-type-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleCourseTypeCheckbox(node, event) {
                var schoolId = node.getAttr('school-id'),
                    courseTypeId = node.getAttr('course-type-id'),
                    val = node.is(':checked');

                this.dom.find('.class input[type=checkbox][install-group][school-id=' + schoolId + '][course-type-id=' + courseTypeId + ']:not([disabled])')
                    .forEach(function (_) {
                        chlk.controls.CheckboxControl.SET_CHECKED_VALUE(_, val);
                    });

                setTimeout(function () {
                    var anyChecked = this.dom.find('.course input[type=checkbox][school-id=' + schoolId + ']:checked:not(:disabled)').exists(),
                        anyUnchecked = this.dom.find('.course input[type=checkbox][school-id=' + schoolId + ']:not(:checked):not(:disabled)').exists(),
                        anyPartiallyChecked = this.dom.find('.course input[type=checkbox][school-id=' + schoolId + '].partially-checked:not(:disabled)').exists();

                    var allSchoolsChk = this.dom.find('.school input[school-id=' + schoolId + ']');
                    if (anyChecked && !anyPartiallyChecked && !anyUnchecked) {
                        chlk.controls.CheckboxControl.SET_CHECKED(allSchoolsChk);
                    } else if (!anyChecked && !anyPartiallyChecked) {
                        chlk.controls.CheckboxControl.SET_UNCHECKED(allSchoolsChk);
                    } else {
                        chlk.controls.CheckboxControl.SET_PARTIALLY_CHECKED(allSchoolsChk);
                    }

                    anyChecked = this.dom.find('.school input[type=checkbox]:checked:not([school-id=-1]):not(:disabled)').exists();
                    anyUnchecked = this.dom.find('.school input[type=checkbox]:not(:checked):not([school-id=-1]):not(:disabled)').exists();
                    anyPartiallyChecked = this.dom.find('.school input[type=checkbox].partially-checked:not([school-id=-1]):not(:disabled)').exists();

                    allSchoolsChk = this.dom.find('.school input[school-id=-1]');
                    if (anyChecked && !anyPartiallyChecked && !anyUnchecked) {
                        chlk.controls.CheckboxControl.SET_CHECKED(allSchoolsChk);
                    } else if (!anyChecked && !anyPartiallyChecked) {
                        chlk.controls.CheckboxControl.SET_UNCHECKED(allSchoolsChk);
                    } else {
                        chlk.controls.CheckboxControl.SET_PARTIALLY_CHECKED(allSchoolsChk);
                    }
                }.bind(this), 1);

                this.refreshPrice_();
            },

            [ria.mvc.DomEventBind('click', '.course[filter-course-type-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleCourseTypeClassesVisibility(node, event) {
                var schoolId = node.getAttr('school-id'),
                    courseTypeId = node.getAttr('filter-course-type-id');

                this.dom.find('.class').addClass('x-hidden');
                this.dom.find('.class[school-id=' + schoolId +'][course-type-id=' + courseTypeId + ']').removeClass('x-hidden');

                this.dom.find('.course.active').removeClass('active');
                node.addClass('active');
            }
        ]);
});