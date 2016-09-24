from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

        self.settings_data_for_mark_undone = {'option': '3'}

        # making admin announcement as 'undone'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)

        # filter: last gr. period, earliest
        self.settings_data = {'sortType': '0', 'gradingPeriodId': self.gr_periods()[-1]}
        self.post('/Feed/SetSettings.json?', self.settings_data)

        self.do_feed_list_and_verify(0)

    def do_feed_list_and_verify(self, start, count=2500):
        # get a current time
        self.current_time = time.strftime('%Y-%m-%d')

        list_items_json_unicode = self.get_admin('/Feed/DistrictAdminFeed.json?' + 'gradeLevelIds=' + '&start=' + str(start) + '&complete=' + str(False) + '&count=' + str(count))
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

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
        else:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all) == 0, 'There are no items!')
        decoded_list_1 = [x.encode('utf-8') for x in list_for_start_date]
        decoded_list_2 = [x.encode('utf-8') for x in list_for_end_date]

        def get_item_date(one_item):
            return one_item['adminannouncementdata']['expiresdate']

        for item in dictionary_verify_annoucementviewdatas_all:
            complete = item['complete']

            self.assertTrue(complete == False, "Verify item's state on false")

            self.assertLessEqual(decoded_list_1[-1], get_item_date(item),
                                 'Date of an admin announcement is out of the beginning of the last gr. period, ' + str(
                                     item["id"]))

            self.assertGreaterEqual(decoded_list_2[-1], get_item_date(item),
                                    'Date of an activity/supplemental is out of the end of the last gr. period ' + str(
                                        item["id"]))

    def tearDown(self):
        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

if __name__ == '__main__':
    unittest.main()