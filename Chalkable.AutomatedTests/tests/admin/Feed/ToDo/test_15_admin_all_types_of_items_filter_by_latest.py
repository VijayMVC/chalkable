from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # reset all filters on the feed
        self.dict = {}
        self.post_admin('/Feed/SetSettings.json?', self.dict)

        self.settings_data_for_mark_undone = {'option': '3'}

        # making admin announcements as 'undone'
        self.post_admin('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)

        self.settings_data = {'sortType': '1'}
        self.post_admin('/Feed/SetSettings.json?', self.settings_data)

        self.do_feed_list_and_verify(0)

    def do_feed_list_and_verify(self, start, count=2000):
        list_items_json_unicode = self.get_admin('/Feed/DistrictAdminFeed.json?' + 'gradeLevelIds=' + '&start=' + str(start) + '&complete=' + str(False) + '&count=' + str(count))
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

        def get_item_date(one_item):
            return one_item['adminannouncementdata']['expiresdate']

        def get_item_id(one_item):
            return one_item['id']

        def get_item_type(one_item):
            return one_item['type']

        last_item = None
        for item in dictionary_verify_annoucementviewdatas_all:
            if last_item != None:
                self.assertTrue(get_item_date(last_item) >= get_item_date(item),
                                "Items are sorted not in earliest order" + ": " +
                                str(get_item_date(last_item)) + " " +
                                str(get_item_date(item)) + " " + "item_type: " + str(get_item_type(last_item)) +
                                " " + "item_id: " + str(get_item_id(item)))

            last_item = item

    def tearDown(self):
        # reset all filters on the feed
        self.dict = {}
        self.post_admin('/Feed/SetSettings.json?', self.dict)

if __name__ == '__main__':
    unittest.main()