REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

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
                dom_.find('#' + id).on('change', function(){
                    var lastValue = hidden.getData('value');
                    var newValue = !lastValue;
                    hidden.setData('value', newValue);
                    hidden.setValue(newValue);
                });
                hidden.setData('value', value || false);
            }
        ]);
});