REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.school.School');
REQUIRE('chlk.templates.school.School');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.Schools*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/Schools.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Schools', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.school.School), 'items',
            [ria.templates.ModelBind],
            Number, 'pageindex',
            [ria.templates.ModelBind],
            Number, 'pagesize',
            [ria.templates.ModelBind],
            Number, 'totalcount',
            [ria.templates.ModelBind],
            Number, 'totalpages',
            [ria.templates.ModelBind],
            Boolean, 'hasnextpage',
            [ria.templates.ModelBind],
            Boolean, 'haspreviouspage'
        ])
});