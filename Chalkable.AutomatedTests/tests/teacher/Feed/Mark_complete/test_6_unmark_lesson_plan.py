from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

        self.settings_data_for_mark_done = {'option': '3'}

        # making all types of items as 'done'
        self.post('/Announcement/Done.json?', self.settings_data_for_mark_done)

        # filter: lesson plans
        self.settings_data = {'announcementType': '3', 'sortType': '0'}
        self.post('/Feed/SetSettings.json?', self.settings_data)

        # get all lesson plans
        self.dict2 = {'classId': '', 'count': '10', 'start': '0'}
        feed_list1 = self.post('/Feed/List.json', self.dict2)

        if len(feed_list1['data']['annoucementviewdatas'])!= 0:
            id_of_the_first_activity = feed_list1['data']['annoucementviewdatas'][0]['id']

            status_of_the_first_activity_true = feed_list1['data']['annoucementviewdatas'][0]['complete']

            self.assertTrue(status_of_the_first_activity_true == True, 'Activity is marked as done')

            # posting uncomplete
            self.dict3 = {'announcementId': str(id_of_the_first_activity), 'announcementType': '3', 'complete': 'False'}
            self.post('/Announcement/Complete', self.dict3)

            # get all lesson plans
            self.dict4 = {'classId': '', 'complete': 'False', 'count': '10', 'start': '0'}
            feed_list2 = self.post('/Feed/List.json', self.dict4)

            status_of_the_first_activity_false = feed_list2['data']['annoucementviewdatas'][0]['complete']
            self.assertTrue(status_of_the_first_activity_false == False, 'Activity was not marked as done')

    def tearDown(self):
        # reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

if __name__ == '__main__':
    unittest.main()