REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassExplorerTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsForExplorerListTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassExplorerPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassExplorerTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsForExplorerListTpl, 'apps', '.suggested-apps-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassExplorerPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '.more-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function moreStandardsClick(node, event){
                node.parent().find('.hidden-item, .less-button').setCss('display', 'block');
                node.hide();
            },

            [ria.mvc.DomEventBind('click', '.less-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function lessStandardsClick(node, event){
                node.parent().find('.hidden-item').hide();
                node.parent().find('.more-button').show();
                var item = this.dom.find('.block-item.standard.pressed');
                if(item.exists() && item.hasClass('hidden-item')){
                    this.dom.find('.suggested-apps-container').setHTML('');
                    item.removeClass('pressed');
                }

                node.hide();
            },

            [ria.mvc.DomEventBind('click', '.standard:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function standardClick(node, event){
                this.dom.find('.standard.pressed').removeClass('pressed');
                setTimeout(function(){
                    node.addClass('pressed');
                }, 1);
                var top = node.offset().top - node.parent('.explorer-view').offset().top - (node.height() + 2 * (parseInt(node.getCss('border-width'), 10) || 1)) / 2;
                this.dom.find('.suggested-apps-container').setCss('margin-top', top);
            }
        ]);
});