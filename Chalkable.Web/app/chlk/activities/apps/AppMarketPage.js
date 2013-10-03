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
            },

            [ria.mvc.DomEventBind('click', 'input[type=checkbox]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleCategories(node, event){
                var selectedType = node.getAttr('name').replace('app-category-', '');
                var checkboxes = this.dom.find('input[type=checkbox]:not(:disabled)');

                var res = [];

                if (selectedType == 'all'){
                    this.dom.find('input[type=checkbox]:not(:disabled)').filter(function(elem){
                        return elem.getAttr('name') != node.getAttr('name');
                    }).setAttr('checked', false);

                    checkboxes
                        .filter(function(el){
                            return !el.is(":checked");
                        })
                        .forEach(function(el){
                            res.push(el.getAttr('name').split('app-category-').pop());
                        });
                }
                else{
                    this.dom.find('input[name=app-category-all]').setAttr('checked', false);


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
            }
        ]);
});