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

            self.teacher.get_json('/Student/Explorer.json?' + 'personId=' + str(student_id))

            self.teacher.get_json('/Application/SuggestedApps.json?' + 'start=' + str(0) + '&count=' + str(9999) + '&myAppsOnly=' + str(True))

            # typed manually
            self.teacher.get_json('/Application/SuggestedApps.json?' + 'abIds=' + str('1919e2e0-9d49-11e0-8793-50509dff4b22') + '&start=' + str(0) + '&count=' + str(9999) + '&myAppsOnly=' + str(True))

    def test_student_profile_info_explorer(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()