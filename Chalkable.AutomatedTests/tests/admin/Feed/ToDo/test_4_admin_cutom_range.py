from base_auth_test import *
import unittest

class TestCustomRange(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        # reset settings
        self.admin.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.admin.post_json('/Announcement/UnDone.json?', data={'option': 3})

        # getting grading periods
        list_for_start_date = []
        list_for_end_date = []
        get_grading_periods = self.admin.get_json('/GradingPeriod/List.json?')
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
        self.admin.post_json('/Feed/SetSettings.json?', data={'sortType': '0',
                                                                'fromDate': self.current_date_minus_10,
                                                                'toDate': self.current_date_plus_10})

    def internal_(self):
        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.admin.get_json(
                '/Feed/DistrictAdminFeed.json?' + 'gradeLevelIds=' + '&start=' + str(start) + '&complete=' + str(
                    False) + '&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

            return dictionary_verify_annoucementviewdatas_all

        def get_item_date(one_item):
            return one_item['adminannouncementdata']['expiresdate']

        for item in list_items_json_unicode(0, 2500):
            self.assertTrue((get_item_date(item) >= self.current_date_minus_10) and (
            get_item_date(item) <= self.current_date_plus_10),
                            'Admin announcements out off the custom range are shown ' + str(item["id"]))

    def test_mark_done_all_items(self):
        self.internal_()

    def tearDown(self):
        # reset all filters on the feed
        self.admin.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()