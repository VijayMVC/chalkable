REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var r20 = /%20/g,
        rbracket = /\[\]$/,
        rCRLF = /\r?\n/g,
        rreturn = /\r/g,
        rsubmitterTypes = /^(?:submit|button|image|reset|file)$/i,
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

    function serializeForm(form) {
        var elements = form.elements || [];

        return ria.__API.clone(elements)
            .filter(function(_) {
                var type = _.type;
                // Use .is(":disabled") so that fieldset[disabled] works
                return _.name && !ria.dom.Dom(_).is( ":disabled" ) &&
                    rsubmittable.test( _.nodeName ) && !rsubmitterTypes.test( type ) &&
                    ( _.checked || !manipulation_rcheckableType.test( type ) );
            })
            .map(function(elem) {
                var val = valueOfElement(elem);

                return val == null ?
                    null :
                    isArray( val ) ?
                        val.map(function(val){
                            return { name: elem.name, value: val.replace( rCRLF, "\r\n" ) };
                        }) :
                    { name: elem.name, value: val.replace( rCRLF, "\r\n" ) };
            });
    }

    /** @class chlk.controls.ActionFormControl */
    CLASS(
        'ActionFormControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/action-form.jade')(this);
            },

            [ria.mvc.DomEventBind('click', 'FORM [type=submit]')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submitClicked_($target, event) {
                var $form = $target.parent('FORM');

                $form.setData('submit-name', $target.getAttr('name'));
                $form.setData('submit-value', $target.getValue());
                $form.setData('submit-skip', $target.hasClass('validate-skip'))
            },

            [ria.mvc.DomEventBind('submit', 'FORM')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submit_($target, event) {
                if ($target.hasClass('disabled'))
                    return false;
                var controller = $target.getData('controller');
                if (controller) {

                    var $form = jQuery($target.valueOf()[0]);

                    if(!$target.getData('submit-skip')) {
                        $form.validationEngine('attach', {
                            onSuccess: function(form, status){
                                $form.validationEngine('detach');
                            }
                        });
                    }

                    if($form.validationEngine('validate')) {
                        var action = $target.getData('action');
                        var p = serializeForm($target.valueOf().shift());
                        var params = {};
                        p.forEach(function (o) { params[o.name] = o.value; });

                        /*var dataNames = $target.getData('names');
                        dataNames.forEach(function (name) { params[name] = $target.getData(name)});*/

                        var name = $target.getData('submit-name');
                        var value = $target.getData('submit-value');

                        if (name) {
                            params[name] = value;
                        }

                        $target.setData('submit-name', null);
                        $target.setData('submit-value', null);

                        var isPublic = !!$target.getData('public');
                        setTimeout(function () {
                            var state = this.context.getState();
                            state.setController(controller);
                            state.setAction(action || null);
                            state.setParams([params]);
                            state.setPublic(isPublic);
                            this.context.stateUpdated();
                        }.bind(this), 1);
                    }

                    return false;
                }

                return true;
            },

            [[Object]],
            VOID, function prepareData(attributes){
                var formSelector = attributes.id ? '#' + attributes.id : (attributes.class ? '.' + attributes.class : 'form');
                /*this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        jQuery(formSelector, activity.getDom().valueOf()).validationEngine();
                    }.bind(this));*/
            }
        ])
});