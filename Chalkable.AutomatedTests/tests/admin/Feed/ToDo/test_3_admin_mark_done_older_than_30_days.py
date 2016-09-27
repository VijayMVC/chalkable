from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # reset all filters on the feed
        self.dict = {}
        self.post_admin('/Feed/SetSettings.json?', self.dict)

        self.settings_data_for_mark_undone = {'option': '3'}

        # making all admin announcements as 'undone'
        self.post_admin('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)

        # mark done "Older Than 30 Days"
        self.settings_data = {'option': '2'}
        self.post_admin('/Announcement/Done.json?', self.settings_data)

        self.do_feed_list_and_verify(0)

    def do_feed_list_and_verify(self, start, count=2500):
        # get a current time
        self.current_time = time.strftime('%Y-%m-%d')

        # get a date needed for filtering
        Date = datetime.strptime(self.current_time, "%Y-%m-%d")
        EndDate = Date.today() - timedelta(days=30)
        self.current_date_minus_30 = EndDate.strftime("%Y-%m-%d")

        list_items_json_unicode = self.get_admin('/Feed/DistrictAdminFeed.json?' + 'gradeLevelIds=' + '&start=' + str(start) + '&complete=' + str(False) + '&count=' + str(count))
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

        def get_item_date(one_item):
            return one_item['adminannouncementdata']['expiresdate']

        for item in dictionary_verify_annoucementviewdatas_all:
            self.assertLessEqual(self.current_date_minus_30, get_item_date(item),
                                 'Admin announcements are not marked as complete ' + str(item["id"]))

    def tearDown(self):
        # reset all filters on the feed
        self.dict = {}
        self.post_admin('/Feed/SetSettings.json?', self.dict)

if __name__ == '__main__':
    unittest.main()