REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.AdminAttendanceSummaryTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceNowTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceDayTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceMpTpl');
REQUIRE('chlk.templates.attendance.AttendanceStudentBoxTpl');
REQUIRE('chlk.templates.attendance.AdminStudentsBoxTpl');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.AdminAttendanceSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('attendance')],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.AdminAttendanceSummaryTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.AdminAttendanceNowTpl, '', '.attendance-now', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.AdminAttendanceDayTpl, '', '.attendance-day', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.AdminAttendanceMpTpl, '', '.attendance-mp', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.attendance.AdminAttendanceSummaryTpl)],
        'AdminAttendanceSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [[ArrayOf(chlk.models.people.User), String, Object, ria.dom.Dom, Boolean, Object, String]],
            VOID, function quicksandStudents(students, selector, tpl, target, needPlus, model, msg_){
                students = students ? (needPlus ? students.slice(0,17) : students.slice(0,18)) : [];
                var allTpl = new chlk.templates.attendance.AdminAttendanceSummaryTpl();
                var studentsTpl = new chlk.templates.attendance.AdminStudentsBoxTpl();
                var students = allTpl.getPreparedStudents(students, needPlus);
                studentsTpl.assign(new chlk.models.attendance.AdminStudentsBox(students));
                studentsTpl.setStudents(students);
                var that = this;
                jQuery(this.dom.find(selector).find('.hidden-students-block-2:eq(0)').valueOf())
                    .quicksand(studentsTpl.render());
                setTimeout(function(){
                    tpl.renderTo(target.empty());
                    setTimeout(function(){
                        that.onPartialRefresh_(model, msg_);
                    }, 1);
                }, 750);
                //studentsTpl.renderTo(this.dom.find(selector).find('.hidden-students-block-2:eq(0)').empty());
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_) {
                var isDay = model.getClass() == chlk.models.attendance.AttendanceDaySummary;
                var isMp = model.getClass() == chlk.models.attendance.AttendanceMpSummary;
                if(isDay || isMp){
                    var rule = this.doFindTemplateForPartialModel_(model, msg_ || '');
                    var tpl = rule.tpl;
                    var target = this.dom;
                    if (rule.selector)
                        target = target.find(rule.selector);
                    tpl.assign(model);
                    if(isDay){
                        this.quicksandStudents(model.getStudentsAbsentWholeDay(), '.whole-day', tpl, target, true, model, msg_);
                        this.quicksandStudents(model.getExcusedStudents(), '.excused-students', tpl, target, true, model, msg_);
                        this.quicksandStudents(model.getAbsentStudents(), '.absent-students', tpl, target, true, model, msg_);
                        this.quicksandStudents(model.getLateStudents(), '.late-students', tpl, target, true, model, msg_);
                    }else{
                        this.quicksandStudents(model.getAbsentAndLateStudents(), '.mp-students', tpl, target, false, model, msg_);
                    }
                }else{
                    BASE(model, msg_);
                }

            },

            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.AttendanceStudentBoxTpl)],
            VOID, function addStudentBox(tpl, model, msg_) {
                tpl.renderTo(this.dom);
            },

            [ria.mvc.DomEventBind('change', '.student-search-input')],
                [[ria.dom.Dom, ria.dom.Event, Object]],
                VOID, function selectStudent(node, event, o) {
                    var form = node.parent().find('.show-student-form');
                    var student = o.selected;
                    var date = new Date(form.find('[name="date"]').getValue());
                    var gradeLevelsIds = form.find('[name="gradeLevelsIds"]').getValue();
                    var currentPage = parseInt(form.find('[name="currentPage"]').getValue(), 10) || 1;
                    var target = node.parent('.student');
                    var top = target.offset().top - this.dom.offset().top;
                    var model = new chlk.models.attendance.AttendanceStudentBox(student,
                        new chlk.models.common.ChlkDate(date), top, currentPage, gradeLevelsIds);
                    node.parent('.small-pop-up').hide();
                    this.onPartialRender_(model);
                    this.onPartialRefresh_(model);
                    setTimeout(function(){
                        new ria.dom.Dom('.student.absolute').find('a.img-container').trigger('click');
                    }, 1);
            },

            [ria.mvc.DomEventBind('click', '.attendance-top-box:not(.active-part)')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function boxClick(node, event) {
                    var index = node.getData('index');
                    this.dom.find('.active-part').removeClass('active-part');
                    this.dom.find('.page-' + index).addClass('active-part');
                    node.addClass('active');
                    var left = node.getAttr('left');
                    var text = node.getAttr('text');
                    var slider = this.dom.find('.slider-container');
                    slider.setCss('left', left);
                    slider.find('.active').setHTML(text);
            },

            [ria.mvc.DomEventBind('click', '.for-date-picker')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function forDatePickerClick(node, event) {
                    this.dom.find('#nowDateTime').trigger('focus');
            },

            [ria.mvc.DomEventBind('click', '.plus')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function plusClick(node, event){
                node.find('.for-plus').show();
                setTimeout(function(){
                    node.find('.student-search-input').trigger('focus');
                }, 1);
            },

            [ria.mvc.DomEventBind('change', '.mp-select')],
                [[ria.dom.Dom, ria.dom.Event, Object]],
                VOID, function mpSelectChange(node, event, option) {
                    var value = node.getValue();
                    if(value){
                        node.next().removeClass('white');
                        this.dom.find('.endDate, .startDate').setValue('');
                        var whiteChosen = this.dom.find('.chzn-container.white');
                        if(whiteChosen.exists()){
                            whiteChosen
                                .find('.chzn-single')
                                .find('>span')
                                .setHTML(node.find('option:selected').text());
                            whiteChosen
                                .removeClass('white')
                                .previous()
                                .setValue(value);
                        }else{
                            var from = this.dom.find('#fromMarkingPeriodId');
                            var to = this.dom.find('#toMarkingPeriodId');
                            var fromIndex = from.find('option:selected').index();
                            var toIndex = to.find('option:selected').index();
                            if(fromIndex > toIndex){
                                var target;
                                if(node.getAttr('id') == 'fromMarkingPeriodId')
                                    target = to;
                                else
                                    target = from;
                                if(target)
                                    target.setValue(value)
                                        .next()
                                        .find('.chzn-single')
                                        .find('>span')
                                        .setHTML(node.find('option:selected').text());
                            }
                        }
                        node.parent('form').trigger('submit');
                    }
            },

            [ria.mvc.DomEventBind('click', '.choose-date-option')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function mpChooseDayClick(node, event) {
                    var chosen = node.parent('.chzn-container');
                    var select = this.dom.find('#' + chosen.getAttr('id').split('_')[0]);
                    var pickerId = select.find('option:selected').getData('picker-id');
                    this.dom.find('#' + pickerId).trigger('focus');
            },

            [ria.mvc.DomEventBind('change', '.endDate, .startDate')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function mpDateChange(node, event) {
                    if(node.getValue()){
                        var isStart = node.hasClass('startDate');
                        var otherDom = isStart ? this.dom.find('.endDate') : this.dom.find('.startDate');
                        var chosenId = node.getData('select-id');
                        this.dom.find('#' + chosenId)
                            .addClass('white')
                            .find('.chzn-single')
                            .find('>span')
                            .setHTML(node.getValue());
                        var incorrectParams = isStart && getDate(otherDom.getValue()) < getDate(node.getValue()) || !isStart && getDate(otherDom.getValue()) > getDate(node.getValue());
                        if(!otherDom.getValue() || incorrectParams){
                            otherDom.setValue(node.getValue());
                            otherDom.trigger('change');
                        }else{
                            node.parent('form').trigger('submit');
                        }
                    }
            },

            [ria.mvc.DomEventBind('click', '.more-students')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function moreStudentsClick(node, event) {
                    var hiddenBlock = node.parent('.attendance-students').find('.hidden-students-block.no-used:eq(0)');
                    var height = hiddenBlock.find('.hidden-students-block-2').height();
                    hiddenBlock
                        .setCss('height', height)
                        .removeClass('no-used');
                    if(hiddenBlock.hasClass('last-block'))
                        node.parent('.more-container').hide();
            },

            [ria.mvc.DomEventBind('change', '#nowDateTime')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function selectedDayChange(node, event) {
                    node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('click', '.admin-attendance')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function domClick(node, event) {
                    this.dom.find('.student.absolute').remove();
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);
                new ria.dom.Dom('body').on('click.student', function(node, event){
                    var target = new ria.dom.Dom(event.target);
                    var box = this.dom.find('.rightnow-usually');
                    if(box.exists() && box.is(':visible')){
                        var container1 = box.find('.box-container:eq(0)');
                        var container2 = box.find('.box-container:eq(1)');
                        if(target.isOrInside('.student:not(.grey)') && target.parent('.attendance-now').exists()){
                            var student = target.is('.student') ? target : target.parent('.student');
                            if(student.getAttr('index') > 0){
                                box.removeClass('usually');
                                container1.find('.number').setHTML(student.getData('count').toString());
                                container1.find('.title').setHTML(student.getData('name'));
                                container2.find('.number').setHTML(box.getData('avg').toString());
                                container2.find('.title').setHTML(Msg.School_Avg);
                            }
                        }else{
                            if(!box.hasClass('usually')){
                                box.addClass('usually');
                                container1.find('.number').setHTML(box.getData('rightnow').toString());
                                container1.find('.title').setHTML(Msg.Right_now);
                                container2.find('.number').setHTML(box.getData('usually').toString());
                                container2.find('.title').setHTML(Msg.Usually);
                            }
                        }
                    }
                }.bind(this))
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom('body').off('click.student');
                new ria.dom.Dom('.student.absolute').remove();
            }
        ]);
});