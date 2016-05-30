from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        data = {"ids": "", "income": False}

        post_delete = self.postJSON('/PrivateMessage/Delete.json?', data)

if __name__ == '__main__':
    unittest.main()
