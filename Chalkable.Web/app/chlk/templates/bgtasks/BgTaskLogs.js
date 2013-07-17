REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.bgtasks.BgTaskLog');

NAMESPACE('chlk.templates.bgtasks', function () {

    /** @class chlk.templates.bgtasks.BgTaskLogs*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/bgtasks/BgTaskLogs.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'BgTaskLogs', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.bgtasks.BgTaskLog), 'items'
        ])
});