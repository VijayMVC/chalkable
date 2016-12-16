from base_auth_test import *
import unittest

class TestMarkItems_All_DueBeforeToday_OlderThan30days(BaseTestCase):
    def setUp(self):
        self.student = StudentSession(self).login(user_email, user_pwd)

        # reset settings
        self.student.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.student.post_json('/Announcement/UnDone.json?', data={'option': 3})

        # get a current time
        self.current_time = datetime.now()

        # get a date needed for filtering
        date = datetime.now()
        end_date = date.today() - timedelta(days=29)
        self.current_date_minus_30 = end_date

    def internal_(self, option, start, count):
        self.student.post_json('/Announcement/Done.json?', data={'option': option})

        list_items_json_unicode = self.student.get_json(
            '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

        if option == 3:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all) == 0, 'Items were not marked as done!')

        def get_item_date(one_item):
            if one_item['type'] == 1:
                return datetime.date(datetime.strptime(one_item['classannouncementdata']['expiresdate'], '%Y-%m-%d'))
            if one_item['type'] == 4:
                return datetime.date(
                    datetime.strptime(one_item['supplementalannouncementdata']['expiresdate'], '%Y-%m-%d'))

        def get_lesson_plan_end_date(one_item):
            if one_item['type'] == 3:
                return datetime.date(datetime.strptime(one_item['lessonplandata']['enddate'], '%Y-%m-%d'))

        def current_time_or_time_minus_30(inner_option):
            if inner_option == 2:
                inner_current_time = self.current_date_minus_30
                return datetime.date(inner_current_time)
            if inner_option == 1:
                inner_current_time = self.current_time
                return datetime.date(inner_current_time)

        for item in dictionary_verify_annoucementviewdatas_all:
            if item['type'] == 1 or item['type'] == 4:
                self.assertLessEqual(current_time_or_time_minus_30(option), get_item_date(item),
                                     'Activities and Supplementals are not marked as complete ' + str(item["id"]))

            if item['type'] == 3:
                self.assertTrue((get_lesson_plan_end_date(item) >= current_time_or_time_minus_30(option)),
                                'Lesson plan is not marked as complete ' + str(item["id"]))

    def test_mark_done_all_items(self):
        self.internal_(3, 0, 2500)

    def test_mark_done_due_before_today(self):
        self.internal_(1, 0, 2500)

    def test_mark_done_older_than_30_days(self):
        self.internal_(2, 0, 2500)

    def tearDown(self):
        # reset all filters on the feed
        self.student.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()