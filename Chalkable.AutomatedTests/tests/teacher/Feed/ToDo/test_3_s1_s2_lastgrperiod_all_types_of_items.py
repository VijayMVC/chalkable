from base_auth_test import *
import unittest

class TestAllTypesOfItemsByGradingPeriods(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

        # reset settings
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.teacher.post_json('/Announcement/UnDone.json?', data={'option': 3})

        # get a current time
        self.current_time = datetime.now()

        # getting grading periods
        self.list_for_start_date = []
        self.list_for_end_date = []

        get_grading_periods = self.teacher.get_json('/GradingPeriod/List.json?')
        get_first_period = get_grading_periods['data']

        for item in get_first_period:
            self.list_for_start_date.append(datetime.date(datetime.strptime(item['startdate'], '%Y-%m-%d')))
            self.list_for_end_date.append(datetime.date(datetime.strptime(item['enddate'], '%Y-%m-%d')))

        self.list_for_start_date.sort()
        self.list_for_end_date.sort()

    def internal_(self, gr_periods, start_gr_period, end_gr_period):
        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.teacher.get_json(
                '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

            return dictionary_verify_annoucementviewdatas_all

        def get_item_date(one_item):
            if one_item['type'] == 1:
                return datetime.date(datetime.strptime(one_item['classannouncementdata']['expiresdate'], '%Y-%m-%d'))
            if one_item['type'] == 4:
                return datetime.date(
                    datetime.strptime(one_item['supplementalannouncementdata']['expiresdate'], '%Y-%m-%d'))

        def get_lesson_plan_start_date(one_item):
            if one_item['type'] == 3:
                return datetime.date(datetime.strptime(one_item['lessonplandata']['startdate'], '%Y-%m-%d'))

        def get_lesson_plan_end_date(one_item):
            if one_item['type'] == 3:
                return datetime.date(datetime.strptime(one_item['lessonplandata']['enddate'], '%Y-%m-%d'))

        # filter: last gr. period, earliest
        self.teacher.post_json('/Feed/SetSettings.json?', data={'sortType': '0', 'gradingPeriodId': gr_periods})

        for item in list_items_json_unicode(0, 2500):
            complete = item['complete']

            self.assertTrue(complete == False, "Verify item's state on false")

            if item['type'] == 3:
                self.assertTrue(
                    (get_lesson_plan_start_date(item) >= start_gr_period) and (get_lesson_plan_end_date(item) <=
                    end_gr_period),
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
        self.internal_(self.teacher.gr_periods()[0], self.list_for_start_date[0], self.list_for_end_date[0])

    def test_items_second_gr_periods(self):
        self.internal_(self.teacher.gr_periods()[1], self.list_for_start_date[1], self.list_for_end_date[1])

    def test_items_last_gr_periods(self):
        self.internal_(self.teacher.gr_periods()[-1], self.list_for_start_date[-1], self.list_for_end_date[-1])

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()