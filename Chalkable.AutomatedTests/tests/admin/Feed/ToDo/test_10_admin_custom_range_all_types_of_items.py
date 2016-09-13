from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # reset all filters on the feed
        self.dict = {}
        self.post_admin('/Feed/SetSettings.json?', self.dict)

        self.settings_data_for_mark_undone = {'option': '3'}

        # making all types of items as 'undone'
        self.post_admin('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)

        # getting grading periods
        list_for_start_date = []
        list_for_end_date = []
        get_grading_periods = self.get('/GradingPeriod/List.json?')
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
        self.start_of_marking_period = self.start_date_school_year
        self.end_of_marking_period = self.end_date_school_year

        start_mp_period_in_date_time = datetime.strptime(self.start_of_marking_period, "%Y-%m-%d")
        start_mp_period_plus_10 = start_mp_period_in_date_time.date() + timedelta(days=10)

        end_mp_period_in_date_time = datetime.strptime(self.end_of_grading_period, "%Y-%m-%d")
        end_mp_period_minus_10 = end_mp_period_in_date_time.date() - timedelta(days=10)
        self.current_date_plus2_10 = start_mp_period_plus_10.strftime("%Y-%m-%d")
        self.current_date_minus2_10 = end_mp_period_minus_10.strftime("%Y-%m-%d")

        self.random_date_string_format = self.random_date(self.current_date_plus2_10, self.current_date_minus2_10)

        self.random_date_string_format_correct_format = datetime.strptime(self.random_date_string_format,
                                                                          "%m-%d-%Y").strftime("%Y-%m-%d")

        random_date_date_time_format = datetime.strptime(self.random_date_string_format_correct_format, "%Y-%m-%d")
        random_date_minus_10 = random_date_date_time_format.date() - timedelta(days=10)
        random_date_plus_10 = random_date_date_time_format.date() + timedelta(days=10)
        self.current_date_minus_10 = random_date_minus_10.strftime("%Y-%m-%d")
        self.current_date_plus_10 = random_date_plus_10.strftime("%Y-%m-%d")

        self.settings_data = {'sortType': '0', 'fromDate': self.current_date_minus_10, 'toDate': self.current_date_plus_10}
        self.post_admin('/Feed/SetSettings.json?', self.settings_data)

        self.do_feed_list_and_verify(0)

    def do_feed_list_and_verify(self, start, count=2000):
        # get a current time
        self.current_time = time.strftime('%Y-%m-%d')

        list_items_json_unicode = self.get_admin('/Feed/DistrictAdminFeed.json?' + 'gradeLevelIds=' + '&start=' + str(start) + '&complete=' + str(False) + '&count=' + str(count))
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

        for item in dictionary_verify_annoucementviewdatas_all:
            def get_item_date(one_item):
                return one_item['adminannouncementdata']['expiresdate']

            self.assertTrue((get_item_date(item) >= self.current_date_minus_10) and (get_item_date(item) <= self.current_date_plus_10), 'Admin announcements out off the custom range are shown ' + str(item["id"]))

    def tearDown(self):
        # reset all filters on the feed
        self.dict = {}
        self.post_admin('/Feed/SetSettings.json?', self.dict)

if __name__ == '__main__':
    unittest.main()