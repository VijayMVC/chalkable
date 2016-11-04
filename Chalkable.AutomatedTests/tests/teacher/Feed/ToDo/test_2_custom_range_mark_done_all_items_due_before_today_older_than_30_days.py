from base_auth_test import *
import unittest


class TestCustomRangeMarkItems(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

        # reset settings
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.teacher.post_json('/Announcement/UnDone.json?', data={'option': 3})

        # get a current time
        self.current_time = time.strftime('%Y-%m-%d')

        # get a date needed for filtering
        date = datetime.strptime(self.current_time, "%Y-%m-%d")
        end_date = date.today() - timedelta(days=30)
        self.current_date_minus_30 = end_date.strftime("%Y-%m-%d")

        # getting grading periods
        list_for_start_date = []
        list_for_end_date = []
        get_grading_periods = self.teacher.get_json('/GradingPeriod/List.json?')
        get_first_period = get_grading_periods['data']
        if len(get_first_period) > 0:
            for item in get_first_period:
                self.startdate_2 = item['startdate']
                list_for_start_date.append(self.startdate_2)
                self.enddate_2 = item['enddate']
                list_for_end_date.append(self.enddate_2)

        decoded_list_1 = [x.encode('utf-8') for x in list_for_start_date]
        decoded_list_2 = [x.encode('utf-8') for x in list_for_end_date]

        # getting a start of the first grading period and an end of the last grading period
        self.start_of_grading_period = decoded_list_1[0]
        self.end_of_grading_period = decoded_list_2[-1]

        start_gr_period_in_date_time = datetime.strptime(self.start_of_grading_period, "%Y-%m-%d")
        start_gr_period_plus_10 = start_gr_period_in_date_time.date() + timedelta(days=10)

        end_gr_period_in_date_time = datetime.strptime(self.end_of_grading_period, "%Y-%m-%d")
        end_gr_period_minus_10 = end_gr_period_in_date_time.date() - timedelta(days=10)
        self.current_date_plus2_10 = start_gr_period_plus_10.strftime("%Y-%m-%d")
        self.current_date_minus2_10 = end_gr_period_minus_10.strftime("%Y-%m-%d")

        self.random_date_string_format = self.random_date(self.current_date_plus2_10, self.current_date_minus2_10)

        self.random_date_string_format_correct_format = datetime.strptime(self.random_date_string_format,
                                                                          "%m-%d-%Y").strftime("%Y-%m-%d")

        random_date_date_time_format = datetime.strptime(self.random_date_string_format_correct_format, "%Y-%m-%d")
        random_date_minus_10 = random_date_date_time_format.date() - timedelta(days=10)
        random_date_plus_10 = random_date_date_time_format.date() + timedelta(days=10)
        self.current_date_minus_10 = random_date_minus_10.strftime("%Y-%m-%d")
        self.current_date_plus_10 = random_date_plus_10.strftime("%Y-%m-%d")

        # filter: custom range, earliest
        self.teacher.post_json('/Feed/SetSettings.json?', data={'sortType': '0',
                                                                'fromDate': self.current_date_minus_10,
                                                                'toDate': self.current_date_plus_10})

    def internal_(self, option):
        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.teacher.get_json(
                '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']
            return dictionary_verify_annoucementviewdatas_all

        def get_lesson_plan_start_date(one_item):
            if one_item['type'] == 3:
                return one_item['lessonplandata']['startdate']

        def get_lesson_plan_end_date(one_item):
            if one_item['type'] == 3:
                return one_item['lessonplandata']['enddate']

        def get_item_date(one_item):
            if one_item['type'] == 1:
                return one_item['classannouncementdata']['expiresdate']
            if one_item['type'] == 4:
                return one_item['supplementalannouncementdata']['expiresdate']

        def current_time_or_time_minus_30(inner_option):
            if inner_option == 2:
                inner_current_time = self.current_date_minus_30
                return inner_current_time
            if inner_option == 1:
                inner_current_time = self.current_time
                return inner_current_time

        # marking items
        self.teacher.post_json('/Announcement/Done.json?', data={'option': option})

        if option == 3:
            self.assertTrue(len(list_items_json_unicode(0, 2500)) == 0, 'Items were not marked as done!')

            # reset all filters on the feed
            self.teacher.post_json('/Feed/SetSettings.json?', data={})

            def get_item_date2(one_item):
                if one_item['type'] == 1:
                    return one_item['classannouncementdata']['expiresdate']
                if one_item['type'] == 4:
                    return one_item['supplementalannouncementdata']['expiresdate']
                if one_item['type'] == 3:
                    return one_item['lessonplandata']['startdate']

            for item in list_items_json_unicode(0, 2500):
                self.assertTrue((get_item_date2(item) < self.current_date_minus_10 or (get_item_date(item) > self.current_date_plus_10),
                                     'Items are not marked as complete ' + str(item["id"])))

        if option == 1 or option == 2:
            for item in list_items_json_unicode(0, 2500):
                if item['type'] == 1 or item['type'] == 4:
                    self.assertTrue(
                        (get_item_date(item) >= current_time_or_time_minus_30(option),
                         'This activity and supplementals were not marked as complete ' + str(item["id"]) + " " + str(
                             get_item_date(item))))

                if item['type'] == 3:
                    self.assertTrue(
                        (get_lesson_plan_start_date(item) >= self.current_date_minus_10 and get_lesson_plan_end_date(
                            item) >= current_time_or_time_minus_30(option)),
                        'This lesson plan have not to be in the list ' + str(item["id"]))

    def test_mark_done_all_items(self):
        self.internal_(3)

    def test_mark_done_due_before_today(self):
        self.internal_(1)

    def test_mark_done_older_than_30_days(self):
        self.internal_(2)

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()