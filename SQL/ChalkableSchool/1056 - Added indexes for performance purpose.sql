Create Index IX_Announcement_Class on Announcement(ClassRef)
GO
Create Index IX_AnnouncementQnA_Announcement on AnnouncementQnA(AnnouncementRef)
GO
Create Index IX_AnnouncementAttachment_Announcement on AnnouncementAttachment(AnnouncementRef)
GO
Create Index IX_Class_PrimaryTeacher on Class(PrimaryTeacherRef)
GO
Create Index IX_ClassPerson_Class on ClassPerson(ClassRef)
GO
Create Index IX_ClassPerson_Person on ClassPerson(PersonRef)
GO
Create Index StudentSchool_Student on StudentSchool(StudentRef)
GO
Create Index StaffSchool_Staff on StaffSchool(StaffRef)
GO