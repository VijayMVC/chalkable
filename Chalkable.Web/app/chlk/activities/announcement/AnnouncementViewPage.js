REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.StudentAnnouncement');
REQUIRE('chlk.templates.announcement.AnnouncementForStudentAttachments');
REQUIRE('chlk.templates.announcement.AnnouncementGradingPartTpl');
REQUIRE('chlk.templates.announcement.AnnouncementQnAs');
REQUIRE('chlk.templates.classes.TopBar');
REQUIRE('chlk.models.grading.Mapping');

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

            chlk.models.grading.Mapping, 'mapping',
            Array, 'applicationsInGradeView',
            Array, 'applications',
            Array, 'autoGradeApps',
            chlk.models.people.User, 'owner',
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementGradingPartTpl)],
            VOID, function updateGradingPart(tpl, model, msg_) {
                tpl.options({
                    gradeViewApps: this.getApplicationsInGradeView(),
                    readonly: false,
                    userRole: this.getRole(),
                    autoGradeApps: this.getAutoGradeApps(),
                    applications: this.getApplications(),
                    owner: this.getOwner(),
                    announcementId: this.getAnnouncementId()
                });
                var container = this.dom.find('.grading-part');
                container.empty();
                tpl.renderTo(container.removeClass('loading'));
                var grid = this.dom.find('.grades-individual');
                grid.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [grid.find('.row:eq(0)'), 0]);
            },

            [ria.mvc.DomEventBind('keypress', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputClick(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    var value = node.getValue();
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
                    var row = node.parent('.row');
                    this.selectRow(this.dom.find('.grades-individual').find('.row:eq(' + (parseInt(row.getAttr('index'),10) + 1) + ')'));
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
            },

            [ria.mvc.DomEventBind('keypress', '.comment-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function commentPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    this.updateItem(node, false);
                    node.parent('.small-pop-up').addClass('x-hidden');
                }
            },

            [ria.mvc.DomEventBind('click', '.drop-link')],
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

            /*[ria.mvc.DomEventBind('keyup', '.edit-question-input')],
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

            [ria.mvc.DomEventBind('click')],
            [[ria.dom.Dom, ria.dom.Event]],
            function wholeDomClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.hasClass('comment-grade') && !target.parent('.comment-grade').exists())
                    this.dom.find(('.small-pop-up:visible')).hide();
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
                        var next = row.next();
                        if(next.exists()){
                            row.removeClass('selected');
                            next.addClass('selected');
                            jQuery(next.find('.grade-input').valueOf()).focus();
                        }
                    },1);
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                model.getStudentAnnouncements() && this.setMapping(model.getStudentAnnouncements().getMapping());
                this.setOwner(model.getOwner());
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

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.StudentAnnouncement, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateItem(tpl, model, msg_) {
                tpl.setOwner(this.getOwner());
                tpl.options({
                    gradingMapping: this.getMapping(),
                    applicationsInGradeView: this.getApplicationsInGradeView(),
                    readonly: false,
                    gradingStyle: parseInt(this.dom.find('[name=gradingStyle]').getValue(), 10)
                });
                var container = this.dom.find('#top-content-' + model.getId().valueOf());
                container.empty();
                tpl.renderTo(container.removeClass('loading'));
                var gradedCount = this.dom.find('.grade-input[value]').count();
                this.dom.find('#graded-count').setHTML(gradedCount.toString());
            }
        ]
    );
});