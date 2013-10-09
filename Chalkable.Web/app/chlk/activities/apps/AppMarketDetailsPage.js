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
                this.dom.find('.rating-hint').hide();
            },

            [ria.mvc.DomEventBind('click', '.cancel-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelReview(node, event){
                this.dom.find('.add-review').addClass('x-hidden');
                this.dom.find('.rating-hint').hide();
                this.dom.find('.review-text').setValue('');
                this.dom.find('[name=newRating]:checked').setAttr('checked', false);
            },

            [ria.mvc.DomEventBind('click', '.submit-review-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitReview(node, event){
                var rating = this.dom.find('[name=newRating]:checked').getValue();
                if (!rating){
                    this.dom.find('.rating-hint').show();
                    return;
                }
                //todo: disable button and submit form
            }
        ]);
});