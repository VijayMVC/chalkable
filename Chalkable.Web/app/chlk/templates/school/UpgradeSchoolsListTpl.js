REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.school.SchoolListViewData');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.UpgradeSchoolsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/UpgradeSchoolsList.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolListViewData)],
        'UpgradeSchoolsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'paginatedSchools',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.DistrictId, 'districtId'
        ]);
});
