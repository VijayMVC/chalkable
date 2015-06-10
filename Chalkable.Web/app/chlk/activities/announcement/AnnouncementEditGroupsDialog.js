REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.group.GroupsListTpl');
REQUIRE('chlk.templates.group.AnnouncementGroupTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AnnouncementEditGroupsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.group.GroupsListTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.group.GroupsListTpl, null, null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'AnnouncementEditGroupsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('change', '.student-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentCheck(node, event, value_){
                node.parent('form').trigger('submit');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.AnnouncementGroupTpl)],
            VOID, function newGroup(tpl, model, msg_) {
                var newGroup = ria.dom.Dom('.new-group');
                tpl.renderTo(newGroup.setHTML(''));
                setTimeout(function(){
                    newGroup.find('.group-name').trigger('focus');
                }, 1);
            }
        ]);
});