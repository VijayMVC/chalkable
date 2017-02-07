REQUIRE('chlk.templates.reports.BaseAttendanceReportTpl');
REQUIRE('chlk.models.feed.FeedPrintingViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.FeePrintingReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/FeedPrintingReport.jade')],
        [ria.templates.ModelBind(chlk.models.feed.FeedPrintingViewData)],
        'FeePrintingReportTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'minStart',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'maxEnd',

            [ria.templates.ModelPropertyBind],
            Boolean, 'lessonPlanOnly',

            [ria.templates.ModelPropertyBind],
            Boolean, 'includeDetails',

            [ria.templates.ModelPropertyBind],
            Boolean, 'includeAttachments',

            [ria.templates.ModelPropertyBind],
            Boolean, 'includeHiddenActivities',

            [ria.templates.ModelPropertyBind],
            Boolean, 'includeHiddenAttributes',

            [ria.templates.ModelPropertyBind],
            Boolean, 'groupByStandards',

            [ria.templates.ModelPropertyBind],
            Boolean, 'editableLPOption',

            [ria.templates.ModelPropertyBind],
            Boolean, 'importantOnly',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            function getTooltip(){
                if(!this.isEditableLPOption()){
                    var res = "The feed is configured to only show ";

                    if(this.getAnnouncementType() == chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN)
                        return res + "lesson plans";

                    if(this.getAnnouncementType() == chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT)
                        return res + "activities";
                }

                return null;
            }
        ])
});