REQUIRE('chlk.templates.announcement.AnnouncementFormTpl');
REQUIRE('chlk.models.announcement.LessonPlanForm');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    /** @class chlk.templates.announcement.LessonPlanFormTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.LessonPlanForm)],
        'LessonPlanFormTpl', EXTENDS(chlk.templates.announcement.AnnouncementFormTpl), []);
});