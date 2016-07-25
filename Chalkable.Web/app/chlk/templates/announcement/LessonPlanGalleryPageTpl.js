REQUIRE('chlk.templates.announcement.LessonPlanGalleryDialogTpl');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    /** @class chlk.templates.announcement.LessonPlanGalleryPageTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanGalleryPage.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.LessonPlanGalleryViewData)],
        'LessonPlanGalleryPageTpl', EXTENDS(chlk.templates.announcement.LessonPlanGalleryDialogTpl), []);
});