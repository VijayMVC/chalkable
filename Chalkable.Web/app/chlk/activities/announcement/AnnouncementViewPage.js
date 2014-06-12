REQUIRE('chlk.activities.lib.TemplatePage');

REQUIRE('chlk.templates.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.StudentAnnouncement');
REQUIRE('chlk.templates.announcement.AnnouncementForStudentAttachments');
REQUIRE('chlk.templates.announcement.AnnouncementGradingPartTpl');
REQUIRE('chlk.templates.announcement.AnnouncementQnAs');
REQUIRE('chlk.templates.announcement.AddStandardsTpl');
REQUIRE('chlk.templates.announcement.AnnouncementViewStandardsTpl');
REQUIRE('chlk.templates.grading.GradingCommentsTpl');
REQUIRE('chlk.templates.classes.TopBar');

REQUIRE('chlk.models.grading.AlertsEnum');

NAMESPACE('chlk.activities.announcement', function () {

    var slideTimeout;

    /** @class chlk.activities.announcement.UpdateAnnouncementItemViewModel*/
    CLASS(
        'UpdateAnnouncementItemViewModel', [
            chlk.models.announcement.Announcement, 'announcement',

            chlk.models.announcement.StudentAnnouncement, 'currentItem',

            [[chlk.models.announcement.Announcement, chlk.models.announcement.StudentAnnouncement]],
            function $(announcement_, currentItem_){
                BASE();
                if(announcement_)
                    this.setAnnouncement(announcement_);
                if(currentItem_)
                    this.setCurrentItem(currentItem_);
            }
        ]);

    /** @class chlk.activities.announcement.UpdateAnnouncementItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementView.jade')],
        [ria.templates.ModelBind(chlk.activities.announcement.UpdateAnnouncementItemViewModel)],
        'UpdateAnnouncementItemTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.Announcement, 'announcement',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.StudentAnnouncement, 'currentItem'
        ]);

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
            Boolean, 'ableToGrade',
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
                    gradable: this.isAbleToGrade()//this.isGradable()
                });
                var container = this.dom.find('.grading-part');
                container.empty();
                tpl.renderTo(container.removeClass('loading'));
                var grid = this.dom.find('.grades-individual');
                grid.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [grid.find('.row:eq(0)'), 0]);
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
                        if(this.setGrade(node, true))
                            this.selectRow(this.dom.find('.grades-individual').find('.row:eq(' + (parseInt(row.getAttr('index'),10) + 1) + ')'));
                        event.preventDefault();
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

            [[ria.dom.Dom, Boolean]],
            function setGrade(node, checkOnly_){
                if(!node.hasClass('error') && !node.hasClass('not-equals')){
                    var value = node.getValue();
                    var savedValue = node.getData('value');
                    var notEquals = value == savedValue || (!value && !savedValue);
                    if(notEquals)
                    node.parent().find('.grading-input-popup').find('.with-value').forEach(function(item){
                        if(item.checked() && !item.getData('value') || !item.checked() && item.getData('value'))
                            notEquals = false;
                    });
                    if(notEquals){
                        var row = node.parent('.row');
                        this.selectNextRow(row);
                        return false;
                    }


                    if(!value || (this.getSuggestedValues(value).length == 0 && Number.isNaN(Number.parseInt(value))))
                        value = '';

                    if(!checkOnly_)
                        this.setItemValue(value, node, true);
                    return true;
                }
                return false;
            },

            function selectRow(row){
                if(row.exists())
                    this.dom.find('.grades-individual').trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [row, parseInt(row.getAttr('index'), 10)]);
                else
                    this.setGrade(this.dom.find('.row.selected').find('.grade-input'));
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

            [ria.mvc.DomEventBind('click', '.back-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            function backClick(node, event){
                history.back();
                return false;
            },

            [ria.mvc.DomEventBind('blur', '.disabled-grade')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeBlur(node, event){
                //node.removeClass('with-grid-focus');
            },

            [ria.mvc.DomEventBind('click', '.grading-input-popup .labeled-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            function checkboxClick(node, event){
                node.parent('.grading-input-popup').addClass('changed');
            },

            [ria.mvc.DomEventBind('click')],
            [[ria.dom.Dom, ria.dom.Event]],
            function wholeDomClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.hasClass('comment-grade') && !target.parent('.comment-grade').exists())
                    this.dom.find(('.small-pop-up:visible')).hide();
            },

            function selectNextRow(row){
                setTimeout(function(){
                    var next = row.next().next();
                    var selected = row.parent().find('.row.selected');
                    if(next.exists() && !(selected.exists() && selected.getAttr('index') != row.getAttr('index'))){
                        row.removeClass('selected');
                        next.addClass('selected');
                        //jQuery(next.find('.grade-input:not(.with-grid-focus)').valueOf()).focus();
                        jQuery(next.find('.grade-input').valueOf()).focus();
                    }
                },1);
            },

            [[ria.dom.Dom, Boolean, Boolean]],
            VOID, function updateItem(node, selectNext_, noStandardUpdates_){
                var row = node.parent('.row');
                var form = row.find('form');
                form.trigger('submit');
                if(selectNext_){
                    this.selectNextRow(row);
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                var allScores = [];
                if(!this.getRole().isStudent()){
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
                }

                this.setOwner(model.getOwner());
                this.setMaxScore(model.getMaxScore());
                this.setStudentAnnouncements(model.getStudentAnnouncements()
                    ? model.getStudentAnnouncements().getItems() :[]);
                this.setApplicationsInGradeView(model.getGradeViewApps());
                this.setApplications(model.getApplications());
                this.setAutoGradeApps(model.getAutoGradeApps());
                this.setAnnouncementId(model.getId());
                this.setGradable(model.isGradable());
                this.setAbleToGrade(model.isAbleToGrade());

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
                        var popUp = node.find('.grading-input-popup:visible');
                        dom.find('.grading-input-popup').hide();
                        if(popUp.hasClass('changed'))
                            popUp.parent('form').trigger('submit');
                    }
                });
            },

            [ria.mvc.PartialUpdateRule(chlk.activities.announcement.UpdateAnnouncementItemTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateItem(allTpl, allModel, msg_) {
                var tpl = new chlk.templates.announcement.StudentAnnouncementsTpl;
                var announcement = allModel.getAnnouncement();
                var model = announcement.getStudentAnnouncements();
                this.setStudentAnnouncements(model.getItems());

                tpl.assign(model);
                tpl.options({
                    announcementId: this.getAnnouncementId(),
                    gradable: this.isAbleToGrade() // this.isGradable()
                });
                tpl.renderTo(this.dom.find('.student-announcements-top-panel').empty());
                var itemModel = allModel.getCurrentItem();
                var itemTpl = new chlk.templates.announcement.StudentAnnouncement;
                itemTpl.assign(itemModel);
                itemTpl.options({
                    maxScore: this.getMaxScore(),
                    readonly: !this.isAbleToGrade(),//!this.isGradable(),
                    ableDropStudentScore : this.isAbleDropStudentScore(),
                    ableToExempt : this.isAbleToExempt()
                });
                var container = this.dom.find('#grade-container-' + itemModel.getStudentId().valueOf());
                if(itemModel.isEmptyGrade()){
                    container.parent('form').addClass('empty-grade-form');
                }
                if(itemModel.needStrikeThrough())
                    container.parent('.row').addClass('dropped-value');
                else
                    container.parent('.row').removeClass('dropped-value');
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

            /*[ria.mvc.DomEventBind('click', '.row.selected')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeInputBlur(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!node.is('.grade-autocomplete') && !node.isOrInside('.action-link'))
                    this.setGrade(node);
            },*/

            [ria.mvc.DomEventBind('keyup', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeKeyUp(node, event){
                var suggestions = [];
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
                var fillItem = node.parent().find('.fill-grade');
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase():
                    case Msg.Exempt.toLowerCase():
                    case Msg.Incomplete.toLowerCase():
                    case Msg.Late.toLowerCase(): fillItem.setAttr('disabled', true);break;
                    default: value ? fillItem.setAttr('disabled', false) : fillItem.setAttr('disabled', true);
                }
                if(!isDown && !isUp){
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                        if(!node.hasClass('error')){
                            if(list.exists() && list.find('.see-all').hasClass('hovered'))
                                list.find('.see-all').trigger('click');
                        }
                        return false;
                    }else{
                        if(value){
                            var text = node.getValue() ? node.getValue().trim() : '';
                            var parsed = parseFloat(text);
                            if(!Number.isNaN(parsed)){
                                node.removeClass('error');
                                node.removeClass('not-equals');
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
                                        if(item.toLowerCase() == node.getValue().toLowerCase())
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
                    }
                    this.updateDropDown(suggestions, node);
                }
                var id = parseInt(node.parent('form').find('[name=studentid]').getValue(), 10), value = parseFloat(node.getValue());
                if(value){
                    var maxValue = this.getMaxScore();
                    this.getStudentAnnouncements().forEach(function(item){
                        if(item.getStudentId().valueOf() == id){
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

            [ria.mvc.DomEventBind('click', '.grade-autocomplete, .grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeInputClick(node, event){
                this.hideDropDown();
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
                        if(Number.isNaN(numericValue) && value){
                            var allScores = this.getAllScores();
                            allScores = allScores.filter(function(score){
                                return score.toLowerCase() == value.toLowerCase();
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
                var res = node.find('.input-container').find('.error').valueOf().length == 0;
                if(res){
                    node.removeClass('empty-grade-form');
                    var row = node.parent('.row');
                    var container = row.find('.top-content');
                    container.addClass('loading');
                    row.find('.grading-input-popup').hide();
                    var input = node.find('.grade-input');
                    var value = (input.getValue() || '').toLowerCase();
                    if(value == 'dropped' || value == 'exempt')
                        input.setValue(input.getData('grade-value'));
                    if(!node.getData('able-drop')){
                        node.find('.dropped-checkbox').setValue(false);
                        node.find('.dropped-hidden').setValue(false);
                    }
                }
                return res;
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
            },

            [ria.mvc.DomEventBind('click', '.cant-drop:checked')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            Boolean, function cantDropClick(node, event){
                return false;
            },

            [ria.mvc.DomEventBind('click', '.fill-grade-container')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function fillGradeClick(node, event){
                var form = node.parent('form');
                form.trigger('submit');
                var input = form.find('input[name=gradevalue]');
                var value = input.getValue();
                if(value  && !input.hasClass('error') && value.toLowerCase() != 'dropped' && value.toLowerCase() != 'exempt')
                    this.dom.find('.empty-grade-form').forEach(function(form){
                        form.find('input[name=gradevalue]').setValue(value);
                        form.trigger('submit');
                    })
            }
        ]
    );
});