REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.ScrollBoxControl*/
    CLASS(
        'ScrollBoxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/scroll-box.jade')(this);
            },

            String, 'formId',

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.setFormId(attributes.formId);
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                    }.bind(this));
                return attributes;
            },

            [[Object]],
            VOID, function update(node){
                var formId = this.getFormId();
                var that = this;
                jQuery(window).scroll(function(){
                    if  (jQuery(window).scrollTop() == jQuery(document).height() - jQuery(window).height()){
                        jQuery('#' + formId).parent().find('.scrollbox-loader').removeClass('x-hidden');
                        jQuery('#' + formId).find('input[name=scroll]').val(1);
                        var totalCount = jQuery('#' + formId).find('input[name=totalCount]').val() | 0;

                        var start = jQuery('#' + formId).find('input[name=start]').val() | 0;
                        var pageSize = jQuery('#' + formId).find('input[name=pageSize]').val() | 0;

                        if (start < totalCount){
                            jQuery('#' + formId).find('input[name=start]').val(start + pageSize);
                            jQuery('#' + formId).trigger('submit');
                        }
                        else{
                            jQuery('#' + formId).parent().find('.scrollbox-loader').addClass('x-hidden');
                            return false;
                        }
                    }
                });
            }
        ]);
});