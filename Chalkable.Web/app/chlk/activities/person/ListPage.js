REQUIRE('chlk.activities.person.PersonGrid');
REQUIRE('chlk.templates.teacher.StudentsList');

NAMESPACE('chlk.activities.person', function () {

    /** @class chlk.activities.person.ListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.teacher.StudentsList)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersGrid, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersForGrid, window.noLoadingMsg, '.people-list', ria.mvc.PartialUpdateRuleActions.Append)],
        'ListPage', EXTENDS(chlk.activities.person.PersonGrid), [
            /*[ria.mvc.DomEventBind('click', '.first-last')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function classClick(node, event){
                var classId = node.getAttr('classId');
                this.dom.find('input[name=classid]').setValue(classId);
            }*/
        ]);
});