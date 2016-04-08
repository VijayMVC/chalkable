# Feed > ToDo. ["Items older than 30 days", "Any time", "All Types", "Earliest"]
# http://screencast.com/t/jLpDvTAcpG

from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        self.settings_data_for_mark_undone = {'option':'3'}
        
        #data that will need for filtering by lesson plans for grading period 1
        self.settings_data_lesson_plan = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan)   
        
        #get 'done' lesson plans of the grading period 1
        done_lesson_plans_json_unicode = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(500))
        annoucementviewdatas_json_unicode = done_lesson_plans_json_unicode['data']['annoucementviewdatas']

        self.id_of_lessonplan_and_activities_gradingperiod1 = []
        self.id_of_lessonplan_and_activities_gradingperiod2 = []
        
        #making all of these lesson plans 'unmark'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)
      
        #data that needed for filtering by lesson plans for grading period 2
        self.settings_data_lesson_plan_2 = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan_2)
        
        #get 'done' lesson plans of the grading period 2
        done_lesson_plans_json_unicode_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(500))
        annoucementviewdatas_json_unicode_2 = done_lesson_plans_json_unicode_2['data']['annoucementviewdatas']       
        
        #making all of these lesson plans 'unmark'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)
            
#########################################################################################################################
        #data that needed for filtering by activities for grading period 1
        self.settings_data_activity = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity)   
        
        #get 'done' activities of the grading period 1
        done_activities_json_unicode = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(500))
        for_item_id_1 = done_activities_json_unicode['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)
        
        #data that needed for filtering by activities for grading period 2
        self.settings_data_activity_2 = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity_2)   
        
        #get 'done' activities of the grading period 2
        done_activities_json_unicode_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(500))
        for_item_id_2 = done_activities_json_unicode_2['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)  
           
#########################################################################################################################

        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
        self.assertFalse(set(self.id_of_lessonplan_and_activities_gradingperiod2) & set(self.id_of_lessonplan_and_activities_gradingperiod1), 'Items are in different grading periods')
       
        #get a needed date
        self.current_time = time.strftime('%Y-%m-%d')     
        Date = datetime.strptime(self.current_time, "%Y-%m-%d")
        StartDate = Date.today() - timedelta(days=10)
        EndDate = Date.today() + timedelta(days=10)
        self.current_date_minus_10 = StartDate.strftime("%Y-%m-%d")
        self.current_date_plus_10 = EndDate.strftime("%Y-%m-%d")
        
        self.settings_data = {'sortType':'1', 'announcementType':'3', 'fromDate': self.current_date_minus_10, 'toDate':self.current_date_plus_10}
        self.post('/Feed/SetSettings.json?', self.settings_data)  
        
        #get items 'Items older than 30 days'
        self.settings_data = {'option':'2', 'annType':'3'}
        self.post('/Announcement/Done.json?', self.settings_data)   
        
        #set filter by activities
        self.type_of_activity = {'type':'3'}
 
        self.do_feed_list_and_verify(0)    

    def do_feed_list_and_verify(self, start, count=2000):
        #get a current time
        self.current_time = time.strftime('%Y-%m-%d')  
        
        #get a date needed for filtering
        Date = datetime.strptime(self.current_time, "%Y-%m-%d")
        EndDate = Date.today()- timedelta(days=30)
        self.current_date_minus_30 = EndDate.strftime("%Y-%m-%d")
    
        list_items_json_unicode = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=false&count='+str(count))
        dictionary_verify_settingsforfeed = list_items_json_unicode['data']['settingsforfeed']
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists')
        self.assertTrue('gradingperiodid' in dictionary_verify_settingsforfeed, 'announcementtype exists')

        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype'])
        sorttype = str(dictionary_verify_settingsforfeed['sorttype'])
        gradingperiodid = str(dictionary_verify_settingsforfeed['gradingperiodid'])
            
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
            self.assertTrue(len(annoucementviewdatas_json_unicode) == 0, 'There are no grading period!')
        
        #getting decoded list
        decoded_list_1 = [x.encode('utf-8') for x in list_for_start_date]
        decoded_list_2 = [x.encode('utf-8') for x in list_for_end_date]   
        
     
        list_for_date = []
        
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']
        if len(dictionary_verify_annoucementviewdatas_all) > 0:
            for item in dictionary_verify_annoucementviewdatas_all:
                type = str(item['type'])
                lessonplandata = item['lessonplandata']
                complete = item['complete']
                self.assertTrue(complete == False, "Verify item's state on false")
                startdate = lessonplandata['startdate']
                enddate = lessonplandata['enddate']
                list_for_date.append(startdate)
                self.assertLessEqual(self.current_date_minus_30, enddate, 'Current date <= enddate of a lesson plan ' + str(item["id"]))
                self.assertDictContainsSubset(self.type_of_activity, {'type':type}, 'Only activities are shown')
            decoded_list = [x.encode('utf-8') for x in list_for_date]
            sorted_list_dates = sorted(decoded_list, reverse=True)
            self.assertTrue(decoded_list == sorted_list_dates, 'Items are sorted in earliest order')
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