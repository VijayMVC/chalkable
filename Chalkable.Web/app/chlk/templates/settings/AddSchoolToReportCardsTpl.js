REQUIRE('chlk.models.settings.AddSchoolToReportCardsViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.AddSchoolToReportCardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/AddSchoolToReportCards.jade')],
        [ria.templates.ModelBind(chlk.models.settings.AddSchoolToReportCardsViewData)],
        'AddSchoolToReportCardsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'schools',

            [ria.templates.ModelPropertyBind],
            String, 'requestId',

            [ria.templates.ModelPropertyBind],
            Array, 'excludeIds'
        ])
});