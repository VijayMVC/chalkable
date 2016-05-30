from base_auth_test import *


class TestStudentProfileInfo(BaseAuthedTestCase):
    def test_student_getstudents_get(self):
        dictionary_get_students_list = self.get(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=10')
        self.assertGreater(len(dictionary_get_students_list['data']), 5, 'At least 5 students')

        fifth_student = dictionary_get_students_list['data'][4]
        #print fifth_student
        self.assertTrue('id' in fifth_student, 'id exists')
        self.assertTrue('firstname' in fifth_student, 'firstname exists')
        self.assertTrue('lastname' in fifth_student, 'lastname exists')

    def test_student_info_get(self):
        dictionary_get_students_list = self.get(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=10')
        self.assertGreater(len(dictionary_get_students_list['data']), 5, 'At least 5 students')

        fifth_student = dictionary_get_students_list['data'][4]
        self.assertTrue('id' in fifth_student, 'id exists')

        student_id_str = str(fifth_student['id'])

        student_info = self.get('/Student/Info.json?personId=' + student_id_str)

        for t in (
            'active', 'address', 'age', 'birthdate', 'caneditlogin', 'displayname', 'email', 'firstname', 'fullname',
            'gender', 'gradelevel', 'gradelevels', 'hasmedicalalert', 'healthconditions', 'id', 'isallowedinetaccess',
            'lastname', 'login', 'parents', 'phones', 'role', 'salutation', 'schoolid', 'specialinstructions',
            'spedstatus', 'studentcontacts'):
            self.assertTrue(t in student_info['data'], 'Expect ' + t + ' field in response')


if __name__ == '__main__':
    unittest.main()
