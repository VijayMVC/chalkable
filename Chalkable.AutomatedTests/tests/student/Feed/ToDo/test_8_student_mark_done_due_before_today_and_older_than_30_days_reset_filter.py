from base_auth_test import *
import unittest

class TestCustomRangeDueBeforeTodayResentFilters(BaseTestCase):
    def setUp(self):
        self.student = StudentSession(self).login(user_email, user_pwd)

        # reset settings
        self.student.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.student.post_json('/Announcement/UnDone.json?', data={'option': 3})

        # get a current time
        self.current_time = datetime.date(datetime.now())

        # get a date needed for filtering
        date = datetime.now()
        end_date = date.today() - timedelta(days=29)
        self.current_date_minus_30 = datetime.date(end_date)

        # getting grading periods
        list_for_start_date = []
        list_for_end_date = []
        get_grading_periods = self.student.get_json('/GradingPeriod/List.json?')
        get_first_period = get_grading_periods['data']
        if len(get_first_period) > 0:
            for item in get_first_period:
                list_for_start_date.append(datetime.strptime(item['startdate'], '%Y-%m-%d'))
                list_for_end_date.append(datetime.strptime(item['enddate'], '%Y-%m-%d'))

        list_for_start_date.sort()
        list_for_end_date.sort()

        # getting a start of the first grading period and an end of the last grading period
        self.start_of_grading_period = list_for_start_date[0]
        self.end_of_grading_period = list_for_end_date[-1]

        #start_gr_period_in_date_time = datetime.strptime(self.start_of_grading_period, "%Y-%m-%d")
        start_gr_period_plus_10 = self.start_of_grading_period + timedelta(days=10)

        #end_gr_period_in_date_time = datetime.strptime(self.end_of_grading_period, "%Y-%m-%d")
        end_gr_period_minus_10 = self.end_of_grading_period - timedelta(days=10)

        self.random_date_string_format = self.random_date(start_gr_period_plus_10.strftime("%Y-%m-%d"), end_gr_period_minus_10.strftime("%Y-%m-%d"))

        self.random_date_string_format_correct_format = datetime.strptime(self.random_date_string_format,
                                                                          "%m-%d-%Y").strftime("%Y-%m-%d")

        random_date_date_time_format = datetime.strptime(self.random_date_string_format_correct_format, "%Y-%m-%d")

        self.random_date_minus_10 = random_date_date_time_format.date() - timedelta(days=10)
        self.random_date_plus_10 = random_date_date_time_format.date() + timedelta(days=10)

        # filter: custom range, earliest
        self.student.post_json('/Feed/SetSettings.json?', data={'sortType': '0',
                                                                'fromDate': self.random_date_minus_10.strftime("%Y-%m-%d"),
                                                                'toDate': self.random_date_plus_10.strftime("%Y-%m-%d")})

    def internal_(self, option):
        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.student.get_json(
                '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']
            return dictionary_verify_annoucementviewdatas_all

        def get_lesson_plan_start_date(one_item):
            if one_item['type'] == 3:
                return datetime.date(datetime.strptime(one_item['lessonplandata']['startdate'], '%Y-%m-%d'))

        def get_lesson_plan_end_date(one_item):
            if one_item['type'] == 3:
                return datetime.date(datetime.strptime(one_item['lessonplandata']['enddate'], '%Y-%m-%d'))

        def get_item_date(one_item):
            if one_item['type'] == 1:
                return datetime.date(datetime.strptime(one_item['classannouncementdata']['expiresdate'], '%Y-%m-%d'))
            if one_item['type'] == 4:
                return datetime.date(
                    datetime.strptime(one_item['supplementalannouncementdata']['expiresdate'], '%Y-%m-%d'))

        def current_time_or_time_minus_30(inner_option):
            if inner_option == 2:
                inner_current_time = self.current_date_minus_30
                return inner_current_time
            if inner_option == 1:
                inner_current_time = self.current_time
                return inner_current_time

        # marking items
        self.student.post_json('/Announcement/Done.json?', data={'option': option})

        if option == 1:
            for item in list_items_json_unicode(0, 2500):
                if self.current_time > self.random_date_plus_10:
                    self.assertFalse(item['complete'], "Verify item's state on false")

                    if item['type'] == 1 or item['type'] == 4:
                        self.assertTrue(
                            (get_item_date(item) < self.random_date_minus_10) or (
                            get_item_date(item) > self.random_date_plus_10),
                            'This activity/supplemental has not to be in the list ' + str(item["id"]) + " " + str(
                                get_item_date(item)))

                    if item['type'] == 3:
                        self.assertTrue(
                            (get_lesson_plan_start_date(item) > self.random_date_plus_10) or (
                                get_lesson_plan_start_date(item) < self.random_date_minus_10) or ((
                                get_lesson_plan_start_date(
                                    item) >= self.random_date_minus_10) and (get_lesson_plan_end_date(
                                    item) >= self.current_time)),
                            'This lesson plan has not to be in the list ' + str(item["id"]))

                if (self.current_time >= self.random_date_minus_10) and (
                    self.current_time <= self.random_date_plus_10):
                    self.assertFalse(item['complete'], "Verify item's state on false")

                    if item['type'] == 1 or item['type'] == 4:
                        self.assertTrue(
                            (get_item_date(item) < self.random_date_minus_10) or (
                            get_item_date(item) >= self.current_time),
                            'This activity/supplemental has not to be in the list ' + str(item["id"]) + " " + str(
                                get_item_date(item)) + " " + str(get_item_date(item)))

                    if item['type'] == 3:
                        self.assertTrue(
                            (get_lesson_plan_start_date(item) > self.random_date_plus_10) or (
                                get_lesson_plan_start_date(item) < self.random_date_minus_10) or ((
                                get_lesson_plan_start_date(
                                    item) >= self.random_date_minus_10) and (get_lesson_plan_end_date(
                                    item) >= self.current_time)),
                            'This lesson plan have not to be in the list ' + str(item["id"]))

                if (self.current_time < self.random_date_minus_10):
                    self.assertFalse(item['complete'], "Verify item's state on false")

                    if item['type'] == 1 or item['type'] == 4:
                        self.assertTrue(
                            (get_item_date(item) >= self.current_time) or (get_item_date(item) <= self.current_time)
                            ,
                            'This activity have not to be in the list ' + str(item["id"]) + " " + str(
                                get_item_date(item)))

                    if item['type'] == 3:
                        self.assertTrue(
                            (get_lesson_plan_start_date(item) >= self.current_time) or (
                                get_lesson_plan_start_date(item) <= self.current_time)
                            ,
                            'These lesson plans have not to be in the list ' + str(item["id"]))

        if option == 2:
            for item in list_items_json_unicode(0, 2500):
                if self.current_time > self.random_date_plus_10:
                    self.assertFalse(item['complete'], "Verify item's state on false")

                    if item['type'] == 1 or item['type'] == 4:
                        self.assertTrue(
                            (get_item_date(item) < self.random_date_minus_10) or (
                            get_item_date(item) > self.random_date_plus_10) or ((
                            get_item_date(item) >= self.random_date_minus_10) and (get_item_date(
                                item) <= self.current_date_minus_30)),
                            'This activity/supplemental has not to be in the list ' + str(item["id"]) + " " + str(
                                get_item_date(item)))

                    if item['type'] == 3:
                        self.assertTrue(
                            (get_lesson_plan_start_date(item) > self.random_date_plus_10) or (
                                get_lesson_plan_start_date(item) < self.random_date_minus_10) or ((
                                get_lesson_plan_start_date(
                                    item) >= self.random_date_minus_10) and (get_lesson_plan_end_date(
                                    item) <= self.current_date_minus_30)),
                            'This lesson plan has not to be in the list ' + str(item["id"]))

                if (self.current_time >= self.random_date_minus_10) and (
                    self.current_time <= self.random_date_plus_10):
                    self.assertFalse(item['complete'], "Verify item's state on false")

                    if item['type'] == 1 or item['type'] == 4:
                        self.assertTrue(
                            (get_item_date(item) < self.random_date_minus_10) or (
                            get_item_date(item) >= self.current_time) or (
                                (get_item_date(item) >= self.random_date_minus_10) and (get_item_date(
                                item) <= self.current_date_minus_30)),
                            'This activity/supplemental has not to be in the list ' + str(item["id"]) + " " + str(
                                get_item_date(item)))

                    if item['type'] == 3:
                        self.assertTrue(
                            (get_lesson_plan_start_date(item) > self.random_date_plus_10) or (
                                get_lesson_plan_start_date(item) < self.random_date_minus_10) or ((
                                get_lesson_plan_start_date(
                                    item) >= self.random_date_minus_10) and (get_lesson_plan_end_date(
                                    item) <= self.current_date_minus_30)),
                            'This lesson plan has not to be in the list ' + str(item["id"]))

                if (self.current_time < self.random_date_minus_10):
                    self.assertFalse(item['complete'], "Verify item's state on false")

                    if item['type'] == 1 or item['type'] == 4:
                        self.assertTrue(
                            (get_item_date(item) >= self.current_time) or (get_item_date(item) <= self.current_time)
                            ,
                            'This activity/supplemental has not to be in the list ' + str(item["id"]) + " " + str(
                                get_item_date(item)))

                    if item['type'] == 3:
                        self.assertTrue(
                            (get_lesson_plan_start_date(item) >= self.current_time) or (
                                get_lesson_plan_start_date(item) <= self.current_time)
                            ,
                            'These lesson plans have not to be in the list ' + str(item["id"]))

    def test_mark_done_due_before_today_reset_filter(self):
        self.internal_(1)

    def test_mark_done_older_than_30_days_reset_filter(self):
        self.internal_(2)

    def tearDown(self):
        # reset all filters on the feed
        self.student.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()