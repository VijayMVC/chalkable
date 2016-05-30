import unittest
import requests
from base_config import *

class BaseAuthedTestCaseTeacher(unittest.TestCase):
    def setUp(self):
        # info for the teacher
        s = requests.Session()
        payload = {'UserName': user_email, 'Password': user_pwd, 'remember': 'false'}
        url = chlk_server_url + '/User/LogOn.aspx'
        r = s.post(url, data=payload)

        page_as_text = r.text
        page_as_one_string = str(page_as_text)

        self.session = s

    def get(self, url, status=200, success=True):
        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r = s.get(chlk_server_url + url, headers=headers)
        return self.verify_response(r, status, success)

    def postJSON(self, url, obj, status=200, success=True):
        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r = s.post(chlk_server_url + url, json=obj, headers=headers)
        return self.verify_response(r, status, success)

    def post(self, url, params, status=200, success=True):

        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r = s.post(chlk_server_url + url, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url)
            return None

        return self.verify_response(r, status, success)

    def verify_response(self, r, status, success):
        self.assertEquals(r.status_code, status, 'Response status code: ' + str(r.status_code) + ', body:' + r.text)

        try:
            data = r.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r.url)
            return None

        self.assertEquals(data['success'], success, 'API success')

        return data


if __name__ == '__main__':
    unittest.main()
