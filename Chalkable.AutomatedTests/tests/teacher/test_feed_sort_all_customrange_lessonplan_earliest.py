# Feed > All. ["Custom Range", "Lesson Plan", "Earliest"]
# http://screencast.com/t/ocy3rge0qdY
from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
        self.type_of_activity = {'type':'3'}
        
        current_time = time.strftime('%Y-%m-%d')     
        Date = datetime.strptime(current_time, "%Y-%m-%d")
        StartDate = Date.today() - timedelta(days=10)
        EndDate = Date.today() + timedelta(days=10)
        self.current_date_minus_10 = StartDate.strftime("%Y-%m-%d")
        self.current_date_plus_10 = EndDate.strftime("%Y-%m-%d")
        
        self.settings_data = {'announcementType':'3', 'sortType':'0', 'fromDate': self.current_date_minus_10, 'toDate':self.current_date_plus_10}
        self.post('/Feed/SetSettings.json?', self.settings_data)  
    
        self.do_feed_list_and_verify(0)    
    
    def do_feed_list_and_verify(self, start, count=750):
        list_items_json_unicode = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=true&count='+str(count))
        settingsforfeed_json_unicode = list_items_json_unicode['data']['settingsforfeed']
        self.assertTrue('announcementtype' in settingsforfeed_json_unicode, 'announcementtype exists')
        self.assertTrue('sorttype' in settingsforfeed_json_unicode, 'sorttype exists')
        self.assertTrue('gradingperiodid' in settingsforfeed_json_unicode, 'announcementtype exists')
        
        announcementtype = str(settingsforfeed_json_unicode['announcementtype'])
        sorttype = str(settingsforfeed_json_unicode['sorttype'])
        gradingperiodid = str(settingsforfeed_json_unicode['gradingperiodid'])
        fromdate = settingsforfeed_json_unicode['fromdate']
        todate = settingsforfeed_json_unicode['todate']
        annoucementviewdatas_json_unicode = list_items_json_unicode['data']['annoucementviewdatas']  
        
        self.assertDictContainsSubset(self.settings_data, {'announcementType':announcementtype, 'sortType':sorttype, 'fromDate':fromdate, 'toDate':todate}, 'key/value pair exists')
        
        list_for_start_date = []
        list_for_end_date = []
        
        grading_periods_json_unicode = self.get('/GradingPeriod/List.json?')
        first_period_json_unicode = grading_periods_json_unicode['data']
        if len(first_period_json_unicode) > 0:
            for item in first_period_json_unicode:
                self.startdate_2 = item['startdate']
                list_for_start_date.append(self.startdate_2)
                self.enddate_2 = item['enddate']
                list_for_end_date.append(self.enddate_2)
        else:
            self.assertTrue(len(annoucementviewdatas_json_unicode) == 0, 'There are no items!')
        
        #getting decoded list
        decoded_list_1 = [x.encode('utf-8') for x in list_for_start_date]
        decoded_list_2 = [x.encode('utf-8') for x in list_for_end_date]
        
        #verify that state of items is 'done'
        for item in annoucementviewdatas_json_unicode:
            complete = item['complete']
            type = str(item['type'])
            self.assertTrue(complete == True, "Verify item's state on true")
            self.assertDictContainsSubset(self.type_of_activity, {'type':type}, 'Only activities are shown')
 
        list_for_date = []
        
        #Verify only items for a Custom Range
        if len(annoucementviewdatas_json_unicode) > 0:
            for item in annoucementviewdatas_json_unicode:
                type = str(item['type'])
                lessonplandata = item['lessonplandata']
                startdate = lessonplandata['startdate']
                self.assertLessEqual(self.current_date_minus_10, startdate, 'Verify exprisesdate of activity ' + str(item["id"]))
                self.assertGreaterEqual(self.current_date_plus_10, startdate, 'Verify exprisesdate of activity ' + str(item["id"]))
                list_for_date.append(startdate)
            decoded_list = [x.encode('utf-8') for x in list_for_date]
            sorted_list_dates = sorted(decoded_list, key = lambda d: map(int, d.split('-')))
            self.assertTrue(decoded_list == sorted_list_dates, 'Items are sorted in earliest order')
        else:
            self.assertTrue(len(annoucementviewdatas_json_unicode) == 0, 'There are no items!')
    
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