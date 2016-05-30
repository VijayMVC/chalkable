import unittest
import requests
from base_config import *

class BaseAuthedTestCaseStudent(unittest.TestCase):
    def setUp(self):
        # info for the student
        s_student = requests.Session()
        payload_student = {'UserName': user_email_student, 'Password': user_pwd_student, 'remember': 'false'}
        url_student = chlk_server_url + '/User/LogOn.aspx'
        r_student = s_student.post(url_student, data=payload_student)

        page_as_text_student = r_student.text
        page_as_one_string_student = str(page_as_text_student)

        self.session_student = s_student

    def get_student(self, url_student, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_student = s_student.get(chlk_server_url + url_student, headers=headers)
        return self.verify_response_student(r_student, status, success)

    def postJSON_student(self, url_student, obj, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_student = s_student.post(chlk_server_url + url_student, json=obj, headers=headers)
        return self.verify_response_student(r_student, status, success)

    def post_student(self, url_student, params, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r_student = s_student.post(chlk_server_url + url_student, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url_student)
            return None

        return self.verify_response_student(r_student, status, success)

    def verify_response_student(self, r_student, status, success):
        self.assertEquals(r_student.status_code, status, 'Response status code')

        try:
            data = r_student.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r_student.url_student)
            return None

        self.assertEquals(data['success'], success, 'API success')

        return data


if __name__ == '__main__':
    unittest.main()
