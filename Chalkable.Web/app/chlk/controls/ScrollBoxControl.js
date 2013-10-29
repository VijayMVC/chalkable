REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.ScrollBoxControl*/
    CLASS(
        'ScrollBoxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/scroll-box.jade')(this);
            },

            Object, 'configs',

            [[Object, Object]],
            VOID, function prepareData(data, configs_) {
                var configs = {
                    itemsName: 'Items',
                    start: 0,
                    totalCount: null,
                    pageSize: 10,
                    interval: 250
                };
                if(configs_){
                    if(data.getTotalCount){
                        configs_.totalCount = data.getTotalCount();
                        configs_.pageSize = data.getPageSize();
                    }
                    configs = Object.extend(configs, configs_);
                }
                var getItemsMethod = 'get' + configs.itemsName;
                this.setConfigs(configs);
                this.context.getDefaultView().getCurrent()
                    .addRefreshCallback(function (activity, model, msg_) {
                        var scrollBox = new ria.dom.Dom('.chlk-scrollbox');
                        this.addInfiniteScroll(scrollBox);
                    }.bind(this));
            },

            [[ria.dom.Dom]],
            VOID, function addInfiniteScroll(scrollBox) {
                var configs = this.getConfigs();
                var baseContentHeight = scrollBox.height();
                var pageHeight = document.documentElement.clientHeight;
                var contentHeight = baseContentHeight + scrollBox.offset().top;
                var scrollPosition = 0;
                configs.currentStart = configs.start + configs.pageSize;

                var onScrollDown = function(){
                    if(!scrollBox.hasClass('scroll-freezed')){
                        scrollPosition = window.pageYOffset;
                        if(configs.totalCount > configs.currentStart){
                            if((contentHeight - pageHeight - scrollPosition) < 200){

                                var form = new ria.dom.Dom("#" + configs.formId);

                                form.find('[name=scroll]').setValue(1);
                                form.trigger('submit');

                                configs.currentStart += configs.pageSize;
                                form.find('[name=start]').setValue(configs.currentStart);
                                var div = new ria.dom.Dom('<div class="horizontal-loader"></div>');

                                scrollBox.addClass('scroll-freezed');
                                scrollBox.appendChild(div);
                                contentHeight += baseContentHeight;
                            }
                        }
                        else{
                            clearInterval(interval);
                        }
                    }
                    var node = scrollBox.find('.horizontal-loader');
                    if(node.exists()){
                        scrollBox.removeClass('scroll-freezed');
                        node.remove();
                    }
                };
                var interval = setInterval(onScrollDown, configs.interval);
            }

        ]);
});