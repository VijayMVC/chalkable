from base_auth_test import *
import unittest

class TestUnmarkItems(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

        # reset settings
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'done'
        self.teacher.post_json('/Announcement/Done.json?', data={'option': 3})

    def internal_(self, announcement_type):
        # filter: activities/supplementals/lesson plans
        self.teacher.post_json('/Feed/SetSettings.json?', data={
            'announcementType': announcement_type,
            'sortType': 0
        })

        # get all activities/supplementals/lesson plans
        feed_list1 = self.teacher.post_json('/Feed/List.json', data={
            'classId': '',
            'complete': 'False',
            'count': 10,
            'start': 0
        })

        if len(feed_list1['data']['annoucementviewdatas']) != 0:
            self.assertTrue(feed_list1['data']['annoucementviewdatas'][0]['complete'],
                             'Item is not marked as done')

            # posting uncomplete
            self.teacher.post_json('/Announcement/Complete', data={
                'announcementId': feed_list1['data']['annoucementviewdatas'][0]['id'],
                'announcementType': announcement_type,
                'complete': 'False'
            })

            # get all activities/supplementals/lesson plans
            feed_list2 = self.teacher.post_json('/Feed/List.json', data={'classId': '', 'count': 10, 'start': 0})

            self.assertFalse(feed_list2['data']['annoucementviewdatas'][0]['complete'],
                            'Item is marked as done')

    def test_unmark_activity(self):
        self.internal_(1)

    def test_unmark_supplemental(self):
        self.internal_(4)

    def test_unmark_lesson_plan(self):
        self.internal_(3)

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()