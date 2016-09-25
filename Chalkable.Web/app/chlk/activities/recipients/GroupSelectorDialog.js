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
            }
        ]);
});