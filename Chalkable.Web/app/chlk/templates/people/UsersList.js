REQUIRE('chlk.models.people.UsersList');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.UsersList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersList.jade')],
        [ria.templates.ModelBind(chlk.models.people.UsersList)],
        'UsersList', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'users',
            [ria.templates.ModelPropertyBind],
            Number, 'selectedIndex',
            [ria.templates.ModelPropertyBind],
            Boolean, 'byLastName',
            [ria.templates.ModelPropertyBind],
            String, 'filter',
            [ria.templates.ModelPropertyBind],
            Number, 'start'
        ])
});
