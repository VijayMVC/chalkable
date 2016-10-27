from base_auth_test import *
import unittest

class TestStudentProfileInfo(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self, user_email, person_id):
        dictionary_get_list_my_students = self.teacher.get_json(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        if len(dictionary_get_list_my_students['data']) > 10:
            student_id = dictionary_get_list_my_students['data'][10]['id']
            student_info = self.teacher.get_json('/Student/Info.json?' + 'personId=' + str(student_id))

        post_update_email = self.teacher.post_json('/Person/UpdateInfo.json',
                                           data={"email": user_email, "personId": person_id})

    def internal2_(self):
        student_info = self.teacher.get_file_(
            '/Student/DownloadHealthFormDocument?' + 'studentId=' + str(3688) + '&healthFormId=' + str(12))

    def test_student_profile_info_tab(self):
        self.internal_("CAYERS@sti.com9", 3688)

    def test_student_profile_info_tab_email(self):
        self.internal_("CAYERS@sti.com", 3688)

    def test_student_profile_info_tab_health_report(self):
        self.internal2_()

if __name__ == '__main__':
    unittest.main()