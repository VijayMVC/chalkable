REQUIRE('chlk.controls.Base');
REQUIRE('chlk.services.BaseService');
REQUIRE('ria.templates.Template');


NAMESPACE('chlk.controls', function () {
    /** @class chlk.controls.SearchBoxControl */
    CLASS(
        'SearchBoxControl', EXTENDS(chlk.controls.Base), [

            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/SearchBox.jade')(this);
            },

            [[Object, ClassOf(chlk.services.BaseService), String, ClassOf(ria.templates.Template)]],
            VOID, function initialize(attrs, service, method, tpl)
            {
                attrs.id = attrs.id || ria.dom.NewGID();
                var serviceIns = this.getContext().getService(service);
                var ref = ria.reflection.ReflectionClass(service);
                var methodRef = ref.getMethodReflector(method);
                var stub = function () { return methodRef.invokeOn(serviceIns, ria.__API.clone(arguments)); };
                this.queueReanimation_(attrs.id, attrs["default-value"] ? attrs["default-value"].valueOf() : null, stub, tpl);
            },

            [[String, String, Function, ClassOf(ria.templates.Template)]],
            VOID, function queueReanimation_(id, defaultValue_, serviceF, tpl) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        if (defaultValue_)
                            ria.dom.Dom('#' + id + '-hidden').setValue(defaultValue_);
                        this.reanimate_(ria.dom.Dom('#' + id), serviceF, tpl, activity, model)
                    }.bind(this));
            },

            [[ria.dom.Dom, Function, ClassOf(ria.templates.Template), ria.mvc.IActivity, Object]],
            VOID, function reanimate_(node, serviceF, tplClass, activity, model) {
                var id = node.getAttr("id");
                var tpl = new tplClass();
                jQuery(node.valueOf()).autocomplete({
                    source: function( request, response ) {
                        serviceF(request.term)
                            .then(response);
                    },
                    focus: function() {
                        // prevent value inserted on focus
                        return false;
                    },
                    select: function( event, ui ) {
                        var li = jQuery(event.toElement).closest('li');
                        ria.dom.Dom('#' + id + '-hidden').setValue(li.data('value'));
                        node.setValue(li.data('title'));
                        console.log("1");
                        return false;
                    },
                    change: function( event, ui ) {
                        if (!ria.dom.Dom('#' + id + '-hidden').getValue())
                            ria.dom.Dom('#' + id).setValue(null);
                        console.log("2");
                    }
                }).data( "ui-autocomplete" )._renderItem = function( ul, item ) {
                    var fixedInstance = ria.__API.inheritFrom(item.constructor);
                    for(var k in item) if (item.hasOwnProperty(k))
                        fixedInstance[k] = item[k];
                    tpl.assign(fixedInstance);
                    return jQuery(jQuery.parseHTML(tpl.render())).appendTo(ul);
                };
            }

        ]);
});
