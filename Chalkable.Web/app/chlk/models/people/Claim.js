NAMESPACE('chlk.models.people', function () {
    "use strict";

     /**@class chlk.models.people.UserPermissionEnum*/
    ENUM('UserPermissionEnum',{
        ACCESS_PAST_ACADEMIC_SESSIONS: 'Access Past Academic Sessions',
        VIEW_ACADEMIC_SESSION: 'View Academic Session',
        MAINTAIN_ADDRESS: 'Maintain Address',
        VIEW_ADDRESS: 'View Address',
        MAINTAIN_ATTENDANCE: 'Maintain Attendance',
        VIEW_ATTENDANCE: 'View Attendance',
        MAINTAIN_CLASSROOM_ABSENCE_REASONS: 'Maintain Classroom Absence Reasons',
        MAINTAIN_CLASSROOM_ATTENDANCE: 'Maintain Classroom Attendance',
        MAINTAIN_CLASSROOM_GRADES: 'Maintain Classroom Grades',
        MAINTAIN_CLASSROOM_LUNCH_COUNT: 'Maintain Classroom Lunch Count',
        MAINTAIN_CLASSROOM_ROSTER: 'Maintain Classroom Roster',
        REPOST_CLASSROOM_ATTENDANCE: 'Repost Classroom Attendance',
        VIEW_CLASSROOM_ABSENCE_REASONS: 'View Classroom Absence Reasons',
        VIEW_CLASSROOM_ATTENDANCE: 'View Classroom Attendance',
        VIEW_CLASSROOM_GRADES: 'View Classroom Grades',
        VIEW_CLASSROOM_LUNCH_COUNT: 'View Classroom Lunch Count',
        VIEW_CLASSROOM_ROSTER: 'View Classroom Roster',
        MAINTAIN_DISCIPLINE: 'Maintain Discipline',
        VIEW_DISCIPLINE: 'View Discipline',
        CHANGE_ACTIVITY_DATES: 'Change Activity Dates',
        MAINTAIN_CLASSROOM: 'Maintain Classroom',
        MAINTAIN_GRADE_BOOK_AVERAGING_METHOD: 'Maintain Grade Book Averaging Method',
        MAINTAIN_GRADE_BOOK_CATEGORIES: 'Maintain Grade Book Categories',
        MAINTAIN_STANDARDS_OPTIONS: 'Maintain Standards Options',
        MAINTAIN_STUDENT_AVERAGES: 'Maintain Student Averages',
        RECONCILE_GRADE_BOOK: 'Reconcile GradeBook',
        VIEW_CLASSROOM: 'View Classroom',
        MAINTAIN_GRADING: 'Maintain Grading',
        VIEW_GRADING: 'View Grading',
        MAINTAIN_LOOKUP: 'Maintain Lookup',
        VIEW_LOOKUP: 'View Lookup',
        MAINTAIN_PERSON: 'Maintain Person',
        VIEW_PERSON: 'View Person',
        VIEW_COURSE: 'View Course',
        VIEW_MODEL: 'View Model',
        VIEW_SECTION: 'View Section',
        MAINTAIN_LOCKER: 'Maintain Locker',
        VIEW_LOCKER: 'View Locker',
        VIEW_STAFF: 'View Staff',
        MAINTAIN_STUDENT: 'Maintain Student',
        MAINTAIN_STUDENT_FORM: 'Maintain Student Form',
        VIEW_HEALTH_CONDITION: 'View Health Condition',
        VIEW_MEDICAL: 'View Medical',
        VIEW_REGISTRATION: 'View Registration',
        VIEW_SPECIAL_EDUCATION: 'View Special Education',
        VIEW_SPECIAL_INSTRUCTIONS: 'View Special Instructions',
        VIEW_STUDENT: 'View Student',
        VIEW_STUDENT_COMMENDATIONS: 'View Student Commendations',
        VIEW_STUDENT_CUSTOM: 'View Student Custom',
        VIEW_STUDENT_FORM: 'View Student Form',
        VIEW_STUDENT_MISCELLANEOUS: 'View Student Miscellaneous',
        MAINTAIN_STUDENT_FILTER: 'Maintain Student Filter',
        VIEW_STUDENT_FILTER: 'View Student Filter'
    });

    /** @class chlk.models.people.Claim*/
    CLASS('Claim',  [
        String, 'type',
        ArrayOf(chlk.models.people.UserPermissionEnum), 'values',

        [[chlk.models.people.UserPermissionEnum]],
        Boolean, function hasPermission(permission){
            var values = this.getValues();
            return values && values.length > 0
                && values.filter(function(value){return value == permission}).length > 0;
        }
    ]);
});
