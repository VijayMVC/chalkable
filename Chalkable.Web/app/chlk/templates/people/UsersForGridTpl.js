REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.UsersForGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersForGrid.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'UsersForGridTpl', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'items',

            Boolean, 'hasAccess',

            [[chlk.models.people.User]],
            Boolean, function canViewInfo(user){
                var currentRole = this.getUserRole();
                var currentUser = this.getCurrentUser();
                return !currentRole || !currentUser || currentUser.getId() == user.getId()
                    || currentRole.isTeacher() || currentRole.isAdmin();
            }
        ])
});
