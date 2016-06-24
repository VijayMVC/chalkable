REQUIRE('chlk.templates.announcement.AnnouncementFormTpl');
REQUIRE('chlk.models.announcement.LessonPlanForm');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    /** @class chlk.templates.announcement.LessonPlanDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanDialog.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm)],
        'LessonPlanDialogTpl', EXTENDS(chlk.templates.announcement.AnnouncementFormTpl), [

        ]);
});