REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.attendance.StudentDayAttendanceTpl');
REQUIRE('chlk.templates.attendance.StudentReasonsComboTpl');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.StudentDayAttendancePopup */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(true)],
        [chlk.activities.lib.isConstantPosition(true)],
        [ria.mvc.ActivityGroup('StudentDayAttendancesPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.StudentDayAttendanceTpl)],
        'StudentDayAttendancePopup', EXTENDS(chlk.activities.lib.TemplatePopup), [
            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.StudentReasonsComboTpl)],
            VOID, function doUpdateItem(tpl, model, msg_) {
                tpl.options({
                    currentReasons: this.getCurrentReasons()
                });
                var container = this.dom.find('.reasons-container.current');
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
                    return item.hasLevel(value);
                });
                this.setCurrentReasons(currentReasons);
                var row = node.parent('.row');
                row
                    .addClass('changed')
                    .find('.reasons-container')
                    .addClass('current');
                this.onPartialRender_(new chlk.models.attendance.StudentDayAttendances, chlk.activities.lib.DontShowLoader());
                this.onPartialRefresh_(new chlk.models.attendance.StudentDayAttendances, chlk.activities.lib.DontShowLoader());
                if(row.hasClass('all-row')){
                    this.changeSelects(node, '.attendance-type-select');
                }
            },

            [[ria.dom.Dom, String]],
            VOID, function changeSelects(node, selector){
                var select = this.dom
                    .find('.row:not(.all-row)')
                    .find(selector);

                select.forEach(function(item){
                    if(item.getValue() != node.getValue()){
                        item
                            .setValue(node.getValue())
                            .trigger('change', {})
                            .trigger('liszt:updated');
                    }
                })
            },

            [ria.mvc.DomEventBind('change', '.attendance-reason-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function reasonChange(node, event, options){
                var row = node.parent('.row');
                row.addClass('changed');
                if(row.hasClass('all-row')){
                    this.changeSelects(node, '.attendance-reason-select');
                }
            },

            [ria.mvc.DomEventBind('click', '.save-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function saveClick(node, event){
                var changed = this.dom.find('.row.changed:not(.all-row)');
                var classIds = [];
                var attendanceTypes = [];
                var attReasons = [];
                changed.forEach(function(node){
                    classIds.push(node.getData('classid'));
                    attendanceTypes.push(node.find('.attendance-type-select').getValue());
                    attReasons.push(node.find('.attendance-reason-select').getValue() || -1);
                });
                this.dom.find('[name="classIds"]').setValue(classIds.join(','));
                this.dom.find('[name="attendanceTypes"]').setValue(attendanceTypes.join(','));
                this.dom.find('[name="attReasons"]').setValue(attReasons.join(','));
                this.dom.find('form').trigger('submit');
                if(this.dom.find('.is-new-student').getValue()){
                    new ria.dom.Dom('.add-new-student')
                        .setValue(attendanceTypes.join(','))
                        .trigger('change');
                }
                this.close();
            },

            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);
                this.setReasons(model.getReasons());
                this.dom.find('.all-select').setValue('');
            }
        ]);
});