REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.templates.announcement.LessonPlanFormTpl');
REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AnnouncementTitleTpl');
REQUIRE('chlk.templates.SuccessTpl');
REQUIRE('chlk.templates.standard.AnnouncementStandardsTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsListTpl');

NAMESPACE('chlk.activities.announcement', function () {

    /** @class chlk.activities.announcement.LessonPlanFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.LessonPlanFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.AnnouncementStandardsTpl, '', '.standards-list' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsListTpl, '', '.suggested-apps-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'LessonPlanFormPage', EXTENDS(chlk.activities.announcement.AnnouncementFormPage), [

        ]
    );
});