REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.recipients.GroupSelectorTpl');
REQUIRE('chlk.templates.controls.group_people_selector.GroupsListItemsTpl');

NAMESPACE('chlk.activities.recipients', function(){

    /**@class chlk.activities.recipients.GroupSelectorDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.recipients.GroupSelectorTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.controls.group_people_selector.GroupsListItemsTpl, '', '.groups-cnt .items-cnt-2', ria.mvc.PartialUpdateRuleActions.Prepend)],
        'GroupSelectorDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.PartialUpdateRule(null, 'edit-group')],
            VOID, function editGroup(tpl, model, msg_) {
                var id = model.getId().valueOf(),
                    groupNodes = this.dom.find('.group-item[data-id=' + id + ']');

                groupNodes.forEach(function(groupNode){
                    groupNode.find('.group-name').setHTML(model.getName());
                    groupNode.find('.students-count').setHTML(model.getStudentCount().toString());
                });
            },

            OVERRIDE, Object, function isReadyForClosing() {
                var selectedGroupsOnStart = this.dom.find('.selected-groups-on-start').getValue(),
                    selectedStudentsOnStart = this.dom.find('.selected-students-on-start').getValue(),
                    selectedGroups = this.dom.find('.selected-groups').getValue(),
                    selectedStudents = this.dom.find('.selected-students').getValue(),
                    equal = true;

                selectedGroupsOnStart = selectedGroupsOnStart ? selectedGroupsOnStart.split(',') : [];
                selectedStudentsOnStart = selectedStudentsOnStart ? selectedStudentsOnStart.split(',') : [];
                selectedGroups = selectedGroups ? selectedGroups.split(',') : [];
                selectedStudents = selectedStudents ? selectedStudents.split(',') : [];

                var arr = [[selectedGroupsOnStart, selectedGroups], [selectedStudentsOnStart, selectedStudents]];

                arr.forEach(function(checkItems){
                    var selectedOnStart = checkItems[0],
                        selected = checkItems[1];

                    if(selectedOnStart.length != selected.length)
                        equal = false;
                    else
                        selectedOnStart.forEach(function(item){
                            if(selected.indexOf(item) == -1)
                                equal = false;
                        });
                });

                if (!equal) {
                    return this.view.ShowLeaveConfirmBox();
                }

                return true;
            }
        ]);
});