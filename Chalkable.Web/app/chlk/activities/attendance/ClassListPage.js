REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.ClassAttendance');
REQUIRE('chlk.templates.attendance.ClassList');

NAMESPACE('chlk.activities.attendance', function () {
    "use strict";

    /** @class chlk.activities.attendance.ClassListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.ClassList)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassList, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassListPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._typesEnum = chlk.models.attendance.AttendanceTypeEnum;
                this._comboTimer = null;
                this._gridEvents = chlk.controls.GridEvents;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassAttendance)],
            VOID, function doUpdateItem(tpl, model, msg_) {
                this.dom.find('.keyboard-suggestion').hide();
                var container = this.dom.find('.container-' + model.getClassPersonId().valueOf());
                container.empty();
                tpl.renderTo(container);
                var row = container.parent('.row');
                row.find('.student-attendance-container').removeClass('active');
                if(row.hasClass('selected'))
                    this.showDropDown();
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.KEY_DOWN.valueOf(), '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event, Number]],
            VOID, function gridKeyDownSelect(node, event, key_) {
                switch(key_){
                    case ria.dom.Keys.ENTER.valueOf(): node.trigger(this._gridEvents.SELECT_NEXT_ROW.valueOf());break;
                    case ria.dom.Keys.LEFT.valueOf(): node.find('.row.selected').find('.left-arrow').trigger('click');break;
                    case ria.dom.Keys.RIGHT.valueOf(): node.find('.row.selected').find('.right-arrow').trigger('click');break;
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
                    row_.find('form').trigger('submit');
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
                        this.updateReasons();break;
                    case ria.dom.Keys.LEFT.valueOf(): node.parent('.row.selected').find('.left-arrow').trigger('click');break;
                    case ria.dom.Keys.RIGHT.valueOf(): node.parent('.row.selected').find('.right-arrow').trigger('click');break;
                }
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
                var id = option.getData('id');
                var grid = option.parent('.chlk-grid');
                var form = option.parent('form');
                var row = option.parent('.row');
                form.find('input[name=attendancereasonid]').setValue(id);
                id && form.find('input[name=attendanceReasonDescription]').setValue(option.getHTML());
                form.trigger('submit');
                grid.trigger(this._gridEvents.SELECT_NEXT_ROW.valueOf());
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), '#class-attendance-list-panel')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            VOID, function onStudentDeselect(grid, event, row, index){
                var form = row.find('.student-attendance-form');
                if(form.hasClass('need-present'))
                    form.trigger('submit');
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                this.afterRefresh(model);
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg) {
                BASE(model, msg);
                if(!msg)
                    this.afterRefresh(model);
            },

            [[chlk.models.attendance.ClassList]],
            VOID, function afterRefresh(model){
                //this.showDropDown();
                if(model.getItems().filter(function(item){
                    return item.getType() != this._typesEnum.NA.valueOf()
                }.bind(this)).length == 0)
                    this.dom.find('.keyboard-suggestion').show();
            }/*,

            OVERRIDE, VOID, function onCreate_() {
                BASE();
                var that = this;

                jQuery(this.dom.valueOf()).on('keydown', '.combo-input', function keyComboArrowsClick(event) {
                    var node = new ria.dom.Dom(this);
                    var parent = node.parent('.combo-list'), prev, next;
                    var selected = parent.find('.option.selected');
                    switch (event.which){
                        case ria.dom.Keys.DOWN.valueOf():
                            next = selected.next('.option');
                            if(next.exists()){
                                selected.removeClass('selected');
                                next.addClass('selected');
                            }; break;
                        case ria.dom.Keys.UP.valueOf():
                            prev = selected.previous('.option');
                            if(prev.exists()){
                                selected.removeClass('selected');
                                prev.addClass('selected');
                            }; break;
                        case ria.dom.Keys.ENTER.valueOf():
                            that.updateReasons();
                    }
                });
            }*/
        ]);
});