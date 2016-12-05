from datetime import date
from base_auth_test import *
import unittest

class TestStudentProfileSchedule(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        dictionary_get_list_my_students = self.teacher.get_json(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        today = date.today()
        current_date = str(today.month) + str("-") + str(today.day) + str("-") + str(today.year)

        if len(dictionary_get_list_my_students['data']) > 10:
            student_id = dictionary_get_list_my_students['data'][10]['id']

            self.teacher.get_json('/AnnouncementCalendar/Week.json?' + 'personId=' + str(student_id))

            self.teacher.get_json(
                '/AnnouncementCalendar/Week.json?' + 'date=' + str(current_date) + '&personId=' + str(student_id))

            self.teacher.get_json(
                '/AnnouncementCalendar/Week.json?' + 'date=' + str(current_date) + '&personId=' + str(student_id))

            self.teacher.get_json('/AnnouncementCalendar/List.json?' + 'date=' + str(current_date) + '&personId=' + str(student_id))

    def test_student_profile_info_tab(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()