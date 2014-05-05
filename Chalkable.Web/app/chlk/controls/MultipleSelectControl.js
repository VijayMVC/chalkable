REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.MultipleSelectControl */
    CLASS(
        'MultipleSelectControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/multiple-select.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.chlk-multiple-select>a')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function selectClick(node, event) {
                node.parent().toggleClass('selected');
            },

            [ria.mvc.DomEventBind('change', '.chlk-multiple-select-list > input[type=hidden]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function selectChange(node, event) {
                var arr = [];
                node.parent()
                    .find('.labeled-checkbox')
                    .find('input[type=checkbox]:checked')
                    .next()
                    .forEach(function(node){
                        arr.push(node.getHTML());
                    });
                var select = node.parent('.chlk-multiple-select');
                var div = select.find('>a >.chlk-multiple-select-text');
                var text = arr.length ? arr.join(',') : div.getData('placeholder');
                var suffix = select.getData('suffix-if-one');
                if(suffix && arr.length == 1)
                    text += suffix;
                div.setHTML(text);
            }
        ]);
});