from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_attendance(self):
        get_attendance_summary = self.get('/Attendance/AttendanceSummary.json?')
        get_not_taken_attendance_classes= self.get('/NotTakenAttendanceClasses.json?')

if __name__ == '__main__':
    unittest.main()
