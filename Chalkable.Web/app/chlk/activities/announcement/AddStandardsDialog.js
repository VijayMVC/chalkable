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
                    var id = node.getData('id') || '';
                    var standardIds = this.dom.find('.standard-ids').getValue().split(',');
                    this.dom.find('input[name=standardid]').setValue(id);
                    var btnC = this.dom.find('.add-standard-btn');
                    var btn = btnC.find('button');
                    if(id && standardIds.indexOf(id.toString()) == -1){
                        btnC.removeClass('disabled');
                        btnC.removeAttr('disabled');
                        btn.removeAttr('disabled');
                    }else{
                        btnC.addClass('disabled');
                        btnC.setAttr('disabled', true);
                        btn.setAttr('disabled', true);
                    }
                    node.parent('td').find('~ td').remove();
                }
            },

            [ria.mvc.DomEventBind('click', '.add-standard-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                node.setAttr('disabled', true);
            }
        ]);
});