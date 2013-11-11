REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.CheckboxListControl */
    CLASS(
        'CheckboxListControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/checkbox-list.jade')(this);
            },

            [ria.mvc.DomEventBind('change', '.checkbox-list input[type=checkbox]')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onChange(node, event){
                var $target = node.parent('.checkbox-list');
                var checkboxes = $target.find('input[type=checkbox]:not(:disabled)');
                var res = [];
                var prefix = $target.getData('prefix');
                var name = $target.getData('name');
                checkboxes
                    .filter(function(el){
                        return el.is(":checked");
                    })
                    .forEach(function(el){
                        res.push(el.getAttr('name').split(prefix).pop());
                    });
                res = res.join(',');
                var newNode = $target.find("input[name=" + name + "]");
                if(! newNode.exists())
                    newNode = new ria.dom.Dom("input[name=" + name + "]");
                newNode.setValue(res);
            },

            [ria.mvc.DomEventBind('click', '.checkbox-list label[for]')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onClicked(node, event){
                node.parent().find('input[type="checkbox"]').trigger('click');
                return false;
            }
        ]);
});