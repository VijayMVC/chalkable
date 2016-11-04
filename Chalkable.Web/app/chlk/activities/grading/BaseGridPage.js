REQUIRE('chlk.templates.grading.GradingClassSummaryGridTpl');
REQUIRE('chlk.templates.grading.GradingInputTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingGridSummaryCellTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassSummaryGridAvgsTpl');
REQUIRE('chlk.templates.grading.GradingCommentsTpl');
REQUIRE('chlk.templates.grading.StudentAverageTpl');
REQUIRE('chlk.templates.grading.AvgCodesPopupTpl');
REQUIRE('chlk.templates.grading.StudentAverageInputTpl');

REQUIRE('chlk.activities.lib.TemplatePage');

REQUIRE('chlk.models.common.Array');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');
REQUIRE('chlk.models.grading.AvgCodesPopupViewData');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.BaseGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryGridTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'BaseGridPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            Array, 'allScores',

            chlk.models.id.ClassId, 'classId',

            function $() {
                BASE();

                this._lastModel = null;
            },

            /* Grading period blocks loading */

            [ria.mvc.DomEventBind('click', '.mp-title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseClick(node, event){
                var nodeT = new ria.dom.Dom(event.target);
                var dom = this.dom;
                if(!nodeT.hasClass('gp-button')){
                    var parent = node.parent('.marking-period-container');

                    var mpData = parent.find('.mp-data');

                    if(parent.hasClass('open')){
                        jQuery(mpData.valueOf()).animate({
                            height: 0
                        }, 500);

                        mpData.addClass('with-data');

                        setTimeout(function(){
                            parent.removeClass('open');
                        }, 500);
                    }else{
                        var items = this.dom.find('.marking-period-container.open');
                        var itemsMp = items.find('.mp-data');
                        jQuery(itemsMp.valueOf()).animate({height: 0}, 500);
                        if(mpData.hasClass('with-data')){
                            mpData.removeClass('with-data');
                            this.openGradingPeriod(mpData);
                        }else{
                            this.loadGradingPeriod(parent);
                        }
                        dom.find('.mp-data.with-data')
                            .removeClass('with-data')
                            .find('.grades-contaner')
                            .setHTML('');
                        setTimeout(function(){
                            items.removeClass('open');
                            itemsMp.find('.grades-contaner').setHTML('');
                        }, 500);
                        //parent.addClass('open');

                    }
                }
            },

            [[ria.dom.Dom]],
            function loadGradingPeriod(container){
                container.find('.load-grading-period').trigger('submit');
            },

            function openGradingPeriod(container){
                container.parent('.marking-period-container').addClass('open');
                var annContainer = container.find('.grades-container');
                container.setCss('height', 0);
                jQuery(container.valueOf()).animate({
                    height: (annContainer.height() + container.find('.buttons-row').height() + parseInt(annContainer.getCss('margin-bottom'), 10))
                }, 500);
            },

            /* Partial Update rules */

            VOID, function updateGradingPeriodPartRule_(tpl, model) {
                var mpData = this.dom.find('.mp-data[data-grading-period-id=' + model.getGradingPeriod().getId().valueOf() + ']');
                var container = mpData.find('.grades-container');
                var tooltipText = model.getTooltipText(), parent = mpData.parent();
                tpl.options({
                    classId: this.getClassId()
                });
                tpl.renderTo(container.setHTML(''));
                if(model.getGradingItems().length && model.getStudents().length){
                    parent.removeClass('no-items');
                }else{
                    parent.addClass('no-items');
                }
                setTimeout(function(){
                    this.openGradingPeriod(mpData);
                    parent.find('.mp-name').setData('tooltip', tooltipText);
                }.bind(this), 1);
            },

            /* Scores drop down */

            function getScores_(node_){
                return this.getAllScores();
            },

            [[String, ria.dom.Dom]],
            Array, function getSuggestedValues(text, node){
                var text = text.toLowerCase();
                var res = [];
                this.getScores_(node).forEach(function(item){
                    if(item[0].toLowerCase().indexOf(text) == 0)
                        res.push(item);
                });
                return res;
            },

            VOID, function updateDropDown(suggestions, node, all_){
                var list = this.dom.find('.autocomplete-list');
                if(suggestions.length || node.hasClass('error')){
                    var html = '';
                    suggestions.forEach(function(item){
                        html+='<div class="autocomplete-item" data-tooltip-type="overflow" data-tooltip="'
                            + item[0] + '" data-id="' + item[1] + '">' + item[0] + '</div>'
                    });
                    if(!all_)
                        html += '<div class="autocomplete-item see-all">See all »</div>';
                    var top = node.offset().top - list.parent().offset().top + node.height() + 3;
                    var left = node.offset().left - list.parent().offset().left;
                    list.setCss('top', top)
                        .setCss('left', left)
                        .setCss('width', node.width());
                    list.setHTML(html)
                        .show();
                    this.hideGradingPopUp();
                }else{
                    this.hideDropDown();
                }
            },

            VOID, function hideDropDown(){
                var list = this.dom.find('.autocomplete-list');
                list.setHTML('')
                    .hide();
            },

            VOID, function hideGradingPopUp(){
                this.dom.find('.grading-input-popup').hide();
            },

            /* Dropdown events */

            [ria.mvc.DomEventBind('click', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
                var cell = this.dom.find('.active-cell');
                var input = cell.find('.value-input');
                input.removeClass('not-equals');
                this.updateDropDown(this.getScores_(input), input, true);
                return false;
            },

            [ria.mvc.DomEventBind('mouseover', '.autocomplete-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function itemHover(node, event){
                if(!node.hasClass('hovered'))
                    node.parent().find('.hovered').removeClass('hovered');
                node.addClass('hovered');
            },

            [ria.mvc.DomEventBind('click', '.autocomplete-item:not(.see-all)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function listItemBtnClick(node, event){
                var text = node.getHTML().trim();
                this.hideDropDown();
                var value = text, isFill = false;
                var cell = this.dom.find('.active-cell');
                cell.find('.error').removeClass('error');
                var input = cell.find('.value-input');
                if(text.toLowerCase().indexOf('fill') > -1){
                    isFill = true;
                    value = text.split('(fill all)')[0].trim();
                }
                input.removeClass('not-equals');
                this.setItemValue(value, input, true);
                if(isFill){
                    this.fillAllOneValue(cell, value);
                }
            },

            /* Comments */

            [ria.mvc.DomEventBind('focus', '.comment-value')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentFocus(node, event){
                this.hideGradingPopUp();
            },

            [ria.mvc.DomEventBind('keyup', '.comment-value')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function commentKeyUp(node, event, options_){
                var popUp = node.parent().find('.grading-comments-list');
                if(popUp.is(':visible') && (event.which == ria.dom.Keys.UP.valueOf()
                    || event.which == ria.dom.Keys.DOWN.valueOf() || event.which == ria.dom.Keys.ENTER.valueOf())
                    && popUp.find('.item').exists()){
                        var selected = popUp.find('.item.selected'), next = selected;
                        if(!selected.exists())
                            selected = popUp.find('.item:first');
                        switch(event.which){
                            case ria.dom.Keys.UP.valueOf():
                                if(selected.previous().exists()){
                                    selected.removeClass('selected');
                                    selected.previous().addClass('selected');
                                }
                                break;
                            case ria.dom.Keys.DOWN.valueOf():
                                if(selected.next().exists()){
                                    selected.removeClass('selected');
                                    selected.next().addClass('selected');
                                }
                                break;
                            case ria.dom.Keys.ENTER.valueOf():
                                this.setCommentByNode(next);
                                break;
                        }
                }else{
                    if(node.getValue() && node.getValue().trim())
                        popUp.hide();
                    else
                        popUp.show();
                }
            },

            function setCommentByNode(node){
                var popUp = node.parent('.popup-bubble');
                var input = popUp.find('.comment-value');
                input.setValue(node.getHTML());
                popUp.find('.grading-comments-list').hide();
            },

            [ria.mvc.DomEventBind('click', '.grading-comments-list .item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentItemClick(node, event){
                this.setCommentByNode(node);
            },

            [ria.mvc.DomEventBind('mouseover', '.grading-comments-list .item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentItemMouseOver(node, event){
                if(!node.hasClass('selected')){
                    node.parent('.grading-comments-list').find('.selected').removeClass('selected');
                    node.addClass('selected');
                }
            },

            [ria.mvc.DomEventBind('click', '.comment-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentBtnClick(node, event){
                this.hideGradingPopUp();
                var active = this.dom.find('.active-cell');
                var popUp = this.dom.find('.popup-bubble.comment');
                var main = this.dom.parent('#main');
                var comment = active.find('.comment-value').getValue();
                popUp.find('textarea').setValue(comment);
                popUp.show();
                popUp.find('.grading-comments-list').show();
                var container = active.parent('.mps-container');
                var left = active.offset().left - container.offset().left - (popUp.width() + parseInt(popUp.getCss('padding-left'), 10)
                    + parseInt(popUp.getCss('padding-right'), 10) - active.width())/2;
                var bottom = container.height() - active.offset().top + container.offset().top;

                popUp.setCss('bottom', bottom);
                popUp.setCss('left', left);
                if(comment)
                    popUp.find('.grading-comments-list').hide();
                setTimeout(function(){
                    popUp.find('.comment-value').trigger('focus');
                }, 1);
            },

            [ria.mvc.DomEventBind('click', '.cancel-comment')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelBtnClick(node, event){
                node.parent('.popup-bubble.comment').hide();
                var cell = this.dom.find('.active-cell');
                setTimeout(function(){
                    cell.find('input').trigger('focus');
                }, 1);
                var commentInput = cell.find('.comment-value');
                commentInput.setValue(commentInput.getData('comment'));
            },

            [ria.mvc.DomEventBind('click', '.add-comment')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addCommentBtnClick(node, event){
                var cell = this.dom.find('.active-cell');
                var commentInput = cell.find('.comment-value');
                var comment = (node.parent('.popup-bubble').find('textarea').getValue() || '').trim();
                commentInput.setValue(comment).setData('comment', comment);
                cell.find('[name=isCommentChanged]').setValue(true);
                node.parent('.popup-bubble.comment').hide();
                setTimeout(function(){
                    cell.find('input').trigger('focus');
                }, 1);
                this.updateValue(false);
            },

            /* Grade value input manipulations*/

            [[ria.dom.Dom, String, Boolean]],
            function setItemState_(node, stateName, selectNext_){
                node.setValue(node.getData('grade-value'));
                node.parent('form').find('[name=' + stateName +']').setValue(true);
                this.updateValue(selectNext_);
            },

            function setItemValue(value, input, selectNext){
                value = value || '';
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase(): this.setItemState_(input, 'dropped', selectNext); break;
                    case Msg.Incomplete.toLowerCase(): this.setItemState_(input, 'isincomplete', selectNext); break;
                    case Msg.Late.toLowerCase(): this.setItemState_(input, 'islate', selectNext); break;
                    case Msg.Exempt.toLowerCase(): this.setItemState_(input, 'isexempt', selectNext); break;
                    default:{
                        var numericValue = parseFloat(value);
                        if(isNaN(numericValue) && value){
                            var allScores = this.getScores_(input);
                            allScores = allScores.filter(function(score){
                                return score[0].toLowerCase() == value.toLowerCase();
                            });
                            if(allScores.length == 0) return;
                        }
                        input.setValue(value);
                        this.updateValue(selectNext);
                    }
                }
            },

            [[Boolean]],
            function updateValue(selectNext_){
                var activeCell = this.dom.find('.active-cell');
                activeCell.find('form').trigger('submit');

                if(selectNext_ && !activeCell.hasClass('avg-value-container')){
                    var nextCell = activeCell.next().find('.grade-info');
                    setTimeout(function(){
                        if(nextCell.exists())
                            nextCell.trigger('click');
                        else
                            activeCell.find('.grade-info').trigger('click');
                    }.bind(this), 1);
                }
            },

            /* Fill all */

            function fillAll(activeCell, submitCurrent_){
                var form = activeCell.find('form');
                activeCell.removeClass('active-cell');
                activeCell.find('.grade-info').removeClass('empty-grade');
                this.hideGradingPopUp();
                if(submitCurrent_)
                    form.trigger('submit');
                var value = form.find('.value-input').getValue();
                var that = this;
                activeCell.parent('.grade-container').find('.empty-grade').forEach(function(item){
                    var cell = item.parent('.grade-value');
                    var model = that.getModelFromCell(cell);
                    model.setGradeValue(value);
                    that.addFormToActiveCell(cell, model);
                    cell.find('form').trigger('submit');
                });
            },

            function fillAllOneValue(activeCell, value){
                var form = activeCell.find('form');
                activeCell.removeClass('active-cell');
                activeCell.find('.grade-info').removeClass('empty-grade');
                this.hideGradingPopUp();
                var that = this;
                activeCell.parent('.grade-container').find('.empty-grade').forEach(function(item){
                    var cell = item.parent('.grade-value');
                    var model = that.getModelFromCell(cell);
                    switch(value.toLowerCase()){
                        case Msg.Dropped.toLowerCase(): model.setDropped(true); break;
                        case Msg.Incomplete.toLowerCase(): model.setIncomplete(true); break;
                        case Msg.Late.toLowerCase(): model.setLate(true); break;
                        case Msg.Exempt.toLowerCase(): model.setExempt(true); break;
                        default:{
                            model.setGradeValue(value);
                        }
                    }
                    that.addFormToActiveCell(cell, model);
                    cell.find('form').trigger('submit');
                });
            },

            [ria.mvc.DomEventBind('click', '.fill-grade-container')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function fillGradeClick(node, event){
                var activeCell = node.parent('.active-cell');
                var input = activeCell.find('.value-input');
                var value = input.getValue();
                if(value && !input.hasClass('error') && value.toLowerCase() != 'dropped' && value.toLowerCase() != 'exempt')
                    this.fillAll(activeCell, true);
            },

            /* Grade value input events */

            /*[ria.mvc.DomEventBind('contextmenu', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeMouseDown(node, event){
                var cell = node.parent('.grade-value');
                if(!cell.hasClass('avg-value-container') || cell.find('.grade-info').getData('may-be-exempt')){
                    var popUp = this.dom.find('#grading-popup');
                    var gradesPageOffset = this.dom.find('.grades-page').offset();
                    var cellOffset = cell.offset();
                    var top = cellOffset.top - gradesPageOffset.top + cell.height();
                    var left = cellOffset.left - gradesPageOffset.left;
                    popUp.setCss('top', top);
                    popUp.setCss('left', left);

                    setTimeout(function(){
                        popUp.show();
                    }, 10);

                    this.hideDropDown();
                    return false;
                }
                return true;
            },*/

            [ria.mvc.DomEventBind('focus', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeFocus(node, event){
                node.select();
            },

            [ria.mvc.DomEventBind('click', '.value-input, .grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeClick(node, event){
                this.hideDropDown();
            },

            [ria.mvc.DomEventBind('keydown', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeUpDownKeyDown(node, event){
                if(event.keyCode == ria.dom.Keys.TAB.valueOf())
                    return false;

                var list = this.dom.find('.autocomplete-list:visible'), canGoDown, hovered;

                if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                    if(list.exists()){
                        hovered = list.find('.hovered');
                        if(hovered.exists()){
                            hovered.trigger('click');
                            return false;
                        }
                    }
                    if(node.hasClass('error'))
                        return false;

                    this.setItemValue(node.getValue(), node, true);

                }
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                if(isDown || isUp){
                    if(list.exists()){
                        hovered = list.find('.hovered');
                        if(hovered.exists()){
                            if(isDown){
                                if(hovered.next().exists()){
                                    hovered.removeClass('hovered');
                                    hovered.next().addClass('hovered');
                                }else
                                    canGoDown = true;
                            }
                            if(isUp && hovered.previous().exists()){
                                hovered.removeClass('hovered');
                                hovered.previous().addClass('hovered');
                            }
                        }else{
                            if(isDown){
                                list.find('.autocomplete-item:eq(0)').addClass('hovered');
                            }
                        }
                        event.stopPropagation();
                    }
                }

                var curCell = node.parent('.grade-value'), cell, curBlock, valueInput, needsTimeout, eps = 10;
                switch(event.keyCode){
                    case ria.dom.Keys.UP.valueOf():
                        if(!list.is(':visible') || !list.find('.hovered').previous().exists())
                            cell = curCell.previous('.grade-value');
                        break;
                    case ria.dom.Keys.DOWN.valueOf():
                        if(!list.is(':visible') || canGoDown)
                            cell = curCell.next('.grade-value');
                        break;
                }

                switch(event.keyCode){
                    case ria.dom.Keys.LEFT.valueOf():
                        valueInput = curCell.find('.value-input');
                        if(valueInput.getSelectedText() || valueInput.getCursorPosition() == 0){
                            curBlock = curCell.parent('.grade-container').previous('.grade-container');
                            var fixedBlock = curCell.parent('.ann-types-container').find('.fixed-block');

                            if(curBlock.exists() && curBlock.hasClass('total-points') || !curBlock.exists() && !curCell.parent('.fixed-block').exists())
                                curBlock = fixedBlock.find('.grade-container:last');

                            if(curBlock.exists()){
                                if(!curBlock.parent('.fixed-block').exists() && curBlock.offset().left < (fixedBlock.offset().left + fixedBlock.width() - eps)){
                                    curBlock.parent('.grid-toolbar').find('.prev-button').trigger('click');
                                    needsTimeout = true;
                                }
                                cell = curBlock.find('.grade-value[row-index=' + curCell.getAttr('row-index') + ']');
                            }
                        }
                        break;
                    case ria.dom.Keys.RIGHT.valueOf():
                        valueInput = curCell.find('.value-input');
                        var value = valueInput.getValue() || '';
                        if(valueInput.getSelectedText() || valueInput.getCursorPosition() == value.length){
                            curBlock = curCell.parent('.grade-container').next('.grade-container');

                            if(!curBlock.exists() && curCell.parent('.fixed-block').exists()){
                                curBlock = curCell.parent('.fixed-block').next().find('.grade-container:first');

                                var toolbar = curBlock.parent('.lr-toolbar'),
                                    secondContainer = toolbar.find('.second-container'),
                                    curLeft = parseInt(secondContainer.getCss('left'), 10);

                                if(curLeft < 0){
                                    toolbar.trigger(chlk.controls.LRToolbarEvents.CLEAR_LEFT.valueOf());
                                    needsTimeout = true;
                                }
                            }

                            if(curBlock.exists() && curBlock.hasClass('total-points'))
                                curBlock = curBlock.next('.grade-container');

                            if(curBlock.exists()){
                                var parentBlock = curBlock.parent('.grades-container');
                                if(curBlock.offset().left > parentBlock.offset().left + parentBlock.width() - eps){
                                    curBlock.parent('.grid-toolbar').find('.next-button').trigger('click');
                                    needsTimeout = true;
                                }
                                cell = curBlock.find('.grade-value[row-index=' + curCell.getAttr('row-index') + ']');
                            }
                        }
                        break;
                }
                if(cell && cell.exists() && cell.hasClass('gradable')){
                    this.submitActiveForm();
                    if(needsTimeout)
                        setTimeout(function(){this.showCell(cell)}.bind(this), 500);
                    else{
                        this.showCell(cell);
                        cell.find('.value-input').addClass('cell-showed');
                    }
                }
            },

            [ria.mvc.DomEventBind('keyup', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeKeyUp(node, event){
                var suggestions = [], cell = node.parent('.active-cell');
                var list = this.dom.find('.autocomplete-list:visible');
                var value = (node.getValue() || '').trim(), fillItem = this.dom.find('#grading-popup').find('.fill-grade');
                if(!value){
                    node.addClass('empty-grade');
                    node.removeClass('error');
                }
                else{
                    node.removeClass('empty-grade');
                }
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase():
                    case Msg.Exempt.toLowerCase():
                    case Msg.Incomplete.toLowerCase():
                    case Msg.Late.toLowerCase(): fillItem.setAttr('disabled', true);break;
                    default: value ? fillItem.setAttr('disabled', false) : fillItem.setAttr('disabled', true);
                }
                if(!node.hasClass('cell-showed') &&
                    event.keyCode != ria.dom.Keys.UP.valueOf() &&
                    event.keyCode != ria.dom.Keys.DOWN.valueOf()
                ){
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf() && !node.hasClass('error')){

                    }else{
                        node.removeClass('not-equals');
                        if(value){
                            var text = node.getValue() ? node.getValue().trim() : '';
                            suggestions = this.updateInputByText(text, node);
                        }
                    }
                    if(!(event.keyCode == ria.dom.Keys.ENTER.valueOf() && list.exists()))
                        this.updateDropDown(suggestions, node);
                }
                node.removeClass('cell-showed');

                this.afterGradeKeyUp_(cell);
            },

            [[String, ria.dom.Dom]],
            function updateInputByText(text, node){
                var suggestions = text  ? this.getSuggestedValues(text, node) : [];
                if(!suggestions.length)
                    node.addClass('error');
                else{
                    node.removeClass('error');
                    var p = false;
                    suggestions.forEach(function(item){
                        if(item[0].toLowerCase() == node.getValue().toLowerCase())
                            p = true;
                    });
                    if(!p){
                        node.addClass('not-equals');
                    }
                }
                return suggestions;
            },

            [[ria.dom.Dom]],
            function afterGradeKeyUp_(cell){},

            [ria.mvc.DomEventBind('dblclick', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputDblClickClick(node, event){
                node.removeClass('not-equals');
                this.updateDropDown(this.getScores_(node), node, true);
            },

            /* Other events */

            [ria.mvc.DomEventBind('contextmenu', '.grade-info, .avg-text')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function cellContextMenu(node, event){
                var cell = node.parent('.grade-value');
                if(cell.hasClass('gradable')){
                    this.submitActiveForm();
                    this.showCell(cell);
                    setTimeout(function(){
                        cell.find('.value-input').trigger('contextmenu');
                    }, 1);
                    return false;
                }
                return true;
            },

            [ria.mvc.DomEventBind('click', '.grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradingPopUpClick(node, event){
                setTimeout(function(){
                    this.dom.find('.active-cell').find('.value-input').trigger('focus');
                }.bind(this), 1)
            },

            [ria.mvc.DomEventBind('change', '.main-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function mainDropChange(node, event, options_){
                var cell = this.dom.find('.active-cell');
                if(node.parent('.fill-grade-container').exists())
                    cell.find('.fill-grade-container').trigger('click');
                else{
                    var name = node.getAttr('name');
                    this.dom.find('.active-cell').find('[name=' + name + ']').setValue(node.checked());
                }

            },

            /* Data sorting */

            [[Array, Object, Function]],
            function reorder_(source, remap, studentIdProvider) {
                var result = new Array(source.length);
                source.forEach(function (_) {
                    var studentId = studentIdProvider(_);

                    VALIDATE_ARG('studentId', [chlk.models.id.SchoolPersonId], studentId);

                    var index = remap[studentId];

                    Assert(index != null);

                    result[index] = _;
                });
                return result;
            },

            function strcmp_(_1, _2) {
                var v1 = _1[1], v2 = _2[1];
                var def = typeof v1 === 'string' || typeof v2 === 'string' ? '' : Number.NEGATIVE_INFINITY;
                v1 = v1 != null ? v1 : def;
                v2 = v2 != null ? v2 : def;
                return v1 < v2 ? -1 : v1 > v2 ? 1 : _1[0] - _2[0];
            },

            /* Grade form */

            function submitActiveForm(){
                var activeCell = this.dom.find('.active-cell');
                if(activeCell.exists()){
                    activeCell.find('form').trigger('submit');
                }
                activeCell.removeClass('active-cell');
                activeCell.find('.grading-form').remove();
            },

            function addFormToActiveCell(cell, model_){
                var model;
                if(model_){
                    model = model_;
                    var studentId = cell.find('.grade-info').getData('studentid');
                    model.setStudentId(new chlk.models.id.SchoolPersonId(studentId));
                }else{
                    model = this.getModelFromCell(cell);
                }

                var tpl = this.prepareTplForForm_(cell, model);

                tpl.assign(model);

                tpl.renderTo(cell);

                this.addPopUpByModel(cell, model);
            },

            function addPopUpByModel(cell, model){

            },

            function prepareTplForForm_(cell, model){},

            function getFormModelClass_(){
                return chlk.models.announcement.ShortStudentAnnouncementViewData
            },

            function serializeFromForm(form){
                var o = form.serialize(true);
                var js = new ria.serialize.JsonSerializer();
                return js.deserialize(o, this.getFormModelClass_())
            },

            function beforeFormSubmit_(form, value, model, isAvg_){},

            function afterFormSubmit_(form, isAvg_){},

            [ria.mvc.DomEventBind('submit', '.grading-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradingFormSubmit(node, event){
                var input = node.find('.value-input');
                if(!input.hasClass('error') && !input.hasClass('not-equals')){
                    this.hideGradingPopUp();
                    var value = (input.getValue() || '').toLowerCase();
                    var isAvg = node.hasClass('avg-form');

                    var activeCell = node.parent('.grade-value');
                    this.dom.find('.autocomplete-list:visible').hide();
                    var model = this.getModelFromCell(activeCell), p = false;
                    var resModel = this.serializeFromForm(node);

                    if(isAvg){
                        var isComment = node.find('.is-comment').getValue();
                        var equalsValues = (input.getValue() || '') == (input.getData('grade-value').toString() || '');
                        if(model.isExempt() && resModel.isExempt() && value != 'exempt' )
                            node.find('[name=isexempt]').setValue(false);
                        p = equalsValues && !isComment && this.getBooleanValue_(model.isExempt()) == this.getBooleanValue_(resModel.isExempt());
                        if(!p && isComment && equalsValues && !node.parent('.grade-value').find('.edited').exists()){
                            input.setValue('');
                        }
                    }else{
                        p = (model.getGradeValue() || '') == (resModel.getGradeValue() || '') && (!model.isDropped ||
                            this.getBooleanValue_(model.isDropped()) == this.getBooleanValue_(resModel.isDropped()) &&
                            this.getBooleanValue_(model.isLate()) == this.getBooleanValue_(resModel.isLate()) &&
                            this.getBooleanValue_(model.isExempt()) == this.getBooleanValue_(resModel.isExempt()) &&
                            this.getBooleanValue_(model.isIncomplete()) == this.getBooleanValue_(resModel.isIncomplete()))&&
                            (model.getComment() || "") == (resModel.getComment() || "");
                    }
                    if(p){
                        event.preventDefault();
                        return false;
                    }

                    if(value == 'dropped' || value == 'exempt'){
                        var gradeValue = (input.getData('grade-value') || '').toLowerCase();
                        gradeValue = (gradeValue == 'exempt') ? '' : input.getData('grade-value');
                        input.setValue(gradeValue);
                    }

                    if(!node.getData('able-drop')){
                        node.find('.dropped-checkbox').setValue(false);
                        node.find('.dropped-hidden').setValue(false);
                    }
                    if(isAvg){
                        activeCell.removeClass('active-cell');
                        activeCell.find('.grade-text').addClass('for-edit');
                    }else{
                        this.updateFlagByModel(model, activeCell);
                        activeCell.find('.grade-text').setHTML('...');
                    }
                    this.beforeFormSubmit_(node, value, resModel, isAvg);
                    var mp = node.parent('.marking-period-container');
                    mp.find('.comment-button').hide();
                }else{
                    event.preventDefault();
                    return false;
                }
            },

            /* Left - Right Toolbar */

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.AFTER_RENDER.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event]],
            function afterTbRender(node, event){
                this.beforeTbAnimation(node);
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.ARROW_ENABLED.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Boolean, Number]],
            function arrowEnabled(node, event, arrow, isLeft_, index_){
                arrow.parent('.marking-period-container').find(isLeft_ ? '.prev-arrow' : '.next-arrow').removeClass('disabled');
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.ARROW_DISABLED.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Boolean, Number]],
            function arrowDisabled(node, event, arrow, isLeft_, index_){
                arrow.parent('.marking-period-container').find(isLeft_ ? '.prev-arrow' : '.next-arrow').addClass('disabled');
            },

            [ria.mvc.DomEventBind('click', '.next-arrow')],
            [[ria.dom.Dom, ria.dom.Event]],
            function nextArrowClick(node, event){
                node.parent('.marking-period-container').find('.next-button').trigger('click');
            },

            [ria.mvc.DomEventBind('click', '.prev-arrow')],
            [[ria.dom.Dom, ria.dom.Event]],
            function prevArrowClick(node, event){
                node.parent('.marking-period-container').find('.prev-button').trigger('click');
            },

            Number, function getColumns(){
                return 5;
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.BEFORE_ANIMATION.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event, Boolean, Number]],
            function beforeTbAnimation(toolbar, event_, isLeft_, index_){
                var num = toolbar.getData('currentCount');
                this.dom.find('.last-container').removeClass('last-container').removeClass('delay');
                var startIndex = index_ ? index_ * num + num : num;
                var node = toolbar.find('.dotted-container:eq(' + startIndex + ')');
                if(!node.is(':last-child')){
                    if(isLeft_)
                        node.addClass('delay');
                    setTimeout(function(){
                        node.addClass('last-container');
                    },1);
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.BEFORE_RESIZE.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event]],
            function beforeTbResize(toolbar, event_){
                var padding = this.getTbPadding();
                var maxWidth = ria.dom.Dom('#content').width() - padding;
                toolbar.setCss('width', maxWidth);
                var firstContainer = toolbar.find('.first-container');
                firstContainer.setCss('width', maxWidth);
            },

            /* Activity events */

            OVERRIDE, VOID, function onModelWait_(msg_) {
                BASE(msg_);
                if(msg_ == chlk.activities.lib.DontShowLoader()){
                    this.dom.find('.for-edit').setHTML('...');
                }
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.grading_popup');
                new ria.dom.Dom().off('click.grade');
                jQuery(window).off('resize.grade')
            },

            function prepareGradingModel_(model){
                return model;
            },

            Number, 'tbPadding',

            function preparePadding_(model){
                var avgsCount = this.dom.find('.marking-period-container.open').find('.avgs-container').count();
                this.setTbPadding(412 + 128 * avgsCount);
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.preparePadding_(model);
                var gradingModel = this.prepareGradingModel_(model);
                this.setClassId(gradingModel.getClassId());
                this.openGradingPeriod(this.dom.find('.open.marking-period-container').find('.mp-data'));
                this.prepareAllScores(gradingModel);
                this.addEvents();
                var toolbar = this.dom.find('.grid-toolbar');
                if(toolbar.exists()){
                    var parent = toolbar.parent('.marking-period-container');
                    if(toolbar.find('.prev-button').hasClass('disabled'))
                        parent.find('.prev-arrow').addClass('disabled');
                    if(toolbar.find('.next-button').hasClass('disabled'))
                        parent.find('.next-arrow').addClass('disabled');
                }else{
                    this.dom.find('.prev-arrow').addClass('disabled');
                    this.dom.find('.next-arrow').addClass('disabled');
                }
            },

            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);
                this.preparePadding_(model);
            },

            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(model instanceof chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData)
                    this.preparePadding_(model);
            },

            function prepareAllScores(model){},

            function addEvents(){
                var dom = this.dom, that = this;

                new ria.dom.Dom().on('click.grade', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.grading-comments-list')){
                        dom.find('.grading-comments-list').hide();
                    }

                    var popUp = dom.find('.popup-bubble.comment, .popup-bubble.codes');
                    if(!node.hasClass('value-input') && !node.hasClass('arrow')
                        && !node.isOrInside('.popup-bubble.comment') && !node.isOrInside('.grading-input-popup')
                        && !node.isOrInside('.autocomplete-list') && !node.isOrInside('.popup-bubble.codes')){
                            dom.find('.autocomplete-list').setHTML('').hide();
                            if(!node.hasClass('gp-button')){
                                popUp.hide();
                                var parent = (node.hasClass('grade-value') && node.hasClass('gradable')) ? node : node.parent('.grade-value.gradable');

                                that.submitActiveForm();
                                if(parent.exists() && !node.hasClass('alert-flag')){
                                    that.showCell(parent);
                                }else{
                                    dom.find('.active-row').removeClass('active-row');
                                    dom.find('.gp-button').hide();
                                }
                            }
                    }

                    if(!node.isOrInside('.grading-input-popup')){
                        that.hideGradingPopUp();
                    }
                });
            },

            Boolean, function getBooleanValue_(value){
                return value == 'false' ? false : !!value;
            },

            function showCell(parent){
                if(parent.exists()){
                    if(!parent.hasClass('active-row')){
                        var index = parent.getAttr('row-index');
                        this.dom.find('.active-row').removeClass('active-row');
                        this.dom.find('[row-index=' + index + ']').addClass('active-row');
                    }
                    parent.addClass('active-cell');

                    this.addFormToActiveCell(parent);
                    this.afterCellShow(parent);

                    setTimeout(function(){
                        parent.find('.value-input').trigger('focus');
                    },1)
                }else{
                    this.dom.find('.active-row').removeClass('active-row');
                    this.dom.find('.comment-button').hide();
                }
            },

            function updateFlagByModel(model, cell){}
        ]);
});