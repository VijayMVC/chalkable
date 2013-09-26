REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.attendance.StudentDayAttendanceTpl');
REQUIRE('chlk.templates.attendance.StudentReasonsComboTpl');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.StudentDayAttendancePopup */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [ria.mvc.ActivityGroup('StudentDayAttendancesPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.StudentDayAttendanceTpl)],
        'StudentDayAttendancePopup', EXTENDS(chlk.activities.lib.TemplatePopup), [
            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.StudentReasonsComboTpl)],
            VOID, function doUpdateItem(tpl, model, msg_) {
                tpl.options({
                    currentReasons: this.getCurrentReasons()
                });
                var container = this.dom.find('.attendance-column.wide.reasons.current');
                container
                    .empty()
                    .removeClass('current');
                tpl.renderTo(container);
                container
                    .find('select')
                    .setValue('');
            },

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'currentReasons',

            [ria.mvc.DomEventBind('change', '.attendance-type-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function typeChange(node, event, options){
                var value = node.getValue();
                var currentReasons = this.getReasons().filter(function(item){
                    return item.getAttendanceType() == value;
                });
                this.setCurrentReasons(currentReasons);
                node
                    .parent('.row')
                    .addClass('changed')
                    .find('.attendance-column.wide.reasons')
                    .addClass('current');
                this.onPartialRender_(new chlk.models.attendance.StudentDayAttendances, chlk.activities.lib.DontShowLoader());
                this.onPartialRefresh_(new chlk.models.attendance.StudentDayAttendances, chlk.activities.lib.DontShowLoader());
            },

            [ria.mvc.DomEventBind('change', '.attendance-reason-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function reasonChange(node, event, options){
                node
                    .parent('.row')
                    .addClass('changed');
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setReasons(model.getReasons());
            },

            OVERRIDE, VOID, function onStop_() {
                var changed = this.dom.find('.row.changed');
                if(changed.count()){
                    var base = BASE;
                    var classPersonIds = [];
                    var classPeriodIds = [];
                    var attendanceTypes = [];
                    var attReasons = [];
                    changed.forEach(function(node){
                        classPersonIds.push(node.getData('classpersonid'));
                        classPeriodIds.push(node.getData('classperiodid'));
                        attendanceTypes.push(node.find('.attendance-type-select').getValue());
                        attReasons.push(node.find('.attendance-reason-select').getValue() || -1);
                    });
                    this.dom.find('[name="classPersonIds"]').setValue(classPersonIds.join(','));
                    this.dom.find('[name="classPeriodIds"]').setValue(classPeriodIds.join(','));
                    this.dom.find('[name="attendanceTypes"]').setValue(attendanceTypes.join(','));
                    this.dom.find('[name="attReasons"]').setValue(attReasons.join(','));
                    this.dom.find('form').trigger('submit');
                    setTimeout(function(){
                        base();
                    }, 10)
                }else{
                    BASE();
                }
            }
        ]);
});