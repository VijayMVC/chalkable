REQUIRE('chlk.templates.people.UsersList');

NAMESPACE('chlk.templates.people', function () {
    "use strict";
    /** @class chlk.templates.people.UsersGrid*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersGrid.jade')],
        [ria.templates.ModelBind(chlk.models.people.UsersList)],
        'UsersGrid', EXTENDS(chlk.templates.people.UsersList), [])
});
