REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.school.SchoolPeople');
REQUIRE('chlk.templates.people.UsersList');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolPeoplePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.school.SchoolPeople)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersList, '', '.people-list-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'SchoolPeoplePage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('change', '#roleId')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function roleSelect(node, event) {
                this.dom.find('#people-form').triggerEvent('submit');
            },

            [ria.mvc.DomEventBind('change', '#gradeLevelId')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradeLevelSelect(node, event) {
                this.dom.find('#people-form').triggerEvent('submit');
            },

            [ria.mvc.DomEventBind('click', '.action-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function actionButtonClick(node, event) {
                var value = node.getData('value');
                this.dom.find('input[name="byLastName"]').setValue(value || '');
                this.dom.find('#people-form').triggerEvent('submit');
            }
        ]);
});