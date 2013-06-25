REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.SchoolList');

REQUIRE('chlk.templates.School');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.Schools*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/Schools.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Schools', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.School), 'items',
            [ria.templates.ModelBind],
            Number, 'count',
            [ria.templates.ModelBind],
            Number, 'pageSize',
            [ria.templates.ModelBind],
            Number, 'page'
        ])
});