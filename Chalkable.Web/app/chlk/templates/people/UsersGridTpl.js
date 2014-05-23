REQUIRE('chlk.templates.people.UsersListTpl');

NAMESPACE('chlk.templates.people', function () {
    "use strict";
    /** @class chlk.templates.people.UsersGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersGrid.jade')],
        [ria.templates.ModelBind(chlk.models.people.UsersList)],
        'UsersGridTpl', EXTENDS(chlk.templates.people.UsersListTpl), [
            String, 'rolesText'
        ])
});
