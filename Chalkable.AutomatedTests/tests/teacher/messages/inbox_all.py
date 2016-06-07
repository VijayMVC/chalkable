from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        get_messages_inbox_all = self.get('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(10) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False))
        total_count = get_messages_inbox_all['totalcount']
        data = get_messages_inbox_all['data']
        total_pages = get_messages_inbox_all['totalpages']

        if total_count > 10:
            self.assertTrue(total_pages > 1, 'total_pages must be more than 1')

if __name__ == '__main__':
    unittest.main()