REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.UsersForGrid*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersForGrid.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'UsersForGrid', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'items'
        ])
});
