from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        first_year = self.list_of_years()[0]
        get_messages_inbox_all = self.get('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(10) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False) + '&acadYear=' + str(first_year))


if __name__ == '__main__':
    unittest.main()