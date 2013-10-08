REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppMarketDetails');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppMarketDetailsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('app-market')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppMarketDetails)],
        'AppMarketDetailsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.write-review-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleReviewArea(node, event){
                this.dom.find('.add-review').toggleClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', '.cancel-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelReview(node, event){
                this.dom.find('.add-review').addClass('x-hidden');
                //todo: reset ratings here also
            },

            [ria.mvc.DomEventBind('click', '.review-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleRoleReviews(node, event){
                this.dom.find('.rating-roles').toggleClass('x-hidden');
                this.dom.find('.add-review').addClass('x-hidden');
            }
        ]);
});