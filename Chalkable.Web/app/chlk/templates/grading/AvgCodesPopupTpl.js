REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.AvgCodesPopupViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.AvgCodesPopupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/AvgCodesPopup.jade')],
        [ria.templates.ModelBind(chlk.models.grading.AvgCodesPopupViewData)],
        'AvgCodesPopupTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AvgCodeHeaderViewData), 'headers',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AvgComment), 'comments',

            [ria.templates.ModelPropertyBind],
            String, 'studentName',

            [ria.templates.ModelPropertyBind],
            String, 'gradingPeriodName',

            [ria.templates.ModelPropertyBind],
            Number, 'averageId',

            [ria.templates.ModelPropertyBind],
            Number, 'rowIndex'
        ])
});