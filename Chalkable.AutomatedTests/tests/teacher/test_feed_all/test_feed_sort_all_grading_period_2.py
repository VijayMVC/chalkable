#Feed > All. Verify that all items in a state 'done' and all items are in the grading period 2

from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        self.settings_data = {'sortType':'0', 'gradingPeriodId': self.gr_periods()[1]}
    
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
        self.assertDictContainsSubset(self.settings_data, {'announcementType':announcementtype, 'sortType':sorttype, 'gradingPeriodId':gradingperiodid}, 'key/value pair exists')
        
        list_for_start_date = []
        list_for_end_date = []
        get_grading_periods = self.get('/GradingPeriod/List.json?')
        get_first_period = get_grading_periods['data']
        if len(get_first_period) > 0:
            for item in get_first_period:
                self.startdate_2 = item['startdate']
                list_for_start_date.append(self.startdate_2)
                self.enddate_2 = item['enddate']
                list_for_end_date.append(self.enddate_2)
        else:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all) == 0, 'There are no items!')    
        decoded_list_1 = [x.encode('utf-8') for x in list_for_start_date]
        decoded_list_2 = [x.encode('utf-8') for x in list_for_end_date]
        #print decoded_list_1
        #print decoded_list_2
        
        #verify that state of items is 'done'
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']  
        for item in dictionary_verify_annoucementviewdatas_all:
            complete = item['complete']
            self.assertTrue(complete == True, "Verify item on true")
 
        self.assertTrue(gradingperiodid == self.gr_periods()[1], "Verify item on grading period")
        
        #verify that only shown items for S2
        dictionary_verify_annoucementviewdatas_all_2 = dictionary_get_items['data']['annoucementviewdatas']
        if len(dictionary_verify_annoucementviewdatas_all_2) > 0:
            for item in dictionary_verify_annoucementviewdatas_all_2:
                type = str(item['type'])
                if type == '3':
                    lessonplandata = item['lessonplandata']
                    startdate = lessonplandata['startdate']
                    self.assertLessEqual(decoded_list_1[1], startdate, 'Verify startdate of lessonplan ' + str(item["id"]))
                    self.assertGreaterEqual(decoded_list_2[1], startdate, 'Verify startdate of lessonplan ' + str(item["id"]))
                if type == '1':
                    classannouncementdata = item ['classannouncementdata']
                    expiresdate = classannouncementdata['expiresdate']
                    self.assertLessEqual(decoded_list_1[1], expiresdate, 'Verify exprisesdate of activity ' + str(item["id"]))
                    self.assertGreaterEqual(decoded_list_2[1], expiresdate, 'Verify exprisesdate of activity ' + str(item["id"]))
        else:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all_2) == 0, 'There are no items!')

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