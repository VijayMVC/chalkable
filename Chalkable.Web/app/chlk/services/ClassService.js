REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.classes.ClassForTopBar');
REQUIRE('chlk.models.classes.ClassForWeekMask');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.classes.ClassSummary');
REQUIRE('chlk.models.classes.ClassInfo');
REQUIRE('chlk.models.classes.ClassSchedule');
REQUIRE('chlk.models.classes.ClassGradingViewData');

REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ClassService*/
    CLASS(
        'ClassService', EXTENDS(chlk.services.BaseService), [

            ArrayOf(chlk.models.classes.ClassForTopBar), 'classesToFilter',
            ArrayOf(chlk.models.classes.ClassForTopBar), 'classesToFilterWithAll',


            //TODO: refactor
            [[Boolean, Boolean, Boolean]],
            Array, function getClassesForTopBar(withAll_, forCurrentMp_, isNotAbleToGetClasses_) {
                var res = this.getClassesToFilter(), res1 = this.getClassesToFilterWithAll();
                //if(res)
                    //return withAll_ ? res1 : res;
                var classes = window.classesToFilter;
                if(forCurrentMp_){
                    var mpId = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD).getId().valueOf();
                    classes = window.classesToFilter.filter(function(item){
                        return item.markingperiodsid.indexOf(mpId) > -1
                    })
                }
                res = new chlk.lib.serialize.ChlkJsonSerializer().deserialize(classes, ArrayOf(chlk.models.classes.ClassForTopBar));
                this.setClassesToFilter(res);
                var classesToFilterWithAll = window.classesToFilter.slice();
                classesToFilterWithAll.unshift({
                    name: 'All',
                    description: 'All',
                    id: ''
                });
                res1 = chlk.lib.serialize.ChlkJsonSerializer().deserialize(classesToFilterWithAll, ArrayOf(chlk.models.classes.ClassForTopBar));
                this.setClassesToFilterWithAll(res1);
                if(isNotAbleToGetClasses_){
                    res = [];
                    res1 = res1.filter(function(item){return item.getName() == 'All';})
                }
                return withAll_ ? res1 : res;
            },

            [[chlk.models.id.ClassId]],
            chlk.models.classes.ClassForTopBar, function getClassById(classId) {
                var classes = this.getClassesForTopBar(), res;
                classes.forEach(function(item){
                    if(item.getId() == classId)
                        res = item;
                });
                return res;
            },

            //TODO: refactor
            [[chlk.models.id.ClassId]],
            chlk.models.classes.ClassForWeekMask, function getClassAnnouncementInfo(id){
                var res = window.classesInfo[id.valueOf()];
                if(!res)
                    return null;
                res.classId = id.valueOf();
                res = new chlk.lib.serialize.ChlkJsonSerializer().deserialize(res, chlk.models.classes.ClassForWeekMask);
                return res;
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getSummary(classId) {
                return this.get('Class/ClassSummary.json', chlk.models.classes.ClassSummary, {
                    classId: classId.valueOf()
                });
            },
            [[chlk.models.id.ClassId]],
            ria.async.Future, function getInfo(classId){
                return this.get('Class/ClassInfo.json', chlk.models.classes.ClassInfo,{
                    classId: classId && classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getGrading(classId){
                return this.get('Class/ClassGrading.json', chlk.models.classes.ClassGradingViewData,{
                    classId: classId && classId.valueOf()
                })
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getSchedule(classId, date_){
                return this.get('Class/ClassSchedule.json', chlk.models.classes.ClassSchedule,{
                    classId: classId && classId.valueOf(),
                    date: date_ && date_.toString('mm-dd-yy')
                })
                .then(function(data){
                    this.getContext().getService(chlk.services.CalendarService)
                        .saveDayCalendarDataInSession(data.getCalendarDayItems());
                    return data;
                }, this);
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getAttendance(classId){
                return this.get('Class/ClassAttendance.json', chlk.models.classes.ClassAttendanceSummary,{
                    classId: classId && classId.valueOf()
                })
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getAppsInfo(classId){
                return this.get('Class/ClassApps.json', chlk.models.classes.ClassApps,{
                    classId: classId && classId.valueOf()
                })
            },


        ])
});