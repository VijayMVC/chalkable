REQUIRE('chlk.templates.district.BaseStatisticTpl');
REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolTeachersStatisticTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolTeachersSummaryGrid.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'SchoolTeachersStatisticTpl', EXTENDS(chlk.templates.district.BaseStatisticTpl), [

        ])
});