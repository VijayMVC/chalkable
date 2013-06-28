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

    function serializeForm(form) {
        var elements = form.elements || [];

        return ria.__API.clone(elements)
            .filter(function(_) {
                var type = this.type;
                // Use .is(":disabled") so that fieldset[disabled] works
                return this.name && !ria.dom.Dom(this).is( ":disabled" ) &&
                    rsubmittable.test( this.nodeName ) && !rsubmitterTypes.test( type ) &&
                    ( this.checked || !manipulation_rcheckableType.test( type ) );
            })
            .map(function(elem) {
                var val = valueOfElement(elem);

                return val == null ?
                    null :
                    jQuery.isArray( val ) ?
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

            /*function $() {
                jQuery('BODY')
                .on('submit', 'FORM', this.submit.asJQueryEventHandler())
                .on('click', 'FORM input[type=submit]', function(e) {
                    var $target = jQuery(e.target),
                        $form = $target.closest('FORM');

                    $form.attr('data-submit-name', $target.attr('name'));
                    $form.attr('data-submit-value', $target.val());
                });
            },*/

            [ria.mvc.DomEventBind('submit', 'FORM')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submit_($target, event) {
                if ($target.hasClass('disabled'))
                    return false;

                var controller = $target.getData('controller');
                var action = $target.getData('action');

                if (controller || action) {
                    try {
                        var state = this.context.getState();
                        if (controller)
                            state.setController(controller);

                        if (action)
                            state.setAction(action);

                        state.setPublic(!!$target.getData('public'));

                        var p = serializeForm($target.valueOf().shift());
                        var params = {};
                        p.forEach(function (o) { params[o.name] = o.value; });

                        var name = $target.getData('submit-name');
                        var value = $target.getData('submit-value');

                        if (name) {
                            params[name] = value;
                        }

                        $target.setAttr('data-submit-name', null);
                        $target.setAttr('data-submit-value', null);

                        state.setParams([params]);

                        this.context.stateUpdated();
                    } catch (e) {
                        setTimeout(function () { throw e; }, 1);
                    }
                    return false;
                }

                return true;
            }
        ])
});