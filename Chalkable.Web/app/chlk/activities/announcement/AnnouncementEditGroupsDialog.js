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

            [ria.mvc.DomEventBind('keyup', '.group-name')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function groupNameKeyUp(node, event){
                node.parent('.group-container').find('.create-group').setProp('disabled', !node.getValue() || node.getValue() == node.getData('value'));
            },

            [ria.mvc.DomEventBind('blur', '.group-name')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function groupNameBlur(node, event){
                node.parent('.group-container').find('.create-group').fadeOut();
            },

            [ria.mvc.DomEventBind('focus', '.group-name')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function groupNameFocus(node, event){
                node.parent('.group-container').find('.create-group').show();
            },

            [ria.mvc.DomEventBind('click', '.delete-btn:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function deleteClick(node, event){
                if(!node.hasClass('action-link'))
                    node.parent('.group-container').fadeOut();
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.AnnouncementGroupTpl)],
            VOID, function newGroup(tpl, model, msg_) {
                var newGroup = ria.dom.Dom('.new-group');
                tpl.setAnnouncementId(model.getAnnouncementId());
                tpl.renderTo(newGroup.setHTML(''));
                setTimeout(function(){
                    newGroup.find('.group-name').trigger('focus');
                }, 1);
            }
        ]);
});