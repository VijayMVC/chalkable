from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        list_of_classes = self.list_of_classes
        one_random_class = random.choice(list_of_classes)
        school_year = self.school_year()


        get_class_summary = self.get('/Class/ClassesStats.json?' + 'schoolYearId=' + str(school_year) + '&start=' + str(0) + '&count=' +str(250) + '&sortType=' + str())

        get_class_summary_data = get_class_summary['data']

        list_for_class_id = []
        
        for i in get_class_summary_data:
            list_for_class_id.append(i['id'])

        random_class_id = random.choice(list_for_class_id)

        get_class_summary = self.get('/Class/Summary.json?' + 'classId=' + str(random_class_id))

        get_attendance_year_page_of_one_class = self.get('/Class/AttendanceSummary.json?' + 'classId=' + str(random_class_id) + '&dateType=' + str(0))

        get_attendance_grading_period_page_of_one_class = self.get('/Class/AttendanceSummary.json?' + 'classId=' + str(random_class_id) + '&dateType=' + str(1))

        get_attendance_month_page_of_one_class = self.get('/Class/AttendanceSummary.json?' + 'classId=' + str(random_class_id) + '&dateType=' + str(2))

        get_attendance_week_page_of_one_class = self.get('/Class/AttendanceSummary.json?' + 'classId=' + str(random_class_id) + '&dateType=' + str(3))

if __name__ == '__main__':
    unittest.main()