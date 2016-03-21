REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppGeneralTpl');
REQUIRE('chlk.templates.developer.AppReviewsTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppGeneralInfoPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppGeneralTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.developer.AppReviewsTpl, 'loadReviews', '.rating-persons', ria.mvc.PartialUpdateRuleActions.Append)],
        'AppGeneralInfoPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});