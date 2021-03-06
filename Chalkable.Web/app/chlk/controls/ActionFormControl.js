REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var r20 = /%20/g,
        rbracket = /\[\]$/,
        rCRLF = /\r?\n/g,
        rreturn = /\r/g,
        rsubmitterTypes = /^(?:submit|button|image|reset)$/i,
        manipulation_rcheckableType = /^(?:checkbox|radio)$/i,
        rsubmittable = /^(?:input|select|textarea|keygen)/i;

    function isNodeName( elem, name ) {
        return elem.nodeName && elem.nodeName.toLowerCase() === name.toLowerCase();
    }

    var valHooks = {
        option: function( elem ) {
            // attributes.value is undefined in Blackberry 4.7 but
            // uses .value. See #6932
            var val = elem.attributes.value;
            return !val || val.specified ? elem.value : elem.text;
        },

        select: function( elem ) {
            var value, option,
                options = elem.options,
                index = elem.selectedIndex,
                one = elem.type === "select-one" || index < 0,
                values = one ? null : [],
                max = one ? index + 1 : options.length,
                i = index < 0 ?
                    max :
                    one ? index : 0;

            // Loop through all the selected options
            for ( ; i < max; i++ ) {
                option = options[ i ];

                // IE6-9 doesn't update selected after form reset (#2551)
                if ( ( option.selected || i === index ) &&
                    // Don't return options that are disabled or in a disabled optgroup
                    ( option.getAttribute("disabled") === null ) &&
                    ( !option.parentNode.disabled || !isNodeName( option.parentNode, "optgroup" ) ) ) {

                    // Get the specific value for the option
                    value = valueOfElement( option );

                    // We don't need an array for one selects
                    if ( one ) {
                        return value;
                    }

                    // Multi-Selects return an array
                    values.push( value );
                }
            }

            return values;
        }
    };

    function valueOfElement( elem ) {
        var ret;
        var hooks = valHooks[ elem.type ] || valHooks[ elem.nodeName.toLowerCase() ];
        if ( hooks && (ret = hooks( elem, "value" )) !== undefined ) {
            return ret;
        }
        ret = elem.value;
        return typeof ret === "string" ?
            // handle most common string cases
            ret.replace(rreturn, "") :
            // handle cases where value is null/undef or number
            ret == null ? "" : ret;
    }

    function isArray( obj ) {
        return toString.call(obj) === "[object Array]";
    }

    /** @class chlk.controls.ActionFormControl */
    CLASS(
        'ActionFormControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/action-form.jade')(this);
            },

            [ria.mvc.DomEventBind('click', 'FORM BUTTON')],
            [[ria.dom.Dom, ria.dom.Event]],
            function buttonClicked($target, event) {
                if ($target.hasClass('disabled') || $target.hasAttr('disabled'))
                    return false;
            },

            [ria.mvc.DomEventBind('click', 'FORM [type=submit]')],
            [[ria.dom.Dom, ria.dom.Event]],
            function submitClicked($target, event) {
                var $form = $target.parent('FORM');

                if ($target.hasClass('disabled') || $target.hasAttr('disabled'))
                    return false;

                $form.setData('submit-name', $target.getAttr('name'));
                $form.setData('submit-value', $target.getValue() || $target.getAttr('value'));
                $form.setData('submit-skip', $target.hasClass('validate-skip'));
                $form.setData('submit-no-update', $target.getData('no-update'));
            },

            [ria.mvc.DomEventBind('submit', 'FORM')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submit($target, event) {
                $target.removeClass('cancelled-submit');
                if ($target.hasClass('disabled') || $target.hasClass('working'))
                    return false;
                var controller = $target.getData('controller');
                if (controller) {

                    var $form = jQuery($target.valueOf()[0]);

                    if(!$target.getData('submit-skip') && (this.isOnlySubmitValidate() || $form.attr('onlySubmitValidate'))) {
                        $form.validationEngine('attach', {
                            onSuccess: function(form, status){
                                $form.validationEngine('detach');
                            }
                        });
                    }

                    if($target.getData('submit-skip') || $form.validationEngine('validate')) {
                        var action = $target.getData('action');
                        var p = jQuery($form.valueOf()).serializeArray();
                        var params = {};
                        p.forEach(function (o) { params[o.name] = o.value; });
                        $target.find('[type=file]').forEach(function(item){
                            params[item.getAttr('name')] = item.valueOf()[0].files;
                        });

                        Object.keys(params).forEach(function (key) {
                            var path = key.split('.');
                            if (path.length < 2) return;

                            var p = params;
                            do {
                                var root = path.shift();
                                var def = isNaN(parseInt(path[0], 10)) ? {} : [];
                                p = p[root] = p[root] || def;
                            } while (path.length > 1);

                            p[path.shift()] = params[key];
                            delete params[key];
                        });

                        var name = $target.getData('submit-name');
                        var value = $target.getData('submit-value');
                        var noWorking = $target.getData('submit-no-update');

                        if (name) {
                            params[name] = value;
                        }

                        $target.setData('submit-name', null);
                        $target.setData('submit-value', null);
                        $target.setData('submit-no-update', null);
                        $target.setData('submit-type', value);

                        if(!$target.hasClass('no-working') && !noWorking)
                            $target.addClass('working');

                        var isPublic = !!$target.getData('public');
                        setTimeout(function () {
                            var state = this.context.getState();
                            state.setController(controller);
                            state.setAction(action || null);
                            state.setParams([params]);
                            state.setPublic(isPublic);
                            this.context.stateUpdated();
                        }.bind(this), 1);
                    }else{
                        $target.addClass('cancelled-submit');
                    }

                    return false;
                }

                return true;
            },

            Boolean, 'onlySubmitValidate',

            [[Object]],
            VOID, function prepareData(attributes){
                attributes.id = attributes.id || ria.dom.Dom.GID();
                if(attributes.onlySubmitValidate){
                    this.setOnlySubmitValidate(true);
                }else{
                    this.context.getDefaultView()
                        .onActivityRefreshed(function (activity, model) {
                            var form = jQuery('#' + attributes.id);
                            form.validationEngine('attach', {
                                onFieldSuccess: function(field){
                                    if (field !== undefined)
                                        field.parent().addClass('validate-ok');
                                },
                                onFieldFailure: function(field){
                                    if (field !== undefined)
                                        field.parent().removeClass('validate-ok');
                                }
                            });
                        }.bind(this));
                }
            }
        ])
});