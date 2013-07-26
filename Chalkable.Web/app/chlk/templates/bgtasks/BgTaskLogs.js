REQUIRE('chlk.models.bgtasks.BgTask');
REQUIRE('chlk.models.bgtasks.BgTasksLogListViewData');
REQUIRE('chlk.models.id.BgTaskId');

NAMESPACE('chlk.templates.bgtasks', function () {

    /** @class chlk.templates.bgtasks.BgTaskLogs*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/bgtasks/BgTaskLogs.jade')],
        [ria.templates.ModelBind(chlk.models.bgtasks.BgTasksLogListViewData)],
        'BgTaskLogs', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'items',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.BgTaskId, 'bgTaskId'
        ])
});