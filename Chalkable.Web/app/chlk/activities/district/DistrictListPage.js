REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.district.Districts');

NAMESPACE('chlk.activities.district', function () {

    /** @class chlk.activities.districts.DistrictsListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.district.Districts)],
        'DistrictListPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});