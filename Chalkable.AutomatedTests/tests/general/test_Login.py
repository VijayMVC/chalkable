from base_auth_test import *


class TestLogin(BaseAuthedTestCase):
    def test_login(self):
        self.assertIsNotNone(self.session)
        self.assertIsNotNone(self.districtId)
        self.assertIsNotNone(self.schoolYearId)


if __name__ == '__main__':
    unittest.main()
