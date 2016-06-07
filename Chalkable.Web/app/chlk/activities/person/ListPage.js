REQUIRE('chlk.activities.person.PersonGrid');
REQUIRE('chlk.templates.teacher.StudentsList');

NAMESPACE('chlk.activities.person', function () {

    /** @class chlk.activities.person.ListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.teacher.StudentsList)],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersGridTpl, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        //[ria.mvc.PartialUpdateRule(chlk.templates.people.UsersForGridTpl, chlk.activities.lib.DontShowLoader(), '.people-list', ria.mvc.PartialUpdateRuleActions.Append)],
        'ListPage', EXTENDS(chlk.activities.person.PersonGrid), [

            Boolean, 'messagingDisabled',

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(model instanceof chlk.models.people.UsersList){
                    var totalNode = this.dom.find('.total-count');
                    var oldText = totalNode.getHTML();
                    var newText = model.getUsers().getTotalCount() + oldText.slice(oldText.indexOf(' '));
                    totalNode.setHTML(newText);
                }
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                this.setMessagingDisabled(model.getUsersList().isMessagingDisabled() || false);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.people.UsersForGridTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function usersForGridAppend(tpl, model, msg_) {
                tpl.options({
                    messagingDisabled: this.isMessagingDisabled()
                });
                var grid = this.dom.find('.people-list');
                ria.dom.Dom(tpl.render()).appendTo(grid);
                grid.trigger(chlk.controls.GridEvents.UPDATED.valueOf());
                setTimeout(function(){
                    if(!model.getItems().length)
                        this.dom.find('#people-list-form').trigger(chlk.controls.FormEvents.DISABLE_SCROLLING.valueOf());
                }.bind(this), 1);
            }
        ]);
});