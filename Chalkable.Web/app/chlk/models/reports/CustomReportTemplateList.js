REQUIRE('chlk.models.reports.CustomReportTemplate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";
    /** @class chlk.models.reports.CustomReportTemplateList*/
    CLASS(
        'CustomReportTemplateList', [
            ArrayOf(chlk.models.reports.CustomReportTemplate), 'templates',

            [[ArrayOf(chlk.models.reports.CustomReportTemplate)]],
            function $(templates_){
                BASE();
                if(templates_)
                    this.setTemplates(templates_);
            }
        ]);
});
