from base_auth_test import *
import unittest

class TestAdminMarkItems_All_DueBeforeToday_OlderThan30days(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        # reset settings
        self.admin.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.admin.post_json('/Announcement/UnDone.json?', data={'option': 3})

        # get a current time
        self.current_time = datetime.date(datetime.now())

        # get a date needed for filtering
        date = datetime.now()
        end_date = date.today() - timedelta(days=29)
        self.current_date_minus_30 = datetime.date(end_date)

    def internal_(self, option, start, count):
        self.admin.post_json('/Announcement/Done.json?', data={'option': option})

        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.admin.get_json(
                '/Feed/DistrictAdminFeed.json?' + 'gradeLevelIds=' + '&start=' + str(start) + '&complete=' + str(
                    False) + '&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

            return dictionary_verify_annoucementviewdatas_all

        if option == 3:
            self.assertTrue(len(list_items_json_unicode(0, 2500)) == 0,
                            'Admin announcements were not marked as done!')

        def get_item_date(one_item):
            return datetime.date(datetime.strptime(one_item['adminannouncementdata']['expiresdate'], '%Y-%m-%d'))

        def current_time_or_time_minus_30(inner_option):
            if inner_option == 2:
                inner_current_time_minus_30 = self.current_date_minus_30
                return inner_current_time_minus_30
            if inner_option == 1:
                inner_current_time = self.current_time
                return inner_current_time

        for item in list_items_json_unicode(0, 2500):
                self.assertLessEqual(current_time_or_time_minus_30(option), get_item_date(item),
                                     'Admin announcements are not marked as complete ' + str(item["id"]))

    def test_mark_done_all_items(self):
        self.internal_(3, 0, 2500)

    def test_mark_done_due_before_today(self):
        self.internal_(1, 0, 2500)

    def test_mark_done_older_than_30_days(self):
        self.internal_(2, 0, 2500)

    def tearDown(self):
        # reset all filters on the feed
        self.admin.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()