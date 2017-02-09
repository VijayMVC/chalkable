REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassProfileLunchTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileLunchPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileLunchTpl)],
        'ClassProfileLunchPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('keydown', '.meal-count-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function countKeyDown(node, event){
                var lrub = [ria.dom.Keys.LEFT.valueOf(), ria.dom.Keys.RIGHT.valueOf(), ria.dom.Keys.UP.valueOf(), ria.dom.Keys.DOWN.valueOf(), ria.dom.Keys.ENTER.valueOf()],
                    value = node.getValue() || "";
                if(lrub.indexOf(event.keyCode) > -1){
                    var currentIndex = node.getData('index'), nextIndex = -1,
                        mealsCount = node.parent('.chlk-grid').getData('meal-count'),
                        maxIndex = this.dom.find('.meal-count-input:last').getData('index');

                    switch(event.which){
                        case ria.dom.Keys.UP.valueOf(): nextIndex = currentIndex - mealsCount;break;
                        case ria.dom.Keys.ENTER.valueOf(): event.preventDefault();
                        case ria.dom.Keys.DOWN.valueOf(): nextIndex = currentIndex + mealsCount;break;
                        case ria.dom.Keys.LEFT.valueOf(): if((currentIndex % mealsCount) && (node.getSelectedText() || node.getCursorPosition() == 0)) nextIndex = currentIndex - 1;break;
                        case ria.dom.Keys.RIGHT.valueOf(): if((currentIndex % mealsCount < mealsCount - 1) && (node.getSelectedText() || node.getCursorPosition() == value.length)) nextIndex = currentIndex + 1;break;
                    }

                    if(nextIndex >= 0 && nextIndex <= maxIndex){
                        var nextNode = this.dom.find('.meal-count-input[data-index=' + nextIndex + ']');
                        nextNode.trigger('focus');
                        this.hideGradingPopUp_()
                        setTimeout(function(){
                            nextNode.select();
                        });
                    }

                    return;
                }

                /*if ([46, 8, 9, 27, 110, 190].indexOf(event.keyCode) !== -1 ||
                    (event.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    (event.keyCode >= 35 && event.keyCode <= 40)) {
                    return;
                }

                if ((event.shiftKey || (event.keyCode < 48 || event.keyCode > 57)) && (event.keyCode < 96 || event.keyCode > 105) || value.length >= 2) {
                    event.preventDefault();
                }*/
            },

            [ria.mvc.DomEventBind('keyup', '.meal-count-input')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function mealCountKeyup(node, event, options_){
                var value = this.getParsedValue_(node);
                var btn = this.dom.find('#grading-popup').find('.fill-grade');
                value ? btn.setAttr('disabled', false) : btn.setAttr('disabled', true);
            },

            [ria.mvc.DomEventBind('click', '.fill-grade')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function fillGradeClick(node, event){
                var input = this.dom.find('.meal-active'), that = this;
                var value = this.getParsedValue_(input);
                if(value){
                    var index = input.getData('meal-index');
                    this.dom.find('.meal-count-input[data-meal-index=' + index + ']').forEach(function(item){
                        var curValue = that.getParsedValue_(item);
                        if(!curValue)
                            item.setValue(value)
                                .setData('value', value)
                                .addClass('changed');
                    })
                    this.hideGradingPopUp_();
                }
                this.calculateTotal_(input);
            },

            [ria.mvc.DomEventBind('click', '.clear-grade')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function clearGradeClick(node, event){
                var input = this.dom.find('.meal-active'), that = this;
                var index = input.getData('meal-index');
                this.dom.find('.meal-count-input[data-meal-index=' + index + ']').forEach(function(item){
                    var value = 0
                    if(item.getValue() && parseInt(item.getValue(), 10) !== 0)
                        item.setValue(value)
                            .setData('value', value)
                            .addClass('changed');
                })
                this.hideGradingPopUp_();
                this.calculateTotal_(input);
            },
            
            Number, function getParsedValue_(node){
                var value = (node.getValue() || '').trim();
                value = value ? parseInt(value || 0, 10) : 0;
                return value;
            },

            [ria.mvc.DomEventBind('contextmenu', '.meal-count-input:not([readonly])')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            Boolean, function mealCountContextClick(node, event, options_){
                var value = this.getParsedValue_(node);
                var popUp = this.dom.find('#grading-popup'),
                    btn = popUp.find('.fill-grade');
                value ? btn.setAttr('disabled', false) : btn.setAttr('disabled', true);
                this.setPopUpPosition_(node);
                popUp.show();
                return false;
            },

            VOID, function hideGradingPopUp_(){
                this.dom.find('.grading-input-popup').hide();
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var that = this;

                new ria.dom.Dom().on('click.lunch_profile', function(doc, event){
                    var node = new ria.dom.Dom(event.target);

                    if(!node.isOrInside('.grading-input-popup')){
                        that.hideGradingPopUp_();
                    }
                });

                jQuery(window).on('resize.lunch_profile', function(){
                    that.setPopUpPosition_(that.dom.find('.meal-active'));
                })
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.lunch_profile');
                jQuery(window).off('resize.lunch_profile')
            },

            function setPopUpPosition_(node){
                var popUp = this.dom.find('#grading-popup');
                var gradesPageOffset = this.dom.find('.profile-page').offset();
                var cellOffset = node.offset();
                var top = cellOffset.top - gradesPageOffset.top + node.height();
                var left = cellOffset.left - gradesPageOffset.left;
                popUp.setCss('top', top);
                popUp.setCss('left', left);
                popUp.setCss('width', node.parent().width());
            },

            [ria.mvc.DomEventBind('focus', '.meal-count-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function countFocus(node, event){
                node.select();
                this.dom.find('.meal-active').removeClass('meal-active');
                node.addClass('meal-active');
            },

            [ria.mvc.DomEventBind('change', '.meal-count-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function countChange(node, event){
                var value = node.getValue(), iValue = parseInt(value, 10);
                if(iValue != value || iValue < 0)
                    node.setValue(0);
                else
                    if(iValue > 99)
                        node.setValue(99);

                if(node.getValue() != node.getData('value')){
                    if(!node.hasClass('changed'))
                        node.addClass('changed');
                }else
                    node.removeClass('changed');

                this.calculateTotal_(node);
            },

            function checkSaveBtnState_(){
                var btns = this.dom.find('.lunch-count-btn');
                if(this.dom.find('.meal-count-input.changed').count() > 0){
                    btns.removeAttr('disabled');
                    btns.setProp('disabled', false);
                }else{
                    btns.setAttr('disabled', 'disabled');
                    btns.setProp('disabled', true);
                }
            },

            function calculateTotal_(node){
                var mealIndex = node.getData('meal-index'), total = 0;
                this.dom.find('.meal-count-input[data-meal-index=' + mealIndex + ']').forEach(function(mealInput){
                    total += mealInput.getValue() ? parseInt(mealInput.getValue()) : 0;
                });
                this.dom.find('.total-cell[data-meal-index=' + mealIndex + ']').setHTML(total.toString());
                this.checkSaveBtnState_();
            },

            [ria.mvc.DomEventBind('click', '.lunch-cancel')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelClick(node, event){
                var okClick;
                this.view.ShowLeaveConfirmBox()
                    .then(function (can_cancel) {
                        okClick = true;
                        if (can_cancel === true) {
                            this.dom.find('.cancel-link').trigger('click');
                        }
                    }, this)
                    .complete(function(){
                        this.onConfirmComplete(okClick);
                    }, this);
            },

            [ria.mvc.DomEventBind('click', '.lunch-submit')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                this.dom.find('.meal-count-input.changed').removeClass('changed');
            },

            OVERRIDE, Object, function isReadyForClosing() {
                var okClick;
                if(this.dom.find('.meal-count-input.changed').count() > 0)
                    return this.view.ShowLeaveConfirmBox()
                        .then(function (can_cancel) {
                            okClick = true;
                            return can_cancel;
                        }, this)
                        .complete(function(res){
                            this.onConfirmComplete(okClick);
                        }, this);

                return true;
            },

            function onConfirmComplete(okClick_){
                if(!okClick_){
                    var node = this.dom.find('.lunch-date');
                    node.datepicker('setDate', new Date(node.getData('value')));
                }
            }
        ]);
});