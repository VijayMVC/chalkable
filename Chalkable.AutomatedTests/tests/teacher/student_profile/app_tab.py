from base_auth_test import *


class TestStudentProfileInfo(BaseAuthedTestCase):
    def test_student_info_get(self):
        dictionary_get_list_my_students = self.get(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        student_id = None
        if len(dictionary_get_list_my_students['data']) > 10:
            eleventh_student = dictionary_get_list_my_students['data'][10]
            for key, value in eleventh_student.iteritems():
                if key == 'id':
                    student_id = value
            student_app = self.get('/Student/Apps.json?' + 'studentId=' + str(student_id) + '&start=' +str(0) + '&count=' + str(1000))

if __name__ == '__main__':
    unittest.main()
