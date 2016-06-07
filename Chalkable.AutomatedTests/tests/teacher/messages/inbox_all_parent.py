from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        get_messages_inbox_all = self.get('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(10) + '&income=' + str(True) + '&role=' + str('parent') + '&classOnly=' + str(False))

if __name__ == '__main__':
    unittest.main()