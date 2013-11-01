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

            [[Object, ClassOf(chlk.services.BaseService), String, ClassOf(ria.templates.Template), Array]],
            VOID, function initialize(attrs, service, method, tpl, prependArgs)
            {
                attrs.id = attrs.id || ria.dom.NewGID();
                var serviceIns = this.getContext().getService(service);
                var ref = ria.reflection.ReflectionClass(service);
                var methodRef = ref.getMethodReflector(method);
                var stub = function () { return methodRef.invokeOn(serviceIns, [].concat(prependArgs, ria.__API.clone(arguments))); };
                this.queueReanimation_(attrs.id, attrs["default-value"] ? attrs["default-value"].valueOf() : null, stub, tpl, attrs);
            },

            [[String, String, Function, ClassOf(ria.templates.Template), Object]],
            VOID, function queueReanimation_(id, defaultValue_, serviceF, tpl, attrs) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var node = ria.dom.Dom('#' + id);
                        if(node.exists() && !node.hasClass('search-box-processed')){
                            if (defaultValue_)
                                ria.dom.Dom('#' + id + '-hidden').setValue(defaultValue_);
                            this.reanimate_(node, serviceF, tpl, attrs, activity, model);
                            node.addClass('search-box-processed');
                        }
                    }.bind(this));
            },

            [[ria.dom.Dom, Function, ClassOf(ria.templates.Template), Object, ria.mvc.IActivity, Object]],
            VOID, function reanimate_(node, serviceF, tplClass, attrs, activity, model) {
                var id = node.getAttr("id");
                var tpl = new tplClass();

                var selectHandler = function( event, ui, triggerChange) {
                    var item = ui.item;
                    var li = jQuery(event.currentTarget).find("[data-id=" + item.dataId + "]");
                    ria.dom.Dom('#' + id + '-hidden').setValue(attrs.idValue ? item['get' + attrs.idValue.capitalize()]().valueOf() : li.data('value'));
                    node.setValue(attrs.textValue ? item['get' + attrs.textValue.capitalize()]() : li.data('title'));
                    if (triggerChange)
                        node.trigger('change', {selected: item});
                    return false;
                };

                jQuery(node.valueOf()).autocomplete({
                    minLength: 2,
                    source: function( request, response ) {
                        serviceF(request.term)
                            .then(function(data){
                                if(data instanceof chlk.models.common.PaginatedList)
                                    return data.getItems();
                                return data;
                            })
                            .then(response);
                    },
                    focus: function( event, ui ){
                        return selectHandler(event, ui, false);
                    },
                    select: function( event, ui ){
                        return selectHandler(event, ui, true);
                    },
                    change: function( event, ui ) {
                        if (!ria.dom.Dom('#' + id + '-hidden').getValue())
                            ria.dom.Dom('#' + id).setValue(null);
                    },
                    open: function( event, ui ) {
                        var menu = new ria.dom.Dom('.ui-autocomplete.ui-menu:visible');
                        if(attrs.listCls)
                            menu.addClass(attrs.listCls);
                        if(attrs.target){
                            var node = new ria.dom.Dom(attrs.target);
                            var width = node.width() + parseInt(node.getCss('padding-left'), 10)
                                + parseInt(node.getCss('padding-right'), 10);
                            menu.width(width - 4);
                            menu.setCss('top', node.offset().top + node.height()
                                + parseInt(node.getCss('padding-top'), 10)
                                + parseInt(node.getCss('padding-bottom'), 10));
                            menu.setCss('left', node.offset().left - 1);
                        }
                    }
                }).data( "ui-autocomplete" )._renderItem = function( ul, item ) {
                    var fixedInstance = ria.__API.inheritFrom(item.constructor);
                    for(var k in item) if (item.hasOwnProperty(k))
                        fixedInstance[k] = item[k];
                    tpl.assign(fixedInstance);
                    var li = jQuery.parseHTML(tpl.render());
                    var id = ria.dom.NewGID();
                    jQuery(li).attr("data-id", id);
                    item.dataId = id;
                    return jQuery(li).appendTo(ul);
                };
            }

        ]);
});
