from base_auth_test import *

class TestLogin(BaseAuthedTestCase):
  def test_login(self):
    self.assertIsNotNone(self.session)
           
if __name__ == '__main__':
    unittest.main()    