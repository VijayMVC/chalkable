REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.bgtasks.BgTask');

NAMESPACE('chlk.templates.bgtasks', function () {

    /** @class chlk.templates.bgtasks.BgTasks*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/bgtasks/BgTasks.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'BgTasks', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.bgtasks.BgTask), 'items'
        ])
});