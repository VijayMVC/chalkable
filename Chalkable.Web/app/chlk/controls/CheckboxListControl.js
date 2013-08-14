REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.CheckboxListControl */
    CLASS(
        'CheckboxListControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/checkbox-list.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.checkbox-list')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onClicked($target, node){
                var checkboxes = $target.find('input[type=checkbox]');
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
                new ria.dom.Dom("input[name=" + name + "]").setValue(res);
            }
        ]);
});