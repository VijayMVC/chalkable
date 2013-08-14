REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.UserForGrid*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersForGrid.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'UsersForGrid', EXTENDS(chlk.templates.PaginatedList), [])
});
