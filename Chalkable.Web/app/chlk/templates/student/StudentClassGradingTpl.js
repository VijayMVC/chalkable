REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.ClassPersonGradingInfo');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentClassGradingTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentClassGradingItemView.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ClassPersonGradingInfo)],
        'StudentClassGradingTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassPersonId, 'classPersonId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'personId',

            [ria.templates.ModelPropertyBind],
            String, 'className',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.CourseId, 'courseId',

            [ria.templates.ModelPropertyBind],
            Number, 'classAvg',

            [ria.templates.ModelPropertyBind],
            Number, 'studentAvg',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.ClassPersonGradingItem), 'gradingByAnnouncementTypes',

            Object, function prepareGlanceBoxData(){
                return {
                    value: this.getClassAvg() || 0,
                    title: this.getClassName()
                }
            },
            [[chlk.models.announcement.Announcement]],
            Object, function prepareItemStyleInfo(item){
                return {
                    emptyBoxClass : !item.getAvg() ? 'empty-box' : '',
                    droppedItemClass : item.isDropped() ? 'dropped-item' : ''
                }
            },

            [[chlk.models.grading.ClassPersonGradingItem, chlk.models.announcement.Announcement]],
            String, function getItemTitle(itemType, announcement){
                var res = itemType.getName() + " " + announcement.getOrder();
                if(announcement.getGrade())
                    res = res + " : " + announcement.getGrade();
                return res;
            }


    ]);
});