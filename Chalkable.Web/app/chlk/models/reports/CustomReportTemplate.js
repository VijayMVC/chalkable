REQUIRE('chlk.models.id.CustomReportTemplateId');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.CustomReportTemplateType*/

    ENUM('CustomReportTemplateType', {
        BODY: 1,
        HEADER: 2,
        FOOTER: 3
    });

    /** @class chlk.models.reports.CustomReportTemplate*/
    CLASS(
        'CustomReportTemplate', [
            chlk.models.id.CustomReportTemplateId, 'id',
            String, 'name',
            String, 'layout',
            String, 'style',
            Object, 'icon',

            [ria.serialize.SerializeProperty('hasheader')],
            Boolean, 'availableHeader',

            [ria.serialize.SerializeProperty('hasfooter')],
            Boolean, 'availableFooter',

            chlk.models.reports.CustomReportTemplateType, 'type'
        ]);
});
