from base_auth_test import *
import unittest


class TestFeed(unittest.TestCase):

    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

        # reset settings
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

    def test_feed(self):

        # making all types of items as 'undone'
        self.teacher.post_json('/Announcement/UnDone.json?', data={'option': '3'})

        # filter: activities
        self.teacher.post_json('/Feed/SetSettings.json?', data={'announcementType': '1', 'sortType': '0'})

        # get all activities
        feed_list1 = self.teacher.post_json('/Feed/List.json', data={
            'classId': '',
            'complete': 'False',
            'count': 10,
            'start': 0
        })

        if len(feed_list1['data']['annoucementviewdatas']) != 0:
            self.assertFalse(feed_list1['data']['annoucementviewdatas'][0]['complete'],
                             'Activity is marked as done')

            # posting complete
            self.teacher.post_json('/Announcement/Complete', data={
                'announcementId': feed_list1['data']['annoucementviewdatas'][0]['id'],
                'announcementType': 1,
                'complete': 'True'
            })

            # get all activities
            feed_list2 = self.teacher.post_json('/Feed/List.json', data={'classId': '', 'count': 10, 'start': 0})

            self.assertTrue(feed_list2['data']['annoucementviewdatas'][0]['complete'],
                            'Activity was not marked as done')

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()