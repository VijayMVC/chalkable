REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.school.SchoolPeople');
REQUIRE('chlk.templates.people.UsersListTpl');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolPeoplePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.school.SchoolPeople)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersListTpl, '', '.people-list-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'SchoolPeoplePage', EXTENDS(chlk.activities.lib.TemplatePage), [
            /*[ria.mvc.DomEventBind('change', '#roleId')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function roleSelect(node, event) {
                this.dom.find('#people-form').trigger('submit');
            },

            [ria.mvc.DomEventBind('change', '#gradeLevelId')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradeLevelSelect(node, event) {
                this.dom.find('#people-form').trigger('submit');
            },*/

            [ria.mvc.DomEventBind('change', '[name="rolesId"], [name="gradeLevelsIds"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function roleSelect(node, event) {
                var form = this.dom.find('#people-list-form');
                form.find('input[name="start"]').setValue(0);
                form.trigger('submit');
            },

            [ria.mvc.DomEventBind('click', '.action-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function actionButtonClick(node, event) {
                var value = node.getData('value');
                this.dom.find('input[name="byLastName"]').setValue(value || '');
                this.dom.find('#people-list-form').trigger('submit');
            }
        ]);
});