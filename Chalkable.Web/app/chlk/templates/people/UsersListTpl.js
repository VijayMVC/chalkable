REQUIRE('chlk.models.people.UsersList');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.UsersListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersList.jade')],
        [ria.templates.ModelBind(chlk.models.people.UsersList)],
        'UsersListTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'users',
            [ria.templates.ModelPropertyBind],
            Number, 'selectedIndex',
            [ria.templates.ModelPropertyBind],
            Boolean, 'byLastName',
            [ria.templates.ModelPropertyBind],
            String, 'filter',
            [ria.templates.ModelPropertyBind],
            Number, 'start',
            Boolean, 'my',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasAccess',

            OVERRIDE, chlk.models.common.Role, function getUserRole(){
                return this.getModel().getCurrentUserRole();
            },

            OVERRIDE, chlk.models.people.User, function getCurrentUser(){
                return this.getModel().getCurrentUser();
            },

            String, function getRoleText(){
                if(this.getUserRole().isStudent() && !this.isMy())
                    return Msg.Teacher(users.getTotalCount()!=1);
                else
                    return Msg.Student(users.getTotalCount()!=1);
            },

            String, function getTotalText(){
                var users = this.getUsers();
                var res = users.getTotalCount() + ' ' + this.getRoleText();
                return res;
            }
        ])
});
