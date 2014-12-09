REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.student.StudentProfileExplorerTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsForExplorerListTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileExplorerPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileExplorerTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsForExplorerListTpl, 'apps', '.suggested-apps-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'StudentProfileExplorerPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '.more-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function moreStandardsClick(node, event){
                node.parent().find('.hidden-item').removeClass('hidden-item');
                node.remove();
            },

            [ria.mvc.DomEventBind('click', '.with-code.standard:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function standardClick(node, event){
                this.dom.find('.standard.pressed').removeClass('pressed');
                setTimeout(function(){
                    node.addClass('pressed');
                }, 1);
                var top = node.offset().top - node.parent('.explorer-view').offset().top - (node.height() + 2 * parseInt(node.getCss('border-width'), 10)) / 2;
                this.dom.find('.suggested-apps-container').setCss('margin-top', top);
            }
        ]);
});