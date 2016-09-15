from base_auth_test import *
import unittest


class TestMarkActivitiesAsComplete(unittest.TestCase):

    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

        # reset settings
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.teacher.post_json('/Announcement/UnDone.json?', data={'option': 3})

    def internal_(self, announcement_type):

        # filter: activities
        self.teacher.post_json('/Feed/SetSettings.json?', data={
            'announcementType': announcement_type,
            'sortType': 0
        })

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
                'announcementType': announcement_type,
                'complete': 'True'
            })

            # get all activities
            feed_list2 = self.teacher.post_json('/Feed/List.json', data={'classId': '', 'count': 10, 'start': 0})

            self.assertTrue(feed_list2['data']['annoucementviewdatas'][0]['complete'],
                            'Activity was not marked as done')

    def test_mark_activity_as_complete(self):
        self.internal_(1)

    def test_mark_supplemental_as_complete(self):
        self.internal_(4)

    def test_mark_lesson_plan_as_complete(self):
        self.internal_(3)

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()