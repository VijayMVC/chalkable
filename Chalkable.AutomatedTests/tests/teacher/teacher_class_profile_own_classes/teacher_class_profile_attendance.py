from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        list_of_classes = self.list_of_classes
        one_random_class = random.choice(list_of_classes)

        get_class_summary = self.get('/Class/Summary.json?' + 'classId=' + str(one_random_class))

        get_attendance_year_page_of_one_class = self.get('/Class/AttendanceSummary.json?' + 'classId=' + str(one_random_class) + '&dateType=' + str(0))

        get_attendance_grading_period_page_of_one_class = self.get('/Class/AttendanceSummary.json?' + 'classId=' + str(one_random_class) + '&dateType=' + str(1))

        get_attendance_month_page_of_one_class = self.get('/Class/AttendanceSummary.json?' + 'classId=' + str(one_random_class) + '&dateType=' + str(2))

        get_attendance_week_page_of_one_class = self.get('/Class/AttendanceSummary.json?' + 'classId=' + str(one_random_class) + '&dateType=' + str(3))
        
if __name__ == '__main__':
    unittest.main()