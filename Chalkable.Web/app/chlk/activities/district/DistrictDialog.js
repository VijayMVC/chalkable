REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.district.DistrictDialog');

NAMESPACE('chlk.activities.district', function () {
     /** @class chlk.activities.district.AddDistrictDialog */
     CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.district.DistrictDialog)],
        'DistrictDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
