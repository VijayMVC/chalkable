REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.district.AddDistrictDialog');

NAMESPACE('chlk.activities.district', function () {
     /** @class chlk.activities.district.AddDistrictDialog */
     CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.district.AddDistrictDialog)],
        'AddDistrictDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
