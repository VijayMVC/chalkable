REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.school.UpgradeDistrictsViewData');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.UpgradeDistrictsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/UpgradeDistrictsList.jade')],
        [ria.templates.ModelBind(chlk.models.school.UpgradeDistrictsViewData)],
        'UpgradeDistrictsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'districts',

            [ria.templates.ModelPropertyBind],
            String, 'filter',
        ]);
});
