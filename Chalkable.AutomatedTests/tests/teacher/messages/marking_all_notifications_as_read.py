from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):

        mark_all_notifications = self.get('/Notification/MarkAllAsShown.json?')

if __name__ == '__main__':
    unittest.main()