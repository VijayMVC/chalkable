REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.district.District');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.Districts*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/Districts.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Districts', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.district.District), 'items'
        ])
});