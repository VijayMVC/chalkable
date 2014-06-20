REQUIRE('chlk.activities.profile.SchoolPersonInfoPage');
REQUIRE('chlk.templates.profile.StudentInfoPageTpl');
REQUIRE('chlk.templates.people.Addresses');

NAMESPACE('chlk.activities.profile', function () {
    "use strict";
    /** @class chlk.activities.profile.StudentInfoPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.Addresses, '', '.adresses', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.profile.StudentInfoPageTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.profile.StudentInfoPageTpl)],

        'StudentInfoPage', EXTENDS(chlk.activities.profile.SchoolPersonInfoPage), [
            //todo parents
        ]);
});