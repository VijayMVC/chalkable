from base_auth_test import *
import unittest

class TestStudentProfileGrading(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        dictionary_get_list_my_students = self.teacher.get_json(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        if len(dictionary_get_list_my_students['data']) > 10:
            student_id = dictionary_get_list_my_students['data'][10]['id']
            student_grading = self.teacher.get_json('/Student/GradingSummary.json?' + 'studentId=' + str(student_id))
            student_data = student_grading['data']

            if len(student_data['gradesbygradingperiod']) > 0:
                for i in student_data['gradesbygradingperiod']:
                    self.teacher.get_json(
                        '/Student/GradingDetails.json?' + 'studentId=' + str(student_id) + '&gradingPeriodId=' + str(i['gradingperiod']['id']))

    def test_student_profile_info_tab(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()