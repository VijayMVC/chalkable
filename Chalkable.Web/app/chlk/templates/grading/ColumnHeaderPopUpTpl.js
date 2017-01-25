REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.SubmitDroppedAnnouncementViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.ColumnHeaderPopUpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/ColumnHeaderPopUpTpl.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.SubmitDroppedAnnouncementViewData)],
        'ColumnHeaderPopUpTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'dropped',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementTypeGradingId, 'categoryId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'standardId',
        ]);
});