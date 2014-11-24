REQUIRE('chlk.templates.calendar.announcement.DayPage');
REQUIRE('chlk.models.calendar.announcement.Week');
REQUIRE('chlk.templates.calendar.announcement.WeekCalendarBodyTpl');
REQUIRE('chlk.templates.classes.TopBar');
REQUIRE('chlk.templates.grading.GradeLevelForTopBar');

NAMESPACE('chlk.templates.calendar.announcement', function () {
    "use strict";

    /** @class chlk.templates.calendar.announcement.WeekPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/WeekPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Week)],
        [chlk.activities.lib.PageClass('calendar')],
        'WeekPage', EXTENDS(chlk.templates.calendar.announcement.DayPage), [
            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsForToolBar',

            //TODO: duplicate method
            Object, function getDataForToolBar(){
                var model = this.getModel();
                var res = {};
                res.tpl = chlk.templates.classes.TopBar;
                res.data = model.getTopData().getTopItems();
                res.selectedItemId = model.getTopData().getSelectedItemId();
                res.multiple = false;
                if(model.isAdmin()){
                    res.tpl = chlk.templates.grading.GradeLevelForTopBar;
                    res.data = model.getGradeLevelsForToolBar().getTopItems();
                    res.selectedItemId = null;
                    res.multiple = true;
                    res.selectedIds = model.getGradeLevelsForToolBar().getSelectedIds();
                }
                return res;
            }
        ]);
});