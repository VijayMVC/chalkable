REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.id.CourseId');
REQUIRE('chlk.models.announcement.BaseStudentAnnouncementsViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.StudentAnnouncements*/
    CLASS(
        FINAL, UNSAFE, 'StudentAnnouncements',
                EXTENDS(chlk.models.announcement.BaseStudentAnnouncementsViewData.OF(chlk.models.announcement.StudentAnnouncement)),
                IMPLEMENTS(ria.serialize.IDeserializable),  [

            VOID, function deserialize(raw) {
                this.announcementTypeId = SJX.fromValue(raw.announcementtypeid, Number);
                this.schoolOptions = SJX.fromDeserializable(raw.schooloptions, chlk.models.school.SchoolOption);
                this.announcementTitle = SJX.fromValue(raw.announcmenttitel, String);
                this.classAvg = SJX.fromValue(raw.classavg, String);
                this.className = SJX.fromValue(raw.classname, String);
                this.courseId = SJX.fromValue(raw.courseid, chlk.models.id.CourseId);
                this.gradedStudentCount = SJX.fromValue(raw.gradedstudentcount, Number);
                this.items = SJX.fromArrayOfDeserializables(raw.items, chlk.models.announcement.StudentAnnouncement);
                this.showToStudents = SJX.fromValue(raw.showtostudents, Boolean);
                this.currentItem = SJX.fromDeserializable(raw.currentitem, chlk.models.announcement.StudentAnnouncement);
                this.selectedIndex = SJX.fromValue(raw.selectedindex, Number);
            },

            Number, 'announcementTypeId',
            chlk.models.school.SchoolOption, 'schoolOptions',
            String, 'announcementTitle',

            String, 'className',
            chlk.models.id.CourseId, 'courseId',
            Boolean, 'showToStudents',
            chlk.models.announcement.StudentAnnouncement, 'currentItem',
            Number, 'selectedIndex'
        ]);
});
