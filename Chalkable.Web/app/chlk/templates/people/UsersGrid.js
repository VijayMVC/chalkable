REQUIRE('chlk.templates.people.UsersList');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.UsersList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersGrid.jade')],
        [ria.templates.ModelBind(chlk.models.people.UsersList)],
        'UsersGrid', EXTENDS(chlk.templates.people.UsersList), [])
});
