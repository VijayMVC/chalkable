REQUIRE('chlk.activities.lib.TemplatePage');

REQUIRE('chlk.templates.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.StudentAnnouncement');
REQUIRE('chlk.templates.announcement.AnnouncementForStudentAttachments');
REQUIRE('chlk.templates.announcement.AnnouncementGradingPartTpl');
REQUIRE('chlk.templates.announcement.AnnouncementQnAs');
REQUIRE('chlk.templates.announcement.AddStandardsTpl');
REQUIRE('chlk.templates.grading.GradingCommentsTpl');
REQUIRE('chlk.templates.classes.TopBar');

REQUIRE('chlk.models.grading.AlertsEnum');

NAMESPACE('chlk.activities.announcement', function () {

    var slideTimeout;

    /** @class chlk.activities.announcement.AnnouncementViewPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementView)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementQnAs, 'update-qna', '.questions-and-answers', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.row.selected .grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementForStudentAttachments, 'update-attachments',
            '.student-attachments', ria.mvc.PartialUpdateRuleActions.Replace)],
        'AnnouncementViewPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            Array, 'applicationsInGradeView',
            Array, 'applications',
            Array, 'autoGradeApps',
            chlk.models.people.User, 'owner',
            chlk.models.id.AnnouncementId, 'announcementId',
            Number, 'maxScore',
            ArrayOf(chlk.models.announcement.StudentAnnouncement), 'studentAnnouncements',
            Boolean, 'gradable',
            Boolean, 'ableDropStudentScore',
            Boolean, 'ableToExempt',

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementGradingPartTpl)],
            VOID, function updateGradingPart(tpl, model, msg_) {
                tpl.options({
                    gradeViewApps: this.getApplicationsInGradeView(),
                    readonly: false,
                    userRole: this.getRole(),
                    autoGradeApps: this.getAutoGradeApps(),
                    applications: this.getApplications(),
                    owner: this.getOwner(),
                    announcementId: this.getAnnouncementId(),
                    ableDropStudentScore : this.isAbleDropStudentScore(),
                    gradable: this.isGradable()
                });
                var container = this.dom.find('.grading-part');
                container.empty();
                tpl.renderTo(container.removeClass('loading'));
                var grid = this.dom.find('.grades-individual');
                grid.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [grid.find('.row:eq(0)'), 0]);
            },

            [[ria.dom.Dom]],
            function updateStandardsInfo(node){
                var parent = node.parent('.attachments-container');
                var form = parent.previous('.row').find('form');
                var standardIds = [];
                var standardGrades = [];
                parent.find('.standard-grade').forEach(function(item){
                    standardIds.push(item.getData('id'));
                    standardGrades.push(item.getValue());
                });
                form.find('.standard-grades').setValue(standardGrades.join(','));
                form.find('.standard-ids').setValue(standardIds.join(','));
                form.trigger('submit');
            },

            [ria.mvc.DomEventBind('click', '.make-visible-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function makeVisibleClick(node, event){
                node.parent().parent().addClass('x-hidden');
            },

            [ria.mvc.DomEventBind('keypress', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    if(!node.hasClass('error')){
                        var row = node.parent('.row');
                        if(this.setGrade(node))
                            this.selectRow(this.dom.find('.grades-individual').find('.row:eq(' + (parseInt(row.getAttr('index'),10) + 1) + ')'));
                        return false;
                    }
                }
            },

            [ria.mvc.DomEventBind('click', '.show-student-grades')],
            [[ria.dom.Dom, ria.dom.Event]],
            function showGradesClick(node, event){
                if(new ria.dom.Dom(event.target).hasClass('show-student-grades')){
                    node.toggleClass('show-pop-up');
                    if(!node.hasClass('show-pop-up')){
                        var newValue = node.find('input[type=checkbox]').checked();
                        var oldValue = node.getData('value');
                        if(newValue != oldValue){
                            node.toggleClass('not-show');
                            node.setData('value', newValue);
                            node.find('.show-grades-input').setValue(newValue);
                            node.find('form').trigger('submit');
                        }
                    }
                }
            },

            [ria.mvc.DomEventBind('click', '.fill-grade')],
            [[ria.dom.Dom, ria.dom.Event]],
            function fillClick(node, event){
                var gradeNode = this.dom.find('.empty-grade');
                this.dom.find('.empty-grade').setValue(node.parent('.row').find('.grade-input').getValue());
                this.setGrade(gradeNode);
                node.parent('.grading-input-popup').hide();
                return false;
            },

            [[ria.dom.Dom]],
            function setGrade(node){
                if(!node.hasClass('error') && !node.hasClass('not-equals')){
                    var value = node.getValue();
                    var savedValue = node.getData('value');
                    var notEquals = value == savedValue || (!value && !savedValue);
                    if(notEquals)
                    node.parent().find('.grading-input-popup').find('.with-value').forEach(function(item){
                        if(item.checked() && !item.getData('value') || !item.checked() && item.getData('value'))
                            notEquals = false;
                    });
                    if(notEquals)
                        return false;

                    if(!value || (this.getSuggestedValues(value).length == 0 && !parseFloat(value)))
                        value = '';

                    this.setItemValue(value, node, true);
                    return true;
                }
                return false;
            },

            function selectRow(row){
                if(row.exists())
                    this.dom.find('.grades-individual').trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [row, parseInt(row.getAttr('index'), 10)]);
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_ROW.valueOf(), '.grades-individual')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            function selectStudent(node, event, row, index){
                clearTimeout(slideTimeout);
                slideTimeout = setTimeout(function(){
                    node.find('.attachments-container:eq(' + index + ')').slideDown(500);
                    row.find('.grade-triangle').addClass('down');
                }, 500);
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), '.grades-individual')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            function deSelectStudent(node, event, row, index){
                node.find('.attachments-container:eq(' + index + ')').slideUp(250);
                this.hideDropDown();
                row.find('.grade-triangle').removeClass('down');
                this.setGrade(row.find('.grade-input'));
            },

            [ria.mvc.DomEventBind('click', '.edit-answer-link, .edit-question-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editAnswerClick(node, event){
                var row = node.parent('.row');
                row.find('.edit-answer-block, .edit-question-block').fadeOut(function(){
                    var node = row.find('.edit-answer-input, .edit-question-input');
                    node.removeClass('x-hidden').fadeIn(function(){
                        node.trigger('focus');
                    });
                });
            },

            [ria.mvc.DomEventBind('keyup', '.edit-answer-input, .edit-question-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editAnswerKeyUp(node, event){
                var row = node.parent('.row');
                var button = row.find('.edit-answer-btn, .edit-question-btn');
                if(row.find('.edit-answer-text, .edit-question-text').getHTML() == node.getValue())
                    button.fadeOut();
                else
                    button.fadeIn();
            },

            [ria.mvc.DomEventBind('blur', '.edit-answer-input, .edit-question-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function blurAnswer(node, event){
                var row = node.parent('.row');
                if(!row.find('.edit-answer-btn:visible, .edit-question-btn:visible').exists())
                    row.find('.edit-answer-input, .edit-answer-btn, .edit-question-input, .edit-question-btn').fadeOut(function(){
                        setTimeout(function(){
                            row.find('.edit-answer-block, .edit-question-block').removeClass('x-hidden').fadeIn();
                        }, 500);

                    });
            },

            [ria.mvc.DomEventBind('click', '.comment-grade')],
            [[ria.dom.Dom, ria.dom.Event]],
            function commentClick(node, event){
                var popUp = node.find('.small-pop-up');
                popUp.show();
                var comments = popUp.find('.grading-comments-list');
                if(popUp.find('.comment-input').getValue())
                    comments.hide();
                else
                    comments.show();
                setTimeout(function(){
                    jQuery(popUp.find('textarea').valueOf()).focus();
                }, 10);
            },

            [ria.mvc.DomEventBind('click', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeClick(node, event){
                if(!node.hasClass('with-grid-focus'))
                    node.addClass('with-grid-focus');
            },

            [ria.mvc.DomEventBind('blur', '.disabled-grade')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeBlur(node, event){
                node.removeClass('with-grid-focus');
            },

            [ria.mvc.DomEventBind('click')],
            [[ria.dom.Dom, ria.dom.Event]],
            function wholeDomClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.hasClass('comment-grade') && !target.parent('.comment-grade').exists())
                    this.dom.find(('.small-pop-up:visible')).hide();
            },

            [[ria.dom.Dom, Boolean, Boolean]],
            VOID, function updateItem(node, selectNext_, noStandardUpdates_){
                var row = node.parent('.row');
                var container = row.find('.top-content');
                container.addClass('loading');
                if(!noStandardUpdates_){
                    var standards = row.next().find('.standard-grade');
                    standards.forEach(function(item){
                        item.setValue(row.find('.grade-autocomplete').getValue());
                    });
                    if(standards.valueOf().length)
                        this.updateStandardsInfo(standards);
                }
                var form = row.find('form');
                form.trigger('submit');
                if(selectNext_){
                    setTimeout(function(){
                        var next = row.next().next();
                        var selected = row.parent().find('.row.selected');
                        if(next.exists() && !(selected.exists() && selected.getAttr('index') != row.getAttr('index'))){
                            row.removeClass('selected');
                            next.addClass('selected');
                            jQuery(next.find('.grade-input:not(.with-grid-focus)').valueOf()).focus();
                        }
                    },1);
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                var allScores = [];
                this.setAbleDropStudentScore(model.isAbleDropStudentScore());
                this.setAbleToExempt(model.isAbleToExempt());
                model.getAlternateScores().forEach(function(item){
                    allScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });
                model.getAlphaGrades().forEach(function(item){
                    allScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });

                allScores = allScores.concat(['Incomplete', 'Incomplete (fill all)', 'Late', 'Late (fill all)']);

                if(model.isAbleDropStudentScore()){
                    allScores = allScores.concat(['Dropped', 'Dropped (fill all)']);
                }
                if(model.isAbleToExempt()){
                    allScores = allScores.concat(['Exempt', 'Exempt (fill all)']);
                }
                this.setAllScores(allScores);

                this.setOwner(model.getOwner());
                this.setMaxScore(model.getMaxScore());
                this.setStudentAnnouncements(model.getStudentAnnouncements().getItems());
                this.setApplicationsInGradeView(model.getGradeViewApps());
                this.setApplications(model.getApplications());
                this.setAutoGradeApps(model.getAutoGradeApps());
                this.setAnnouncementId(model.getId());
                this.setGradable(model.isGradable());

                var moving = new ria.dom.Dom('.moving-wrapper');
                if(moving.exists()){
                    this.dom.setCss('display', 'none');
                    setTimeout(function(){
                        this.dom.slideDown(500);
                    }.bind(this), 1);
                    setTimeout(function(){
                        moving.remove();
                    }, 501);
                }

                var dom = this.dom;
                new ria.dom.Dom().on('click.grading_popup', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.grading-input-popup')){
                        dom.find('.grading-input-popup').hide();
                    }
                });
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateItem(allTpl, allModel, msg_) {
                var tpl = new chlk.templates.announcement.StudentAnnouncementsTpl;
                var model = allModel.getStudentAnnouncements();
                this.setStudentAnnouncements(model.getItems());
                var gradedStudentCount = 0, sum = 0, numericGrade;
                model.getItems().forEach(function(item){
                    numericGrade = item.getNumericGradeValue();
                    if(numericGrade || item.getGradeValue()){
                        gradedStudentCount++;
                        sum += (numericGrade || 0);
                    }
                });
                model.setGradedStudentCount(gradedStudentCount);
                if(gradedStudentCount)
                    model.setClassAvg(Math.floor(sum / gradedStudentCount + 0.5));

                tpl.assign(model);
                tpl.options({
                    announcementId: this.getAnnouncementId(),
                    gradable: this.isGradable()
                });
                tpl.renderTo(this.dom.find('.student-announcements-top-panel').empty());
                var itemModel = model.getCurrentItem();
                var itemTpl = new chlk.templates.announcement.StudentAnnouncement;
                itemTpl.assign(itemModel);
                itemTpl.options({
                    maxScore: this.getMaxScore(),
                    readonly: !this.isGradable(),
                    ableDropStudentScore : this.isAbleDropStudentScore(),
                    ableToExempt : this.isAbleToExempt()
                });
                var container = this.dom.find('#grade-container-' + itemModel.getStudentId().valueOf());
                container.empty();
                var topContent = this.dom.find('#top-content-' + itemModel.getStudentId().valueOf());
                topContent.removeClass('loading');
                itemTpl.renderTo(container);
            },

            //TODO copied from GridPage

            ArrayOf(String), 'allScores',

            [[String]],
            ArrayOf(String), function getSuggestedValues(text){
                var text = text.toLowerCase();
                var res = [];
                this.getAllScores().forEach(function(score){
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

            [ria.mvc.DomEventBind('keydown', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeKeyDown(node, event){
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
            Boolean, function gradeKeyUp(node, event){
                var suggestions = [], cell = node.parent('.active-cell');
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                var list = this.dom.find('.autocomplete-list:visible');
                var value = node.getValue();
                if(!value){
                    node.addClass('empty-grade');
                    node.removeClass('error');
                }
                else{
                    node.removeClass('empty-grade');
                }
                if(!isDown && !isUp){
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                        if(!node.hasClass('error')){
                            if(list.exists() && list.find('.see-all').hasClass('hovered'))
                                list.find('.see-all').trigger('click');
                        }
                        return false;
                    }else{
                        var text = node.getValue() ? node.getValue().trim() : '';
                        var parsed = parseFloat(text);
                        if(parsed){
                            node.removeClass('error');
                            if(text && parsed != text){
                                node.addClass('error');
                            }else{
                                this.hideDropDown();
                            }
                        }else{
                            suggestions = text  ? this.getSuggestedValues(text) : [];
                            if(!suggestions.length)
                                node.addClass('error');
                            else{
                                node.removeClass('error');
                                var p = false;
                                suggestions.forEach(function(item){
                                    if(item == node.getValue())
                                        p = true;
                                });
                                if(p){
                                    node.removeClass('not-equals');
                                }else{
                                    node.addClass('not-equals');
                                }
                            }

                            this.updateDropDown(suggestions, node);
                        }
                    }
                    this.updateDropDown(suggestions, node);
                }
                var id = node.getData('id'), value = parseFloat(node.getValue());
                if(value){
                    var maxValue = this.getMaxScore();
                    this.getStudentAnnouncements().forEach(function(item){
                        if(item.getId().valueOf() == id){
                            item.setGradeValue(node.getValue());

                            var flag = node.parent('.grade-block').find('.alert-flag');

                            flag.removeClass(Msg.Error.toLowerCase())
                                .removeClass(Msg.Late.toLowerCase())
                                .removeClass(Msg.Incomplete.toLowerCase())
                                .removeClass(Msg.Absent.toLowerCase())
                                .removeClass(Msg.Multiple.toLowerCase());

                            flag.addClass(item.getAlertClass(maxValue));
                            flag.setData('tooltip', item.getTooltipText(maxValue));
                        }
                    });
                }
                return true;
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
            },

            [ria.mvc.DomEventBind('click', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
                var input = this.dom.find('.row.selected').find('.grade-input');
                input.removeClass('not-equals');
                this.updateDropDown(this.getAllScores(), this.dom.find('.active-cell'), true);
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
                var value = text, isFill = false;
                var input = this.dom.find('.row.selected').find('.grade-input');
                if(text.toLowerCase().indexOf('fill') > -1){
                    isFill = true;
                    value = text.split('(fill all)')[0].trim();
                }
                input.removeClass('not-equals');
                this.setItemValue(value, input, !isFill);
                var that = this;
                if(isFill)
                    this.dom.find('.able-fill-all').forEach(function(node){
                        that.setItemValue(value, node, false);
                    });
                this.hideDropDown();
            },

            [ria.mvc.DomEventBind('change', '.dropped-checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function droppedChange(node, event, options_){
                if(!node.checked()){
                    var input = node.parent('form').find('.grade-autocomplete');
                    input.setValue(input.getData('grade-value'));
                }
            },

//            [ria.mvc.DomEventBind('change', '.undropped-checkbox')],
//            [[ria.dom.Dom, ria.dom.Event, Object]],
//            VOID, function unDroppedChange(node, even, options_){
//                if(node.checked()){
//                    var input = node.parent('.grading-input-popup').find('input[name="dropped"]');
//                    input.setValue(parseInt(!node.checked()));
//                }
//            },

            [ria.mvc.DomEventBind('change', '.exempt-checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function exemptChange(node, event, options_){
                var input = node.parent('form').find('.grade-autocomplete').setValue('');
            },

            function setItemValue(value, input, selectNext){
                input.removeClass('able-fill-all');
                value = value || '';
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase(): {
                        this.setItemState_(input, 'dropped', selectNext);
                    }
                    case Msg.Incomplete.toLowerCase(): this.setItemState_(input, 'isincomplete', selectNext); break;
                    case Msg.Late.toLowerCase(): this.setItemState_(input, 'islate', selectNext); break;
                    case Msg.Exempt.toLowerCase(): this.setItemState_(input, 'isexempt', selectNext); break;
                    default:{
                        var numericValue = parseFloat(value);
                        if(Number.NaN == numericValue){
                            var allScores = this.getAllScores();
                            allScores = allScores.find(function(score){
                                return score.toLowerCase() == value;
                            });
                            if(allScores.length == 0) return;
                        }
                        input.setValue(value);
                        if(value != undefined && value != null && value.trim() != '')
                            this.changeGradingCheckBox_(input.parent('.row'), 'isexempt', false);
                        this.updateItem(input, selectNext);
                    }
                }
            },

            [[ria.dom.Dom, String, Boolean, Boolean]],
            function setItemState_(node, stateName, selectNext_){
                var row = node.parent('.row');
                var input = row.find('.grade-input');
                input.setValue(stateName == 'isexempt' ? '' : input.getData('value'));
                row.find('[name=' + stateName +']').setValue(true);
                this.changeGradingCheckBox_(row, stateName, true);
                this.updateItem(node, selectNext_);
            },

            [[ria.dom.Dom, String, Boolean]],
            function changeGradingCheckBox_(rowItem, checkboxName, state){
                rowItem.find('[name=' + checkboxName +']').setValue(state)
            },

            [ria.mvc.DomEventBind('submit', 'form.update-grade-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submitForm(node, event){
                return node.find('.input-container').find('.error').valueOf().length == 0;
            },

            [ria.mvc.DomEventBind('click', '.grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradingPopUpClick(node, event){
                setTimeout(function(){
                    node.parent('form').find('.grade-autocomplete').trigger('focus');
                }, 1)
            },

            function setCommentByNode(node){
                var popUp = node.parent('.small-pop-up');
                var input = popUp.find('.comment-input');
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

            [ria.mvc.DomEventBind('keyup', '.comment-input')],
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
                                this.updateItem(node, false, true);
                                node.parent('.small-pop-up').hide();
                                node.parent('.comment-grade').find('.comment-text').setHTML(node.getValue() ? Msg.Commented : Msg.Comment);
                                break;
                        }
                }else{
                    if(node.getValue() && node.getValue().trim())
                        popUp.hide();
                    else
                        popUp.show();
                }
            },

            [ria.mvc.DomEventBind('keydown', '.comment-input')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function commentKeyDown(node, event, options_){
                var popUp = node.parent().find('.grading-comments-list');
                if(event.which == ria.dom.Keys.ENTER.valueOf()){
                    if(popUp.is(':visible') && popUp.find('.item').exists()){
                        var selected = popUp.find('.item.selected');
                        if(!selected.exists())
                            selected = popUp.find('.item:first');
                        this.setCommentByNode(selected);
                    }
                    this.updateItem(node, false, true);
                    node.parent('.small-pop-up').hide();
                    node.parent('.comment-grade').find('.comment-text').setHTML(node.getValue() ? Msg.Commented : Msg.Comment);
                }
            }
        ]
    );
});