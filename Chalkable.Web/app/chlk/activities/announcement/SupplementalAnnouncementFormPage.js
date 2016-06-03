REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.templates.announcement.SupplementalAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AnnouncementTitleTpl');
REQUIRE('chlk.templates.SuccessTpl');
REQUIRE('chlk.templates.standard.AnnouncementStandardsTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAttributesTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsListTpl');

NAMESPACE('chlk.activities.announcement', function () {

    var titleTimeout;

    /** @class chlk.activities.announcement.SupplementalAnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.SupplementalAnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.SupplementalAnnouncementFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.AnnouncementStandardsTpl, '', '.standards-list' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsListTpl, '', '.suggested-apps-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'SupplementalAnnouncementFormPage', EXTENDS(chlk.activities.announcement.AnnouncementFormPage), [
            [ria.mvc.DomEventBind('submit', '.announcement-form>FORM')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var value = this.dom.find('.recipients-select').getValue();
                var ids = value ? this.dom.find('.recipients-select').getValue().join(',') : '';
                this.dom.find('.recipient-ids').setValue(ids);
            }
        ]
    );
});