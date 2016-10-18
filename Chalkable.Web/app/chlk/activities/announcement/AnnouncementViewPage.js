REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.activities.common.InfoByMpPage');
REQUIRE('chlk.templates.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.StudentAnnouncement');
REQUIRE('chlk.templates.announcement.AnnouncementForStudentAttachments');
REQUIRE('chlk.templates.announcement.AnnouncementGradingPartTpl');
REQUIRE('chlk.templates.announcement.AnnouncementQnAs');
REQUIRE('chlk.templates.standard.AddStandardsTpl');
REQUIRE('chlk.templates.announcement.AnnouncementViewStandardsTpl');
REQUIRE('chlk.templates.grading.GradingCommentsTpl');
REQUIRE('chlk.templates.announcement.admin.AdminAnnouncementGradingTpl');
REQUIRE('chlk.templates.announcement.AnnouncementDiscussionTpl');
REQUIRE('chlk.templates.announcement.AnnouncementCommentTpl');
REQUIRE('chlk.templates.announcement.AnnouncementCommentAttachmentsTpl');
REQUIRE('chlk.templates.LoadingImageTpl');

REQUIRE('chlk.models.grading.AlertsEnum');

NAMESPACE('chlk.activities.announcement', function () {

    var slideTimeout;

    /** @class chlk.activities.announcement.UpdateAnnouncementItemViewModel*/
    CLASS(
        'UpdateAnnouncementItemViewModel', [
            chlk.models.announcement.FeedAnnouncementViewData, 'announcement',

            chlk.models.announcement.StudentAnnouncement, 'currentItem',

            [[chlk.models.announcement.FeedAnnouncementViewData, chlk.models.announcement.StudentAnnouncement]],
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
            chlk.models.announcement.FeedAnnouncementViewData, 'announcement',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.StudentAnnouncement, 'currentItem'
        ]);

    /** @class chlk.activities.announcement.AnnouncementViewPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementView)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.row.selected .grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementForStudentAttachments, 'update-attachments',
            '#attachments-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementDiscussionTpl, 'discussion', '.discussion-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementView, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        //make base page with accordeon support
        'AnnouncementViewPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            Array, 'applicationsInGradeView',
            Array, 'applications',
            Array, 'standards',
            Array, 'autoGradeApps',
            chlk.models.people.User, 'owner',
            chlk.models.id.AnnouncementId, 'announcementId',
            Number, 'maxScore',
            ArrayOf(chlk.models.announcement.StudentAnnouncement), 'studentAnnouncements',
            Boolean, 'gradable',
            Boolean, 'ableToGrade',
            Boolean, 'ableDropStudentScore',
            Boolean, 'ableToExempt',
            Boolean, 'dropped',
            Boolean, 'moreClicked',

            Array, 'zeroPercentageScores',

            [ria.mvc.PartialUpdateRule(null, 'file-for-comment')],
            VOID, function updateCommentFile(tpl, model, msg_) {
                var tpl = new chlk.templates.announcement.AnnouncementCommentAttachmentsTpl(), attachments = model.getAttachments(),
                    attachmentIdNode, ids = attachments.map(function(attachment){return attachment.getId().valueOf()}),
                    oldIds, container;
                tpl.assign(model);
                if(model.getId() && model.getId().valueOf()){
                    var form = this.dom.find('.post-comment-form:visible[data-id=' + model.getId().valueOf() + ']');
                    container = form.find('.imgs-cnt');
                    attachmentIdNode = form.find('.attachment-id');
                }else{
                    container = this.dom.find('.new-comment .imgs-cnt');
                    attachmentIdNode = this.dom.find('.new-comment .attachment-id');
                }
                tpl.renderTo(container);
                oldIds = attachmentIdNode.getValue() ? attachmentIdNode.getValue().split(',') : [];
                ids = oldIds.concat(ids);
                attachmentIdNode.setValue(ids.join(','));
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementQnAs, 'update-qna')],
            VOID, function updateQnAPart(tpl, model, msg_) {
                var res = 0;

                model.getAnnouncementQnAs().forEach(function(item){
                    if(item.getState() == chlk.models.announcement.QnAState.ANSWERED)
                        res+=1;

                    res+=1;
                });

                this.dom.find('.chat-link').setHTML(res ? res.toString() : '');
            },

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
                    gradable: this.isAbleToGrade(),//this.isGradable(),
                    dropped: this.isDropped(),
                    maxScore: this.getMaxScore()
                });
                var container = this.dom.find('.grading-part');
                container.empty();
                tpl.renderTo(container.removeClass('loading'));
                var grid = this.dom.find('.grades-individual');
                grid.trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [grid.find('.row:eq(0)'), 0]);
            },

            [ria.mvc.DomEventBind('click', '.view-auto-grades')],
            [[ria.dom.Dom, ria.dom.Event]],
            function viewAutoGradesClick(node, event){
                node.hide();
                var parent = node.parent('.item');
                parent.siblings('.item').forEach(function(item){
                    item.find('.decline-auto-grades').trigger('click');
                });
                parent.find('.accept-decline').show();
                var id = node.parent('.item').getData('id');
                var app = this.getAutoGradeApps().filter(function(item){return item.id == id})[0];
                var dom = this.dom;
                app.students.forEach(function(student){
                    var block = dom.find('#grade-container-' + student.id);
                    block.addClass('auto-grade');
                    var grade = student.grade;
                    block.find('.text-value').setHTML(grade);
                    block.find('.grade-input').setValue(grade);
                });
                return false;
            },

            [ria.mvc.DomEventBind('click', '.decline-auto-grades')],
            [[ria.dom.Dom, ria.dom.Event]],
            function declineAutoGradesClick(node, event){
                //node.parent('.item').remove();
                node.parent('.item').find('.accept-decline').hide();
                node.parent('.item').find('.view-auto-grades').show();
                this.dom.find('.auto-grade').forEach(function(block){
                    block.removeClass('auto-grade');
                    var grade = block.find('.grade-input').getData('grade-value');
                    if(grade === undefined)
                        grade = '';
                    block.find('.text-value').setHTML(grade);
                    block.find('.grade-input').setValue(grade);
                });
            },

            [ria.mvc.DomEventBind('click', '.accept-auto-grades')],
            [[ria.dom.Dom, ria.dom.Event]],
            function acceptAutoGradesClick(node, event){
                //node.parent('.item').remove();
                node.parent('.item').find('.accept-decline').hide();
                node.parent('.item').find('.view-auto-grades').show();
                this.dom.find('.auto-grade').forEach(function(block){
                    block.removeClass('auto-grade');
                    block.parent('form').trigger('submit');
                });
            },

            [ria.mvc.DomEventBind('click', '.make-visible-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function makeVisibleClick(node, event){
                node.parent().parent().hide();
            },

            [ria.mvc.DomEventBind('change', '.drop-unDrop-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            function changeDropUnDropp(node, event){
                var dropped = node.checked();
                this.setDropped(dropped);
                node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('focus', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeFocus(node, event){
                setTimeout(function(){
                    node.select();
                }, 10);
            },

            [ria.mvc.DomEventBind('click', '.close-open-control .close')],
            [[ria.dom.Dom, ria.dom.Event]],
            function closeClick(node, event){
                this.hideDropDown();
            },

            [ria.mvc.DomEventBind('keypress', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    if(!node.hasClass('error')){
                        var row = node.parent('.row');
                        this.selectNextRow(row);
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

            function selectRow(row){
                if(row.exists())
                    this.dom.find('.grades-individual').trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [row, parseInt(row.getAttr('index'), 10)]);
                //else
                    //this.setGrade(this.dom.find('.row.selected').find('.grade-input'));
            },

            function selectNextRow(row){
                var next = this.dom.find('.grades-individual').find('.row:eq(' + (parseInt(row.getAttr('index'),10) + 1) + ')');
                if(next.exists())
                    this.selectRow(next);
                else
                    row.find('form').trigger('submit');
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_ROW.valueOf(), '.grades-individual')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number, Boolean]],
            function selectStudent(node, event, row, index, noScroll_){
                clearTimeout(slideTimeout);
                slideTimeout = setTimeout(function(){
                    node.find('.attachments-container:eq(' + index + ').with-data').slideDown(500);
                    row.find('.grade-triangle').addClass('down');
                }, 500);
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), '.grades-individual')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            function deSelectStudent(node, event, row, index){
                node.find('.attachments-container:eq(' + index + ')').slideUp(250);
                this.hideDropDown();
                row.find('.grade-triangle').removeClass('down');
                //this.setGrade(row.find('.grade-input'));
            },

            [ria.mvc.DomEventBind('click', '.comment-text')],
            [[ria.dom.Dom, ria.dom.Event]],
            function commentClick(node, event){
                var popUp = node.parent().find('.small-pop-up'),
                    comments = popUp.find('.grading-comments-list'),
                    $textarea = popUp.find('.comment-input');

                popUp.show();

                if($textarea.getValue()) comments.hide();
                else                     comments.show();

                setTimeout(function(){
                    jQuery($textarea.valueOf()).focus();
                }, 1);
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

            [ria.mvc.DomEventBind('click', '.grading-input-popup .labeled-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            function checkboxClick(node, event){
                node.parent('.grading-input-popup').addClass('changed');
            },

            [ria.mvc.DomEventBind('click')],
            [[ria.dom.Dom, ria.dom.Event]],
            function wholeDomClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.isOrInside('.popup-bubble') && !target.hasClass('comment-text') && !target.parent('.comment-text').exists())
                    this.dom.find(('.small-pop-up:visible')).hide();
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                this.dom.find('.open-on-start').trigger('click').removeClass('open-on-start');
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                this.setMoreClicked(false);
                this.setStandards(model.getStandards());

                var allScores = [], zeroPercentageScores = [], classAnnouncement = model.getClassAnnouncementData();
                var lessonPlan = model.getLessonPlanData();
                if(this.getRole().isTeacher() && classAnnouncement){
                    this.setAbleDropStudentScore(classAnnouncement.isAbleDropStudentScore());
                    this.setDropped(classAnnouncement.isDropped());
                    this.setAbleToExempt(classAnnouncement.isAbleToExempt());
                    model.getAlternateScores().forEach(function(item){
                        allScores.push(item.getName());
                        allScores.push(item.getName() + ' (fill all)');
                        if(item.getPercentOfMaximumScore() == 0){
                            zeroPercentageScores.push(item.getName());
                            zeroPercentageScores.push(item.getName() + ' (fill all)');
                        }
                    });
                    model.getAlphaGrades().forEach(function(item){
                        allScores.push(item.getName());
                        allScores.push(item.getName() + ' (fill all)');
                    });

                    allScores = allScores.concat(['Incomplete', 'Incomplete (fill all)', 'Late', 'Late (fill all)']);

                    if(classAnnouncement.isAbleDropStudentScore()){
                        allScores = allScores.concat(['Dropped', 'Dropped (fill all)']);
                    }
                    if(classAnnouncement.isAbleToExempt()){
                        allScores = allScores.concat(['Exempt', 'Exempt (fill all)']);
                    }
                    this.setAllScores(allScores);
                    this.setZeroPercentageScores(zeroPercentageScores);
                }

                this.setOwner(model.getOwner());
                if(classAnnouncement){
                    this.setMaxScore(classAnnouncement.getMaxScore());
                    this.setStudentAnnouncements(model.getStudentAnnouncements()
                        ? model.getStudentAnnouncements().getItems() :[]);
                    this.setApplicationsInGradeView(model.getGradeViewApps());
                    this.setGradable(classAnnouncement.isGradable());
                    this.setAbleToGrade(classAnnouncement.isAbleToGrade());
                    this.setAutoGradeApps(model.getAutoGradeApps());
                }


                this.setApplications(model.getApplications());
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

                var dom = this.dom, that = this;
                new ria.dom.Dom().on('click.grading_popup', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.grading-input-popup')){
                        var popUp = node.find('.grading-input-popup:visible');
                        that.hideGradingPopUp();
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
                    gradable: this.isAbleToGrade(),
                    dropped: this.isDropped(),
                    ableDropStudentScore : this.isAbleDropStudentScore(),
                    LEIntegrated: this.isLEIntegrated()
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
                var container = this.dom.find('#grade-container-' + itemModel.getStudentId().valueOf()).find('.grade-container');
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
                container.find('.grade-input:visible').trigger('focus').trigger('select');
            },

            //TODO copied from GridPage

            ArrayOf(String), 'allScores',

            [[String, ria.dom.Dom]],
            ArrayOf(String), function getSuggestedValues(text, inputNode){
                var text = text.toLowerCase();
                var res = [];
                this.getScores_(inputNode).forEach(function(score){
                    if(score.toLowerCase().indexOf(text) == 0)
                        res.push(score);
                });
                return res;
            },

            [[ria.dom.Dom]],
            ArrayOf(String), function getScores_(inputNode){
                return inputNode.getData('only-zero-score') ? this.getZeroPercentageScores() : this.getAllScores();
            },


            VOID, function updateDropDown(suggestions, node, all_){
                var list = this.dom.find('.autocomplete-list');
                if(suggestions.length || node.hasClass('error')){
                    var html = '';
                    suggestions.forEach(function(item){
                        html += '<div class="autocomplete-item" data-tooltip-type="overflow" data-tooltip="' + item + '">' + item + '</div>';
                    });
                    if(!all_)
                        html += '<div class="autocomplete-item see-all">See all »</div>';
                    var top = node.offset().top - list.parent().offset().top + node.height();
                    var left = node.offset().left - list.parent().offset().left;
                    list.setCss('top', top)
                        .setCss('left', left);
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

            [ria.mvc.DomEventBind('keydown', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeKeyDown(node, event){
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                var list = this.dom.find('.autocomplete-list:visible');
                if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                    if(!node.hasClass('error')){
                        var hovered = list.find('.hovered');
                        if(list.exists() && hovered.exists()){
                            hovered.trigger('mousedown');
                            return false;
                        }
                    }
                    //event.preventDefault();
                }
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
                if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                    return false;
                }
                if(!isDown && !isUp){
                    node.removeClass('not-equals');
                    var onlyZeroScore = node.getData('only-zero-score');
                    if(value){
                        var text = node.getValue() ? node.getValue().trim() : '';
                        var parsed = parseFloat(text);
                        if(!isNaN(parsed)){
                            node.removeClass('error');
                            if(text && parsed != text || parsed > 9999.99 || parsed < -9999.99 || (onlyZeroScore && parsed != 0)){
                                node.addClass('error');
                            }else{
                                this.hideDropDown();
                            }
                        }else{
                            suggestions = text  ? this.getSuggestedValues(text, node) : [];
                            if(!suggestions.length)
                                node.addClass('error');
                            else{
                                node.removeClass('error');
                                var p = false;
                                suggestions.forEach(function(item){
                                    if(item.toLowerCase() == node.getValue().toLowerCase())
                                        p = true;
                                });
                                if(!p){
                                    node.addClass('not-equals');
                                }
                            }

                            this.updateDropDown(suggestions, node);
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
                this.hideDropDown();
                return false;
            },

            [ria.mvc.DomEventBind('click', '.grade-autocomplete, .grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeInputClick(node, event){
                this.hideDropDown();
            },

            OVERRIDE, VOID, function onStop_() {
                this.dom.find('.close-chat-link').trigger('click');
                BASE();
                new ria.dom.Dom().off('click.grading_popup');
            },

            [ria.mvc.DomEventBind('mousedown', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
                var input = this.dom.find('.row.selected').find('.grade-input');
                input.addClass('disabled-submit');
                input.removeClass('not-equals');
                this.updateDropDown(this.getScores_(input), input, true);
                return false;
            },

            [ria.mvc.DomEventBind('dblclick', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputDblClickClick(node, event){
                var input = this.dom.find('.row.selected').find('.grade-input');
                node.removeClass('not-equals');
                node.addClass('disabled-submit');
                this.updateDropDown(this.getScores_(input), node, true);
            },

            [ria.mvc.DomEventBind('mouseover', '.autocomplete-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function itemHover(node, event){
                if(!node.hasClass('hovered'))
                    node.parent().find('.hovered').removeClass('hovered');
                node.addClass('hovered');
            },

            [ria.mvc.DomEventBind('mousedown', '.autocomplete-item:not(.see-all)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function listItemBtnClick(node, event){
                var text = node.getHTML().trim();
                var value = text, isFill = false;
                var row = this.dom.find('.row.selected');
                var input = row.find('.grade-input');
                //input.addClass('disabled-submit');
                if(text.toLowerCase().indexOf('fill') > -1){
                    isFill = true;
                    value = text.split('(fill all)')[0].trim();
                }
                input.removeClass('not-equals')
                     .removeClass('error')
                     .removeClass('disabled-submit');
                input.setValue(value);
                if(isFill){
                    input.parent('form').trigger('submit');
                    this.dom.find('.able-fill-all').forEach(function(node){
                        node.setValue(value);
                        node.parent('form').trigger('submit');
                    });
                }
                else{
                    this.selectNextRow(row);
                }
                this.hideDropDown();
            },

            [ria.mvc.DomEventBind('change', '.dropped-checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function droppedChange(node, event, options_){
                if(!node.checked()){
                    var input = node.parent('form').find('.grade-autocomplete');
                    input.setValue(input.getData('grade-value'));
                    input.removeAttr('readonly');
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
                var input = node.parent('form').find('.grade-autocomplete');
                if(node.checked())
                    input.setValue('');
                else
                    input.setValue(input.getData('grade-value'));
            },

            [[ria.dom.Dom, String, Boolean, Boolean]],
            function setItemState_(node, stateName){
                var form = node.parent('form');
                var input = form.find('.grade-input');
                input.setValue(stateName == 'isexempt' ? '' : input.getData('grade-value'));
                this.changeGradingCheckBox_(form, stateName, true);
            },

            [[ria.dom.Dom, String, Boolean]],
            function changeGradingCheckBox_(form, checkboxName, state){
                form.find('[name=' + checkboxName +']').setValue(state)
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
                setTimeout(function(){
                    input.trigger('focus');
                }, 100)
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

            [ria.mvc.DomEventBind('keydown', '.comment-input')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function commentKeyUp(node, event, options_){
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
                                return false;
                                break;
                            case ria.dom.Keys.DOWN.valueOf():
                                if(selected.next().exists()){
                                    selected.removeClass('selected');
                                    selected.next().addClass('selected');
                                }
                                return false;
                                break;
                            case ria.dom.Keys.ENTER.valueOf():
                                this.setCommentByNode(next);
                                node.parent('form').trigger('submit');
                                node.parent('.small-pop-up').hide();
                                node.parent('.comment-grade').find('.comment-text').setHTML(node.getValue() ? Msg.Commented : Msg.Comment);
                                break;
                        }
                }
                else{
                    if(node.getValue() && node.getValue().trim()) popUp.hide();
                    else popUp.show();

                    var value = (node.getValue() || '').trim();

                    if (ria.dom.Keys.ENTER.valueOf() == event.which && value){
                        node.parent('form').trigger('submit');
                        node.parent('.small-pop-up').hide();
                    }
                }
            },

            /*[ria.mvc.DomEventBind('keydown', '.comment-input')],
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
                    node.parent('form').trigger('submit');
                    node.parent('.small-pop-up').hide();
                    node.parent('.comment-grade').find('.comment-text').setHTML(node.getValue() ? Msg.Commented : Msg.Comment);
                }
            },*/

            [ria.mvc.DomEventBind('change', '.cant-drop')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function cantDropClick(node, event){
                node.setAttr('disabled', 'disabled');
            },

            [ria.mvc.DomEventBind('click', '.fill-grade-container')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function fillGradeClick(node, event){
                if(!node.find('.checkbox').getAttr('disabled')){
                    this.hideGradingPopUp();
                    var form = node.parent('form');
                    form.trigger('submit');
                    var input = form.find('input[name=gradevalue]');
                    var value = input.getValue();
                    if(value && !input.hasClass('error') && value.toLowerCase() != 'dropped' && value.toLowerCase() != 'exempt')
                        this.dom.find('.empty-grade-form').forEach(function(form){
                            form.find('input[name=gradevalue]').setValue(value);
                            form.trigger('submit');
                        });
                }
            },

            [ria.mvc.DomEventBind('change', '.fill-grade')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            Boolean, function fillGradeChange(node, event){
                return false;
            },

            //Grade Submit

            function canUpdate(node, value_){
                if(node.hasClass('error') || node.hasClass('not-equals'))
                    return false;
                var value = value_ || node.getValue();
                var commentInput = node.parent('form').find('.comment-input');
                var savedValue = node.getData('value');
                var equals = value == savedValue || (!value && !savedValue);
                if(equals)
                    node.parent().find('.grading-input-popup').find('.with-value').forEach(function(item){
                        if(item.checked() && !item.getData('value') || !item.checked() && item.getData('value'))
                            equals = false;
                    });
                var oldVal = commentInput.getData('comment'),
                    val = commentInput.getValue();
                if(equals && val != oldVal && (val || oldVal))
                    equals = false;
                return !equals;
            },

            function updateGradeBeforeSubmit(node, value_){
                var value = value_ || node.getValue();
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase(): this.setItemState_(node, 'dropped');break;
                    case Msg.Incomplete.toLowerCase(): this.setItemState_(node, 'isincomplete'); break;
                    case Msg.Late.toLowerCase(): this.setItemState_(node, 'islate'); break;
                    case Msg.Exempt.toLowerCase(): this.setItemState_(node, 'isexempt'); break;
                    default:{
                        if(value != undefined && value != null && value.trim() != '')
                            this.changeGradingCheckBox_(node.parent('form'), 'isexempt', false);
                    }
                }
            },

            [ria.mvc.DomEventBind('submit', 'form.update-grade-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submitForm(node, event){
                var input = node.find('.grade-input');
                var commentInput = node.find('.comment-input');

                if(input.hasClass('processing'))
                    return false;

                var res = node.find('.input-container').find('.error').valueOf().length == 0;
                if(res){
                    var value = (input.getValue() || '').toLowerCase(),
                        oldValue = input.getData('grade-value'),
                        droppedValue = node.find('[type=checkbox][name=dropped]').checked(),
                        oldDroppedValue = input.getData('dropped'),
                        lateValue = node.find('[type=checkbox][name=islate]').checked(),
                        oldLateValue = input.getData('late'),
                        incompleteValue = node.find('[type=checkbox][name=isincomplete]').checked(),
                        oldIncompleteValue = input.getData('incomplete'),
                        exemptValue = node.find('[type=checkbox][name=isexempt]').checked(),
                        oldExemptValue = input.getData('exempt'),
                        commentValue = (commentInput.getValue() || '').trim(),
                        oldCommentValue = (commentInput.getData('comment') || '').trim(),
                        changed = false;

                    if(value != oldValue && !(!value && !oldValue))
                        changed = true;
                    if(droppedValue && !oldDroppedValue || !droppedValue && oldDroppedValue)
                        changed = true;
                    if(lateValue && !oldLateValue || !lateValue && oldLateValue)
                        changed = true;
                    if(incompleteValue && !oldIncompleteValue || !incompleteValue && oldIncompleteValue)
                        changed = true;
                    if(exemptValue && !oldExemptValue || !exemptValue && oldExemptValue)
                        changed = true;
                    if(commentValue != oldCommentValue /*&& !(!commentValue && !oldCommentValue)*/)
                        changed = true;

                    if(!changed)
                        return false;

                    console.info(input.valueOf(), value, oldValue);
                    this.hideDropDown();
                    this.hideGradingPopUp();
                    if(!this.canUpdate(input))
                        return false;
                    if(value)
                        input.removeClass('able-fill-all');
                    if(value)
                        this.updateGradeBeforeSubmit(input, value);
                    node.removeClass('empty-grade-form');
                    var row = node.parent('.row');
                    var container = row.find('.top-content');
                    container.addClass('loading');
                    if(!node.getData('able-drop')){
                        node.find('.dropped-checkbox').setValue(false);
                        node.find('.dropped-hidden').setValue(false);
                    }
                    var commentInput = node.find('.comment-input');
                    var comment = (commentInput.getValue() || '').trim();
                    commentInput.setValue(comment);
                    commentInput.setData('comment', comment);
                    node.find('.comment-text').setHTML(comment ? Msg.Commented : Msg.Comment.toString());
                    input.addClass('processing');
                }
                return res;
            },

            [ria.mvc.DomEventBind('mousedown', '.grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeInputPopUpMouseDown(node, event){
                node.parent().find('.grade-input').addClass('disabled-submit');
            },

            [ria.mvc.DomEventBind('click', '.grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeInputPopUpClick(node, event){
                node.parent().find('.grade-input').removeClass('disabled-submit');
            },

            Boolean, 'noGradeSave',

            [ria.mvc.DomEventBind('mousedown', '.drop-unDrop-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            function dropUndropMouseDown(node, event){
                this.setNoGradeSave(true);
            },

            [ria.mvc.DomEventBind('blur', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeInputBlur(node, event){
                if(!this.isNoGradeSave() && !node.hasClass('disabled-submit') && !node.parent('form').find('.small-pop-up:visible').exists()){
                    node.parent('form').trigger('submit');
                }

                this.setNoGradeSave(false);
            },

            [ria.mvc.DomEventBind('click', '.attribute-title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseClick(node, event){
                var parent = node.parent('.attribute-item-container');

                var attrData = parent.find('.mp-data');
                var container = attrData.find('.attribute-details');
                jQuery(attrData.valueOf()).animate({
                    height: parent.hasClass('open') ? 0 : (container.height() + parseInt(container.getCss('margin-bottom'), 10))
                }, 500);

                if(parent.hasClass('open')){
                    this.closeBlock(parent);
                }else{
                    var item = this.dom.find('.attribute-item-container.open');
                    jQuery(item.find('.mp-data').valueOf()).animate({
                        height: 0
                    }, 500);
                    this.closeBlock(item);
                    parent.addClass('open');
                }
            },

            function closeBlock(node){
                setTimeout(function(){
                    node.removeClass('open');
                }, 500);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.admin.AdminAnnouncementGradingTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function usersForGridAppend(tpl, model, msg_) {
                tpl.options({
                    applications: this.getApplications(),
                    standards: this.getStandards()
                });

                var grid = this.dom.find('.people-list');

                ria.dom.Dom(tpl.render()).appendTo(grid);
                grid.trigger(chlk.controls.GridEvents.UPDATED.valueOf());
                setTimeout(function(){
                    if(!model.getItems().length)
                        this.dom.find('#people-list-form').trigger(chlk.controls.FormEvents.DISABLE_SCROLLING.valueOf());
                }.bind(this), 1);
            },

            // ------- DISCUSSION --------

            [ria.mvc.DomEventBind('click', '.comment-cancel')],
            [[ria.dom.Dom, ria.dom.Event]],
            function commentCancelClick(node, event){
                var form = node.parent('form:not(.edit-form)');
                form.find('.comment-value').setValue('');
                form.find('.attachment-id').setValue('');
                form.find('.img-cnt').setHTML('');
                node.closest('.qna')
                    .removeClass('for-reply')
                    .removeClass('for-edit');
            },

            [ria.mvc.DomEventBind('click', '.x-remove-icon')],
            [[ria.dom.Dom, ria.dom.Event]],
            function commentRemoveClick(node, event){
                node.parent('.chat-bubble').addClass('for-delete');
            },

            [ria.mvc.DomEventBind('click', '.delete-cancel')],
            [[ria.dom.Dom, ria.dom.Event]],
            function commentRemoveCancelClick(node, event){
                node.parent('.for-delete').removeClass('for-delete');
            },

            [ria.mvc.DomEventBind('click', '.reply-icon')],
            [[ria.dom.Dom, ria.dom.Event]],
            function replyClick(node, event){
                node.closest('.qna').addClass('for-reply').removeClass('for-edit');
            },

            [ria.mvc.DomEventBind('click', '.edit-grey-icon')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editCommentClick(node, event){
                node.closest('.qna').addClass('for-edit').removeClass('for-reply');
            },

            [ria.mvc.DomEventBind('click', '.delete-attachment')],
            [[ria.dom.Dom, ria.dom.Event]],
            function deleteCommentAttachmentClick(node, event){
                var idsNode = node.parent('form').find('.attachment-id'),
                    ids = idsNode.getValue().split(','), currentId = node.getData('id').toString();

                ids.splice(ids.indexOf(currentId), 1);
                idsNode.setValue(ids.length ? ids.join(',') : '');
                node.parent('.img-cnt').removeSelf();
            },

            [ria.mvc.DomEventBind('click', '.edit-form .comment-cancel')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editCancelClick(node, event){
                var parent = node.closest('.qna'),
                    editForm = parent.find('>FORM.edit-form'),
                    idNode = editForm.find('.attachment-id'),
                    textArea = editForm.find('.comment-value'),
                    imgCnt = editForm.find('.imgs-cnt');
                idNode.setValue(idNode.getData('value'));
                textArea.setValue(textArea.getData('value'));
                imgCnt.setHTML(parent.find('>DIV.chat-bubble').find('.imgs-cnt').getHTML());
            }
        ]
    );
});