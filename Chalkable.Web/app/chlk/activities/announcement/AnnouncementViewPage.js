REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.StudentAnnouncement');
REQUIRE('chlk.templates.announcement.AnnouncementForStudentAttachments');
REQUIRE('chlk.templates.announcement.AnnouncementGradingPartTpl');
REQUIRE('chlk.templates.announcement.AnnouncementQnAs');
REQUIRE('chlk.templates.classes.TopBar');
REQUIRE('chlk.models.grading.AlertsEnum');

NAMESPACE('chlk.activities.announcement', function () {

    var slideTimeout;

    /** @class chlk.activities.announcement.AnnouncementViewPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementView)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementQnAs, 'update-qna', '.questions-and-answers', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementForStudentAttachments, 'update-attachments',
            '.student-attachments', ria.mvc.PartialUpdateRuleActions.Replace)],
        'AnnouncementViewPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            Array, 'applicationsInGradeView',
            Array, 'applications',
            Array, 'autoGradeApps',
            chlk.models.people.User, 'owner',
            chlk.models.id.AnnouncementId, 'announcementId',
            Number, 'maxScore',


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
                    ableDropStudentScore : this.isAbleDropStudentScore()
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
                        this.selectRow(this.dom.find('.grades-individual').find('.row:eq(' + (parseInt(row.getAttr('index'),10) + 1) + ')'));
                        this.setGrade(node);
                    }
                }
            },

            [ria.mvc.DomEventBind('keyup', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyUp(node, event){
                var row = node.parent('.row');
                var value = node.getValue();
                var item = row.find('.fill-item');
                if(!value){
                    item.addClass('disabled');
                    node.addClass('empty-grade');
                    node.removeClass('error');
                }
                else{
                    item.removeClass('disabled');
                    node.removeClass('empty-grade');
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

            [ria.mvc.DomEventBind('click', '.alerts-pop-up-item:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function alertClick(node, event){
                var alertsEnum = chlk.models.grading.AlertsEnum;
                var type = new alertsEnum(node.getData('type'));
                switch(type){
                    case alertsEnum.FILL: var gradeNode = this.dom.find('.empty-grade');
                        this.dom.find('.empty-grade').setValue(node.parent('.row').find('.grade-input').getValue());
                        this.setGrade(gradeNode);
                        break;
                    case alertsEnum.DROP: this.dropItem(node); break;
                    case alertsEnum.INCOMPLETE: this.setItemState_(node, 'isincomplete'); break;
                    case alertsEnum.LATE: this.setItemState_(node, 'islate'); break;
                    case alertsEnum.ABSENT: this.setItemState_(node, 'isabsent'); break;
                    case alertsEnum.EXEMPT: this.setItemState_(node, 'isexempt'); break;
                }
                new ria.dom.Dom('.alerts-pop-up:visible').hide();
            },

            [[ria.dom.Dom]],
            function setGrade(node){
                if(!node.hasClass('error')){
                    var value = node.getValue();
                    var savedValue = node.getData('value');
                    if(value == savedValue || (!value && !savedValue))
                        return;
                    this.updateItem(node, true);
                }
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

            [ria.mvc.DomEventBind('keypress', '.comment-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function commentPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    this.updateItem(node, false);
                    node.parent('.small-pop-up').hide();
                    node.parent('.comment-grade').find('.comment-text').setHTML(node.getValue() ? Msg.Commented : Msg.Comment);
                }
            },

            [[ria.dom.Dom, String, Boolean]],
            function setItemState_(node, stateName, selectNext_){
                var row = node.parent('.row');
                var input = row.find('.grade-input');
                input.setValue(input.getData('value'));
                row.find('[name=' + stateName +']').setValue(true);
                this.updateItem(node, selectNext_);
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

            [ria.mvc.DomEventBind('click', '.grade-triangle')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeTriangleClick(node, event){
                setTimeout(function(){
                    node.parent('.row').find('.alerts-pop-up').show();
                }, 10);
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
                var popUp = new ria.dom.Dom('.alerts-pop-up:visible');
                if(popUp.exists() && !target.isOrInside('.alerts-pop-up'))
                    popUp.hide();
            },

            [[ria.dom.Dom, Boolean]],
            VOID, function updateItem(node, selectNext_){
                var row = node.parent('.row');
                var container = row.find('.top-content');
                container.addClass('loading');
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
                model.getAlternateScores().forEach(function(item){
                    allScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });
                model.getAlphaGrades().forEach(function(item){
                    allScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });

                allScores = allScores.concat(['Exempt', 'Exempt (fill all)', 'Incomplete', 'Incomplete (fill all)',
                    'Late', 'Late (fill all)', 'Dropped', 'Dropped (fill all)', 'Absent', 'Absent (fill all)']);
                this.setAllScores(allScores);

                this.setOwner(model.getOwner());
                this.setMaxScore(model.getMaxScore());
                this.setApplicationsInGradeView(model.getGradeViewApps());
                this.setApplications(model.getApplications());
                this.setAutoGradeApps(model.getAutoGradeApps());
                this.setAnnouncementId(model.getId());
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
                //new ria.dom.Dom().on('click', function(){console.info('aaaa');this.wholeDomClick()}.bind(this));
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                //new ria.dom.Dom().off('click', this.wholeDomClick);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateItem(allTpl, allModel, msg_) {//console.info('doUpdateItem', allModel.getStudentAnnouncements().getCurrentItem().getId().valueOf());
                var tpl = new chlk.templates.announcement.StudentAnnouncementsTpl;
                var model = allModel.getStudentAnnouncements();
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
                    announcementId: this.getAnnouncementId()
                });
                tpl.renderTo(this.dom.find('.student-announcements-top-panel').empty());
                var itemModel = model.getCurrentItem();
                var itemTpl = new chlk.templates.announcement.StudentAnnouncement;
                itemTpl.assign(itemModel);
                itemTpl.options({
                    maxScore: this.getMaxScore()
                });
                //console.info('container', this.dom.find('#grade-container-' + itemModel.getId().valueOf()).valueOf(), this.dom.find('#top-content-' + itemModel.getId().valueOf()).valueOf());
                var container = this.dom.find('#grade-container-' + itemModel.getId().valueOf());
                container.empty();
                this.dom.find('#top-content-' + itemModel.getId().valueOf()).removeClass('loading');
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

            [ria.mvc.DomEventBind('keyup', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradeKeyUp(node, event){
                var suggestions = [], cell = node.parent('.active-cell');
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
                    }
                }else{
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf() && !node.hasClass('error')){
                        if(list.exists() && list.find('.see-all').hasClass('hovered')){
                            list.find('.see-all').trigger('click');
                            return false;
                        }
                        else
                            this.setValue(cell);
                    }else{
                        var text = node.getValue() ? node.getValue().trim() : '';
                        var parsed = parseInt(text,10);
                        if(parsed){
                            node.removeClass('error');
                            if(parsed != text){
                                node.addClass('error');
                            }else{
                                this.hideDropDown();
                            }
                        }else{
                            suggestions = text  ? this.getSuggestedValues(text) : [];
                            if(!suggestions.length)
                                node.addClass('error');
                            else
                                node.removeClass('error');
                            this.updateDropDown(suggestions, node);
                        }
                    }
                    this.updateDropDown(suggestions, node);
                }
            },

            [ria.mvc.DomEventBind('click', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
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
                if(text.toLowerCase().indexOf('fill')){
                    isFill = true;
                    value = text.split('(fill all)')[0].trim();
                }
                this.setItemValue(value, input, !isFill);
                var that = this;
                if(isFill)
                    this.dom.find('.able-fill-all').forEach(function(node){
                        that.setItemValue(value, node, false);
                    });
                this.hideDropDown();
            },

            function setItemValue(value, input, selectNext){
                input.removeClass('able-fill-all');
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase(): this.setItemState_(input, 'dropped', selectNext); break;
                    case Msg.Incomplete.toLowerCase(): this.setItemState_(input, 'isincomplete', selectNext); break;
                    case Msg.Late.toLowerCase(): this.setItemState_(input, 'islate', selectNext); break;
                    case Msg.Absent.toLowerCase(): this.setItemState_(input, 'isabsent', selectNext); break;
                    case Msg.Exempt.toLowerCase(): this.setItemState_(input, 'isexempt', selectNext); break;
                    default: input.setValue(value);this.updateItem(input, selectNext);
                }
            }
        ]
    );
});