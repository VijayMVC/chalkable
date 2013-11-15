REQUIRE('chlk.templates.announcement.BaseAnnouncementFormTpl');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.classes.ClassesForTopBar');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    ASSET('~/assets/jade/activities/announcement/BaseAnnouncementForm.jade')();

    /** @class chlk.templates.announcement.AnnouncementFormTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm)],
        'AnnouncementFormTpl', EXTENDS(chlk.templates.announcement.BaseAnnouncementFormTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassForWeekMask, 'classInfo'
        ]);
});