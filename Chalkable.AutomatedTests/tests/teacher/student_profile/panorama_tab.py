from base_auth_test import *
import unittest

class TestStudentProfilePanorama(BaseAuthedTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        dictionary_get_list_my_students = self.teacher.get_json(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        if len(dictionary_get_list_my_students['data']) > 10:
            student_id = dictionary_get_list_my_students['data'][10]['id']

            self.teacher.post_json('/Student/Panorama.json', data={'studentId': student_id})

    def test_student_profile_info_panorama(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()