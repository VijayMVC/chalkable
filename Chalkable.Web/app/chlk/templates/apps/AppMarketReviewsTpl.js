REQUIRE('chlk.models.apps.AppMarketApplication');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarketReviewsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppMarketReviews.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketApplication)],
        'AppMarketReviewsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppRating, 'applicationRating',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId,  'id'

        ])
});