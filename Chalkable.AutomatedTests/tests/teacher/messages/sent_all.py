from base_auth_test import *
import unittest

class TestSentAll(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        get_messages_inbox_all = self.teacher.get_json('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(10) + '&income=' + str(False) + '&role=' + '&classOnly=' + str(False))
        total_count = get_messages_inbox_all['totalcount']
        #data = get_messages_inbox_all['data']
        total_pages = get_messages_inbox_all['totalpages']

        if total_count > 10:
            self.assertTrue(total_pages > 1, 'total_pages must be more than 1')
        else:
            self.assertTrue(total_pages == 0, 'total_pages must be 0')

    def test_sent_all(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()