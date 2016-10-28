from base_auth_test import *
import unittest

class TestAllTypesOfItemsByGradingPeriods(BaseTestCase):
    def setUp(self):
        self.student = StudentSession(self).login(user_email, user_pwd)

        # reset settings
        self.student.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.student.post_json('/Announcement/UnDone.json?', data={'option': 3})

        # get a current time
        self.current_time = time.strftime('%Y-%m-%d')

        # getting grading periods
        list_for_start_date = []
        list_for_end_date = []
        get_grading_periods = self.student.get_json('/GradingPeriod/List.json?')
        get_first_period = get_grading_periods['data']

        for item in get_first_period:
            startdate_2 = item['startdate']
            list_for_start_date.append(startdate_2)
            enddate_2 = item['enddate']
            list_for_end_date.append(enddate_2)

        self.decoded_list_1 = [x.encode('utf-8') for x in list_for_start_date]
        self.decoded_list_2 = [x.encode('utf-8') for x in list_for_end_date]

    def internal_(self, gr_periods, start_gr_period, end_gr_period):
        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.student.get_json(
                '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

            return dictionary_verify_annoucementviewdatas_all

        def get_item_date(one_item):
            if one_item['type'] == 1:
                return one_item['classannouncementdata']['expiresdate']
            if one_item['type'] == 4:
                return one_item['supplementalannouncementdata']['expiresdate']

        def get_lesson_plan_start_date(one_item):
            if one_item['type'] == 3:
                return one_item['lessonplandata']['startdate']

        def get_lesson_plan_end_date(one_item):
            if one_item['type'] == 3:
                return one_item['lessonplandata']['enddate']

        # filter: last gr. period, earliest
        self.student.post_json('/Feed/SetSettings.json?', data={'sortType': '0', 'gradingPeriodId': gr_periods})

        for item in list_items_json_unicode(0, 2500):
            complete = item['complete']

            self.assertTrue(complete == False, "Verify item's state on false")

            if item['type'] == 3:
                self.assertTrue(
                    get_lesson_plan_start_date(item) >= start_gr_period and get_lesson_plan_end_date(item) <=
                    end_gr_period,
                    'Lesson plan is out of the last gr. period, ' + str(item["id"]) + ' ' + str(get_lesson_plan_start_date(item)) + ' >= ' + str(start_gr_period) + ' and ' + ' ' + str(get_lesson_plan_end_date(item)) + ' <= ' +
                    str(end_gr_period))

            if item['type'] == 1 or item['type'] == 4:
                self.assertLessEqual(start_gr_period, get_item_date(item),
                                     'Date of an activity/supplemental is out of the beginning of the last gr. period, ' + str(
                                         item["id"]))

                self.assertGreaterEqual(end_gr_period, get_item_date(item),
                                        'Date of an activity/supplemental is out of the end of the last gr. period ' + str(
                                            item["id"]))

    def test_items_first_gr_period(self):
        self.internal_(self.student.gr_periods()[0], self.decoded_list_1[0], self.decoded_list_2[0])

    def test_items_second_gr_periods(self):
        self.internal_(self.student.gr_periods()[1], self.decoded_list_1[1], self.decoded_list_2[1])

    def test_items_last_gr_periods(self):
        self.internal_(self.student.gr_periods()[-1], self.decoded_list_1[-1], self.decoded_list_2[-1])

    def tearDown(self):
        # reset all filters on the feed
        self.student.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()