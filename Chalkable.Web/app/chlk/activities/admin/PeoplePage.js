REQUIRE('chlk.activities.person.PersonGrid');
REQUIRE('chlk.templates.people.UsersForGridTpl');
REQUIRE('chlk.templates.people.UsersGridTpl');
REQUIRE('chlk.templates.admin.SchoolPersonListTpl');

NAMESPACE('chlk.activities.admin', function (){
    "use strict";
    /**@class chlk.activities.admin.PeoplePage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.admin.SchoolPersonListTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersGridTpl, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersForGridTpl, chlk.activities.lib.DontShowLoader(), '.people-list', ria.mvc.PartialUpdateRuleActions.Append)],
        'PeoplePage', EXTENDS(chlk.activities.person.PersonGrid), [

            [ria.mvc.DomEventBind('change', '[name="rolesId"], [name="gradeLevelsIds"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function roleSelect(node, event) {
                var form = this.dom.find('#people-list-form');
                form.find('input[name="start"]').setValue(0);
                form.trigger('submit');
            }
    ]);
});