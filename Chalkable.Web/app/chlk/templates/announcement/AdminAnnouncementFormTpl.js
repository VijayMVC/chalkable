REQUIRE('chlk.templates.announcement.BaseAnnouncementFormTpl');
REQUIRE('chlk.models.announcement.AnnouncementForm');

NAMESPACE('chlk.templates.announcement', function(){
    "use strict";
    /**@class chlk.templates.announcement.AdminAnnouncementFormTpl*/

    ASSET('~/assets/jade/activities/announcement/BaseAnnouncementForm.jade')();

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AdminAnnouncementForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm)],
        'AdminAnnouncementFormTpl', EXTENDS(chlk.templates.announcement.BaseAnnouncementFormTpl),[

            [ria.templates.ModelPropertyBind],
            Array, 'recipients'
        ]);
});