REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppMarket');
REQUIRE('chlk.templates.apps.AppMarketAppsTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppMarketPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppMarket)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppMarketAppsTpl, 'updateApps', '.apps', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppMarketAppsTpl, 'scrollApps', '.apps', ria.mvc.PartialUpdateRuleActions.Append)],
        [chlk.activities.lib.PartialUpdateClass('partial-update-market')],

        'AppMarketPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.price-type')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function togglePriceType(node, event){
                this.dom.find('.price-type').removeClass('pressed');
                node.addClass('pressed');
                this.dom.find('input[name=priceType]').setValue(node.getAttr('data-price-type'));
                this.resetScrolling_();
                this.dom.find('#app-market-filter').trigger('submit');
            },

            [ria.mvc.DomEventBind('click', '.title.btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSubjects(node, event){
                this.dom.find('.subject-filter .items').toggleClass('x-hidden');
            },

            [ria.mvc.DomEventBind('change', '#app-sort-type')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function sortingChanged(node, event, data){
                this.resetScrolling_();
                this.dom.find('#app-market-filter').trigger('submit');
            },


            [ria.mvc.DomEventBind('sliderChanged', '#gradeLevels')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradeLevelsChanged(node, event){
                this.resetScrolling_();
                this.dom.find('#app-market-filter').trigger('submit');
            },

            function resetScrolling_(){
                this.dom.find('input[name=scroll]').setValue(0);
                this.dom.find('input[name=start]').setValue(0);

            },


            [ria.mvc.DomEventBind('click', 'input[type=checkbox]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleCategories(node, event){
                var selectedType = node.getAttr('name').replace('app-category-', '');
                var checkboxes = this.dom.find('input[type=checkbox]:not(:disabled)');
                var eventName = chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf();

                var res = [];

                if (selectedType == 'all'){
                    if (checkboxes.count() > 1){
                    this.dom.find('input[type=checkbox]:not(:disabled)').filter(function(elem){
                    return elem.getAttr('name') != node.getAttr('name');
                    }).trigger(eventName, [false]);

                    checkboxes
                        .filter(function(el){
                            return !el.is(":checked");
                        })
                        .forEach(function(el){
                            res.push(el.getAttr('name').split('app-category-').pop());
                        });
                    }
                }
                else{
                    this.dom.find('input[name=app-category-all]').trigger(eventName, [false]);

                    checkboxes
                        .filter(function(el){
                            return el.is(":checked");
                        })
                        .forEach(function(el){
                            res.push(el.getAttr('name').split('app-category-').pop());
                        });
                }
                res = res.join(',');
                new ria.dom.Dom("input[name=selectedCategories]").setValue(res);
                this.resetScrolling_();


                this.dom.find('#app-market-filter').trigger('submit');


            }
        ]);
});