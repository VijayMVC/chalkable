from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

        self.settings_data_for_mark_undone = {'option': '3'}

        # making all types of items as 'undone'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)

        # mark done "Due Before Today"
        self.settings_data = {'option': '1'}
        self.post('/Announcement/Done.json?', self.settings_data)

        self.do_feed_list_and_verify(0)

    def do_feed_list_and_verify(self, start, count=2000):
        # get a current time
        self.current_time = time.strftime('%Y-%m-%d')

        list_items_json_unicode = self.get(
            '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

        def get_item_date(one_item):
            if one_item['type'] == 1:
                return one_item['classannouncementdata']['expiresdate']
            if one_item['type'] == 4:
                return one_item['supplementalannouncementdata']['expiresdate']

        def get_lesson_plan_end_date(one_item):
            if one_item['type'] == 3:
                return one_item['lessonplandata']['enddate']

        for item in dictionary_verify_annoucementviewdatas_all:
            if item['type'] == 1 or item['type'] == 4:
                self.assertLessEqual(self.current_time, get_item_date(item),
                                     'Activities and Supplementals are not marked as complete ' + str(item["id"]))

            if item['type'] == 3:
                #self.assertTrue((get_lesson_plan_start_date(item) >= self.current_time) or (get_lesson_plan_start_date(item) < self.current_time and get_lesson_plan_end_date(item) >= self.current_time),
                              #       'Lesson plan is not marked as complete ' + str(item["id"]))
                self.assertTrue((get_lesson_plan_end_date(item) >= self.current_time),
                                'Lesson plan is not marked as complete ' + str(item["id"]))

    def tearDown(self):
        # reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

if __name__ == '__main__':
    unittest.main()