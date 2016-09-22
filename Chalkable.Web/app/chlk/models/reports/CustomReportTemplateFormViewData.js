REQUIRE('chlk.models.reports.CustomReportTemplate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.CustomReportTemplateFormViewData*/
    CLASS(
        'CustomReportTemplateFormViewData', [

            chlk.models.reports.CustomReportTemplate, 'reportTemplate',

            ArrayOf(chlk.models.reports.CustomReportTemplate), 'headers',

            ArrayOf(chlk.models.reports.CustomReportTemplate), 'footers',

            [[chlk.models.reports.CustomReportTemplate,
                ArrayOf(chlk.models.reports.CustomReportTemplate),
                ArrayOf(chlk.models.reports.CustomReportTemplate)]],
            function $(template, headers, footers){
                BASE();
                if(template)
                    this.setReportTemplate(template);
                if(headers)
                    this.setHeaders(headers);
                if(footers)
                    this.setFooters(footers);
            }
        ]);
});
