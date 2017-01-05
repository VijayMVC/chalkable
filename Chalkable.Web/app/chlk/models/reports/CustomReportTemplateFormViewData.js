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
            function $(template_, headers_, footers_){
                BASE();
                if(template_)
                    this.setReportTemplate(template_);
                if(headers_)
                    this.setHeaders(headers_);
                if(footers_)
                    this.setFooters(footers_);
            }
        ]);
});
