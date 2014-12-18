REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.InstallAppDialogTpl');

NAMESPACE('chlk.activities.apps', function () {
    /** @class chlk.activities.apps.InstallAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.InstallAppDialogTpl)],
        [chlk.activities.lib.ModelWaitClass('install-app-dialog-model-wait dialog-model-wait')],
        [chlk.activities.lib.PartialUpdateClass('app-market-install')],
        'InstallAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.DomEventBind('click', '.chlk-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){

                var ids = [{
                        id: chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ,
                        name: 'classes' }, {
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
                    }
                ];

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

            [ria.mvc.DomEventBind('change', 'input[type=checkbox]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleInstallGroups(node, event){
                var groupType = node.getAttr('install-group');

                var installGroups = this.dom.find('input[install-group]:checked:not(:disabled)');
                var isInstallBtnEnabled = installGroups.count() > 0;
                var event = chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf();

                if (isInstallBtnEnabled) {
                    this.dom.find('.chlk-button').removeAttr('disabled');
                    this.dom.find('.chlk-button').removeClass('disabled');
                    this.dom.find('button').removeAttr('disabled');
                } else {
                    this.dom.find('.chlk-button').setAttr('disabled', 'disabled');
                    this.dom.find('.chlk-button').addClass('disabled');
                    this.dom.find('button').setAttr('disabled', 'disabled');
                }

                if(groupType == chlk.models.apps.AppInstallGroupTypeEnum.ALL
                    || groupType == chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER){
                    this.dom.find('input[install-group]:not(:disabled)').filter(function(elem){
                        return elem.getAttr('install-group') != node.getAttr('install-group');
                    }).trigger(event, [false]);
                }else{
                    this.dom.find('input[install-group=5]').trigger(event, [false]);
                    this.dom.find('input[install-group=6]').trigger(event, [false]);
                }
            }
        ]);
});