REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {
    var otherInputWithFocusClass = 'with-grid-focus';
    var selectedRowClass = 'selected';
    var noRowClickEventClass = 'no-row-click';

    /** @class chlk.controls.GridEvents */
    ENUM('GridEvents', {
        DESELECT_ROW: 'deselectrow',
        SELECT_ROW: 'selectrow',
        AFTER_ROW_SELECT: 'afterrowselect',
        FOCUS: 'focus',
        KEY_DOWN: 'keydown',
        SELECT_NEXT_ROW: 'selectnextrow',
        SELECT_PREV_ROW: 'selectprevrow'
    });

    /** @class chlk.controls.ListView */
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
                this.setConfigs(configs);
                this.setGrid(null);

                this.setCount(data.length != undefined ? data.length : data[getItemsMethod]().length);
                this.context.getDefaultView().getCurrent()
                    .addRefreshCallback(function (activity, model) {
                        if(!this.getGrid()){
                            var grid = new ria.dom.Dom('.chlk-grid');
                            this.setGrid(grid);
                            if(this.getCurrentIndex() !== undefined && !new ria.dom.Dom(':focus').exists())
                                this.focusGrid();
                            if(configs.selectedIndex !== undefined){
                                var selectedRow = grid.find('.row:eq(' + configs.selectedIndex  + ')');
                                grid.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [selectedRow, parseInt(configs.selectedIndex, 10)]);
                            }
                            if(configs.infiniteScroll && !grid.hasClass('with-scroller'))
                                this.addInfiniteScroll(grid);
                        }
                    }.bind(this));
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_ROW.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            VOID, function selectRow(node, event, row, index) {
                if(row.exists()){
                    node.find('.row.selected').removeClass(selectedRowClass);
                    row.addClass(selectedRowClass);
                    this.setCurrentIndex(index || parseInt(row.getAttr('index'), 10));
                    this.scrollToElement();
                    this.focusGrid();
                    node.trigger(chlk.controls.GridEvents.AFTER_ROW_SELECT.valueOf(), [row, index]);
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            VOID, function deselectRow(node, event, row, index) {
                row.removeClass(selectedRowClass);
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_NEXT_ROW.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function selectNextRow(node, event) {
                var selectedRow = node.find('.row.selected');
                var next = selectedRow.next('.row');
                if(next.exists()){
                    node.trigger(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), [selectedRow, parseInt(selectedRow.getAttr('index'), 10)]);
                    node.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [next, parseInt(next.getAttr('index'), 10)]);
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_PREV_ROW.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function selectPrevRow(node, event) {
                var selectedRow = node.find('.row.selected');
                var prev = selectedRow.previous('.row');
                if(prev.exists()){
                    node.trigger(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), [selectedRow, parseInt(selectedRow.getAttr('index'), 10)]);
                    node.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [prev, parseInt(prev.getAttr('index'), 10)]);
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.FOCUS.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function focusGrid(node, event) {
                this.onFocusGrid(node, event);
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

            [ria.mvc.DomEventBind('click', '.chlk-grid .row')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function rowClick(node, event) {
                var target = new ria.dom.Dom(event.target);
                if(!(target.hasClass(noRowClickEventClass) || target.parent('.' + noRowClickEventClass).exists())){
                    if(node.hasClass('selected')){
                        this.focusGrid();
                    }else{
                        var grid = this.getGrid();
                        var selectedRow = grid.find('.row.selected');
                        if(selectedRow.exists()){
                            grid.trigger(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), [selectedRow, parseInt(selectedRow.getAttr('index'), 10)]);
                        }
                        var index = parseInt(node.getAttr('index'), 10);
                        grid.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [node, parseInt(selectedRow.getAttr('index'), 10)]);
                    }

                }
            },

            [ria.mvc.DomEventBind('keydown', '.grid-focus, .' + otherInputWithFocusClass)],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function keyArrowsClick(node, event) {
                var currentIndex = this.getCurrentIndex();
                var parent = node.parent('.chlk-grid');
                if((event.which == 38 && currentIndex) || (event.which == 40 && currentIndex < this.getCount() - 1)){
                    var selectedRow = parent.find('.row.selected');
                    parent.trigger(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), [selectedRow, parseInt(selectedRow.getAttr('index'), 10)]);
                    if(event.which == 38){
                        currentIndex--;
                    }else{
                        currentIndex++;
                    }
                    parent.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [parent.find('.row[index="' + currentIndex + '"]'), currentIndex]);
                }
                parent.trigger(chlk.controls.GridEvents.KEY_DOWN.valueOf(), [event.which]);
            },

            [[ria.dom.Dom]],
            VOID, function focusGrid() {
                var node = this.getGrid();
                var row = node.find('.row.selected');
                if(row.exists()){
                    var focusNode = node.find('.row.selected').find('.' + otherInputWithFocusClass);
                    if(focusNode.exists()){
                        focusNode.valueOf()[0].focus()
                    }else{
                        node.find('.grid-focus').valueOf()[0].focus();
                    }
                }else{
                    node.find('.grid-focus').valueOf()[0].focus();
                }
            },

            VOID, function scrollToElement(){
                var el = this.getGrid().find('.row.selected');
                var demo = new ria.dom.Dom('#demo-footer');
                window.scrollTo(0, el.offset().top + el.height()/2 - (window.innerHeight - (demo.height() || 0))/2);
            }
        ]);
});