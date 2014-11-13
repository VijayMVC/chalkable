REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var hideClass = 'x-hidden';

    /** @class chlk.controls.LRToolbarEvents */
    ENUM('LRToolbarEvents', {
        //ria.dom.Dom arrow, Number index, Boolean hasLeft, Boolean hasRight
        AFTER_ANIMATION: 'afteranimation',

        //ria.dom.Dom arrow, Boolean isLeft, Number index
        ARROW_CLICK: 'arrowclick',

        //ria.dom.Dom arrow, Number index
        BEFORE_ANIMATION: 'beforeanimation',

        //ria.dom.Dom dot, Boolean isLeft, Number index
        DOT_CLICK: 'dotclick',

        AFTER_RENDER: 'afterrender'
    });

    var self = null;

    /** @class chlk.controls.LeftRightToolbarControl */
    CLASS(
        'LeftRightToolbarControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/left-right-tb.jade')(this);

                self = this;
            },

            //Object, 'configs',

            Object, 'defaultConfigs',

            //Object, 'currentIndex',

            [
                [Object, Object, String, String, Object]
            ],
            Object, function prepareData(attributes_, data_, controller_, action_, params_) {
                var configs = {
                    itemsCount: 8,
                    fixedPadding: false,
                    hideArrows: false,
                    disabledClass: 'disabled',
                    disablePrevButton: true,
                    disableNextButton: true,
                    needDots: true,
                    pressedClass: 'pressed',
                    pressAfterClick: true,
                    equalItems: true,
                    padding: 0,
                    itemClass: ''
                };
                if (attributes_) {
                    configs = Object.extend(configs, attributes_);
                }
                if (configs.hideArrows)
                    configs.disabledClass = hideClass;

                if(data_){
                    if (data_.length > configs.itemsCount) {
                        configs.disableNextButton = false;
                        if (configs.needDots) {
                            var dots = [],
                                dotsCount = Math.ceil(data_.length / configs.itemsCount);
                            for (var i = 0; i < dotsCount; i++)
                                dots.push(i == 0);
                            configs.dots = dots;
                        }
                    } else {
                        configs.needDots = false
                    }

                    configs.pagesCount = Math.ceil(data_.length / configs.itemsCount);
                }else{
                    configs.equalItems = false;
                    configs.pressAfterClick = attributes_.pressAfterClick;
                }

                if (configs.multiple) {
                    configs.controller = controller_;
                    configs.action = action_;
                    configs.params = params_ || [];
                }

                return configs;
            },

            [
                [Object, Object]
            ],
            Object, function processAttrs(attributes, configs) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                attributes['data-configs'] = configs;
                var that = this;
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var toolbar = activity.getDom().find('#' + attributes.id);
                        if (toolbar.exists()) {
                            toolbar.setData('currentIndex', 0);
                            if(!configs.equalItems){
                                var t2 = toolbar.find('.second-container');
                                var t3 = toolbar.find('.third-container');
                                if(t3.exists() && t3.width() > t2.width()){
                                    toolbar.find('.next-button').removeClass(configs.disabledClass);
                                    var dotsK = Math.ceil(t3.width()/(t2.width() - configs.padding));
                                    configs.pagesCount = dotsK;
                                    toolbar.setData('configs', configs);
                                    var res = '<a class="current" index="0"></a>';
                                    for(var i = 0; i < dotsK - 1; i++){
                                        res += '<a index="' + (i + 1) + '"></a>';
                                    }
                                    toolbar.find('.paginator').setHTML(res);
                                }
                            }
                            if (configs.pressAfterClick) {
                                var pressedIndex = parseInt(toolbar.find('.pressed').getAttr('index'), 10);
                                var pageIndex = Math.floor(pressedIndex / configs.itemsCount) || 0;
                                that.setPageByCurrentDot(null, toolbar, pageIndex);
                                toolbar.on('click', '.second-container>*:not(.pressed)', function (node, event) {
                                    if (!configs.multiple)
                                        toolbar.find('.second-container>.pressed').removeClass('pressed');
                                    setTimeout(function () {
                                        node.addClass('pressed');
                                    }, 1);
                                });
                            }
                            if (configs.multiple) {
                                toolbar.on('click', '.second-container>*', function (node, event) {
                                    var state = that.context.getState();
                                    state.setController(configs.controller);
                                    state.setAction(configs.action);
                                    var params = configs.params;
                                    var ids = Array.isArray(configs.selectedIds) ? configs.selectedIds : configs.selectedIds.split(',');
                                    var currentId = node.getData('id');
                                    if (currentId) {
                                        if (node.hasClass('pressed'))
                                            ids.splice(ids.indexOf(currentId), 1);
                                        else
                                            ids.push(currentId);
                                        params.push(ids.join(','));
                                    }
                                    state.setParams(params);
                                    state.setPublic(false);
                                    that.context.stateUpdated();
                                });
                            }
                            toolbar.on('click', '.arrow:not(.disabled)', function (node, event) {
                                var index = toolbar.getData('currentIndex'), isLeft = false;
                                if (node.hasClass('prev-button')) {
                                    toolbar.setData('currentIndex', --index);
                                    isLeft = true;
                                } else {
                                    toolbar.setData('currentIndex', ++index);
                                }
                                toolbar.trigger(chlk.controls.LRToolbarEvents.ARROW_CLICK.valueOf(), [node, isLeft, index]);
                                var configs = toolbar.getData('configs');
                                if (configs.needDots) {
                                    var dot = toolbar.find('.paginator A[index="' + index + '"]');
                                    var current =  toolbar.find('.paginator .current');
                                    current.removeClass('current');
                                    dot.addClass('current');
                                }
                                toolbar.trigger(chlk.controls.LRToolbarEvents.BEFORE_ANIMATION.valueOf(), [isLeft, index]);
                                toolbar.setData('currentIndex', index);
                                var nextButton = toolbar.find('.next-button');
                                var prevButton = toolbar.find('.prev-button');
                                var width = toolbar.find('.first-container').width();
                                var secondContainer = toolbar.find('.second-container');
                                var hasLeft = false, hasRight = false;
                                if(toolbar.is(':visible')){
                                    var currentLeft = parseInt(secondContainer.getCss('left'), 10),
                                        diff = width - configs.padding;
                                    var left = isLeft ? currentLeft + diff : currentLeft - diff;
                                    var thirdContainer = secondContainer.find('.third-container');
                                    if (thirdContainer.exists() && thirdContainer.width() <= secondContainer.width() - left) {
                                        //left = secondContainer.width() - thirdContainer.width();
                                        nextButton.addClass(configs.disabledClass);
                                    } else {
                                        nextButton.removeClass(configs.disabledClass);
                                        hasRight = true;
                                    }
                                    if(left > 0)
                                        left = 0;
                                    else
                                        if(left && configs.itemClass){
                                            var itemWidth = toolbar.find('.' + configs.itemClass).width();
                                            left = Math.floor(left/itemWidth + 0.5) * itemWidth;
                                        }

                                    secondContainer.setCss('left', left);
                                    if (left == 0) {
                                        prevButton.addClass(configs.disabledClass);
                                    } else {
                                        prevButton.removeClass(configs.disabledClass);
                                        hasLeft = true;
                                    }
                                    var interval = setInterval(function(){
                                        var eps = 10, curLeft = parseInt(secondContainer.getCss('left'), 10);
                                        if(curLeft > left - eps && curLeft < left + eps){
                                            toolbar.trigger(chlk.controls.LRToolbarEvents.AFTER_ANIMATION.valueOf(), [isLeft, index, hasLeft, hasRight]);
                                            clearInterval(interval);
                                        }
                                    }, 10)
                                }
                            });
                            toolbar.find('.paginator').on('click', 'a:not(.current)', function (node, event) {
                                that.setPageByCurrentDot(node, toolbar);
                                return false;
                            });
                            toolbar.trigger(chlk.controls.LRToolbarEvents.AFTER_RENDER.valueOf());
                        }
                    }.bind(this));
                return attributes;
            },

            [
                [ria.dom.Dom, ria.dom.Dom, Number, Boolean]
            ],
            VOID, function setPageByCurrentDot(node_, toolbar, index_, isLeft_) {
                var configs = toolbar.getData('configs'), node;
                index_ = (index_ || index_ >= 0) ? index_ : parseInt(node_.getAttr('index'), 10);
                if (configs.needDots) {
                    node = node_ || toolbar.find('.paginator A[index="' + index_ + '"]');
                    var current =  toolbar.find('.paginator .current');
                    current.removeClass('current');
                    node.addClass('current');
                    if(node_){
                        isLeft_ = current.getAttr('index') < node.getAttr('index');
                        toolbar.trigger(chlk.controls.LRToolbarEvents.DOT_CLICK.valueOf(), [node, isLeft_, index_]);
                    }
                }
                toolbar.trigger(chlk.controls.LRToolbarEvents.BEFORE_ANIMATION.valueOf(), [isLeft_, index_]);
                toolbar.setData('currentIndex', index_);
                var nextButton = toolbar.find('.next-button');
                var prevButton = toolbar.find('.prev-button');
                var width = toolbar.find('.first-container').width();
                var secondContainer = toolbar.find('.second-container');
                var left = (configs.padding - width) * index_;
                secondContainer.setCss('left', left);
                var hasLeft = false, hasRight = false;
                if(toolbar.is(':visible')){
                    if (index_ == 0) {
                        prevButton.addClass(configs.disabledClass);
                    } else {
                        prevButton.removeClass(configs.disabledClass);
                        hasLeft = true;
                    }
                    var thirdContainer = secondContainer.find('.third-container');
                    if ((index_ == configs.pagesCount - 1 ) || !configs.pagesCount || (thirdContainer.exists() && thirdContainer.width() == secondContainer.width() - left)) {
                        nextButton.addClass(configs.disabledClass);
                    } else {
                        nextButton.removeClass(configs.disabledClass);
                        hasRight = true;
                    }
                    var interval = setInterval(function(){
                        var eps = 10, curLeft = parseInt(secondContainer.getCss('left'), 10);
                        if(curLeft > left - eps && curLeft < left + eps){
                            toolbar.trigger(chlk.controls.LRToolbarEvents.AFTER_ANIMATION.valueOf(), [isLeft_, index_, hasLeft, hasRight]);
                            clearInterval(interval);
                        }
                    }, 10)
                }
            },

            [[ria.dom.Dom]],
            Number, function GET_CURRENT_PAGE(node) {
                return node.getData('currentIndex') | 0;
            },

            [[ria.dom.Dom, Number]],
            VOID, function SET_CURRENT_PAGE(node, index) {
                self.setPageByCurrentDot(null, node, index);
            }
        ]);
});