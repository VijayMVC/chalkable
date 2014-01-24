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
            String, 'mustScrollField',
            String, 'startField',
            String, 'pageSizeField',
            String, 'actualCountField',

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.setFormId(attributes.formId);
                this.setMustScrollField(attributes.mustScrollField || 'scroll');
                this.setPageSizeField(attributes.pageSizeField || 'pageSize');
                this.setActualCountField(attributes.actualCountField || 'actualCount');
                this.setStartField(attributes.startField || 'start');

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
                var scrollField = 'input[name=' + this.getMustScrollField() + ']';
                var pageSizeField = 'input[name=' + this.getPageSizeField() + ']';
                var actualCountField = 'input[name=' + this.getActualCountField() + ']';
                var startField = 'input[name=' + this.getStartField() + ']';

                //todo: add ability min result actualCount < pageSize don't download anything more
                jQuery(window).scroll(function(){
                    if  (jQuery(window).scrollTop() == jQuery(document).height() - jQuery(window).height()){
                        jQuery('#' + formId).parent().find('.scrollbox-loader').removeClass('x-hidden');
                        jQuery('#' + formId).find(scrollField).val(1);

                        var start = jQuery('#' + formId).find(startField).val() | 0;
                        var pageSize = jQuery('#' + formId).find(pageSizeField).val() | 0;

                        if (actualCount == pageSize){
                            jQuery('#' + formId).find(startField).val(start + pageSize);
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