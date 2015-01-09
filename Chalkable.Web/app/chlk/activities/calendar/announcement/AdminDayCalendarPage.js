REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.announcement.AdminDayCalendarTpl');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.AdminDayCalendarPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('calendar')],
        [ria.mvc.TemplateBind(chlk.templates.calendar.announcement.AdminDayCalendarTpl)],
        'AdminDayCalendarPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._HIDDEN_CLASS = 'x-hidden';
                this._POP_UP_CLASS_NAME = 'admin-day-calendar-popup';
                this._POP_UP_CONTAINER_ID = '#chlk-pop-up-container';
            },

            [ria.mvc.DomEventBind('click', '.admin-day-teachers')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function showTeachersNames(node, event){
                this.showClassesOrTeachers_(node, false);
            },

            [ria.mvc.DomEventBind('click', '.admin-day-classes')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function showClassesNames(node, event){
                this.showClassesOrTeachers_(node, true);
            },

            [ria.mvc.DomEventBind('mouseover', '.calendar-day-item .class-info')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function showClassDetailedInfoEvent(node, event){
                var toolTipHtml = node.getData('tool-tip');
                var popUpContainer = new ria.dom.Dom(this._POP_UP_CONTAINER_ID);
                var popUp = new ria.dom.Dom(jQuery.parseHTML(toolTipHtml)).appendTo(popUpContainer);
                var offset = node.offset();
                popUpContainer.removeClass(this._HIDDEN_CLASS);
                popUpContainer.addClass('popup-right');
                popUpContainer.setCss('left', offset.left + 30 + (popUpContainer.width() / 2));
                popUpContainer.setCss('top', offset.top - 25);
            },

            [ria.mvc.DomEventBind('mouseleave', '.calendar-day-item .class-info, ' + this._POP_UP_CONTAINER_ID )],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function hideClassDetailedInfoEvent(node, event){
                this.hideClassDetailedInfo_();
            },


            function hideClassDetailedInfo_(){
                var container = new ria.dom.Dom(this._POP_UP_CONTAINER_ID);
                container.remove(new ria.dom.Dom('.' + this._POP_UP_CLASS_NAME));
                container.addClass(this._HIDDEN_CLASS);
            },

            function showClassesOrTeachers_(pressedElem, showClasses){
                var elemNameForShow = '.teacher-name';
                var elemNameForHide = '.class-name';
                var unPressedBtnName = '.admin-day-classes';
                if(showClasses){
                    elemNameForShow = '.class-name';
                    unPressedBtnName = '.admin-day-teachers';
                    elemNameForHide = '.teacher-name';
                }
                pressedElem.addClass('pressed');
                pressedElem.parent().find(unPressedBtnName).removeClass('pressed');
                var elements = this.dom.find('.calendar-body .items .class-info');
                elements.find(elemNameForShow).removeClass(this._HIDDEN_CLASS);
                elements.find(elemNameForHide).addClass(this._HIDDEN_CLASS);
            }
        ]);

});