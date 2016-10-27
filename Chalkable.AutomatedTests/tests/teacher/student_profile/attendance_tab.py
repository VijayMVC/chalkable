from datetime import date
from base_auth_test import *
import unittest

class TestStudentProfileAttendance(BaseAuthedTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        dictionary_get_list_my_students = self.teacher.get_json(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        today = date.today()
        current_date = str(today.month) + str("-") + str(today.day) + str("-") + str(today.year)

        if len(dictionary_get_list_my_students['data']) > 10:
            student_id = dictionary_get_list_my_students['data'][10]['id']

            student_attendance = self.teacher.get_json('/Student/AttendanceSummary.json?' + 'studentId=' + str(student_id))
            student_data = student_attendance['data']

            self.teacher.get_json('/AttendanceCalendar/MonthForStudent.json?' + 'studentId=' + str(student_id))

            self.teacher.get_json('/AttendanceCalendar/MonthForStudent.json?' + 'date=' + str(current_date) + '&studentId=' + str(student_id))

            if len(student_data['gradingperiods']) > 0:
                for i in student_data['gradingperiods']:
                    self.teacher.get_json(
                        '/Student/AttendanceSummary?' + 'studentId=' + str(student_id) + '&gradingPeriodId=' + str(
                            i['id']))

    def test_student_profile_info_attendance(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()