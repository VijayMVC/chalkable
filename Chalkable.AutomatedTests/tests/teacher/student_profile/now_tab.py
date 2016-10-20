from base_auth_test import *
import unittest

class TestStudentProfileNow(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        school_year = self.teacher.school_year()

        dictionary_get_list_my_students = self.teacher.get_json(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        list_for_class_id_from_api = []

        if len(dictionary_get_list_my_students['data']) > 10:
            student_id = dictionary_get_list_my_students['data'][10]['id']

            student_now = self.teacher.get_json('/Student/Summary.json?' + '&schoolPersonId=' + str(student_id))
            student_now_data = student_now['data']

            for k in student_now_data['classessection']:
                class_id = k['id']
                list_for_class_id_from_api.append(class_id)

            # connecting to SQL server
            cnxn = pyodbc.connect('DRIVER={SQL Server};SERVER=me0buyg8np.database.windows.net;DATABASE=83009FE9-8594-4E33-A09E-1EF4F81D0E8D;UID=chalkableadmin;PWD=Hellowebapps1!')
            cursor = cnxn.cursor()

            # getting data from SQL server
            cursor.execute('Select Distinct ClassRef From ClassPerson JOIN Class On ClassPerson.ClassRef=Class.id Where ClassPerson.PersonRef =' + str(student_id) + 'and Class.SchoolYearRef =' + str(school_year) + 'and ClassPerson.IsEnrolled=1')
            list_for_class_id_from_sql = []

            for row in cursor.fetchall():
                temporary_variable = row[0]
                list_for_class_id_from_sql.append(temporary_variable)

            self.assertItemsEqual(list_for_class_id_from_api, list_for_class_id_from_sql, "Student doesn't have all classes: " + str(set(list_for_class_id_from_api) ^ set(list_for_class_id_from_sql)))

    def test_student_profile_now_tab(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()