REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.announcement.AdminDayCalendarTpl');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.AdminDayCalendarPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('calendar')],
        [ria.mvc.TemplateBind(chlk.templates.calendar.announcement.AdminDayCalendarTpl)],
        'AdminDayCalendarPage', EXTENDS(chlk.activities.lib.TemplatePage), [


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

            [ria.mvc.DomEventBind('hover', '.calendar-day-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function showClassDetailedInfo(node, event){
                var toolTipHtml = node.getData('tool-tip');
                (new ria.dom.Dom())
                    .fromHTML(toolTipHtml)
                    .appendTo(node);
            },

            [ria.mvc.DomEventBind('mouseleave', '.calendar-day-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function hideClassDetailedInfo(node, event){

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
                elements.find(elemNameForShow).removeClass('x-hidden');
                elements.find(elemNameForHide).addClass('x-hidden');
            }
        ]);

});