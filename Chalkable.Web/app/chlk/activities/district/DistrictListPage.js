REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.district.Districts');

NAMESPACE('chlk.activities.district', function () {

    /** @class chlk.activities.districts.DistrictsListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.district.Districts)],
        'DistrictListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});