REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.AddStandardsTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddStandardsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        //[chlk.activities.lib.BodyClass('left-0')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AddStandardsTpl)],
        'AddStandardsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('click', '.column-cell')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cellClick(node, event){
                if(!node.hasClass('pressed')){
                    this.dom.find('.column-cell.pressed').removeClass('pressed');
                    node.addClass('pressed');
                    this.dom.find('.standards-description').setHTML(node.getData('description'));
                }
            }
        ]);
});