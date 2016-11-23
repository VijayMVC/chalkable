from base_auth_test import *
import unittest

class TestMarkAdminAnnouncementAsUncomplete(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        # reset settings
        self.admin.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'done'
        self.admin.post_json('/Announcement/Done.json?', data={'option': 3})

    def internal_(self):
        # get all admin announcements
        feed_list1 = self.admin.post_json('/Feed/DistrictAdminFeed.json?', data={'gradeLevelIds': '', 'count': '10', 'start': '0'})

        self.assertTrue(len(feed_list1['data']['annoucementviewdatas']) != 0, "Admin announcements must be markd as complete")

        feed_list3 = self.admin.post_json('/Feed/DistrictAdminFeed.json?',
                                          data={'gradeLevelIds': '', 'complete': 'False', 'count': '10', 'start': '0'})

        self.assertTrue(len(feed_list3['data']['annoucementviewdatas']) == 0,
                        "Admin announcements must be markd as complete")

        if len(feed_list1['data']['annoucementviewdatas']) != 0:
            self.assertTrue(feed_list1['data']['annoucementviewdatas'][0]['complete'], 'Admin announcements is marked as done')

            # posting uncomplete
            self.admin.post_json('/Announcement/Complete', data={'announcementId': str(feed_list1['data']['annoucementviewdatas'][0]['id']), 'announcementType': '2', 'complete': 'False'})

            # get all admin announcements
            feed_list2 = self.admin.post_json('/Feed/DistrictAdminFeed.json?', data={'gradeLevelIds': '', 'complete': 'False', 'count': '10', 'start': '0'})

            self.assertFalse(feed_list2['data']['annoucementviewdatas'][0]['complete'], 'Activity was not marked as done')

    def test_mark_lesson_plan_as_complete(self):
        self.internal_()

    def tearDown(self):
        # reset all filters on the feed
        self.admin.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()