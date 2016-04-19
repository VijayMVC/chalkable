REQUIRE('chlk.templates.district.BaseStatisticTpl');
REQUIRE('chlk.models.admin.BaseStatisticGridViewData');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolTeachersStatisticTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolTeachersSummaryGrid.jade')],
        [ria.templates.ModelBind(chlk.models.admin.BaseStatisticGridViewData)],
        'SchoolTeachersStatisticTpl', EXTENDS(chlk.templates.district.BaseStatisticTpl), [
        ])
});