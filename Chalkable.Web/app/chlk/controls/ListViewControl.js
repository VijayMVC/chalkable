REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {
    var otherInputWithFocusClass = 'with-grid-focus';

    /** @class app.controls.ListView */
    CLASS(
        'ListViewControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/list-view.jade')(this);
                this.setDefaultConfigs({
                    selectedIndex:null,
                    infiniteScroll: false,
                    itemsName: 'Items',
                    start: 0,
                    totalCount: null,
                    pageSize: 10,
                    interval: 250
                })
            },

            Object, 'configs',

            Object, 'defaultConfigs',

            [[Object, Object]],
            VOID, function prepareData(data,configs_) {
                var configs = this.getDefaultConfigs();
                if(configs_){
                    if(data.getTotalCount){
                        configs_.totalCount = data.getTotalCount();
                        configs_.pageSize = data.getPageSize();
                    }
                    configs = Object.extend(configs, configs_);
                }
                var getItemsMethod = 'get' + configs.itemsName;
                configs.selectedIndex !== undefined && this.setCurrentIndex(configs.selectedIndex);
                this.setConfigs(configs);

                this.setCount(data.length != undefined ? data.length : data[getItemsMethod]().length);
                this.context.getDefaultView().getCurrent()
                    .addRefreshCallback(function (activity, model) {
                        if(this.getCurrentIndex() !== undefined && !new ria.dom.Dom(':focus').exists())
                            this.focusGrid();
                        var grid = new ria.dom.Dom('.chlk-grid');
                        this.setGrid(grid);
                        if(configs.infiniteScroll && !grid.hasClass('with-scroller'))
                            this.addInfiniteScroll(grid);
                    }.bind(this));
            },

            [[ria.dom.Dom]],
            VOID, function addInfiniteScroll(grid) {
                grid.addClass('with-scroller');
                var baseContentHeight = grid.height(), configs = this.getConfigs();
                var pageHeight = document.documentElement.clientHeight, scrollPosition,
                    contentHeight = baseContentHeight + grid.offset().top, interval;
                configs.currentStart = configs.start + configs.pageSize;
                interval = setInterval(function(){
                    if(!grid.hasClass('scroll-freezed')){
                        scrollPosition = window.pageYOffset;
                        if(configs.totalCount > configs.currentStart){
                            if((contentHeight - pageHeight - scrollPosition) < 400){
                                var form = grid.parent('form');
                                form.find('[name=start]').setValue(configs.currentStart);
                                jQuery(grid.valueOf()).parents('form').find('.scroll-start-button').click();
                                configs.currentStart += configs.pageSize;
                                var div = new ria.dom.Dom('<div class="horizontal-loader"></div>');
                                grid.addClass('scroll-freezed');
                                grid.appendChild(div);
                                contentHeight += baseContentHeight;
                            }

                        }
                        else{
                            clearInterval(interval);
                        }
                    }
                    var node = grid.find('.horizontal-loader');
                    if(node.exists() && node.next().exists()){
                        grid.removeClass('scroll-freezed');
                        node.remove();
                    }
                }, configs.interval);
            },

            Number, 'currentIndex',
            Number, 'count',
            ria.dom.Dom, 'grid',

            [ria.mvc.DomEventBind('click', '.chlk-grid .row:not(.selected)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function rowClick(node, event) {
                var parent = node.parent('.chlk-grid');
                parent.find('.row.selected').removeClass('selected');
                node.addClass('selected');
                this.setCurrentIndex(parseInt(node.getAttr('index'), 10));
            },

            [ria.mvc.DomEventBind('keydown', '.grid-focus, .' + otherInputWithFocusClass)],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function keyArrowsClick(node, event) {
                var currentIndex = this.getCurrentIndex();
                if((event.which == 38 && currentIndex) || (event.which == 40 && currentIndex < this.getCount() - 1)){
                    var parent = node.parent('.chlk-grid');
                    parent.find('.row.selected').removeClass('selected');
                    if(event.which == 38){
                        currentIndex--;
                    }else{
                        currentIndex++;
                    }
                    parent
                        .find('.row[index="' + currentIndex + '"]')
                        .addClass('selected');
                    this.setCurrentIndex(currentIndex);
                    this.scrollToElement();
                    this.focusGrid();
                }
            },

            [ria.mvc.DomEventBind('click', '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onFocusGrid(node, event) {
                var target = new ria.dom.Dom(event.target);
                if(target.is(':not(input, textarea)')){
                    var Y = window.scrollY;
                    this.focusGrid(node);
                    window.scrollTo(0, Y);
                }
            },

            [[ria.dom.Dom]],
            VOID, function focusGrid(node_) {
                node_ = node_ && node_.valueOf()[0] ? node_ : new ria.dom.Dom('.chlk-grid');
                var focusNode = node_.find('.row.selected').find('.' + otherInputWithFocusClass);
                if(focusNode.exists()){
                    focusNode.valueOf()[0].focus()
                }else{
                    node_.find('.grid-focus').valueOf()[0].focus();
                }
            },

            VOID, function scrollToElement(){
                var el = this.getGrid().find('.row.selected');
                var demo = new ria.dom.Dom('#demo-footer');
                window.scrollTo(0, el.offset().top + el.height()/2 - (window.innerHeight - (demo.height() || 0))/2);
            }
        ]);
});