from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_attendance(self):
        get_apps = self.get('/Application/MyApps.json?' + 'start=' + str(0) + '&count=' + str(1000))

if __name__ == '__main__':
    unittest.main()