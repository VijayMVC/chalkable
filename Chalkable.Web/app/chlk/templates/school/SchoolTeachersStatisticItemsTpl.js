REQUIRE('chlk.templates.district.BaseStatisticTpl');
REQUIRE('chlk.models.admin.BaseStatisticGridViewData');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolTeachersStatisticItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolTeachersSummaryGridItems.jade')],
        [ria.templates.ModelBind(chlk.models.admin.BaseStatisticGridViewData)],
        'SchoolTeachersStatisticItemsTpl', EXTENDS(chlk.templates.district.BaseStatisticTpl), [
        ])
});