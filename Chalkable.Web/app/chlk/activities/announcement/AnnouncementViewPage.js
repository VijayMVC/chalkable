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
            //ria.dom.Dom, 'currentContainer',

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

            function getGradeNumber(value){
                return value;
            },

            [ria.mvc.DomEventBind('keyup', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyUp(node, event){
                var row = node.parent('.row');
                var value = node.getValue();
                var item = row.find('.fill-item');
                if(parseInt(value, 10) == value){
                    if(value > this.getMaxScore()){
                        row.find('.alert-flag').addClass('error');
                    }else{
                        row.find('.alert-flag').removeClass('error');
                    }
                }else{
                    row.find('.alert-flag').removeClass('error');
                }
                if(!value){
                    item.addClass('disabled');
                    node.addClass('empty-grade');
                    node.removeClass('error');
                }
                else{
                    item.removeClass('disabled');
                    node.removeClass('empty-grade');
                    var valueFromLetter = this.getGradeNumber(value);
                    if(parseInt(value, 10) != value && !valueFromLetter && value != Msg.Dropped && value != Msg.Exempt || parseInt(value, 10) == value && value > this.getMaxScore()){
                        node.addClass('error');
                        if(value > this.getMaxScore()){
                            row.find('.alert-flag').addClass('error');
                        }
                    }
                    else{
                        node.removeClass('error');
                        if(valueFromLetter)
                            node.setValue(valueFromLetter);
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
                    if(value == node.getData('value'))
                        return;
                    value = parseInt(value, 10);
                    if(!value && value != 0)
                        value = '';
                    if(value){
                        if(value > 100)
                            value = 100;
                        if(value < 0)
                            value = 0;
                    }
                    node.setValue(value);
                    if(value != node.getData('value'))
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

            [[ria.dom.Dom]],
            function dropItem(node){
                var row = node.parent('.row');
                row.find('[name=dropped]').setValue(true);
                this.updateItem(node, true);
            },

            [[ria.dom.Dom, String]],
            function setItemState_(node, stateName){
                var row = node.parent('.row');
                row.find('[name=' + stateName +']').setValue(true);
                this.updateItem(node, true);
            },


            /*[ria.mvc.DomEventBind('click', '.drop-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function dropClick(node, event){
                var row = node.parent('.row');
                if(node.hasClass('undrop')){
                    row.find('[name=dropped]').setValue(false);
                    this.updateItem(node, false);
                }else{
                    row.find('[name=dropped]').setValue(true);
                    //row.find('.grade-input').setValue('');
                    this.updateItem(node, true);
                }
            },

            [ria.mvc.DomEventBind('keyup', '.edit-question-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editQnASubmitClick(node, event){
                var f = node.parent('.row').parent('form');
                if (!node.getValue())
                    f.find('.edit-answer-input').removeClass('validate[required]');
                else
                    f.find('.edit-answer-input').addClass('validate[required]');
            },

            [ria.mvc.DomEventBind('click', '.edit-question-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editQuestionClick(node, event){
                var row = node.parent('.row');
                row.find('.edit-question-link, .edit-question-text').fadeOut(function(){
                    row.find('.edit-question-input, .edit-question-btn, .cancel-question-link').removeClass('x-hidden').fadeIn();
                });
            },


            [ria.mvc.DomEventBind('click', '.cancel-question-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function cancelQuestionClick(node, event){
                var row = node.parent('.row');
                row.find('.edit-question-input, .edit-question-btn, .cancel-question-link').removeClass('x-hidden').fadeOut(function(){
                    row.find('.edit-question-link, .edit-question-text').fadeIn();
                });
            },*/

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
                //this.setCurrentContainer(container.addClass('loading'));
                var form = row.find('form');
                form.trigger('submit');
                if(selectNext_){
                    setTimeout(function(){
                        var next = row.next().next();
                        if(next.exists()){
                            row.removeClass('selected');
                            next.addClass('selected');
                            jQuery(next.find('.grade-input:not(.with-grid-focus)').valueOf()).focus();
                        }
                    },1);
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setOwner(model.getOwner());
                this.setMaxScore(model.getMaxScore());
                this.setApplicationsInGradeView(model.getGradeViewApps());
                this.setApplications(model.getApplications());
                this.setAutoGradeApps(model.getAutoGradeApps());
                this.setAnnouncementId(model.getId());
                var that = this;
                jQuery(this.dom.valueOf()).on('change', '.grade-select', function(){
                    var node = new ria.dom.Dom(this);
                    var row = node.parent('.row');
                    row.find('.grade-input').setValue(node.getValue());
                    that.updateItem(node, true);
                });
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
            VOID, function doUpdateItem(allTpl, allModel, msg_) {
                var tpl = new chlk.templates.announcement.StudentAnnouncementsTpl;
                var model = allModel.getStudentAnnouncements();
                var gradedStudentCount = 0;
                model.getItems().forEach(function(item){
                    if(!item.isGradeDisabled() && item.getGradeValue())
                        gradedStudentCount++;
                });
                model.setGradedStudentCount(gradedStudentCount);
                tpl.assign(model);
                tpl.options({
                    announcementId: this.getAnnouncementId()
                });
                tpl.renderTo(this.dom.find('.student-announcements-top-panel').empty());
                var itemModel = model.getCurrentItem();
                var itemTpl = new chlk.templates.announcement.StudentAnnouncement;
                itemTpl.assign(itemModel);
                var container = this.dom.find('#grade-container-' + itemModel.getId().valueOf());
                container.empty();
                this.dom.find('#top-content-' + itemModel.getId().valueOf()).removeClass('loading');
                itemTpl.renderTo(container);
                var grades = this.dom.find('.grade-input[value]').valueOf()
                    .map(function(_){return parseInt((new ria.dom.Dom(_)).getValue());});
                var gradedCount = grades.length;
                this.dom.find('#graded-count').setHTML(gradedCount.toString());
                var classAvg = 0;
                for(var i = 0; i < gradedCount; i++)
                    classAvg += grades[i];
                this.dom.find('#class-avg').setHTML(Math.round(classAvg / gradedCount).toString());
            }
        ]
    );
});