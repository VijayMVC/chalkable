REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.LessonPlanCategoriesListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanCategoriesList.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'LessonPlanCategoriesListTpl', EXTENDS(chlk.templates.announcement.LessonPlanSearchTpl), [


        ])
});
