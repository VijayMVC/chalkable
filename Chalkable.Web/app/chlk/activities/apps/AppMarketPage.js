REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppMarket');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppMarketPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppMarket)],

        'AppMarketPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.price-type')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function togglePriceType(node, event){
                this.dom.find('.price-type').removeClass('pressed');
                node.addClass('pressed');
            },

            [ria.mvc.DomEventBind('click', '.title.btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSubjects(node, event){
                this.dom.find('.subject-filter .items').toggleClass('x-hidden');
            }
        ]);
});