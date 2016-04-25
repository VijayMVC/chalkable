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
            String, 'pageIndexField',
            String, 'actualCountField',
            String, 'totalCountField',
            Number, 'scrollOffset',

            function $(){
                BASE();
            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.setFormId(attributes.formId);
                this.setMustScrollField(attributes.mustScrollField || 'scroll');
                this.setPageSizeField(attributes.pageSizeField || 'pageSize');
                this.setActualCountField(attributes.actualCountField || 'actualCount');
                this.setStartField(attributes.startField || 'start');
                this.setTotalCountField(attributes.totalCountField || 'totalCount');
                this.setScrollOffset(attributes.scrollOffset || 100);

                this.setPageIndexField(attributes.pageIndexField || 'pageIndex');

                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                    }.bind(this));


                this.context.getDefaultView().getCurrent()
                    .addRefreshCallback(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                }.bind(this));
                return attributes;
            },

            [[Object]],
            VOID, function update(node){
                var formId = this.getFormId();
                var scrollField = 'input[name=' + this.getMustScrollField() + ']';
                var pageSizeField = 'input[name=' + this.getPageSizeField() + ']';
                var pageIndexField = 'input[name=' + this.getPageIndexField() + ']';
                var startField = 'input[name=' + this.getStartField() + ']';
                var totalCountField = 'input[name=' + this.getTotalCountField() + ']';
                var actualCountField = 'input[name=' + this.getActualCountField() + ']';
                var scrollOffset = this.getScrollOffset();

                jQuery(window).scroll(function(){
                    var jForm = jQuery('#' + formId);
                    if  ((jQuery(window).scrollTop() + scrollOffset >= jQuery(document).height() - jQuery(window).height())){
                        jForm.find(scrollField).val(1);
                        var start = jForm.find(startField).val() | 0;
                        var pageSize = jForm.find(pageSizeField).val() | 0;
                        var totalCount = jForm.find(totalCountField).val() | 0;
                        var actualCount = jForm.find(actualCountField).val() | 0;
                        var pageIndex = jForm.find(pageIndexField).val() | 0;

                        if ((start < totalCount && actualCount == pageSize)){
                            jForm.find(startField).val((pageIndex + 1) * pageSize);
                            jForm.find(pageIndexField).val(pageIndex + 1);
                            jForm.trigger('submit');
                            jForm.find(startField).val(start);
                            jForm.find(pageIndexField).val(pageIndex);
                        }
                        else{
                            return false;
                        }

                    }
                }.bind(this));
            }
        ]);
});