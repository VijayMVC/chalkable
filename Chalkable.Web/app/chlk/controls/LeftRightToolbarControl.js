REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var hideClass = 'x-hidden';

    /** @class chlk.controls.LeftRightToolbarControl */
    CLASS(
        'LeftRightToolbarControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/left-right-tb.jade')(this);
            },

            //Object, 'configs',

            Object, 'defaultConfigs',

            //Object, 'currentIndex',

            [
                [Object, Object, String, String, Object]
            ],
            Object, function prepareData(data, attributes_, controller_, action_, params_) {
                var configs = {
                    //width: 630,
                    itemsCount: 8,
                    fixedPadding: false,
                    hideArrows: false,
                    disabledClass: 'disabled',
                    disablePrevButton: true,
                    disableNextButton: true,
                    needDots: true,
                    pressedClass: 'pressed',
                    pressAfterClick: true
                };
                if (attributes_) {
                    configs = Object.extend(configs, attributes_);
                }
                if (configs.hideArrows)
                    configs.disabledClass = hideClass;
                if (data.length > configs.itemsCount) {
                    configs.disableNextButton = false;
                    if (configs.needDots) {
                        var dots = [],
                            dotsCount = Math.ceil(data.length / configs.itemsCount);
                        for (var i = 0; i < dotsCount; i++)
                            dots.push(i == 0);
                        configs.dots = dots;
                    }
                } else {
                    configs.needDots = false
                }

                configs.pagesCount = Math.ceil(data.length / configs.itemsCount);

                if (configs.multiple) {
                    configs.controller = controller_;
                    configs.action = action_;
                    configs.params = params_ || [];
                }

                //this.setConfigs(configs);
                //this.setCurrentIndex(0);

                return configs;
            },

            [
                [Object, Object]
            ],
            Object, function processAttrs(attributes, configs) {
                attributes.id = attributes.id || ria.dom.NewGID();
                attributes['data-configs'] = configs;
                var that = this;
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var toolbar = activity.getDom().find('#' + attributes.id);
                        if (toolbar.exists()) {
                            toolbar.setData('currentIndex', 0);
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
                                var index = toolbar.getData('currentIndex');
                                if (node.hasClass('prev-button')) {
                                    toolbar.setData('currentIndex', --index);
                                } else {
                                    toolbar.setData('currentIndex', ++index);
                                }
                                that.setPageByCurrentDot(null, toolbar, index);
                            });
                            toolbar.find('.paginator').on('click', 'a:not(.current)', function (node, event) {
                                that.setPageByCurrentDot(node, toolbar);
                                return false;
                            })
                        }
                    }.bind(this));
                return attributes;
            },

            [
                [ria.dom.Dom, ria.dom.Dom, Number]
            ],
            VOID, function setPageByCurrentDot(node_, toolbar, index_) {
                var configs = toolbar.getData('configs'), node;
                index_ = (index_ || index_ >= 0) ? index_ : parseInt(node_.getAttr('index'), 10);
                if (configs.needDots) {
                    node = node_ || toolbar.find('.paginator A[index="' + index_ + '"]');
                    toolbar.find('.paginator .current').removeClass('current');
                    node.addClass('current');
                }
                toolbar.setData('currentIndex', index_);
                var nextButton = toolbar.find('.next-button');
                var prevButton = toolbar.find('.prev-button');
                var width = toolbar.find('.first-container').width();
                var secondContainer = toolbar.find('.second-container');
                secondContainer.setCss('left', -width * index_);
                if (index_ == 0) {
                    prevButton.addClass(configs.disabledClass);
                } else {
                    prevButton.removeClass(configs.disabledClass);
                }
                if (index_ == configs.pagesCount - 1) {
                    nextButton.addClass(configs.disabledClass);
                } else {
                    nextButton.removeClass(configs.disabledClass);
                }
            }
        ]);
});