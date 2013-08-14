REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.profile.InfoView');
REQUIRE('chlk.templates.people.Addresses');

NAMESPACE('chlk.activities.profile', function () {

    var serializer = new ria.serialize.JsonSerializer();

    /** @class chlk.activities.profile.InfoViewPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.profile.InfoView)],
        'InfoViewPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});