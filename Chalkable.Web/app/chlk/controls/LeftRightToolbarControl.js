REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var hideClass = 'x-hidden';

    /** @class chlk.controls.LeftRightToolbarControl */
    CLASS(
        'LeftRightToolbarControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/left-right-tb.jade')(this);
                this.setDefaultConfigs({
                    //width: 630,
                    itemsCount:8,
                    fixedPadding:false,
                    hideArrows: false,
                    disabledClass: 'disabled',
                    disablePrevButton: true,
                    disableNextButton: true,
                    needDots: true,
                    pressedClass: 'pressed',
                    pressAfterClick: true
                })
            },

            Object, 'configs',

            Object, 'defaultConfigs',

            Object, 'currentIndex',

            [[Object, Object]],
            Object, function prepareData(data, attributes_) {
                var configs = this.getDefaultConfigs();
                if(attributes_){
                    configs = Object.extend(configs, attributes_);
                }
                if(configs.hideArrows)
                    configs.disabledClass = hideClass;
                if(data.length > configs.itemsCount){
                    configs.disableNextButton=false;
                    if(configs.needDots){
                        var dots = [],
                            dotsCount = Math.ceil(data.length/configs.itemsCount);
                        for(var i=0; i < dotsCount; i++)
                            dots.push(i==0);
                        configs.dots = dots;
                    }
                }else{
                    configs.needDots=false
                }

                this.setConfigs(configs);
                this.setCurrentIndex(0);
                var that = this;

                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var toolbar = activity.getDom().find('.lr-toolbar');
                        toolbar.on('click', '.arrow:not(.disabled)', function(node, event){
                            var index = that.getCurrentIndex();
                            if(node.hasClass('prev-button')){
                                that.setCurrentIndex(--index);
                                if(configs.fixedPadding){
                                    that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + index + '"]'), event, toolbar);
                                }else{
                                    that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + index + '"]'), event, toolbar);
                                }
                            }else{
                                that.setCurrentIndex(++index);
                                if(configs.fixedPadding){
                                    that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + index + '"]'), event, toolbar);
                                }else{
                                    that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + index + '"]'), event, toolbar);
                                }
                            }
                        });
                        toolbar.find('.paginator').on('click', 'a:not(.current)', function(node, event){
                            that.setPageByCurrentDot(node, event, toolbar);
                            return false;
                        })
                    }.bind(this));

                return configs;
            },

            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom]],
            VOID, function setPageByCurrentDot(node, event, toolbar){
                var width = toolbar.find('.first-container').width();
                var index = node.getAttr('index');
                var secondContainer = toolbar.find('.second-container');
                var nextButton = toolbar.find('.next-button');
                var prevButton = toolbar.find('.prev-button');
                toolbar.find('.paginator .current').removeClass('current');
                node.addClass('current');
                secondContainer.setCss('left', -width * index);
                if(index == 0){
                    prevButton.addClass('disabled');
                }else{
                    prevButton.removeClass('disabled');
                }
                if(index == this.getConfigs().dots.length - 1){
                    nextButton.addClass('disabled');
                }else{
                    nextButton.removeClass('disabled');
                }
            }
        ]);
});