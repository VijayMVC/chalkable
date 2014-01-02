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
            },

            Object, 'configs',

            Number, 'currentIndex',
            Number, 'count',
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
                    needLoadAllButton: false
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
                            if(configs.infiniteScroll && !grid.hasClass('with-scroller'))
                                this.addInfiniteScroll(grid);


//                          if(configs.goTopButton){
//                                //todo : add go top button
//                          }
                        }
                        this.initScrollAction_();
                        if(configs.showLoadAllInPage < (configs.currentStart / configs.pageSize)){
                            this.showLoadAllPopUp_(this.getGrid());
                        }
                        if((configs.currentStart + configs.pageSize) >= configs.totalCount){
                            this.hideLoadAllPopUp_(this.getGrid());
                            this.scrollToBottom_();
                        }
                    }.bind(this));
            },
            VOID, function initScrollAction_(){
                jQuery(window).scroll(function () {
                    var grid = this.getGrid();
                    var backTopNode = jQuery(grid.find('.back-top').valueOf());
                    var showAllButton = grid.find('.load-all-popup');
                    if (jQuery(window).scrollTop() > 100) {
                        this.showGoTopButton_(grid);
                        backTopNode.fadeIn();
                    } else {
                        backTopNode.fadeOut();
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
                this.scrollAction_(grid);
            },

            VOID, function scrollToBottom_(){
                jQuery('body').animate({ scrollTop: jQuery(document).height() }, 1000);
            },
            VOID, function scrollToTop_(){
                jQuery('body').animate({scrollTop: 0}, 1000);
            },

            [ria.mvc.DomEventBind('click', '.back-top A,.back-top-arrow')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function backTopAction(node, event){
                this.scrollToTop_();
            },

            [[ria.dom.Dom]],
            VOID, function scrollAction_(grid){
                //todo: trigger form submit
                var configs = this.getConfigs();
                var div = new ria.dom.Dom('<div class="horizontal-loader"></div>');
                var form = grid.parent('form');
                form.find('[name=start]').setValue(configs.currentStart);
                jQuery(grid.valueOf()).parents('form').find('.scroll-start-button').click();
                grid.addClass('scroll-freezed');
                grid.appendChild(div);
            },

            [[ria.dom.Dom]],
            VOID, function addInfiniteScroll(grid) {
                grid.addClass('with-scroller');
                var baseContentHeight = grid.height();
                var configs = this.getConfigs();

                var pageHeight = document.documentElement.clientHeight;
                var scrollPosition;
                var contentHeight = baseContentHeight + grid.offset().top;
                var interval;

                configs.currentStart = configs.start + configs.pageSize;

                interval = setInterval(function(){
                    if(!grid.hasClass('scroll-freezed')){
                        scrollPosition = window.pageYOffset;
                        if(configs.totalCount > configs.currentStart){
                            if((contentHeight - pageHeight - scrollPosition) < 400){
                                configs.currentStart += configs.pageSize;
                                this.scrollAction_(grid);
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
                }.bind(this), configs.interval);
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
                if((event.which == 38 && currentIndex) || (event.which == 40 && currentIndex < this.getCount() - 1)){
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
                            focusNode.valueOf()[0].focus()
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
                window.scrollTo(0, el.offset().top + el.height()/2 - (window.innerHeight - (demo.height() || 0))/2);
            }
        ]);
});