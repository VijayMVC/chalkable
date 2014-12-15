REQUIRE('chlk.activities.person.PersonGrid');
REQUIRE('chlk.templates.teacher.StudentsList');

NAMESPACE('chlk.activities.person', function () {

    /** @class chlk.activities.person.ListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.teacher.StudentsList)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersGridTpl, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersForGridTpl, chlk.activities.lib.DontShowLoader(), '.people-list', ria.mvc.PartialUpdateRuleActions.Append)],
        'ListPage', EXTENDS(chlk.activities.person.PersonGrid), [

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(model instanceof chlk.models.people.UsersList){
                    var totalNode = this.dom.find('.total-count');
                    var oldText = totalNode.getHTML();
                    var newText = model.getUsers().getTotalCount() + oldText.slice(oldText.indexOf(' '));
                    totalNode.setHTML(newText);
                }
            }
        ]);
});