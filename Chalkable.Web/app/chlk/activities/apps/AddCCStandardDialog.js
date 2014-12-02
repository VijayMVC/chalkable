REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AddCCStandardsTpl');
REQUIRE('chlk.templates.standard.CCStandardListTpl');

NAMESPACE('chlk.activities.apps', function(){

    /**@class chlk.activities.apps.AddCCStandardDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.CCStandardListTpl, '', '.standards-row', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.TemplateBind(chlk.templates.apps.AddCCStandardsTpl)],
        'AddCCStandardDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('click', '.column-cell')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function cellClick(node, event){
                if(node.hasClass('active'))
                    return false;
                node.parent('.column').find('.column-cell.active').removeClass('active');
                node.addClass('active');
                var id = node.getData('id') || '';
                var standardsIdsNode = this.dom.find('.standard-ids');
                var value = standardsIdsNode.getValue();
                var standardIds = value ? value.split(',') : [];
                //this.dom.find('input[name=standardid]').setValue(id);
                var btnC = this.dom.find('.add-standard-btn');
                var btn = btnC.find('button');
                if(id && standardIds.indexOf(id.toString()) == -1){
                    btnC.removeClass('disabled');
                    btnC.removeAttr('disabled');
                    btn.removeAttr('disabled');
                    standardIds.push(id);
                    standardsIdsNode.setValue(standardIds.join(','));
                }else{
                    btnC.addClass('disabled');
                    btnC.setAttr('disabled', true);
                    btn.setAttr('disabled', true);
                }
                node.parent('td').find('~ td').remove();
                return true;
            },

            [ria.mvc.DomEventBind('click', '.add-standard-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                node.setAttr('disabled', true);
            }
        ]);
});