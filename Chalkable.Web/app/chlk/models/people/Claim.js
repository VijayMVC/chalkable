REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;



    /**@class chlk.models.people.UserPermissionEnum*/
    ENUM('UserPermissionEnum',{
        ACCESS_FUTURE_ACADEMIC_SESSIONS: 'Access Future Academic Sessions',
        ACCESS_PAST_ACADEMIC_SESSIONS: 'Access Past Academic Sessions',
        VIEW_ACADEMIC_SESSION: 'View Academic Session',
        MAINTAIN_ADDRESS: 'Maintain Address',
        VIEW_ADDRESS: 'View Address',
        MAINTAIN_ATTENDANCE: 'Maintain Attendance',
        VIEW_ATTENDANCE: 'View Attendance',
        MAINTAIN_CLASSROOM_ABSENCE_REASONS: 'Maintain Classroom Absence Reasons',
        MAINTAIN_CLASSROOM_ATTENDANCE: 'Maintain Classroom Attendance',
        MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN: 'Maintain Classroom Attendance (Admin)',
        MAINTAIN_CLASSROOM_DISCIPLINE: 'Maintain Classroom Discipline',
        MAINTAIN_CLASSROOM_DISCIPLINE_ADMIN: 'Maintain Classroom Discipline (Admin)',
        MAINTAIN_CLASSROOM_GRADES: 'Maintain Classroom Grades',
        MAINTAIN_CLASSROOM_LUNCH_COUNT: 'Maintain Classroom Lunch Count',
        MAINTAIN_CLASSROOM_ROSTER: 'Maintain Classroom Roster',
        REPOST_CLASSROOM_ATTENDANCE: 'Repost Classroom Attendance',
        VIEW_CLASSROOM_ABSENCE_REASONS: 'View Classroom Absence Reasons',
        VIEW_CLASSROOM_ATTENDANCE: 'View Classroom Attendance',
        VIEW_CLASSROOM_ATTENDANCE_ADMIN: 'View Classroom Attendance (Admin)',
        VIEW_CLASSROOM_DISCIPLINE: 'View Classroom Discipline',
        VIEW_CLASSROOM_DISCIPLINE_ADMIN: 'View Classroom Discipline (Admin)',
        VIEW_CLASSROOM_GRADES: 'View Classroom Grades',
        VIEW_CLASSROOM_LUNCH_COUNT: 'View Classroom Lunch Count',
        VIEW_CLASSROOM_ROSTER: 'View Classroom Roster',
        VIEW_CLASSROOM_STUDENTS: 'View Classroom Students',
        MAINTAIN_DISCIPLINE: 'Maintain Discipline',
        VIEW_DISCIPLINE: 'View Discipline',
        CHANGE_ACTIVITY_DATES: 'Change Activity Dates',
        MAINTAIN_CLASSROOM: 'Maintain Classroom',
        MAINTAIN_CLASSROOM_ADMIN: 'Maintain Classroom (Admin)',
        MAINTAIN_GRADE_BOOK_AVERAGING_METHOD: 'Maintain Grade Book Averaging Method',
        MAINTAIN_GRADE_BOOK_CATEGORIES: 'Maintain Grade Book Categories',
        MAINTAIN_GRADE_BOOK_COMMENTS: 'Maintain Grade Book Comments',
        MAINTAIN_STANDARDS_OPTIONS: 'Maintain Standards Options',
        MAINTAIN_STUDENT_AVERAGES: 'Maintain Student Averages',
        RECONCILE_GRADE_BOOK: 'Reconcile GradeBook',
        VIEW_CLASSROOM: 'View Classroom',
        VIEW_CLASSROOM_ADMIN: 'View Classroom (Admin)',
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
        VIEW_STUDENT_FILTER: 'View Student Filter',
        VIEW_TRANSCRIPT: 'View Transcript',
        CHALKABLE_ADMIN: 'Administer Chalkable',
        MAINTAIN_CHALKABLE_DISTRICT_SETTINGS: 'Maintain Chalkable District Settings',
        AWARD_LE_CREDITS_CLASSROOM:'Award LE Credits (Classroom)',
        AWARD_LE_CREDITS:'Award LE Credits',

        /* report Permissions*/

        ATTENDANCE_PROFILE_REPORT: 'Attendance Profile Report',
        ATTENDANCE_PROFILE_REPORT_CLASSROOM: 'Attendance Profile Report Classroom',
        BIRTHDAY_LISTING_REPORT: 'Birthday Listing Report',
        BIRTHDAY_LISTING_REPORT_CLASSROOM: 'Birthday Listing Report Classroom',
        LESSON_PLAN_REPORT: 'Lesson Plan Report',
        LESSON_PLAN_REPORT_CLASSROOM: 'Lesson Plan Report Classroom',
        PROGRESS_REPORT: 'Progress Report',
        WORKSHEET_REPORT: 'Worksheet Report',
        CLASSROOM_ATTENDANCE_REGISTER_REPORT: 'Classroom Attendance Register Report',
        SEATING_CHART_REPORT: 'Seating Chart Report',
        COMPREHENSIVE_PROGRESS_REPORT: 'Comprehensive Progress Report',
        COMPREHENSIVE_PROGRESS_REPORT_CLASSROOM: 'Comprehensive Progress Report Classroom',
        GRADE_BOOK_REPORT: 'Grade Book Report',
        GRADE_BOOK_REPORT_CLASSROOM: 'Grade Book Report Classroom',
        MISSING_ASSIGNMENTS_REPORT: 'Missing Assignments Report',
        GRADE_VERIFICATION_REPORT: 'Grade Verification Report',
        GRADE_VERIFICATION_REPORT_CLASSROOM: 'Grade Verification Report Classroom',


        CONTACT_ADDRESS: 'Contact Address',
        CONTACT_EMPLOYER_NAME: 'Contact EmployerName',
        CONTACT_MARITAL_STATUS: 'Contact MaritalStatus',
        CONTACT_SOCIAL_SECURITY_NUMBER: 'Contact SocialSecurityNumber',
        CONTACT_TELEPHONE: 'Contact Telephone',
        CONTACT_FIRST_NAME: 'Contact First Name',
        CONTACT_LAST_NAME: 'Contact Last Name',
        STAFF_ADDRESS: 'Staff Address',
        STAFF_DISPLAY_NAME: 'Staff Display Name',
        STAFF_FIRST_NAME: 'Staff First Name',
        STAFF_LAST_NAME: 'Staff Last Name',
        STUDENT_ADDRESS: 'Student Address',
        STUDENT_CONTACT: 'Student Contact',
        STUDENT_GRADE: 'Student Grade',
        STUDENT_INTERNET_PASSWORD: 'Student InternetPassword',
        STUDENT_IS_ALLOWED_INET_ACCESS: 'Student IsAllowedInetAccess',
        STUDENT_PHOTOGRAPH: 'Student Photograph',
        STUDENT_SCHOOL: 'Student School',
        STUDENT_SOCIAL_SECURITY_NUMBER: 'Student SocialSecurityNumber',
        STUDENT_STUDENT_NUMBER: 'Student Student Number',
        STUDENT_STATE_ID_NUMBER: 'Student StateIDNumber',
        STUDENT_ALT_STUDENT_NUMBER: 'Student AltStudentNumber',
        STUDENT_TELEPHONE: 'Student Telephone',
        STUDENT_TRANSPORT_METHOD: 'Student TransportMethod',
        STUDENT_DATE_OF_BIRTH: 'Student Date Of Birth',
        STUDENT_FIRST_NAME: 'Student First Name',
        STUDENT_GENDER: 'Student Gender',
        STUDENT_LAST_NAME: 'Student Last Name',

    });

    var UserPermissionMappeerInstance = null;

    /**@class chlk.models.people.UserPermissionMappeer*/

    CLASS('UserPermissionMappeer', [

        // $$ - instance factory
        function $$(instance, Clazz, ctor, args) {
            return UserPermissionMappeerInstance || (UserPermissionMappeerInstance = new ria.__API.init(instance, Clazz, ctor, args));
        },

        function $(){
            BASE();
            this.mapper_ = null;
            this.userPermissionEnum = chlk.models.people.UserPermissionEnum;
            this.registerMapper_();
        },


        [[String]],
        Boolean, function hasKey(key){
            var res =  this.mapper_[key];
            return !(res == null || res == undefined);
        },

        [[String]],
        chlk.models.people.UserPermissionEnum, function map(key){
            if(!this.hasKey(key))
                throw new Exception('Unknown user permission');
            return this.mapper_[key];
        },

        VOID, function registerMapper_(){
            var permissions = this.userPermissionEnum;
            if(this.mapper_ == null || this.mapper_ == undefined){
                this.mapper_ = {};
                this.mapper_[this.userPermissionEnum.ACCESS_FUTURE_ACADEMIC_SESSIONS.valueOf()] = this.userPermissionEnum.ACCESS_FUTURE_ACADEMIC_SESSIONS;
                this.mapper_[this.userPermissionEnum.ACCESS_PAST_ACADEMIC_SESSIONS.valueOf()] = this.userPermissionEnum.ACCESS_PAST_ACADEMIC_SESSIONS;
                this.mapper_[this.userPermissionEnum.VIEW_ACADEMIC_SESSION.valueOf()] = this.userPermissionEnum.VIEW_ACADEMIC_SESSION;
                this.mapper_[this.userPermissionEnum.MAINTAIN_ADDRESS.valueOf()] = this.userPermissionEnum.MAINTAIN_ADDRESS;
                this.mapper_[this.userPermissionEnum.VIEW_ADDRESS.valueOf()] = this.userPermissionEnum.VIEW_ADDRESS;
                this.mapper_[this.userPermissionEnum.MAINTAIN_ATTENDANCE.valueOf()] = this.userPermissionEnum.MAINTAIN_ATTENDANCE;
                this.mapper_[this.userPermissionEnum.VIEW_ATTENDANCE.valueOf()] = this.userPermissionEnum.VIEW_ATTENDANCE;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_ABSENCE_REASONS.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_ABSENCE_REASONS;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_DISCIPLINE.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_DISCIPLINE;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_DISCIPLINE_ADMIN.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_DISCIPLINE_ADMIN;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_GRADES.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_GRADES;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_LUNCH_COUNT.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_LUNCH_COUNT;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_ROSTER.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_ROSTER;
                this.mapper_[this.userPermissionEnum.REPOST_CLASSROOM_ATTENDANCE.valueOf()] = this.userPermissionEnum.REPOST_CLASSROOM_ATTENDANCE;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_ABSENCE_REASONS.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_ABSENCE_REASONS;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_ATTENDANCE.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_ATTENDANCE;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_DISCIPLINE.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_DISCIPLINE;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_DISCIPLINE_ADMIN.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_DISCIPLINE_ADMIN;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_GRADES.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_GRADES;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_LUNCH_COUNT.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_LUNCH_COUNT;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_ROSTER.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_ROSTER;
                this.mapper_[this.userPermissionEnum.MAINTAIN_DISCIPLINE.valueOf()] = this.userPermissionEnum.MAINTAIN_DISCIPLINE;
                this.mapper_[this.userPermissionEnum.VIEW_DISCIPLINE.valueOf()] = this.userPermissionEnum.VIEW_DISCIPLINE;
                this.mapper_[this.userPermissionEnum.CHANGE_ACTIVITY_DATES.valueOf()] = this.userPermissionEnum.CHANGE_ACTIVITY_DATES;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CLASSROOM_ADMIN.valueOf()] = this.userPermissionEnum.MAINTAIN_CLASSROOM_ADMIN;
                this.mapper_[this.userPermissionEnum.MAINTAIN_GRADE_BOOK_AVERAGING_METHOD.valueOf()] = this.userPermissionEnum.MAINTAIN_GRADE_BOOK_AVERAGING_METHOD;
                this.mapper_[this.userPermissionEnum.MAINTAIN_GRADE_BOOK_CATEGORIES.valueOf()] = this.userPermissionEnum.MAINTAIN_GRADE_BOOK_CATEGORIES;
                this.mapper_[this.userPermissionEnum.MAINTAIN_GRADE_BOOK_COMMENTS.valueOf()] = this.userPermissionEnum.MAINTAIN_GRADE_BOOK_COMMENTS;
                this.mapper_[this.userPermissionEnum.MAINTAIN_STANDARDS_OPTIONS.valueOf()] = this.userPermissionEnum.MAINTAIN_STANDARDS_OPTIONS;
                this.mapper_[this.userPermissionEnum.MAINTAIN_STUDENT_AVERAGES.valueOf()] = this.userPermissionEnum.MAINTAIN_STUDENT_AVERAGES;
                this.mapper_[this.userPermissionEnum.RECONCILE_GRADE_BOOK.valueOf()] = this.userPermissionEnum.RECONCILE_GRADE_BOOK;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_ADMIN.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_ADMIN;
                this.mapper_[this.userPermissionEnum.MAINTAIN_GRADING.valueOf()] = this.userPermissionEnum.MAINTAIN_GRADING;
                this.mapper_[this.userPermissionEnum.VIEW_GRADING.valueOf()] = this.userPermissionEnum.VIEW_GRADING;
                this.mapper_[this.userPermissionEnum.MAINTAIN_LOOKUP.valueOf()] = this.userPermissionEnum.MAINTAIN_LOOKUP;
                this.mapper_[this.userPermissionEnum.VIEW_LOOKUP.valueOf()] = this.userPermissionEnum.VIEW_LOOKUP;
                this.mapper_[this.userPermissionEnum.MAINTAIN_PERSON.valueOf()] = this.userPermissionEnum.MAINTAIN_PERSON;
                this.mapper_[this.userPermissionEnum.VIEW_PERSON.valueOf()] = this.userPermissionEnum.VIEW_PERSON;
                this.mapper_[this.userPermissionEnum.VIEW_COURSE.valueOf()] = this.userPermissionEnum.VIEW_COURSE;
                this.mapper_[this.userPermissionEnum.VIEW_MODEL.valueOf()] = this.userPermissionEnum.VIEW_MODEL;
                this.mapper_[this.userPermissionEnum.VIEW_SECTION.valueOf()] = this.userPermissionEnum.VIEW_SECTION;
                this.mapper_[this.userPermissionEnum.MAINTAIN_LOCKER.valueOf()] = this.userPermissionEnum.MAINTAIN_LOCKER;
                this.mapper_[this.userPermissionEnum.VIEW_LOCKER.valueOf()] = this.userPermissionEnum.VIEW_LOCKER;
                this.mapper_[this.userPermissionEnum.VIEW_STAFF.valueOf()] = this.userPermissionEnum.VIEW_STAFF;
                this.mapper_[this.userPermissionEnum.MAINTAIN_STUDENT.valueOf()] = this.userPermissionEnum.MAINTAIN_STUDENT;
                this.mapper_[this.userPermissionEnum.MAINTAIN_STUDENT_FORM.valueOf()] = this.userPermissionEnum.MAINTAIN_STUDENT_FORM;
                this.mapper_[this.userPermissionEnum.VIEW_HEALTH_CONDITION.valueOf()] = this.userPermissionEnum.VIEW_HEALTH_CONDITION;
                this.mapper_[this.userPermissionEnum.VIEW_MEDICAL.valueOf()] = this.userPermissionEnum.VIEW_MEDICAL;
                this.mapper_[this.userPermissionEnum.VIEW_REGISTRATION.valueOf()] = this.userPermissionEnum.VIEW_REGISTRATION;
                this.mapper_[this.userPermissionEnum.VIEW_SPECIAL_EDUCATION.valueOf()] = this.userPermissionEnum.VIEW_SPECIAL_EDUCATION;
                this.mapper_[this.userPermissionEnum.VIEW_SPECIAL_INSTRUCTIONS.valueOf()] = this.userPermissionEnum.VIEW_SPECIAL_INSTRUCTIONS;
                this.mapper_[this.userPermissionEnum.VIEW_STUDENT.valueOf()] = this.userPermissionEnum.VIEW_STUDENT;
                this.mapper_[this.userPermissionEnum.VIEW_STUDENT_COMMENDATIONS.valueOf()] = this.userPermissionEnum.VIEW_STUDENT_COMMENDATIONS;
                this.mapper_[this.userPermissionEnum.VIEW_STUDENT_CUSTOM.valueOf()] = this.userPermissionEnum.VIEW_STUDENT_CUSTOM;
                this.mapper_[this.userPermissionEnum.VIEW_STUDENT_FORM.valueOf()] = this.userPermissionEnum.VIEW_STUDENT_FORM;
                this.mapper_[this.userPermissionEnum.VIEW_STUDENT_MISCELLANEOUS.valueOf()] = this.userPermissionEnum.VIEW_STUDENT_MISCELLANEOUS;
                this.mapper_[this.userPermissionEnum.MAINTAIN_STUDENT_FILTER.valueOf()] = this.userPermissionEnum.MAINTAIN_STUDENT_FILTER;
                this.mapper_[this.userPermissionEnum.VIEW_STUDENT_FILTER.valueOf()] = this.userPermissionEnum.VIEW_STUDENT_FILTER;
                this.mapper_[this.userPermissionEnum.VIEW_TRANSCRIPT.valueOf()] = this.userPermissionEnum.VIEW_TRANSCRIPT;
                this.mapper_[this.userPermissionEnum.CHALKABLE_ADMIN.valueOf()] = this.userPermissionEnum.CHALKABLE_ADMIN;
                this.mapper_[this.userPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS.valueOf()] = this.userPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS;
                this.mapper_[this.userPermissionEnum.AWARD_LE_CREDITS.valueOf()] = this.userPermissionEnum.AWARD_LE_CREDITS;
                this.mapper_[this.userPermissionEnum.AWARD_LE_CREDITS_CLASSROOM.valueOf()] = this.userPermissionEnum.AWARD_LE_CREDITS_CLASSROOM;
                this.mapper_[this.userPermissionEnum.VIEW_CLASSROOM_STUDENTS.valueOf()] = this.userPermissionEnum.VIEW_CLASSROOM_STUDENTS;
                /*report permissions */

                this.mapper_['000001a'] = this.userPermissionEnum.ATTENDANCE_PROFILE_REPORT;
                this.mapper_['000001a_Classroom'] = this.userPermissionEnum.ATTENDANCE_PROFILE_REPORT_CLASSROOM;
                this.mapper_['000002'] = this.userPermissionEnum.BIRTHDAY_LISTING_REPORT;
                this.mapper_['000002_Classroom'] = this.userPermissionEnum.BIRTHDAY_LISTING_REPORT_CLASSROOM;
                this.mapper_['000030'] = this.userPermissionEnum.LESSON_PLAN_REPORT;
                this.mapper_['000030_Classroom'] = this.userPermissionEnum.LESSON_PLAN_REPORT_CLASSROOM;
                this.mapper_['000031'] = this.userPermissionEnum.PROGRESS_REPORT;
                this.mapper_['000032'] = this.userPermissionEnum.WORKSHEET_REPORT;
                this.mapper_['000064'] = this.userPermissionEnum.CLASSROOM_ATTENDANCE_REGISTER_REPORT;
                this.mapper_['000141'] = this.userPermissionEnum.SEATING_CHART_REPORT;
                this.mapper_['000074'] = this.userPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT;
                this.mapper_['000074_Classroom'] = this.userPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT_CLASSROOM;
                this.mapper_['000023'] = this.userPermissionEnum.GRADE_BOOK_REPORT;
                this.mapper_['000023_Classroom'] = this.userPermissionEnum.GRADE_BOOK_REPORT_CLASSROOM;
                this.mapper_['000087'] = this.userPermissionEnum.MISSING_ASSIGNMENTS_REPORT;
                this.mapper_['000090'] = this.userPermissionEnum.GRADE_VERIFICATION_REPORT;
                this.mapper_['000090_Classroom'] = this.userPermissionEnum.GRADE_VERIFICATION_REPORT_CLASSROOM;



                /*view field permission*/
                this.mapper_['Contact.First Name'] = this.userPermissionEnum.CONTACT_FIRST_NAME;
                this.mapper_['Contact.Last Name'] = this.userPermissionEnum.CONTACT_LAST_NAME;
                this.mapper_['Contact.Address'] = this.userPermissionEnum.CONTACT_ADDRESS;
                this.mapper_['Contact.EmployerName'] = this.userPermissionEnum.CONTACT_EMPLOYER_NAME;
                this.mapper_['Contact.MaritalStatus'] = this.userPermissionEnum.CONTACT_MARITAL_STATUS;
                this.mapper_['Contact.SocialSecurityNumber'] = this.userPermissionEnum.CONTACT_SOCIAL_SECURITY_NUMBER;
                this.mapper_['Contact.Telephone'] = this.userPermissionEnum.CONTACT_TELEPHONE;

                this.mapper_['Staff.Address'] = this.userPermissionEnum.STAFF_ADDRESS;
                this.mapper_['Staff.Display Name'] = this.userPermissionEnum.STAFF_DISPLAY_NAME;
                this.mapper_['Staff.First Name'] = this.userPermissionEnum.STAFF_FIRST_NAME;
                this.mapper_['Staff.Last Name'] = this.userPermissionEnum.STAFF_LAST_NAME;

                this.mapper_['Student.Address'] = this.userPermissionEnum.STUDENT_ADDRESS;
                this.mapper_['Student.Contact'] = this.userPermissionEnum.STUDENT_CONTACT;
                this.mapper_['Student.Grade'] = this.userPermissionEnum.STUDENT_GRADE;
                this.mapper_['Student.InternetPassword'] = this.userPermissionEnum.STUDENT_INTERNET_PASSWORD;
                this.mapper_['Student.IsAllowedInetAccess'] = this.userPermissionEnum.STUDENT_IS_ALLOWED_INET_ACCESS;
                this.mapper_['Student.Photograph'] = this.userPermissionEnum.STUDENT_PHOTOGRAPH;
                this.mapper_['Student.School'] = this.userPermissionEnum.STUDENT_SCHOOL;
                this.mapper_['Student.Telephone'] = this.userPermissionEnum.STUDENT_TELEPHONE;
                this.mapper_['Student.TransportMethod'] = this.userPermissionEnum.STUDENT_TRANSPORT_METHOD;
                this.mapper_['Student.Date Of Birth'] = this.userPermissionEnum.STUDENT_DATE_OF_BIRTH;
                this.mapper_['Student.First Name'] = this.userPermissionEnum.STUDENT_FIRST_NAME;
                this.mapper_['Student.Gender'] = this.userPermissionEnum.STUDENT_GENDER;
                this.mapper_['Student.Last Name'] = this.userPermissionEnum.STUDENT_LAST_NAME;
                this.mapper_['Student.Student Number'] = this.userPermissionEnum.STUDENT_STUDENT_NUMBER;
                this.mapper_['Student.SocialSecurityNumber'] = this.userPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER;
                this.mapper_['Student.StateIDNumber'] = this.userPermissionEnum.STUDENT_STATE_ID_NUMBER;

            }
        }
    ]);


    /** @class chlk.models.people.Claim*/
    CLASS(
        UNSAFE, FINAL, 'Claim', IMPLEMENTS(ria.serialize.IDeserializable),  [

        function preparePermisssions_(){
            var mapper = new chlk.models.people.UserPermissionMappeer();
            if(this.permissions == null){
                this.permissions = [];
                var values = this.getValues();
                this.permissions = values.map(function(value){
                    return mapper.hasKey(value) ? mapper.map(value) : null;
                }.bind(this));
            }
        },

        VOID, function deserialize(raw){
            this.type = SJX.fromValue(raw.type, String);
            this.values = SJX.fromArrayOfValues(raw.values, String);
            this.preparePermisssions_();

        },
        String, 'type',
        ArrayOf(String), 'values',

        READONLY, ArrayOf(chlk.models.people.UserPermissionEnum), 'permissions',

        ArrayOf(chlk.models.people.UserPermissionEnum), function getPermissions(){
            this.preparePermisssions_();
            return this.permissions;
        },

        function $(){
            BASE();
            this.permissions = null;
            this._permissionEnum = chlk.models.people.UserPermissionEnum;
        },

        [[chlk.models.people.UserPermissionEnum]],
        Boolean, function hasPermission(permission){
            var permissions = this.getPermissions() || [];
            return permissions.filter(function(item){return item == permission}).length > 0;
        }
    ]);
});
