REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.district.AddDistrictDialog');

NAMESPACE('chlk.activities.district', function () {
     /** @class chlk.activities.district.AddDistrictDialog */
     CLASS(
        [ria.mvc.ActivityGroup('AddDistrict')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [chlk.activities.lib.BindTemplate(chlk.templates.district.AddDistrictDialog)],
        'AddDistrictDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
