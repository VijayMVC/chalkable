from base_auth_test import *
import unittest

class TestStudentProfileExplorer(BaseAuthedTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        dictionary_get_list_my_students = self.teacher.get_json(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        if len(dictionary_get_list_my_students['data']) > 10:
            student_id = dictionary_get_list_my_students['data'][10]['id']

            self.teacher.get_json('/Student/Apps.json?' + 'studentId=' + str(student_id) + '&start=' +str(0) + '&count=' + str(1000))

    def test_student_profile_info_apps(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()