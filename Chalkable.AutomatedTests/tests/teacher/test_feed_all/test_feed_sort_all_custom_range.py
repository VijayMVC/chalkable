#Feed > All. Verify 'Custom Range'.

from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
        current_time = time.strftime('%Y-%m-%d')     
        Date = datetime.strptime(current_time, "%Y-%m-%d")
        StartDate = Date.today() - timedelta(days=5)
        EndDate = Date.today() + timedelta(days=5)
        self.current_date_minus_5 = StartDate.strftime("%Y-%m-%d")
        self.current_date_plus_5 = EndDate.strftime("%Y-%m-%d")
        
        self.settings_data = {'sortType':'0', 'fromDate': self.current_date_minus_5, 'toDate':self.current_date_plus_5}
        
        self.post('/Feed/SetSettings.json?', self.settings_data)   
    
        self.do_feed_list_and_verify(0)    

    def do_feed_list_and_verify(self, start, count=500):
        dictionary_get_items = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=true&count='+str(count))
        dictionary_verify_settingsforfeed = dictionary_get_items['data']['settingsforfeed']
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists')
        self.assertTrue('gradingperiodid' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        
        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype'])
        sorttype = str(dictionary_verify_settingsforfeed['sorttype'])
        gradingperiodid = str(dictionary_verify_settingsforfeed['gradingperiodid'])
    
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']
        if len(dictionary_verify_annoucementviewdatas_all) > 0:
            for item in dictionary_verify_annoucementviewdatas_all:
                type = str(item['type'])
                if type == '3':
                    lessonplandata = item['lessonplandata']
                    for key, value in lessonplandata.iteritems():
                        startdate = lessonplandata['startdate']
                        enddate = lessonplandata['enddate']
                        self.assertLessEqual(self.current_date_minus_5, startdate, 'Verify startdate of lessonplan ' + str(item["id"]))
                        self.assertGreaterEqual(self.current_date_plus_5, startdate, 'Verify startdate of lessonplan ' + str(item["id"]))
                if type == '1':
                    classannouncementdata = item ['classannouncementdata']
                    for key2, value2 in classannouncementdata.iteritems():
                        expiresdate = classannouncementdata['expiresdate']
                        self.assertLessEqual(self.current_date_minus_5, expiresdate, 'Verify exprisesdate of activity ' + str(item["id"]))
                        self.assertGreaterEqual(self.current_date_plus_5, expiresdate, 'Verify exprisesdate of activity ' + str(item["id"]))
        else:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all) == 0, 'There are no items!')
    
    def tearDown(self):
        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
        #marking all items as 'done'
        #get_all_unmarket_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(2000)) 
        self.settings_data = {'option':'3'}
        self.post('/Announcement/Done.json?', self.settings_data) 
            
if __name__ == '__main__':
    unittest.main()