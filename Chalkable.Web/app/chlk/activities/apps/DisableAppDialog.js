REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.DisableAppDialogTpl');

NAMESPACE('chlk.activities.apps', function(){

    /**@class chlk.activities.apps.DisableAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.DisableAppDialogTpl)],

        'DisableAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[


            [ria.mvc.DomEventBind('change', '.all-schools-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            function changeAllSchools(node, event){
                var checked = node.checked();
                var nodes = node.parent('.schools').find('.school-name .box-checkbox');
                var checkBoxes = nodes.find('.school-name-checkbox');
                checkBoxes.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), checked);
                //nodes.find('[name="school"]').setValue(checked);
                //nodes.forEach(function(_){
                //    _.find('.school-name-checkbox').setProp('checked', checked);
                //});
                this.updateSchoolIds_(checked ? checkBoxes : []);
            },

            [ria.mvc.DomEventBind('change', '.school-name-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            function changeSchool(node, event){
                var nodes = node.parent('.schools').find('.school-name .school-name-checkbox');
                var allCount = nodes.valueOf().length;
                var checkedNodes = nodes.filter(function(_){
                    return _.checked();
                });
                var checkedCount = checkedNodes.valueOf().length;

                var allSchools = node.parent('.schools').find('.all-schools-checkbox');
                if(checkedCount == allCount)
                    allSchools.removeClass('partially-checked');
                else if(checkedCount > 0 && !allSchools.hasClass('partially-checked')){
                    allSchools.addClass('partially-checked');
                    allSchools.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), true);
                }
                else if(checkedCount == 0){
                    allSchools.removeClass('partially-checked');
                    allSchools.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), false);
                }
                this.updateSchoolIds_(checkedNodes);
            },

            function updateSchoolIds_(checkSchools){
                var ids = [];
                checkSchools.forEach(function(_){ids.push(_.getData('id'));});
                this.getDom().find('.school-ids').setValue(ids.join(','));
            },
        ]);
});