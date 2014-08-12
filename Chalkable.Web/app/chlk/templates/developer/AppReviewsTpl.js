REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.apps.AppGeneralInfoViewData');

NAMESPACE('chlk.templates.developer', function () {

    /** @class chlk.templates.developer.AppReviewsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/developer/appReviews.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppGeneralInfoViewData)],
        'AppReviewsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'appReviews'
        ])
});



