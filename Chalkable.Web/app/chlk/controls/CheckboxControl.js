REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.CheckBoxEvents */
    ENUM('CheckBoxEvents', {
        CHANGE_VALUE: 'changevalue'
    });

    /** @class chlk.controls.CheckboxControl */
    CLASS(
        'CheckboxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/checkbox.jade')(this);
            },

            [[Object, Boolean]],
            Object, function prepareData(attributes, value) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.addEvents(attributes.id, value, activity.getDom());
                    }.bind(this));
                if(attributes.addCallBack){
                    setTimeout(function(){
                        this.addEvents(attributes.id, value);
                    }.bind(this), 100)
                }
                return attributes;
            },

            [[String, Object, ria.dom.Dom]],
            function addEvents(id, value, dom_){
                dom_ = dom_ || new ria.dom.Dom();
                var hidden = dom_.find('#' + id + '-hidden');
                var node = dom_.find('#' + id);
                node.off('change.check');
                node.off('click.check');
                node.on('click.check', function(node, event){
                    if(node.getAttr('readonly'))
                        event.preventDefault();
                });
                node.on('change.check', function(node, event){
                    if(node.getAttr('readonly'))
                        return false;
                    var lastValue = hidden.getData('value');
                    var newValue = !lastValue;
                    node.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [newValue]);
                });
                hidden.setData('value', value || false);
            },

            [ria.mvc.DomEventBind(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), '.checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Boolean]],
            VOID, function changeValue(node, event, value) {
                var hidden = node.parent().find('.hidden-checkbox');
                hidden.setData('value', value);
                hidden.setValue(value);
                var parent = node.parent('.slide-checkbox');
                if(value){
                    node.setProp('checked', true);
                    parent.setProp('checked', true);
                    node.setAttr('checked', true);
                    parent.setAttr('checked', true);
                }else{
                    node.setProp('checked', false);
                    parent.setProp('checked', false);
                    node.removeAttr('checked');
                    parent.removeAttr('checked');
                }
            },

            [ria.mvc.DomEventBind('change', '.checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Boolean]],
            VOID, function checkBoxChange(node, event, value_) {
                if(!node.checked())
                    node.removeClass('partially-checked');
            },

            [[ria.dom.Dom, Object]],
            VOID, function SET_VALUE(dom, value) {
                dom.setValue(value);
                var node = dom.parent().find('.hidden-checkbox');
                node.setValue(value);
                node.setData('value', value);
            }
        ]);
});