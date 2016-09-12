from base_auth_test import *

class TestStudentProfileInfo(BaseAuthedTestCase):
    def test_student_info_get(self):
        school_year = self.school_year()
        dictionary_get_list_my_students = self.get(
            '/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start=0&count=1000')

        student_id = None
        class_id = None
        list_for_class_id_from_api = []
        if len(dictionary_get_list_my_students['data']) > 10:
            eleventh_student = dictionary_get_list_my_students['data'][10]
            for key, value in eleventh_student.iteritems():
                if key == 'id':
                    student_id = value
            student_now = self.get('/Student/Summary.json?' + '&schoolPersonId=' + str(student_id))
            student_now_data = student_now['data']
            for k in student_now_data['classessection']:
                for key, value in k.iteritems():
                    if key == 'id':
                        class_id = value
                        list_for_class_id_from_api.append(value)

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

if __name__ == '__main__':
    unittest.main()
