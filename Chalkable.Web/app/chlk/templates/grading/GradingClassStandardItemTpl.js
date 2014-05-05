REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradingClassStandardItem');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassStandardItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/MpGradingSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassStandardItem)],
        'GradingClassStandardItemTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.NameId, 'itemDescription',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.BaseAnnouncementViewData), 'announcements',
            [ria.templates.ModelPropertyBind],
            Number, 'percent',
            [ria.templates.ModelPropertyBind],
            Number, 'avg',
            [ria.templates.ModelPropertyBind],
            Number, 'index',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId'
        ]);
});
