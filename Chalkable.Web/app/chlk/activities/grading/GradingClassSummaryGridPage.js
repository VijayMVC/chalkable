REQUIRE('chlk.templates.grading.GradingClassSummaryGridTpl');
REQUIRE('chlk.templates.grading.GradingInputTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingGridSummaryCellTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassSummaryGridAvgsTpl');
REQUIRE('chlk.templates.grading.GradingCommentsTpl');

REQUIRE('chlk.activities.lib.TemplatePage');

REQUIRE('chlk.models.common.Array');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.activities.grading', function () {

    var gradingGridTimer;

    /** @class chlk.activities.grading.GradingClassSummaryGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryGridTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'GradingClassSummaryGridPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.mp-title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseClick(node, event){
                var nodeT = new ria.dom.Dom(event.target);
                var dom = this.dom;
                if(!nodeT.hasClass('comment-button')){
                    var parent = node.parent('.marking-period-container');

                    var mpData = parent.find('.mp-data');

                    if(parent.hasClass('open')){
                        jQuery(mpData.valueOf()).animate({
                            height: 0
                        }, 500);

                        mpData.addClass('with-data');

                        setTimeout(function(){
                            parent.removeClass('open');
                            //mpData.setHTML('');
                        }, 500);
                    }else{
                        var items = this.dom.find('.marking-period-container.open');
                        var itemsMp = items.find('.mp-data');
                        jQuery(itemsMp.valueOf()).animate({height: 0}, 500);
                        if(mpData.hasClass('with-data')){
                            mpData.removeClass('with-data');
                            this.openGradingPeriod(mpData);
                        }else{
                            clearTimeout(gradingGridTimer);
                            parent.find('.load-grading-period').trigger('submit');
                        }
                        dom.find('.mp-data.with-data')
                            .setHTML('')
                            .removeClass('with-data');
                        setTimeout(function(){
                            items.removeClass('open');
                            itemsMp.setHTML('');
                        }, 500);
                        //parent.addClass('open');

                    }
                }
            },

            [ria.mvc.DomEventBind('change', '.dropped-checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function droppedChange(node, event, options_){
                if(!node.checked()){
                    var input = node.parent('form').find('.grade-autocomplete');
                    input.setValue(input.getData('grade-value'));
                }
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

            [ria.mvc.DomEventBind('change', '.exempt-checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function exemptChange(node, event, options_){
                node.parent('form').find('.grade-autocomplete').setValue('');
            },

            function getScores(node){
                var scores = this.getAllScores();
                var parent = node.parent('.grade-value');
                if(parent.getData('able-drop-student-score'))
                    scores.concat(['Dropped', 'Dropped (fill all)']);
                if(parent.getData('able-exempt-student-score'))
                    scores.concat(['Exempt', 'Exempt (fill all)']);
                return scores;
            },

            function openGradingPeriod(container){
                container.parent('.marking-period-container').addClass('open');
                var annContainer = container.find('.ann-types-container');
                container.setCss('height', 0);
                jQuery(container.valueOf()).animate({
                    height: (annContainer.height() + parseInt(annContainer.getCss('margin-bottom'), 10))
                }, 500);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl)],
            VOID, function updateGradingPeriodPart(tpl, model, msg_) {
                var container = this.dom.find('.mp-data[data-grading-period-id=' + model.getGradingPeriod().getId().valueOf() + ']');
                var tooltipText = model.getTooltipText();
                if(!model.isAutoUpdate()){
                    tpl.renderTo(container.setHTML(''));
                    setTimeout(function(){
                        this.openGradingPeriod(container);
                        container.parent().find('.mp-title').setData('tooltip', tooltipText);
                    }.bind(this), 1);
                }else{
                    var dom = this.dom;
                    var avgTpl = new chlk.templates.grading.ShortGradingClassSummaryGridAvgsTpl();
                    var rowIndex = parseInt(dom.find('.active-row').getAttr('row-index'), 10);
                    model.setRowIndex(rowIndex);
                    avgTpl.assign(model);
                    var avgs = container.find('.avgs-container');
                    var html = new ria.dom.Dom().fromHTML(avgTpl.render());
                    html.prependTo(avgs.parent());
                    avgs.remove();
                    model.getGradingItems().forEach(function(item){
                        dom.find('.avg-' + item.getId().valueOf()).setHTML(item.getAvg() ? item.getAvg().toString() : '');
                    });
                    container.parent().find('.mp-title').setData('tooltip', tooltipText);
                }

            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.TeacherClassGradingGridSummaryCellTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function updateGradingCell(tpl, model, msg_) {
                var container = this.dom.find('.item-' + model.getAnnouncementId().valueOf() + '-' + model.getStudentId());
                tpl.options({
                    maxScore: container.getData('max-score')
                });
                tpl.renderTo(container.setHTML(''));
            },

            ArrayOf(String), 'allScores',

            [[String, ria.dom.Dom]],
            ArrayOf(String), function getSuggestedValues(text, node){
                var text = text.toLowerCase();
                var res = [];
                this.getScores(node).forEach(function(score){
                    if(score.toLowerCase().indexOf(text) == 0)
                        res.push(score);
                });
                return res;
            },

            VOID, function updateDropDown(suggestions, node, all_){
                var list = this.dom.find('.autocomplete-list');
                if(suggestions.length || node.hasClass('error')){
                    var html = '<div class="autocomplete-item">' + suggestions.join('</div><div class="autocomplete-item">') + '</div>';
                    if(!all_){
                        html += '<div class="autocomplete-item see-all">See all »</div>';
                        var top = node.offset().top - list.parent().offset().top + node.height() + 43;
                        var left = node.offset().left - list.parent().offset().left + 61;
                        list.setCss('top', top)
                            .setCss('left', left);
                    }
                    list.setHTML(html)
                        .show();
                }else{
                    this.hideDropDown();
                }
            },

            VOID, function hideDropDown(){
                var list = this.dom.find('.autocomplete-list');
                list.setHTML('')
                    .hide();
            },

            [ria.mvc.DomEventBind('change', '.grading-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function gradingSelectChange(node, event, selected_){
                clearTimeout(gradingGridTimer);
                var form = node.parent('form');
                var hidden = form.find('.not-calculate-grid');
                hidden.setValue(true);
                form.trigger('submit');
                setTimeout(function(){
                    hidden.setValue(false);
                }, 1)
            },

            [ria.mvc.DomEventBind('contextmenu', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeMouseDown(node, event){
                node.parent().find('.grading-input-popup').show();
                return false;
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.grading_popup');
                new ria.dom.Dom().off('click.grade');
            },

            [ria.mvc.DomEventBind('keydown', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeKeyDown(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER.valueOf() && node.hasClass('error'))
                    return false;
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                var list = this.dom.find('.autocomplete-list:visible');
                if(isDown || isUp){
                    if(list.exists()){
                        var hovered = list.find('.hovered');
                        if(hovered.exists()){
                            if(isDown && hovered.next().exists()){
                                hovered.removeClass('hovered');
                                hovered.next().addClass('hovered');
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

                return true;
            },

            [ria.mvc.DomEventBind('keyup', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeKeyUp(node, event){
                var suggestions = [], cell = node.parent('.active-cell');
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                var list = this.dom.find('.autocomplete-list:visible');
                var value = (node.getValue() || '').trim();
                if(!value){
                    node.addClass('empty-grade');
                    node.removeClass('error');
                }
                else{
                    node.removeClass('empty-grade');
                }
                if(!isDown && !isUp){
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf() && !node.hasClass('error')){
                        if(list.exists() && list.find('.see-all').hasClass('hovered')){
                            list.find('.see-all').trigger('click');
                            return false;
                        }
                        else
                            this.updateValue(true);
                    }else{
                        if(value){
                            var text = node.getValue() ? node.getValue().trim() : '';
                            var parsed = parseInt(text,10);
                            if(parsed || parsed == 0){
                                node.removeClass('error');
                                if(parsed != text){
                                    node.addClass('error');
                                }else{
                                    this.hideDropDown();
                                }
                            }else{
                                suggestions = text  ? this.getSuggestedValues(text, node) : [];
                                if(!suggestions.length)
                                    node.addClass('error');
                                else
                                    node.removeClass('error');
                                this.updateDropDown(suggestions, node);
                            }
                        }
                    }
                    this.updateDropDown(suggestions, node);
                }
                setTimeout(function(cell){
                    var node = cell.find('.grade-autocomplete');
                    var numericValue = parseInt(node.getValue(), 10);
                    if(numericValue == node.getValue()){
                        var model = this.getModelFromCell(cell);
                        this.updateFlagByModel(model, cell);
                    }
                }.bind(this, cell), 10);

            },

            [[chlk.models.announcement.ShortStudentAnnouncementViewData, ria.dom.Dom]],
            function updateFlagByModel(model, cell){
                var value = cell.find('.grade-autocomplete').getValue();
                model.setGradeValue(value);
                var flag = cell.find('.alert-flag');
                flag.removeClass(Msg.Error.toLowerCase())
                    .removeClass(Msg.Late.toLowerCase())
                    .removeClass(Msg.Incomplete.toLowerCase())
                    .removeClass(Msg.Absent.toLowerCase())
                    .removeClass(Msg.Multiple.toLowerCase());
                var maxValue = cell.getData('max-score');
                flag.addClass(model.getAlertClass(maxValue));
                flag.setData('tooltip', model.getTooltipText(maxValue));
            },

            [ria.mvc.DomEventBind('click', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
                this.updateDropDown(this.getScores(node), this.dom.find('.active-cell'), true);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.grading-select + .chzn-container')],
            [[ria.dom.Dom, ria.dom.Event]],
            function clickSelect(node, event){
                event.stopPropagation();
            },

            [ria.mvc.DomEventBind('mouseover', '.autocomplete-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function itemHover(node, event){
                if(!node.hasClass('hovered'))
                    node.parent().find('.hovered').removeClass('hovered');
                node.addClass('hovered');
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.AFTER_RENDER.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event]],
            function afterTbRender(node, event){
                this.beforeTbAnimation(node);
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.BEFORE_ANIMATION.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event, Boolean, Number]],
            function beforeTbAnimation(toolbar, event_, isLeft_, index_){
                this.dom.find('.transparent-container').removeClass('transparent-container').removeClass('delay');
                var startIndex = index_ ? index_ * 5 + 5 : 5;
                var node = toolbar.find('.dotted-container:eq(' + startIndex + ')');
                if(!node.is(':last-child')){
                    if(isLeft_)
                        node.addClass('delay');
                    setTimeout(function(){
                        node.addClass('transparent-container');
                    },1);
                }
            },

            [ria.mvc.DomEventBind('click', '.comment-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentBtnClick(node, event){
                var active = this.dom.find('.active-cell');
                var popUp = this.dom.find('.chlk-pop-up-container.comment');
                var main = this.dom.parent('#main');
                var bottom = main.height() + main.offset().top - active.offset().top + 73;
                var left = active.offset().left - main.offset().left - 54;
                popUp.setCss('bottom', bottom);
                popUp.setCss('left', left);
                var comment = active.find('.comment-value').getValue();
                popUp.find('textarea').setValue(comment);
                popUp.show();
                popUp.find('.grading-comments-list').show();
                if(comment)
                    popUp.find('.grading-comments-list').hide();
                setTimeout(function(){
                    popUp.find('.comment-value').trigger('focus');
                }, 1)
            },

            function setCommentByNode(node){
                var popUp = node.parent('.chlk-pop-up-container');
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

            [ria.mvc.DomEventBind('click', '.cancel-comment')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelBtnClick(node, event){
                node.parent('.chlk-pop-up-container.comment').hide();
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
                var comment = node.parent('.chlk-pop-up-container').find('textarea').getValue();
                commentInput.setValue(comment).setData('comment', comment);
                node.parent('.chlk-pop-up-container.comment').hide();
                setTimeout(function(){
                    cell.find('input').trigger('focus');
                }, 1);
                this.updateValue(false);
            },

            [ria.mvc.DomEventBind('click', '.grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradingPopUpClick(node, event){
                setTimeout(function(){
                    node.parent('form').find('.grade-autocomplete').trigger('focus');
                }, 1)
            },

            [ria.mvc.DomEventBind('click', '.autocomplete-item:not(.see-all)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function listItemBtnClick(node, event){
                var text = node.getHTML().trim();
                this.hideDropDown();
                var value = text, isFill = false;
                var cell = this.dom.find('.active-cell');
                cell.find('.error').removeClass('error');
                var input = cell.find('.grade-autocomplete');
                if(text.toLowerCase().indexOf('fill') > -1){
                    isFill = true;
                    value = text.split('(fill all)')[0].trim();
                }
                input.removeClass('not-equals');
                this.setItemValue(value, input, !isFill);
                var that = this;
                if(isFill){
                    this.fillAllOneValue(cell, value);
                    this.dom.find('.able-fill-all').forEach(function(node){
                        that.setItemValue(value, node, false);
                    });
                }
            },

            [[ria.dom.Dom, String, Boolean]],
            function setItemState_(node, stateName, selectNext_){
                node.setValue(node.getData('value'));
                node.parent('form').find('[name=' + stateName +']').setValue(true);
                this.updateValue(selectNext_);
            },

            function setItemValue(value, input, selectNext){
                input.removeClass('able-fill-all');
                value = value || '';
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase(): this.setItemState_(input, 'dropped', selectNext); break;
                    case Msg.Incomplete.toLowerCase(): this.setItemState_(input, 'isincomplete', selectNext); break;
                    case Msg.Late.toLowerCase(): this.setItemState_(input, 'islate', selectNext); break;
                    case Msg.Exempt.toLowerCase(): this.setItemState_(input, 'isexempt', selectNext); break;
                    default:{
                        var numericValue = parseFloat(value);
                        if(Number.NaN == numericValue){
                            var allScores = this.getScores(input);
                            allScores = allScores.find(function(score){
                                return score.toLowerCase() == value;
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
                clearTimeout(gradingGridTimer);
                var activeCell = this.dom.find('.active-cell');
                activeCell.find('form').trigger('submit');

                if(selectNext_){
                    var nextCell = activeCell.next().find('.edit-cell');
                    setTimeout(function(){
                        if(nextCell.exists())
                            nextCell.trigger('click');
                        else
                            this.dom.trigger('click');
                    }.bind(this), 1);
                }


            },

            [ria.mvc.DomEventBind('submit', '.grading-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradingFormSubmit(node, event){
                var input = node.find('.grade-autocomplete');
                if(!input.hasClass('error')){
                    var activeCell = node.parent('.grade-value');
                    var model = this.getModelFromCell(activeCell);
                    this.updateFlagByModel(model, activeCell);
                    activeCell.find('.grade-text').setHTML('...');
                    var form = activeCell.parent('.marking-period-container').find('.load-grading-period');
                    this.addTimeOut(form);
                }
            },

            function addTimeOut(form){
                gradingGridTimer = setTimeout(function(){
                    form.find('.auto-update').setValue(true);
                    form.trigger('submit');
                    setTimeout(function(){
                        form.find('.auto-update').setValue(false);
                    }, 1);
                }, 5000);
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var allScores = [];
                model.getAlternateScores().forEach(function(item){
                    allScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });
                model.getAlphaGrades().forEach(function(item){
                    allScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });

                this.openGradingPeriod(this.dom.find('.open.marking-period-container').find('.mp-data'));

                allScores = allScores.concat(['Incomplete', 'Incomplete (fill all)', 'Late', 'Late (fill all)']);
                this.setAllScores(allScores);
                var dom = this.dom;
                var that = this;
                new ria.dom.Dom().on('click.grade', function(doc, event){
                    var dt = getDate();
                    var popUp = dom.find('.chlk-pop-up-container.comment');
                    var node = new ria.dom.Dom(event.target);
                    if(!node.hasClass('grade-autocomplete') && !node.hasClass('arrow')
                        && !node.isOrInside('.chlk-pop-up-container.comment') && !node.isOrInside('.grading-input-popup')
                        && !node.isOrInside('.autocomplete-list')){
                            dom.find('.autocomplete-list').setHTML('').hide();
                            if(!node.hasClass('comment-button')){
                                popUp.hide();
                                var parent = node.parent('.grade-value.gradable');

                                //TODO
                                var activeCell = dom.find('.active-cell');
                                if(activeCell.exists()){
                                    activeCell.find('form').trigger('submit');
                                }
                                activeCell.removeClass('active-cell');
                                activeCell.find('.grading-form').remove();
                                if(parent.exists() && !node.hasClass('alert-flag')){
                                    if(!parent.hasClass('active-row')){
                                        var index = parent.getAttr('row-index');
                                        dom.find('.active-row').removeClass('active-row');
                                        dom.find('[row-index=' + index + ']').addClass('active-row');
                                    }
                                    parent.addClass('active-cell');
                                    that.addFormToActiveCell(parent);
                                    node.parent('.marking-period-container').find('.comment-button').show();
                                    setTimeout(function(){
                                        parent.find('input').trigger('focus');
                                    },1)
                                }else{
                                    dom.find('.active-row').removeClass('active-row');
                                    dom.find('.comment-button').hide();
                                }
                            }
                    }
                });

                var dom = this.dom;
                new ria.dom.Dom().on('click.grading_popup', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.grading-input-popup')){
                        dom.find('.grading-input-popup').hide();
                    }
                });
            },

            [[ria.dom.Dom]],
            chlk.models.announcement.ShortStudentAnnouncementViewData, function getModelFromCell(cell){
                var node = cell.find('.grade-info');
                var model = new chlk.models.announcement.ShortStudentAnnouncementViewData(
                    new chlk.models.id.StudentAnnouncementId(node.getData('id')),
                    new chlk.models.id.AnnouncementId(node.getData('announcementid')),
                    new chlk.models.id.SchoolPersonId(node.getData('studentid')),
                    this.getBooleanValue(node.getData('dropped')),
                    this.getBooleanValue(node.getData('islate')),
                    this.getBooleanValue(node.getData('isexempt')),
                    this.getBooleanValue(node.getData('isabsent')),
                    this.getBooleanValue(node.getData('isincomplete')),
                    node.getData('comment'),
                    cell.find('.grade-text').getData('grade-value')
                );
                return model;
            },

            Boolean, function getBooleanValue(value){
                return value == 'false' ? false : !!value;
            },

            function addFormToActiveCell(cell, model_){
                var tpl = new chlk.templates.grading.GradingInputTpl();
                var model;
                if(model_){
                    model = model_;
                    var studentId = cell.find('.grade-info').getData('studentid');
                    model.setStudentId(new chlk.models.id.SchoolPersonId(studentId));
                }else{
                    model = this.getModelFromCell(cell);
                }

                tpl.assign(model);
                tpl.options({
                    ableDropStudentScore: this.getBooleanValue(cell.getData('able-drop-student-score')),
                    ableExemptStudentScore: this.getBooleanValue(cell.getData('able-exempt-student-score'))
                });
                tpl.renderTo(cell);
            },

            function serializeFromForm(form){
                var o = form.serialize(true);
                var js = new ria.serialize.JsonSerializer();
                return js.deserialize(o, chlk.models.announcement.ShortStudentAnnouncementViewData)
            },

            function fillAll(activeCell, submitCurrent_){
                var form = activeCell.find('form');
                activeCell.removeClass('active-cell');
                activeCell.find('.grade-info').removeClass('empty-grade');
                activeCell.find('.grading-input-popup').hide();
                var model = this.serializeFromForm(form);
                if(submitCurrent_)
                    form.trigger('submit');
                var that = this;
                activeCell.parent('.grade-container').find('.empty-grade').forEach(function(item){
                    var cell = item.parent('.grade-value');
                    that.addFormToActiveCell(cell, model);
                    cell.find('form').trigger('submit');
                });
            },

            function fillAllOneValue(activeCell, value){
                var form = activeCell.find('form');
                activeCell.removeClass('active-cell');
                activeCell.find('.grade-info').removeClass('empty-grade');
                activeCell.find('.grading-input-popup').hide();
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
                this.fillAll(activeCell, true);
            }
        ]);
});