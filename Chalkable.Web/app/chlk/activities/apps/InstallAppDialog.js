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
                    id:chlk.models.apps.AppInstallGroupTypeEnum.GRADELEVEL,
                    name: 'gradeLevels'
                }, {
                    id: chlk.models.apps.AppInstallGroupTypeEnum.DEPARTMENT,
                    name: 'departments'
                }, {
                    id: chlk.models.apps.AppInstallGroupTypeEnum.ROLE,
                    name: 'roles'
                }, {
                    id: chlk.models.apps.AppInstallGroupTypeEnum.ALL,
                    name: 'forAll'
                }, {
                    id: chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER,
                    name: 'currentPerson'
                }, {
                    id: chlk.models.apps.AppInstallGroupTypeEnum.GROUP,
                    name: 'groups'
                }];

                for(var i = 0; i < ids.length; ++i){
                    var selectedIds = [];
                    var groupType = ids[i].id.valueOf();
                    var checkedBoxes = this.dom.find('input[install-group="' + groupType + '"]:checked:not(:disabled)') || [];
                    checkedBoxes.forEach(function(elem){
                        var elemId = elem.getAttr('name').split('chk-').pop();
                        selectedIds.push(elemId);
                    });

                    this.dom.find('input[name=' + ids[i].name + ']').setValue(selectedIds.join(','));
                }
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
                _DEBUG && console.info('starting calc price timeout');
                this.priceCalcTimeout && clearTimeout(this.priceCalcTimeout);
                this.priceCalcTimeout = setTimeout(function(){
                    _DEBUG && console.info('doing calc price');
                    this.prepareAppInstallPostData_();
                    this.dom.find('input[name=submitActionType]').setValue('getAppPrice');
                    this.dom.find('form').trigger('submit');
                }.bind(this), 1700);
            },

            [ria.mvc.DomEventBind('change', 'input[type=checkbox][install-group]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleInstallGroups(node, event){
                var groupType = node.getAttr('install-group');

                var installGroups = this.dom.find('input[install-group]:checked:not(:disabled)');
                var isInstallBtnEnabled = installGroups.count() > 0;
                var event = chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf();
                if(!isInstallBtnEnabled)
                    this.disableInstallBtn_();

                if(groupType == chlk.models.apps.AppInstallGroupTypeEnum.ALL
                    || groupType == chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER){
                    this.dom.find('input[install-group]:not(:disabled)').filter(function(elem){
                        return elem.getAttr('install-group') != node.getAttr('install-group');
                    }).trigger(event, [false]);
                }else{
                    this.dom.find('input[install-group=' + chlk.models.apps.AppInstallGroupTypeEnum.ALL.valueOf() + ']').trigger(event, [false]);
                    this.dom.find('input[install-group=' + chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER.valueOf() + ']').trigger(event, [false]);
                }

                this.refreshPrice_();
            },

            [ria.mvc.DomEventBind('change', '.school input[type=checkbox][school-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSchoolCheckbox(node, event) {
                var schoolId = node.getAttr('school-id'),
                    val = node.is(':checked');

                if (schoolId == -1) {
                    this.dom.find('.class input[type=checkbox]:not([disabled])').setAttr('checked', val);
                    this.dom.find('.school input[type=checkbox]').setAttr('checked', val);
                } else {
                    this.dom.find('.class input[type=checkbox][install-group][school-id=' + schoolId + ']:not([disabled])').setAttr('checked', val)
                }

                this.refreshPrice_();
            },

            [ria.mvc.DomEventBind('click', '.school[filter-school-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSchoolClassesVisibility(node, event) {
                var schoolId = node.getAttr('filter-school-id');
                this.dom.find('.course').addClass('x-hidden').setAttr('selected-school-id', schoolId);
                this.dom.find('.course input[type=checkbox]').setAttr('selected-school-id', schoolId);
                this.dom.find('.class').addClass('x-hidden');

                this.dom.find('.school.active').removeClass('active');
                this.dom.find('.course.active').removeClass('active');
                node.addClass('active');

                if (schoolId != -1) {

                    var selectedCount = 0,
                        allCount = 0;
                    this.dom.find('.school').forEach(function (_) {
                        allCount++;
                        if (_.is(':checked')) selectedCount ++;
                    });

                    if (selectedCount == 0) {
                        this.dom.find('.school input[school-id=-1]').setAttr('checked', false).removeClass('partially-checked');
                    } else if (selectedCount == allCount) {
                        this.dom.find('.school input[school-id=-1]').setAttr('checked', true).removeClass('partially-checked');
                    } else {
                        this.dom.find('.school input[school-id=-1]').setAttr('checked', false).addClass('partially-checked');
                    }

                    var visibleCourseTypes = {};
                    this.dom.find('.class[school-id=' + schoolId + ']')
                        .forEach(function ($node) {
                            visibleCourseTypes[$node.getAttr('course-type-id')] = true;
                        });

                    Object.getOwnPropertyNames(visibleCourseTypes).forEach(function (_) {
                        _ && this.dom.find('.course[filter-course-type-id=' + _ + ']').removeClass('x-hidden');
                    }.bind(this));
                }
            },

            [ria.mvc.DomEventBind('change', '.course input[type=checkbox][course-type-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleCourseTypeCheckbox(node, event) {
                var schoolId = node.getAttr('selected-school-id'),
                    courseTypeId = node.getAttr('course-type-id'),
                    val = node.is(':checked');

                this.dom.find('.class input[type=checkbox][install-group][school-id=' + schoolId + '][course-type-id=' + courseTypeId + ']:not([disabled])').setAttr('checked', val);

                var selectedCount = 0,
                    allCount = 0;
                this.dom.find('.course:not(.x-hidden)').forEach(function (_) {
                    allCount++;
                    if (_.is(':checked')) selectedCount ++;
                });

                if (selectedCount == 0) {
                    this.dom.find('.school input[school-id=' + schoolId + ']').setAttr('checked', false).removeClass('partially-checked');
                } else if (selectedCount == allCount) {
                    this.dom.find('.school input[school-id=' + schoolId + ']').setAttr('checked', true).removeClass('partially-checked');
                } else {
                    this.dom.find('.school input[school-id=' + schoolId + ']').setAttr('checked', false).addClass('partially-checked');
                }

                this.refreshPrice_();
            },

            [ria.mvc.DomEventBind('click', '.course[filter-course-type-id]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleCourseTypeClassesVisibility(node, event) {
                var schoolId = node.getAttr('selected-school-id'),
                    courseTypeId = node.getAttr('filter-course-type-id');

                this.dom.find('.class').addClass('x-hidden');
                this.dom.find('.class[school-id=' + schoolId +'][course-type-id=' + courseTypeId + ']').removeClass('x-hidden');

                this.dom.find('.course.active').removeClass('active');
                node.addClass('active');
            }
        ]);
});