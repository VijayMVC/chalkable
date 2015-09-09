REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppMarketDetails');
REQUIRE('chlk.templates.apps.AppMarketReviewsTpl');
REQUIRE('chlk.templates.apps.AppMarketBanTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppMarketDetailsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('app-market')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppMarketDetails)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppMarketReviewsTpl, 'updateReviews', '.reviews', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppMarketBanTpl, 'banApp', '.ban-app', ria.mvc.PartialUpdateRuleActions.Replace)],
        'AppMarketDetailsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                this.refreshInlineTabsState_();
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);

                if(msg_ == 'updateReviews' && model instanceof chlk.models.apps.AppMarketApplication){
                    this.dom.find('.side-panel')
                            .find('input[value=' + model.getApplicationRating().getAverage() + ']').setAttr('checked', true);
                }

                this.refreshInlineTabsState_();
            },

            [ria.mvc.DomEventBind('click', '.write-review-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleReviewArea(node, event){
                this.dom.find('.add-review').toggleClass('x-hidden');
                this.dom.find('.rating-hint').hide();
            },

            [ria.mvc.DomEventBind('click', '.more-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function moreButtonCliack(node, event){
                node.remove();
                this.dom.find('.hidden-row').removeClass('hidden-row');
            },

            [ria.mvc.DomEventBind('click', '.submit-review-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submitReview(node, event){
                var rating = this.dom.find('[name=newRating]:checked').getValue();
                if (!rating){
                    this.dom.find('.rating-hint').show();
                    return false;
                }
                return true;
            },

            /*[ria.mvc.DomEventBind('click', '.review-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleRoleReviews(node, event){
                 this.dom.find('.rating-roles').toggleClass('x-hidden');
            },*/

            OVERRIDE, VOID, function onPause_() {
                BASE();
                this.dom.find('iframe[wmode]').addClass('x-hidden');
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this.dom.find('iframe[wmode]').removeClass('x-hidden');
            },

            VOID, function refreshInlineTabsState_() {
                this.dom.find('.inline-tabs').forEach(function ($tab) {
                    var activeTab = $tab.find('.tab.active[data-tab-content]').first();

                    if (!activeTab.exists())
                        activeTab = $tab.find('.tab[data-tab-content]').first();

                    if (activeTab.exists()) {
                        this.onTabClick(activeTab, null);
                    }
                }.bind(this))
            },

            [ria.mvc.DomEventBind('click', '.tab[data-tab-content]:not(.active)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onTabClick($node, event) {
                var $parent = $node.parent().parent();

                Assert($parent.hasClass('inline-tabs'));

                var content = $node.getData('tab-content');
                $parent.find('.active')
                    .filter(function ($node) { return $node.getData('tab-content') != content; })
                    .removeClass('active');

                $parent.find('[data-tab-content=' + content + ']').addClass('active');

                return false;
            }
        ]);
});
