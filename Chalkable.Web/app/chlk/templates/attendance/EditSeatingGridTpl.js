REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.EditSeatingGridViewData');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.EditSeatingGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/EditSeatingGrid.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.EditSeatingGridViewData)],
        'EditSeatingGridTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'columns',

            [ria.templates.ModelPropertyBind],
            Number, 'rows',

            [ria.templates.ModelPropertyBind],
            Number, 'minColumns',

            [ria.templates.ModelPropertyBind],
            Number, 'minRows',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            String, 'seatingChartInfo'
        ]);
});