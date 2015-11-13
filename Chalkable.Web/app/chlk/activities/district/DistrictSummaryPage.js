REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.district.DistrictSummaryTpl');

NAMESPACE('chlk.activities.district', function () {

    /** @class chlk.activities.district.DistrictSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.district.DistrictSummaryTpl)],
        'DistrictSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});