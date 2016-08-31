REQUIRE('chlk.models.id.CustomReportTemplateId');

NAMESPACE('chlk.models.reports', function () {
    "use strict";
    /** @class chlk.models.reports.CustomReportTemplate*/
    CLASS(
        'CustomReportTemplate', [
            chlk.models.id.CustomReportTemplateId, 'id',
            String, 'name',
            String, 'layout',
            String, 'style',
            Object, 'icon'
        ]);
});
