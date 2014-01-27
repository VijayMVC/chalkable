REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppGeneral');
REQUIRE('chlk.templates.developer.AppReviewsTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppGeneralInfoPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppGeneral)],
        [ria.mvc.PartialUpdateRule(chlk.templates.developer.AppReviewsTpl, 'loadReviews', '.rating-persons', ria.mvc.PartialUpdateRuleActions.Append)],
        [chlk.activities.lib.PartialUpdateClass('partial-update-general-info')],
        'AppGeneralInfoPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});