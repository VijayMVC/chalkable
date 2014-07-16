REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {
    var otherInputWithFocusClass = 'with-grid-focus';
    var selectedRowClass = 'selected';
    var noRowClickEventClass = 'no-row-click';
    var interval;

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

            function $(){
               BASE();
               this._loadAllButtonClicked = false;
            },

            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/list-view.jade')(this);
            },

            Object, 'configs',

            Number, 'currentIndex',
            Number, 'count',
            Number, 'scrollTop',
            ria.dom.Dom, 'grid',

            [[Object, Object]],
            VOID, function prepareData(data,configs_) {
                var configs = {
                    selectedIndex: null,
                    infiniteScroll: false,
                    itemsName: 'Items',
                    start: 0,
                    totalCount: null,
                    pageSize: 10,
                    interval: 250,
                    goTopButton: false,
                    loadAllButtonTitle: 'LOAD ALL',
                    loadAllPopUpTitle: '',
                    showLoadAllInPage: 4,
                    needGoToButton: false,
                    needLoadAllButton: false,
                    isPaggingModel: false,
                    service: null,
                    method: null,
                    paramsPrepend: []
                };
                if(configs_){
                    if(data.getTotalCount){
                        configs_.totalCount = data.getTotalCount();
                        configs_.pageSize = data.getPageSize();
                        configs_.isPaggingModel = true;
                    }else{
                        if(!configs_.pageSize)
                            configs_.pageSize = data.length;
                        if(data.length < configs_.pageSize)
                            configs_.noScroll = true;
                    }
                    configs = Object.extend(configs, configs_);
                }
                var getItemsMethod = 'get' + configs.itemsName;
                this.setConfigs(configs);
                this.setGrid(null);

                this.setCount(data.length != undefined ? data.length : data[getItemsMethod]().length);
                this.context.getDefaultView().getCurrent()
                    .addRefreshCallback(function (activity, model) {
                        configs = this.getConfigs();
                        if(!this.getGrid()){
                            var grid = new ria.dom.Dom('.chlk-grid');
                            this.setGrid(grid);
                            if((this.getCurrentIndex() || this.getCurrentIndex() == 0)
                                && !new ria.dom.Dom(':focus').exists())
                                this.focusGrid();

                            if(configs.selectedIndex || configs.selectedIndex == 0){
                                var selectedRow = grid.find('.row:eq(' + configs.selectedIndex  + ')');
                                grid.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [selectedRow, parseInt(configs.selectedIndex, 10) || 0]);
                            }
                            if(configs.infiniteScroll && !grid.hasClass('with-scroller') && !configs.noScroll)
                                this.addInfiniteScroll(grid);


//                          if(configs.goTopButton){
//                                //todo : add go top button
//                          }
                        }
                        var isLastLoad = (configs.currentStart + configs.pageSize) > configs.totalCount;
                        this.initScrollAction_();
                        if(configs.showLoadAllInPage < (configs.currentStart / configs.pageSize) && !isLastLoad){
                            this.showLoadAllPopUp_(this.getGrid());
                            this.setScrollTop(new ria.dom.Dom().scrollTop());
                        }
                        if(this._loadAllButtonClicked){
                            this.scrollToBottom_();
                            this._loadAllButtonClicked = false;
                            this.clearInterval_(this.getGrid());
                        }
                        if(isLastLoad){
                            this.hideLoadAllPopUp_(this.getGrid());
                        }
                    }.bind(this));
            },
            VOID, function initScrollAction_(){
                jQuery(window).scroll(function () {
                    var grid = this.getGrid();
                    if(grid){
                        var backTopNode = jQuery(grid.find('.back-top').valueOf());
                        var showAllButton = grid.find('.load-all-popup');
                        if (jQuery(window).scrollTop() > 100) {
                            this.showGoTopButton_(grid);
                            backTopNode.fadeIn();
                        } else {
                            backTopNode.fadeOut();
                        }
                        var scrollTop = this.getScrollTop() || 0;
                        if(scrollTop && new ria.dom.Dom().scrollTop() < scrollTop)
                            this.hideLoadAllPopUp_(this.getGrid());
                    }
                }.bind(this));
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_ROW.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            VOID, function selectRow(node, event, row, index) {
                if(!this.getGrid())
                    this.setGrid(node);
                if(row.exists()){
                    var selectedRow = node.find('.row.selected');
                    if(selectedRow.exists() && !row.is('.selected'))
                        node.trigger(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), [selectedRow, parseInt(selectedRow.getAttr('index'), 10)]);

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
                    node.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [next, parseInt(next.getAttr('index'), 10)]);
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_PREV_ROW.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function selectPrevRow(node, event) {
                var selectedRow = node.find('.row.selected');
                var prev = selectedRow.previous('.row');
                if(prev.exists()){
                    node.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [prev, parseInt(prev.getAttr('index'), 10)]);
                }
            },

            [[ria.dom.Dom]],
            VOID, function hideLoadAllPopUp_(grid){
                var popupNode = grid.find('.load-all-popup');
                jQuery(popupNode.valueOf()).fadeOut(function(){
                    popupNode.addClass('x-hidden');
                });
            },

            [[ria.dom.Dom]],
            VOID, function showLoadAllPopUp_(grid){
               var popupNode = grid.find('.load-all-popup');
               popupNode.removeClass('x-hidden');
               jQuery(popupNode.valueOf()).fadeIn();
            },
            [[ria.dom.Dom]],
            VOID, function hideGoTopButton_(grid){
                var backTopNode = grid.find('.back-top');
                backTopNode.addClass('x-hidden');
            },
            [[ria.dom.Dom]],
            VOID, function showGoTopButton_(grid){
               var backTopNode = grid.find('.back-top');
               backTopNode.removeClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', 'button[name="loadAll"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function loadAllButtonClick(node, event){
                var grid = this.getGrid();
                var form = grid.parent('form');
                var configs = this.getConfigs();
                configs.currentStart = 0;
                form.find('[name=count]').setValue(configs.totalCount);
                this._loadAllButtonClicked = true;
                this.scrollAction_(grid, true);
            },

            VOID, function scrollToBottom_(){
                jQuery('body, html').animate({ scrollTop: jQuery(document).height() }, 1000);
            },
            VOID, function scrollToTop_(){
                jQuery('body, html').animate({scrollTop: 0}, 1000);
            },

            [ria.mvc.DomEventBind('click', '.back-top A,.back-top-arrow')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function backTopAction(node, event){
                this.scrollToTop_();
            },

            [[ria.dom.Dom, Boolean]],
            VOID, function scrollAction_(grid, loadAll_){
                //todo: trigger form submit
                var configs = this.getConfigs();
                var div = new ria.dom.Dom('<div class="horizontal-loader"></div>');
                var form = grid.parent('form');
                grid.addClass('scroll-freezed');
                grid.appendChild(div);
                if(configs.service){
                    var serviceIns = this.getContext().getService(configs.service);
                    var ref = ria.reflection.ReflectionClass(configs.service);
                    var methodRef = ref.getMethodReflector(configs.method);
                    var params = ria.__API.clone(configs.paramsPrepend) || [];
                    if(configs.isPaggingModel)
                        params.unshift(configs.pageSize);
                    params.unshift(configs.currentStart);
                    var tpl = new configs.tpl();
                    var dom = new ria.dom.Dom('.chlk-grid');
                    methodRef.invokeOn(serviceIns, params)
                        .then(function(model){
                            if(loadAll_)
                                dom.find('.row').remove();
                            if(Array.isArray(model)){
                                if(!model.length || model.length < configs.pageSize)
                                    this.clearInterval_(grid);
                                model.forEach(function(item){
                                    tpl.assign(item);
                                    tpl.renderTo(dom);
                                });
                            }
                            else{
                                tpl.assign(model);
                                tpl.renderTo(dom);
                            }
                            this.removeLoader_(grid, true);
                        }, this);
                }else{
                    form.find('[name=start]').setValue(configs.currentStart);
                    if(loadAll_)
                        grid.find('.row').remove();
                    jQuery(grid.valueOf()).parents('form').find('.scroll-start-button').click();
                }
            },

            [[ria.dom.Dom]],
            VOID, function addInfiniteScroll(grid) {
                grid.addClass('with-scroller');
                var baseContentHeight = grid.height();

                var pageHeight = document.documentElement.clientHeight;
                var scrollPosition;
                var contentHeight = baseContentHeight + grid.offset().top;

                var configs = this.getConfigs();
                var size = configs.pageSize;
                configs.currentStart = configs.start + size;
                this.setConfigs(configs);
                interval && clearInterval(interval);
                interval = undefined;

                interval = setInterval(function(){
                    if(interval && !grid.hasClass('scroll-freezed')){
                        scrollPosition = window.pageYOffset;
                        configs = this.getConfigs();
                        if(!configs.isPaggingModel || configs.totalCount > configs.currentStart){
                            if((contentHeight - pageHeight - scrollPosition) < 1000){
                                this.scrollAction_(grid);
                                this.removeLoaderWithInterval_(grid);
                                configs.currentStart += size;
                                contentHeight += baseContentHeight;
                            }
                        }
                        else{
                            this.clearInterval_(grid);
                        }
                    }
                }.bind(this), configs.interval || 250);
            },

            [[ria.dom.Dom]],
            VOID, function removeLoaderWithInterval_(grid){
                var that = this;
                var interval2 = setInterval(function(){
                    var node = grid.find('.horizontal-loader');
                    if(node.exists()){
                        node.remove();
                        that.removeLoader_(grid, true);
                        clearInterval(interval2);
                    }
                }, 500);
            },

            [[ria.dom.Dom]],
            VOID, function clearInterval_(grid){
                clearInterval(interval);
                interval = undefined;
                var configs = this.getConfigs(), that = this;
                if(!configs.service){
                    this.removeLoaderWithInterval_(grid);
                }
            },

            [[ria.dom.Dom, Boolean]],
            VOID, function removeLoader_(grid, forceRemove_){
                var node = grid.find('.horizontal-loader');
                if(grid.hasClass('scroll-freezed')){
                    grid.removeClass('scroll-freezed');
                }
                if(node.exists() && (node.next().exists() || forceRemove_)){
                    grid.removeClass('scroll-freezed');
                    node.remove();
                }
            },

            [ria.mvc.DomEventBind('click', '.chlk-grid .row')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function rowClick(node, event) {
                var target = new ria.dom.Dom(event.target);
                if(!(target.hasClass(noRowClickEventClass) || target.parent('.' + noRowClickEventClass).exists())){
                    if(node.hasClass('selected')){
                        this.focusGrid();
                    }else{
                        var grid = this.getGrid();
                        var index = parseInt(node.getAttr('index'), 10);
                        grid.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [node, parseInt(node.getAttr('index'), 10)]);
                    }

                }
            },

            [ria.mvc.DomEventBind('keydown', '.grid-focus, .' + otherInputWithFocusClass)],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function keyArrowsClick(node, event) {
                var currentIndex = this.getCurrentIndex();
                var parent = node.parent('.chlk-grid');

                //TODO: replace with constants
                var nextRow = parent.find('.row[index="' + currentIndex + '"]');
                if((event.which == 38 && currentIndex) || (event.which == 40 && nextRow.exists())){
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
                //if(!new ria.dom.Dom(':focus').exists()){
                    var node = this.getGrid();
                    var row = node.find('.row.selected');
                    if(row.exists()){
                        var focusNode = node.find('.row.selected').find('.' + otherInputWithFocusClass);
                        if(focusNode.exists()){
                            focusNode.trigger('focus');
                            if(focusNode.hasClass('select-text') && focusNode.getValue())
                                focusNode.valueOf()[0].setSelectionRange(0, focusNode.getValue().length);
                        }else{
                            node.find('.grid-focus').valueOf()[0].focus();
                        }
                    }else{
                        node.find('.grid-focus').valueOf()[0].focus();
                    }
                //}
            },

            VOID, function scrollToElement(){
                var el = this.getGrid().find('.row.selected');
                var demo = new ria.dom.Dom('#demo-footer');
                //window.scrollTo(0, el.offset().top + el.height()/2 - (window.innerHeight - (demo.height() || 0))/2);
                $("html, body").stop( true, true ).animate({
                    scrollTop: el.offset().top + el.height()/2 - (window.innerHeight - (demo.height() || 0))/2
                }, 300, 'linear');
            }
        ]);
});