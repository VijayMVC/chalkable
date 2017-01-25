from base_auth_test import *
import unittest

class TestAllTypesOfItemsFilterByEarliestLatest(BaseTestCase):
    def setUp(self):
        self.student = StudentSession(self).login(user_email, user_pwd)

        # reset settings
        self.student.post_json('/Feed/SetSettings.json?', data={})

        # making all types of items as 'undone'
        self.student.post_json('/Announcement/UnDone.json?', data={'option': 3})

    def internal_(self, sortType):
        def list_items_json_unicode(start, count):
            list_items_json_unicode = self.student.get_json(
                '/Feed/List.json?start=' + str(start) + '&classId=&complete=false&count=' + str(count))
            dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']

            return dictionary_verify_annoucementviewdatas_all

        # filter: activities, supplementals, lesson plans
        self.student.post_json('/Feed/SetSettings.json?', data={'sortType': sortType})

        def get_item_date(one_item):
            if one_item['type'] == 3:
                return datetime.date(datetime.strptime(one_item['lessonplandata']['startdate'], '%Y-%m-%d'))
            if one_item['type'] == 1:
                return datetime.date(datetime.strptime(one_item['classannouncementdata']['expiresdate'], '%Y-%m-%d'))
            if one_item['type'] == 4:
                return datetime.date(
                    datetime.strptime(one_item['supplementalannouncementdata']['expiresdate'], '%Y-%m-%d'))

        def get_item_id(one_item):
            return one_item['id']

        def get_item_type(one_item):
            return one_item['type']

        last_item = None
        for item in list_items_json_unicode(0, 2500):
            if last_item != None:
                if sortType == 0:
                    self.assertTrue(get_item_date(last_item) <= get_item_date(item),
                                    "Items are sorted not in earliest order" + ": " +
                                    str(get_item_date(last_item)) + " " +
                                    str(get_item_date(item)) + " " + "item_type: " + str(get_item_type(last_item)) +
                                    " " + "item_id: " + str(get_item_id(item)))
                if sortType == 1:
                    self.assertTrue(get_item_date(last_item) >= get_item_date(item),
                                    "Items are sorted not in earliest order" + ": " +
                                    str(get_item_date(last_item)) + " " +
                                    str(get_item_date(item)) + " " + "item_type: " + str(get_item_type(last_item)) +
                                    " " + "item_id: " + str(get_item_id(item)))

            last_item = item

    def test_sorting_items_earliest(self):
        self.internal_(0)

    def test_sorting_items_latest(self):
        self.internal_(1)

    def tearDown(self):
        # reset all filters on the feed
        self.student.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()