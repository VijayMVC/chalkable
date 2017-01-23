REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.CustomReportTemplateId');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.CustomReportTemplateType*/

    ENUM('CustomReportTemplateType', {
        BODY: 1,
        HEADER: 2,
        FOOTER: 3
    });

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.reports.CustomReportTemplate*/
    CLASS(
        'CustomReportTemplate', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.CustomReportTemplateId, 'id',
            String, 'name',
            String, 'layout',
            String, 'style',
            Object, 'icon',
            chlk.models.id.CustomReportTemplateId, 'headerId',
            chlk.models.id.CustomReportTemplateId, 'footerId',
            Boolean, 'availableHeader',
            Boolean, 'availableFooter',
            chlk.models.reports.CustomReportTemplateType, 'type',

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.CustomReportTemplateId);
                this.name = SJX.fromValue(raw.name, String);
                this.layout = SJX.fromValue(raw.layout, String);
                this.style = SJX.fromValue(raw.style, String);
                this.icon = raw.icon;
                this.headerId = SJX.fromValue(raw.headerid, chlk.models.id.CustomReportTemplateId);
                this.footerId = SJX.fromValue(raw.footerid, chlk.models.id.CustomReportTemplateId);
                this.availableHeader = SJX.fromValue(raw.hasheader, Boolean);
                this.availableFooter = SJX.fromValue(raw.hasfooter, Boolean);
                this.type = SJX.fromValue(raw.type, chlk.models.reports.CustomReportTemplateType);
            }
        ]);
});
