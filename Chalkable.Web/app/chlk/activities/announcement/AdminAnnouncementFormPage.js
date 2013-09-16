REQUIRE('chlk.activities.announcement.BaseAnnouncementFormPage');

REQUIRE('chlk.templates.announcement.AdminAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.AnnouncementReminder');
REQUIRE('chlk.templates.announcement.LastMessages');


NAMESPACE('chlk.activities.announcement', function(){
    "use strict";
    /**@class chlk.activities.announcement.AdminAnnouncementFormPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AdminAnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementReminder, '', '.reminders', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, '', '.attachments-and-applications', ria.mvc.PartialUpdateRuleActions.Replace)],
//        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AdminAnnouncementFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'AdminAnnouncementFormPage', EXTENDS(chlk.activities.announcement.BaseAnnouncementFormPage),[

            //todo add recipient events

    ]);
});