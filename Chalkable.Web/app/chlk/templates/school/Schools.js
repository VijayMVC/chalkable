REQUIRE('chlk.models.school.SchoolListViewData');
REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.DistrictId');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.Schools*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/Schools.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolListViewData)],
        'Schools', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'items',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.DistrictId, 'districtId'
        ])
});