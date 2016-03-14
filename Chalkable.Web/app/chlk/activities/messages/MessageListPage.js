REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.messages.MessageList');

NAMESPACE('chlk.activities.messages', function () {

    /** @class chlk.activities.messages.MessageListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('messages')],
        [ria.mvc.TemplateBind(chlk.templates.messages.MessageList)],
        'MessageListPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '#delete-button, #mark-read-button, #mark-unread-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function changeMessagesState(node, event){
                var s = "";
                var first = true;
                this.dom.find('[name=ch]:checked:visible').forEach(
                    function(element)
                    {
                        if (first)
                            first = false;
                        else
                            s += ",";
                        s += element.getData('id');
                    }
                );
                this.dom.find('[name=selectedIds]').setValue(s);
                return true;
            },

            [ria.mvc.DomEventBind('change', '#checkboxall')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function checkAll(node, event){
                this.dom.find('[name=ch]:visible').forEach(
                    function(element)
                    {
                        element.valueOf()[0].checked = node.is(":checked");
                        element.setAttr("checked", node.is(":checked"));
                    }
                );
            },

            [ria.mvc.DomEventBind('click', '.unread .msg-info')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function clickUnRead(node, event){
                var unReadNode = node.parent();
                if(unReadNode.hasClass('unread') && unReadNode.getData('can-read'))
                    unReadNode.removeClass('unread').addClass('read');
            },



            [ria.mvc.DomEventBind('keypress', '.search-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function SearchPress(node, event) {
                if (event.keyCode == ria.dom.Keys.ENTER) {
                    this.submitForm_(node);
                }
            },

            [ria.mvc.DomEventBind('change', '.year-select')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function SelectYear(node, event, options) {
                this.submitForm_(node);
            },

            [[ria.dom.Dom]],
            VOID, function submitForm_(childNode){
                childNode.parent('form').trigger('submit');
            }
        ]);
});