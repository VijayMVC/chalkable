REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.LessonPlanViewData');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.LessonPlanAutoCompleteTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanAutoComplete.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.LessonPlanViewData)],
        'LessonPlanAutoCompleteTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'title'
    ]);
});