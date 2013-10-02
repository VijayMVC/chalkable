REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.grading.GradingClassSummaryItem');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassSummaryItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/MpGradingSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryItem)],
        'GradingClassSummaryItemTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementType, 'type',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.Announcement), 'announcements',
            [ria.templates.ModelPropertyBind],
            Number, 'percent',
            [ria.templates.ModelPropertyBind],
            Number, 'avg',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId'
        ]);
});
