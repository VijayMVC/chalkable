from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
            data = {"ids": "", "read": True}

            post_unread = self.postJSON('/PrivateMessage/MarkAsRead.json?', data)

if __name__ == '__main__':
    unittest.main()