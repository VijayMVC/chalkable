REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.LessonPlanSearchTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanSearch.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'LessonPlanSearchTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.CategoryViewData), 'categories',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.LessonPlanViewData, 'lessonPlanData',

            [ria.templates.ModelPropertyBind],
            String, 'filter',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.LpGalleryCategoryId, 'galleryCategoryForSearch'

        ])
});
