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
                var alphaGrade = item.getStandardGrading().getGradeValue()
                if(alphaGrade && alphaGrade.trim() != '')
                    return  this.getStandardColorByAlphaGrade_(alphaGrade);
                return this.getStandardColorByNumericValue_(item.getStandardGrading().getNumericGrade());
            },

            [[String]],
            String, function getStandardColorByAlphaGrade_(grade){
                if(!grade) return '';
                grade = grade.toUpperCase();
                if(['A+', 'A', 'A-', 'B+', 'B', 'B-'].indexOf(grade) >= 0)
                    return 'green';
                if(['C+', 'C', 'C-'].indexOf(grade) >= 0)
                    return 'yellow';
                return 'red';
            },

            [[Number]],
            String, function getStandardColorByNumericValue_(grade){
                if(!grade && grade !== 0)
                    return '';
                if(grade >= 75)
                    return 'green';
                if(grade >= 65)
                    return 'yellow';
                return 'red';
            },

            String, function showNoClassesMsg(){
                if(this.getUserRole().isTeacher())
                    return 'No teacher classes yet';
                return 'No classes yet';
            },

            [[chlk.models.student.StudentClassExplorer]],
            Boolean, function showMoreButton(item){
                return ((item.getAnnouncement() ? 1 : 0) + item.getStandards().length) > 4
            },

            [[chlk.models.student.StudentClassExplorer, Number]],
            Boolean, function showStandard(item, index){
                var announcementsLength = (item.getAnnouncement() ? 1 : 0);

                return !this.showMoreButton(item) || (announcementsLength + index < 3);
            },

            [[chlk.models.announcement.ClassAnnouncementViewData]],
            String, function getDaysForAnnouncement(announcement){
                announcement.prepareExpiresDateText();
                return announcement.getExpiresDateText();
            }
        ]);
});