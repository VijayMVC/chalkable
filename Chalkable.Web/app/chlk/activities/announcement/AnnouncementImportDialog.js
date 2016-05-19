REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.AnnouncementImportTpl');
REQUIRE('chlk.templates.announcement.AnnouncementImportItemsTpl');

NAMESPACE('chlk.activities.announcement', function(){

    var currentGradeLevel;

    /**@class chlk.activities.announcement.AnnouncementImportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementImportTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementImportItemsTpl, 'list-update', '.items-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'AnnouncementImportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('change', '.all-tasks-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function allTasksSelect(node, event, selected_){
                this.dom.find('.announcement-item-check').forEach(function(element){
                    element.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [node.is(':checked')]);
                });

                this.updateSubmitBtn_();
            },

            [ria.mvc.DomEventBind('change', '.announcement-item-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function feedItemSelect(node, event, selected_){
                this.updateSubmitBtn_();
            },

            [ria.mvc.DomEventBind('change', '.copy-to-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function classSelect(node, event, selected_){
                this.dom.find('.list-update-btn').trigger('click');
            },

            function updateSubmitBtn_(){
                var btn = this.dom.find('.import-btn');
                if(this.dom.find('.announcement-item-check:checked').count() > 0){
                    btn.removeAttr('disabled');
                    btn.setProp('disabled', false);
                }else{
                    btn.setAttr('disabled', 'disabled');
                    btn.setProp('disabled', true);
                }
            }

        ]);
});