REQUIRE('chlk.activities.attendance.BaseSummaryPage');
REQUIRE('chlk.templates.attendance.AdminAttendanceSummaryTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceNowTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceDayTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceMpTpl');
REQUIRE('chlk.templates.attendance.AdminStudentsBoxTpl');
REQUIRE('chlk.templates.attendance.AttendanceStudentBoxTpl');
REQUIRE('chlk.templates.attendance.AdminStudentSearchTpl');

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
        'AdminAttendanceSummaryPage', EXTENDS(chlk.activities.attendance.BaseSummaryPage), [

            [[Boolean, String]],
            function getMarkerConfigs_(enabled, color_){
                return enabled ? {
                    enabled: true,
                    symbol: 'circle',
                    radius: 3,
                    fillColor: '#ffffff',
                    lineWidth: 2,
                    lineColor: color_,
                    states: {
                        hover: {
                            radius: 6,
                            lineWidth: 2,
                            enabled: true
                        }
                    }
                } : {
                    enabled: false,
                    states: {
                        hover: {
                            enabled: false
                        }
                    }
                };
            },

            [ria.mvc.DomEventBind('change', '.attendance-type-chart-select')],
                [[ria.dom.Dom, ria.dom.Event, Object]],
                VOID, function selectAttendanceType(node, event, o) {
                    var colors = ['#e49e3c', '#b93838', '#5093a7'];
                    var types = ['late', 'absent', 'excused'];
                    var type = node.getValue();
                    var num = type == chlk.models.attendance.AttendanceTypeEnum.LATE.valueOf() ? 0 :
                        (type == chlk.models.attendance.AttendanceTypeEnum.ABSENT.valueOf() ? 1 : 2);
                    var chart = node.parent().find('.top-chart').getData('chart'), that = this;
                    chart.series.forEach(function(ser, i){
                        if(i == num){
                            ser.update({
                                marker : that.getMarkerConfigs_(true, colors[i]),
                                color: colors[i],
                                zIndex: 10,
                                enableMouseTracking: true
                            });
                        }
                        else{
                            ser.update({
                                marker : that.getMarkerConfigs_(false),
                                color: "#c1c1c1",
                                zIndex: 1,
                                enableMouseTracking: false
                            });
                        }
                    });
                    node.next()
                        .find('.chzn-single span')
                        .removeClass('late')
                        .removeClass('absent')
                        .removeClass('excused')
                        .addClass(types[num]);
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


            OVERRIDE, Date, function getDate(str_,a_,b_){
                return this.getSchoolYearServerDate(str_,a_,b_);
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
                        var incorrectParams = isStart
                            && this.getDate(otherDom.getValue()) < this.getDate(node.getValue())
                            || !isStart && this.getDate(otherDom.getValue()) > this.getDate(node.getValue());
                        if(!otherDom.getValue() || incorrectParams){
                            otherDom.setValue(node.getValue());
                            otherDom.trigger('change');
                        }else{
                            node.parent('form').trigger('submit');
                        }
                    }
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

            [ria.mvc.DomEventBind('change', '.add-new-student')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function addedNewStudent(node, event) {
                    var typesArray = node.getValue().split(',');
                    var student = this.getAddedStudent();
                    var studentBoxModel = new chlk.models.attendance.AttendanceStudentBox(student);
                    var tpl = new chlk.templates.attendance.AttendanceStudentBoxTpl();
                    tpl.assign(studentBoxModel);
                    var html = tpl.render();
                    var that = this;
                    typesArray.forEach(function(elem, pos){
                        if(typesArray.indexOf(elem) == pos){
                            var container = that.dom.find('.all-students.attendance-' + elem);
                            var block = container.find('.visible-block:visible').find('.hidden-students-block-2');
                            if(block.exists() && !block.find('.student.new').exists()){
                                new ria.dom.Dom().fromHTML(html).prependTo(block);
                                if(block.hasClass('need-button'))
                                    new ria.dom.Dom().fromHTML('<button class="all-button new">' + Msg.All + '</button>').appendTo(block);
                                container.find('.hidden-students-block-2').addClass('animate');
                                var secondBlock = container.find('.hidden-students');
                                if(secondBlock.hasClass('need-big-height')){
                                    secondBlock.setCss('height', secondBlock.getCss('height') + 113 + 'px');
                                }
                            }
                        }
                    })
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom('body').off('click.student');
                new ria.dom.Dom('.student.absolute').remove();
            }
        ]);
});