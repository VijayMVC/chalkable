REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.SelectControl */
    CLASS(
        'SelectControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/select.jade')(this);
            },

            [[Object, Object]],
            VOID, function updateSelect(node, attributes){
                var that = this;
                var select = node.chosen({disable_search_threshold: attributes.maxLength || 1000});
                if(!attributes.multiple) {
                    select.change(function () {
                        var node = jQuery(this);
                        node.find('option[selected]').attr('selected', false);
                        var option = node.find('option[value="' + node.val() + '"]');
                        option.attr('selected', true);
                        var controller = node.data('controller');
                        if (controller) {
                            var action = node.data('action');
                            var paramsArr = node.data('params') || [];
                            var params = paramsArr.slice();
                            params.unshift(node.val());
                            var additionalParams = option.data('additional-params');
                            if (additionalParams && additionalParams.length)
                                params = params.concat(additionalParams);
                            var state = that.context.getState();
                            state.setController(controller);
                            state.setAction(action);
                            state.setParams(params);
                            state.setPublic(false);
                            that.context.stateUpdated();
                        }
                    });
                }
            },



            //TODO: make base jquery control method
            [[Object, String]],
            Object, function processAttrs(attributes, hiddenName_) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var select = jQuery('#'+attributes.id);

                        if(hiddenName_){
                            select.parent().find('[name=' + hiddenName_ + ']').val(select.val());

                            select.on('change', function(){
                                var selVal = select.val(),
                                    value = Array.isArray(selVal) ? selVal.join(',') : selVal;
                                jQuery(this).parent().find('[name=' + hiddenName_ + ']').val(value);
                            });
                        }

                        select.on('chosen:hiding_dropdown', function(){
                            var selVal = select.val();
                            if(!selVal && attributes["data-placeholder"])
                                select.find('+DIV').find('SPAN').html(attributes["data-placeholder"]);
                        });

                        select.on('chosen:showing_dropdown', function(){
                            var select = $(this);
                            var chosenEl = select.find('+DIV');
                            select.find('option').each(function(index, option){
                                var tooltip = $(option).data('tooltip');
                                if(tooltip){
                                    chosenEl.find('li:eq(' + index + ')').attr('data-tooltip', tooltip);
                                    chosenEl.find('li:eq(' + index + ')').attr('data-tooltip-type', 'overflow');
                                }
                            })
                                select.find('+DIV').find('SPAN').html(attributes["data-placeholder"]);
                        });

                        if(attributes.firstEmpty){
                            select.find('option:eq(0)').html('&nbsp;');
                        }

                        this.updateSelect(select, attributes);

                        if(!select.find('option:selected')[0] && attributes.firstEmpty && attributes["data-placeholder"]){
                            select.find('+DIV').find('SPAN').html(attributes["data-placeholder"]);
                        }

                        if(attributes.multiple && !attributes['placeholder-on-start-only'] && attributes["data-placeholder"]){
                            setTimeout(function(){
                                select.find('+DIV').find('input').attr('placeholder', attributes["data-placeholder"]);
                            }, 100);
                        }
                    }.bind(this));
                return attributes;
            }
        ]);
});