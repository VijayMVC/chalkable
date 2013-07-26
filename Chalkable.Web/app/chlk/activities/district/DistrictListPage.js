REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.district.Districts');

NAMESPACE('chlk.activities.district', function () {

    /** @class chlk.activities.districts.DistrictsListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.district.Districts)],
        'DistrictListPage', EXTENDS(ria.mvc.TemplateActivity), []);
});