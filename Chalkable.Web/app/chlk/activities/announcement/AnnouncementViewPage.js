REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.StudentAnnouncement');
REQUIRE('chlk.templates.classes.TopBar');
REQUIRE('chlk.models.grading.Mapping');

NAMESPACE('chlk.activities.announcement', function () {

    /** @class chlk.activities.announcement.AnnouncementViewPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementView)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementQnAs, 'update-qna', '.questions-and-answers', ria.mvc.PartialUpdateRuleActions.Replace)],
        'AnnouncementViewPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            //ria.dom.Dom, 'currentContainer',

            chlk.models.grading.Mapping, 'mapping',
            Array, 'applicationsInGradeView',
            chlk.models.people.User, 'owner',

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
                    this.updateItem(node, true);
                }
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
                    row.find('.grade-input').setValue('');
                    this.updateItem(node, true);
                }
            },

            [ria.mvc.DomEventBind('click', '#edit-question-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editQuestionClick(node, event){
                var row = node.parent('.row');
                row.find('#edit-question-input').removeClass('x-hidden');
                row.find('#edit-question-btn').removeClass('x-hidden');
                row.find('#edit-question-link').addClass('x-hidden');
                row.find('#edit-question-text').addClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', '#edit-answer-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editAnswerClick(node, event){
                var row = node.parent('.row');
                row.find('#edit-answer-input').removeClass('x-hidden');
                row.find('#edit-answer-btn').removeClass('x-hidden');
                row.find('#edit-answer-link').addClass('x-hidden');
                row.find('#edit-answer-text').addClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', '.comment-grade')],
            [[ria.dom.Dom, ria.dom.Event]],
            function commentClick(node, event){
                var popUp = node.find('.small-pop-up');
                popUp.removeClass('x-hidden');
                setTimeout(function(){
                    jQuery(popUp.find('textarea').valueOf()).focus();
                }, 10);
            },

            [ria.mvc.DomEventBind('click')],
            [[ria.dom.Dom, ria.dom.Event]],
            function wholeDomClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.hasClass('comment-grade') && !target.parent('.comment-grade').exists())
                    this.dom.find(('.small-pop-up:visible')).addClass('x-hidden');
            },

            [[ria.dom.Dom, Boolean]],
            VOID, function updateItem(node, selectNext_){
                var row = node.parent('.row');
                var container = row.find('.top-content');
                container.addClass('loading');
                //this.setCurrentContainer(container.addClass('loading'));
                var form = row.find('form');
                form.triggerEvent('submit');
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
                this.setApplicationsInGradeView(model.getApplications().filter(function(item){return item.applicationviewdata.showingradeview}));
                var that = this;
                jQuery(this.dom.valueOf()).on('change', '.grade-select', function(){
                    var node = new ria.dom.Dom(this);
                    var row = node.parent('.row');
                    row.find('.grade-input').setValue(node.getValue());
                    that.updateItem(node, true);
                });
                //new ria.dom.Dom().on('click', function(){console.info('aaaa');this.wholeDomClick()}.bind(this));
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                //new ria.dom.Dom().off('click', this.wholeDomClick);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.StudentAnnouncement)],
            VOID, function doUpdateItem(tpl, model, msg_) {
                tpl.setOwner(this.getOwner());
                tpl.options({
                    gradingMapping: this.getMapping(),
                    applicationsInGradeView: this.getApplicationsInGradeView(),
                    notAnnouncement: !!this.dom.find('[name=notAnnouncement]').getValue(),
                    readonly: !!this.dom.find('[name=readonly]').getValue(),
                    gradingStyle: parseInt(this.dom.find('[name=gradingStyle]').getValue(), 10)
                });
                var container = this.dom.find('#top-content-' + model.getId().valueOf());
                container.empty();
                tpl.renderTo(container.removeClass('loading'));
                var gradedCount = this.dom.find('.grade-input[value]').count();
                this.dom.find('#graded-count').setHTML(gradedCount.toString());
                /*setTimeout(function(){
                    if(container.parent('.row').hasClass('selected'))
                        jQuery(container.find('.grade-input').valueOf()).focus();
                },1);*/
            },

            //TODO: the same logic as on the feed
            [ria.mvc.DomEventBind('click', 'a.star')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function starAnnouncement(node, event){
                if (node.parent().parent().getAttr("class").indexOf("starred") != -1)
                    node.parent().parent().removeClass("starred");
                else
                    node.parent().parent().addClass("starred");
                return true;
            }
        ]
    );
});