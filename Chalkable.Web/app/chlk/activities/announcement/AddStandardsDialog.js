REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.AddStandardsTpl');
REQUIRE('chlk.templates.standard.StandardsListTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddStandardsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsListTpl, '', '.standards-row', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AddStandardsTpl)],
        'AddStandardsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('click', '.column-cell')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cellClick(node, event){
                if(!node.hasClass('active')){
                    node.parent('.column').find('.column-cell.active').removeClass('active');
                    node.addClass('active');
                    node.parent('td').find('~ td').remove();
                }
            }
        ]);
});