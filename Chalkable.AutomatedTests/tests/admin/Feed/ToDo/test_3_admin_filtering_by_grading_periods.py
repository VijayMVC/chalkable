from base_auth_test import *
import unittest

class TestByGradingPeriods(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        # reset settings
        self.admin.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.admin.post_json('/Announcement/UnDone.json?', data={'option': 3})

        # get a current time
        self.current_time = time.strftime('%Y-%m-%d')

        # getting grading periods
        list_for_start_date = []
        list_for_end_date = []
        get_grading_periods = self.admin.get_json('/GradingPeriod/List.json?')
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
            list_items_json_unicode = self.admin.post_json('/Feed/DistrictAdminFeed.json?' + 'gradeLevelIds=' + '&start=' + str(start) + '&complete=' + str(False) + '&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

            return dictionary_verify_annoucementviewdatas_all

        def get_item_date(one_item):
            return one_item['adminannouncementdata']['expiresdate']

        # filter: last gr. period, earliest
        self.admin.post_json('/Feed/SetSettings.json?', data={'sortType': '0', 'gradingPeriodId': gr_periods})

        for item in list_items_json_unicode(0, 2500):

            self.assertFalse(item['complete'], "Verify item's state on false")

            self.assertLessEqual(start_gr_period, get_item_date(item),
                                 'Date of an admin announcement is out of the beginning of the gr. period, ' + str(
                                     item["id"]))

            self.assertGreaterEqual(end_gr_period, get_item_date(item),
                                    'Date of an admin announcement is out of the end of the gr. period' + str(
                                        item["id"]))

    def test_items_first_gr_period(self):
        self.internal_(self.admin.gr_periods()[0], self.decoded_list_1[0], self.decoded_list_2[0])

    def test_items_second_gr_periods(self):
        self.internal_(self.admin.gr_periods()[1], self.decoded_list_1[1], self.decoded_list_2[1])

    def test_items_last_gr_periods(self):
        self.internal_(self.admin.gr_periods()[-1], self.decoded_list_1[-1], self.decoded_list_2[-1])

    def tearDown(self):
        # reset all filters on the feed
        self.admin.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()