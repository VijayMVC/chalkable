from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
            data = {"ids": "", "read": False}

            post_read = self.postJSON('/PrivateMessage/MarkAsRead.json?', data)

if __name__ == '__main__':
    unittest.main()