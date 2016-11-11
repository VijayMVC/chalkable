from base_auth_test import *
import unittest

class TestActivitiesSupplementalsLessonPlansMarkDoneAllItems(BaseTestCase):
    def setUp(self):
        self.student = StudentSession(self).login(user_email, user_pwd)

        # reset settings
        self.student.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.student.post_json('/Announcement/UnDone.json?', data={'option': 3})

    def internal_(self, announcementType):
        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.student.get_json(
                '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

            return dictionary_verify_annoucementviewdatas_all

        # filter: activities, supplementals, lesson plans
        self.student.post_json('/Feed/SetSettings.json?', data={'announcementType': str(announcementType), 'sortType': '0'})

        for item in list_items_json_unicode(0, 2500):
            if announcementType == 1:
                self.assertTrue(item['type'] == 1, "Not only activities are shown ")
            if announcementType == 4:
                self.assertTrue(item['type'] == 4, "Not only activities are shown ")
            if announcementType == 3:
                self.assertTrue(item['type'] == 3, "Not only activities are shown ")

    def test_sorting_by_activities(self):
        self.internal_(1)

    def test_sorting_by_lesson_plans(self):
        self.internal_(3)

    def test_sorting_by_supplementals(self):
        self.internal_(4)

    def tearDown(self):
        # reset all filters on the feed
        self.student.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()