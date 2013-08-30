REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.search.SearchItem');

NAMESPACE('chlk.templates.search', function () {

    /** @class chlk.templates.search.SiteSearch*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/sidebars/SiteSearch.jade')],
        [ria.templates.ModelBind(chlk.models.search.SearchItem)],
        'SiteSearch', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            Number, 'searchType',

            [ria.templates.ModelPropertyBind],
            Number, 'roleId',

            [ria.templates.ModelPropertyBind],
            String, 'gender'
        ])
});

