REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.common.InfoMsg');

NAMESPACE('chlk.activities.common', function () {
     /** @class chlk.activities.common.InfoMsgDialog */
     CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.common.InfoMsg)],
        'InfoMsgDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [
            [[Object]],
            OVERRIDE, VOID, function onRender_(data) {
                 BASE(data);
                 var node = this.dom.find('.centered');
                 node.setCss('margin-top', -parseInt(node.height()/2, 10) + 'px');
                 node.setCss('margin-left', -parseInt(node.width()/2, 10) + 'px');
             },


            [ria.mvc.DomEventBind('click', 'button, a')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onBtnClick(node, event) {
                this.close();
                event.preventDefault();
            }
         ]);
 });
