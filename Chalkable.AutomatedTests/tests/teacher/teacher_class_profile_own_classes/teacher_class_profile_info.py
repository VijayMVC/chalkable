from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        list_of_classes = self.list_of_classes
        one_random_class = random.choice(list_of_classes)
        get_info_page_of_one_class = self.get('/Class/ClassInfo.json?' + 'classId=' + str(one_random_class))

if __name__ == '__main__':
    unittest.main()