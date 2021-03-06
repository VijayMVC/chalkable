REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.school.SchoolSisInfo');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolSisPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.school.SchoolSisInfo)],
        'SchoolSisPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});