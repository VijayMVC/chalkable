from base_auth_test import *
import unittest

class TestActivitiesSupplementalsLessonPlansMarkDoneAllItems(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

        # reset settings
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.teacher.post_json('/Announcement/UnDone.json?', data={'option': 3})

    def internal_(self, announcementType):
        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.teacher.get_json(
                '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

            return dictionary_verify_annoucementviewdatas_all

        # filter: activities, supplementals, lesson plans
        self.teacher.post_json('/Feed/SetSettings.json?', data={'announcementType': str(announcementType), 'sortType': '0'})

        # mark done "All Items"
        self.teacher.post_json('/Announcement/Done.json?', data={'option': '3', 'annType': str(announcementType)})

        # reset settings
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

        for item in list_items_json_unicode(0, 2500):
            if announcementType == 1:
                self.assertTrue(item['type'] != 1, "Activities are shown ")
            if announcementType == 4:
                self.assertTrue(item['type'] != 4, "Verify item's state on false")
            if announcementType == 3:
                self.assertTrue(item['type'] != 3, "Lesson plans are shown ")

    def test_mark_done_all_activities(self):
        self.internal_(1)

    def test_mark_done_all_lesson_plans(self):
        self.internal_(3)

    def test_mark_done_all_supplementals(self):
        self.internal_(4)

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()