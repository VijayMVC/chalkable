REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.common.InfoMsg');

NAMESPACE('chlk.activities.common', function () {
    /** @class chlk.activities.common.InfoMsgDialog */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.common.InfoMsg)],
        [ria.mvc.ActivityGroup('InfoMessage')],
        'InfoMsgDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.DomEventBind('click', 'button, a')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onBtnClick(node, event) {
                this.close();
                event.preventDefault();
                this.find('input[name=button]').setValue(node.getData('value') || null);
            },


            [ria.mvc.DomEventBind('keyup', 'input[type=text]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onTextKeyUp(node, event) {

                var disabled = (node.getValue() || '').length == 0;
                this.find('button, a')
                    .filter(function(item) { return item.getData('value'); })
                    .toggleClass('disabled', disabled)
                    .toggleAttr('disabled', disabled);
            },



            OVERRIDE, Object, function getModalResult() {
                var buttonValue = this.getButtonValue_();
                return buttonValue ? this.getInputValue_() || buttonValue : null;
            },

            Object, function getButtonValue_() {
                return this.dom.find('input[name=button]').getValue();
            },

            Object, function getInputValue_() {
                return this.dom.find('input[name=value]').getValue();
            }
        ]);
 });
