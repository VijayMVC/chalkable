REQUIRE('chlk.activities.attendance.BasePostAttendancePage');
REQUIRE('chlk.templates.attendance.ClassAttendanceTpl');
REQUIRE('chlk.templates.attendance.ClassList');
REQUIRE('chlk.templates.common.InfoMsg');

NAMESPACE('chlk.activities.attendance', function () {
    "use strict";

    /** @class chlk.activities.attendance.ClassListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.ClassList)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassList, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassListPage', EXTENDS(chlk.activities.attendance.BasePostAttendancePage), [


            function $(){
                BASE();
                this._typesEnum = chlk.models.attendance.AttendanceTypeEnum;
                this._comboTimer = null;
                this._gridEvents = chlk.controls.GridEvents;
                this._LEFT_ARROW = 'left-arrow';
                this._RIGHT_ARROW = 'right-arrow';
                this._reasons = null;
                this._classAttendances = null;
                this._currentStudentAtt = null;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassAttendanceTpl)],
            VOID, function doUpdateItem(tpl, model, msg_) {
                this._needPopUp = true;
                this.dom.find('.keyboard-suggestion').hide();
                var container = this.dom.find('.container-' + model.getStudent().getId().valueOf());
                container.empty();
                tpl.renderTo(container);
                var row = container.parent('.row');
                row.find('.student-attendance-container').removeClass('active');
                if(row.hasClass('selected') && !row.hasClass('reason-changed')){
                    this.showDropDown();
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.KEY_DOWN.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event, Number]],
            VOID, function gridKeyDownSelect(node, event, key_) {
                switch(event.which){
                    case ria.dom.Keys.ENTER.valueOf(): node.trigger(this._gridEvents.SELECT_NEXT_ROW.valueOf());break;
                    case ria.dom.Keys.LEFT.valueOf(): node.find('.row.selected').find('.' + this._LEFT_ARROW).trigger('click');break;
                    case ria.dom.Keys.RIGHT.valueOf(): node.find('.row.selected').find('.' + this._RIGHT_ARROW).trigger('click');break;
                    case ria.dom.Keys.ESC.valueOf(): this.hideDropDown();break;
                }
            },

            VOID, function hideDropDown(){
                var dropDown = this.dom.find('.combo-list:visible');
                if(dropDown.exists()){
                    dropDown.hide();
                    dropDown.parent('.student-attendance-container').removeClass('active');
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.FOCUS.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gridFocus(node, event) {
                this.hideDropDown();
            },

            [ria.mvc.DomEventBind('mouseover', '.combo-list .option')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function optionHover(node, event) {
                var parent = node.parent('.combo-list');
                if(!node.hasClass('selected')){
                    parent.find('.selected').removeClass('selected');
                    node.addClass('selected');
                    parent.find('input.combo-input').trigger('focus');
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.AFTER_ROW_SELECT.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            VOID, function rowSelect(node, event, row_, index_) {
                if(row_ && row_.find('input[name=type]').getValue() == this._typesEnum.NA.valueOf()){
                    row_.find('input[name=type]').setValue(this._typesEnum.PRESENT.valueOf());
              //      row_.find('form').trigger('submit');
                }else{
                    this.showDropDown();
                }
            },

            [ria.mvc.DomEventBind('focus', '.combo-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function focusDropDown(node, event) {
                event.stopPropagation();
            },

            VOID, function showDropDown(){
                clearTimeout(this._comboTimer);
                var row = this.dom.find('#class-attendance-list-panel').find('.row.selected');
                this._comboTimer = setTimeout(function(){
                    var list = row.find('.combo-list:hidden');
                    if(row.hasClass('selected') && list.exists()){
                        row.find('.combo-list').show();
                        row.find('.student-attendance-container').addClass('active');
                        row.find('input.combo-input').trigger('focus');
                    }
                }, 500);
            },

            [ria.mvc.DomEventBind('keydown', '.combo-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function keyComboArrowsClick(node, event) {
                var parent = node.parent('.combo-list'), prev, next;
                var selected = parent.find('.option.selected');
                switch (event.which){
                    case ria.dom.Keys.DOWN.valueOf():
                        next = selected.next('.option');
                        if(next.exists()){
                            selected.removeClass('selected');
                            next.addClass('selected');
                        }else{
                            this.dom.find('.chlk-grid').trigger(this._gridEvents.SELECT_NEXT_ROW.valueOf());
                        }; break;
                    case ria.dom.Keys.UP.valueOf():
                        prev = selected.previous('.option');
                        if(prev.exists()){
                            selected.removeClass('selected');
                            prev.addClass('selected');
                        }else{
                            this.dom.find('.chlk-grid').trigger(this._gridEvents.SELECT_PREV_ROW.valueOf());
                        }; break;
                    case ria.dom.Keys.ENTER.valueOf():
                        this.updateReasons();event.preventDefault();break;
                    case ria.dom.Keys.LEFT.valueOf(): node.parent('.row.selected').find('.' + this._LEFT_ARROW).trigger('click');break;
                    case ria.dom.Keys.RIGHT.valueOf(): node.parent('.row.selected').find('.' + this._RIGHT_ARROW).trigger('click');break;
                }
            },

            [ria.mvc.DomEventBind('click', '.left-arrow, .right-arrow')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function arrowClick(node, event){
                var currentAttN = node.parent().find('.current-attendance');
                var attendRow = node.parent('.student-attendance-form');
                var type = attendRow.getData('type');
                type = this.changeAttendanceType_(node.getAttr('class'), type);
                this.changeAttendance_(attendRow.getData('person-id'), type, null);
                this.showDropDown();
            },

            [[Number, Number, Number, String]],
            VOID, function changeAttendance_(personId, type, reasonId, level_){
               var atts = this._classAttendances.filter(function(item){return item.getStudentId().valueOf() == personId});
               if(atts && atts.length > 0){
                   var att = atts[0];
                   att.setType(type);
                   if(level_)
                        att.setLevel(level_);
                   /*if(!reasonId){
                       var level = att.getLevel();
                       var reasons = att.getReasons().filter(function(item){return item.isDefaultReason(level);});
                       if(reasons.length > 0){
                           reasonId = reasons[0].getId().valueOf();
                       }
                   }*/
                   if(reasonId){
                       var reason = this._reasons.filter(function(item){return item.getId().valueOf() == reasonId;})[0];
                       att.setAttendanceReasonId(reason.getId());
                       att.setAttendanceReason(reason);
                   }else{
                       att.setAttendanceReasonId(null);
                       att.setAttendanceReason(null);
                   }
                   this.renderStudentAttendance_(att);
               }
            },

            [[String, Number]],
            Number,  function changeAttendanceType_(arrowClass, currentType){
                var attTypeEnum = chlk.models.attendance.AttendanceTypeEnum;
                var types = [
                    attTypeEnum.PRESENT.valueOf(),
                    attTypeEnum.ABSENT.valueOf(),
                    attTypeEnum.LATE.valueOf()
                ];
                var index = types.indexOf(currentType);
                var increment = arrowClass == this._LEFT_ARROW ? -1 : arrowClass == this._RIGHT_ARROW ? 1 : 0;
                if(increment == -1 && index == 0)
                    index = types.length;
                else if(increment == 1 && index == types.length - 1)
                    index = -1;
                index += increment;
                return types[index];
            },

            [[chlk.models.attendance.ClassAttendance]],
            VOID, function renderStudentAttendance_(studentAttendance){
                if(studentAttendance){
                    this.onPartialRender_(studentAttendance, chlk.activities.lib.DontShowLoader());
                    this.onPartialRefresh_(studentAttendance, chlk.activities.lib.DontShowLoader());
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this._reasons = model.getReasons();
                this._classAttendances = model.getItems();
            },

            [ria.mvc.DomEventBind('click', '.current-attendance')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function comboClick(node, event) {
                this.showDropDown();
            },

            [ria.mvc.DomEventBind('click', '.combo-list .option')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function optionClick(node, event) {
                var parent = node.parent('.combo-list');
                var selected = parent.find('.option.selected');
                selected.removeClass('selected');
                node.addClass('selected');
                this.updateReasons();
            },

            VOID, function updateReasons(){
                var option = this.dom.find('.option.selected:visible');
                option.parent('.row').addClass('reason-changed');
                var id = option.getData('id');
                var level = option.getData('level');
                var grid = option.parent('.chlk-grid');
                var form = option.parent('.student-attendance-form');
                var row = option.parent('.row');
                this.changeAttendance_(form.getData('person-id'), form.getData('type'), id, level);
                grid.trigger(this._gridEvents.SELECT_NEXT_ROW.valueOf());
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), '#class-attendance-list-panel')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            VOID, function onStudentDeselect(grid, event, row, index){
                row.find('.student-attendance-container').removeClass('active');
                row.find('.combo-list').hide();
                row.removeClass('reason-changed');
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                this.afterRefresh(model);
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(!msg_)
                    this.afterRefresh(model);
                if(msg_ == 'saved'){
                    this.dom.addClass('saved');
                    setTimeout(function(){
                        this.dom.removeClass('saved');
                    }.bind(this), 2000);
                }
            },

            [[chlk.models.attendance.ClassList]],
            VOID, function afterRefresh(model){
                if(model.getItems().filter(function(item){
                    return item.getType() != this._typesEnum.NA.valueOf()
                }.bind(this)).length == 0)
                    this.dom.find('.keyboard-suggestion').show();
            },

            Array, function getAttendances_(){
                var res = [];
                var attendancesNodes = this.dom.find('[name="attendance"]').valueOf();
                var len = attendancesNodes.length, i, node;
                for(i=0;i<len;i++){
                    node = new ria.dom.Dom(attendancesNodes[i]);
                    res.push({
                        personId: node.getData('person-id'),
                        type: node.getData('type'),
                        level: node.getData('level'),
                        attendanceReasonId: node.getData('reason-id')
                    });
                }
                return res;
            },

            [ria.mvc.DomEventBind('click', '#submit-attendance-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                jQuery(node.valueOf()[0]).text('SAVING');
                this.dom.find('#all-present-link').setAttr('disabled','disabled');
                var attendancesNode = this.dom.find('input[name="attendancesJson"]');
                attendancesNode.setValue(JSON.stringify(this.getAttendances_()));
                this._needPopUp = false;
                node.parent('form').trigger('submit');
            }
        ]);
});