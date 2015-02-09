REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.student.StudentExplorerViewData');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentProfileExplorerTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileExplorerView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentExplorerViewData)],
        'StudentProfileExplorerTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.student.StudentExplorer)),[

            [ria.templates.ModelPropertyBind],
            chlk.models.student.StudentExplorer, 'studentExplorer',

            [[chlk.models.student.StudentClassExplorer, Number]],
            String, function getClassBlockTitle(item, index){
                var studentName = this.getStudentExplorer().getFirstName(), res = '';
                if(index == 0)
                    res = studentName + '\'s weakest class is ';
                res += item.getClazz().getName() + (item.getAvg() === null ? '' : ' - Avg. ' + Math.round(item.getAvg()));
                return res;
            },

            [[chlk.models.standard.StandardForExplorer]],
            String, function getStandardColor(item){
                var grade = item.getStandardGrading().getNumericGrade();
                if(!grade)
                    return '';
                if(grade >= 75)
                    return 'green';
                if(grade >= 65)
                    return 'yellow';
                return 'red';
            },

            [[chlk.models.student.StudentClassExplorer]],
            Boolean, function showMoreButton(item){
                return ((item.getAnnouncement() ? 1 : 0) + item.getStandards().length) > 4
            },

            [[chlk.models.student.StudentClassExplorer, Number]],
            Boolean, function showStandard(item, index){
                var announcementsLength = (item.getAnnouncement() ? 1 : 0);

                return !this.showMoreButton(item) || (announcementsLength + index < 3);
            }

        ]);
});