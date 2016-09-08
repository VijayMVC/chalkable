from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

        self.settings_data_for_mark_undone = {'option': '3'}

        # making all types of items as 'undone'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)

        # filter: supplementals
        self.settings_data = {'announcementType': '4', 'sortType': '0'}
        self.post('/Feed/SetSettings.json?', self.settings_data)

        # mark done "All Items"
        self.settings_data = {'option': '3', 'annType': '4'}
        self.post('/Announcement/Done.json?', self.settings_data)

        # reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

        self.do_feed_list_and_verify(0)

    def do_feed_list_and_verify(self, start, count=2000):
        list_items_json_unicode = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=false&count='+str(count))
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

        for item in dictionary_verify_annoucementviewdatas_all:
            self.assertTrue(item['type'] != 4, "Verify item's state on false")

    def tearDown(self):
        # reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

if __name__ == '__main__':
    unittest.main()